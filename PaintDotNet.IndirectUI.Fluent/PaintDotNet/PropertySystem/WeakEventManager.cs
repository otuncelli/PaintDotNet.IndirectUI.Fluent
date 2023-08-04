using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PaintDotNet.PropertySystem
{
    internal sealed class ValueChangedEventManager : WeakEventManager<ValueChangedEventManager, Property, ValueEventHandler<object>, ValueEventArgs<object>>
    {
        protected override void StartListening(Property source) => source.ValueChanged += DeliverEvent;
        protected override void StopListening(Property source) => source.ValueChanged -= DeliverEvent;
    }

    internal sealed class ReadOnlyChangedEventHandler : WeakEventManager<ReadOnlyChangedEventHandler, Property, ValueEventHandler<bool>, ValueEventArgs<bool>>
    {
        protected override void StartListening(Property source) => source.ReadOnlyChanged += DeliverEvent;
        protected override void StopListening(Property source) => source.ReadOnlyChanged -= DeliverEvent;
    }

    internal abstract class WeakEventManager<TManager, TEventSource, TEventHandler, TEventArgs> 
        where TManager: WeakEventManager<TManager, TEventSource, TEventHandler, TEventArgs>, new()
    {
        private static readonly object StaticSource = new();

        /// <summary>
        /// Mapping between the target of the delegate (for example a Button) and the handler (EventHandler).
        /// Windows Phone needs this, otherwise the event handler gets garbage collected.
        /// </summary>
        private readonly ConditionalWeakTable<object, List<Delegate>> targetToEventHandler = new();

        /// <summary>
        /// Mapping from the source of the event to the list of handlers. This is a CWT to ensure it does not leak the source of the event.
        /// </summary>
        private readonly ConditionalWeakTable<object, WeakHandlerList> sourceToWeakHandlers = new();

        private static readonly Lazy<TManager> current = new(() => new TManager());

        public static TManager Current => current.Value;

        /// <summary>
        /// Adds a weak reference to the handler and associates it with the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="handler">The handler.</param>
        public static void AddHandler(TEventSource source, TEventHandler handler)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (handler == null) throw new ArgumentNullException("handler");

            if (!typeof(TEventHandler).GetTypeInfo().IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("Handler must be Delegate type");
            }

            Current.PrivateAddHandler(source, handler);
        }

        /// <summary>
        /// Removes the association between the source and the handler.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="handler">The handler.</param>
        public static void RemoveHandler(TEventSource source, TEventHandler handler)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            if (!typeof(TEventHandler).GetTypeInfo().IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("handler must be Delegate type");
            }

            Current.PrivateRemoveHandler(source, handler);
        }

        /// <summary>
        /// Delivers the event to the handlers registered for the source. 
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="TEventArgs"/> instance containing the event data.</param>
        public static void DeliverEvent(object sender, TEventArgs args)
        {
            Current.PrivateDeliverEvent(sender, args);
        }

        /// <summary>
        /// Override this method to attach to an event.
        /// </summary>
        /// <param name="source">The source.</param>
        protected abstract void StartListening(TEventSource source);

        /// <summary>
        /// Override this method to detach from an event.
        /// </summary>
        /// <param name="source">The source.</param>
        protected abstract void StopListening(TEventSource source);

        private void PrivateAddHandler(TEventSource source, TEventHandler handler)
        {
            AddWeakHandler(source, handler);
            AddTargetHandler(handler);
        }

        private void AddWeakHandler(TEventSource source, TEventHandler handler)
        {
            if (sourceToWeakHandlers.TryGetValue(source, out WeakHandlerList weakHandlers))
            {
                // clone list if we are currently delivering an event
                if (weakHandlers.IsDeliverActive)
                {
                    weakHandlers = weakHandlers.Clone();
                    sourceToWeakHandlers.Remove(source);
                    sourceToWeakHandlers.Add(source, weakHandlers);
                }
                weakHandlers.AddWeakHandler(source, handler);
            }
            else
            {
                weakHandlers = new WeakHandlerList();
                weakHandlers.AddWeakHandler(source, handler);

                sourceToWeakHandlers.Add(source, weakHandlers);
                StartListening(source);
            }

            Purge(source);
        }

        private void AddTargetHandler(TEventHandler handler)
        {
            var @delegate = handler as Delegate;
            object key = @delegate.Target ?? StaticSource;

            if (targetToEventHandler.TryGetValue(key, out List<Delegate> delegates))
            {
                delegates.Add(@delegate);
            }
            else
            {
                delegates = new List<Delegate>
                {
                    @delegate
                };

                targetToEventHandler.Add(key, delegates);
            }
        }

        private void PrivateRemoveHandler(TEventSource source, TEventHandler handler)
        {
            RemoveWeakHandler(source, handler);
            RemoveTargetHandler(handler);
        }

        private void RemoveWeakHandler(TEventSource source, TEventHandler handler)
        {
            if (sourceToWeakHandlers.TryGetValue(source, out WeakHandlerList weakHandlers))
            {
                // clone list if we are currently delivering an event
                if (weakHandlers.IsDeliverActive)
                {
                    weakHandlers = weakHandlers.Clone();
                    sourceToWeakHandlers.Remove(source);
                    sourceToWeakHandlers.Add(source, weakHandlers);
                }

                if (weakHandlers.RemoveWeakHandler(source, handler) && weakHandlers.Count == 0)
                {
                    sourceToWeakHandlers.Remove(source);
                    StopListening(source);
                }
            }
        }

        private void RemoveTargetHandler(TEventHandler handler)
        {
            Delegate @delegate = handler as Delegate;
            object key = @delegate.Target ?? StaticSource;

            if (targetToEventHandler.TryGetValue(key, out List<Delegate> delegates))
            {
                delegates.Remove(@delegate);

                if (delegates.Count == 0)
                {
                    targetToEventHandler.Remove(key);
                }
            }
        }

        private void PrivateDeliverEvent(object sender, TEventArgs args)
        {
            object source = sender ?? StaticSource;
            bool hasStaleEntries = false;
            if (sourceToWeakHandlers.TryGetValue(source, out WeakHandlerList weakHandlers))
            {
                using (weakHandlers.DeliverActive())
                {
                    hasStaleEntries = weakHandlers.DeliverEvent(source, args);
                }
            }

            if (hasStaleEntries)
            {
                Purge(source);
            }
        }

        private void Purge(object source)
        {
            if (sourceToWeakHandlers.TryGetValue(source, out WeakHandlerList weakHandlers))
            {
                if (weakHandlers.IsDeliverActive)
                {
                    weakHandlers = weakHandlers.Clone();
                    sourceToWeakHandlers.Remove(source);
                    sourceToWeakHandlers.Add(source, weakHandlers);
                }
                else
                {
                    weakHandlers.Purge();
                }
            }
        }

        private class WeakHandler
        {
            private readonly WeakReference source;
            private readonly WeakReference originalHandler;

            public bool IsActive => source != null && source.IsAlive && originalHandler != null && originalHandler.IsAlive;

            public TEventHandler Handler => originalHandler == null ? default : (TEventHandler)originalHandler.Target;

            public WeakHandler(object source, TEventHandler originalHandler)
            {
                this.source = new WeakReference(source);
                this.originalHandler = new WeakReference(originalHandler);
            }

            public bool Matches(object source, TEventHandler handler)
            {
                return this.source != null &&
                    ReferenceEquals(this.source.Target, source) &&
                    originalHandler != null &&
                    (ReferenceEquals(originalHandler.Target, handler) ||
                    (originalHandler.Target is PropertyChangedEventHandler &&
                    handler is PropertyChangedEventHandler &&
                    Equals((originalHandler.Target as PropertyChangedEventHandler).Target,
                        (handler as PropertyChangedEventHandler).Target)));
            }
        }

        private class WeakHandlerList
        {
            private int deliveries;
            private readonly List<WeakHandler> handlers;

            public WeakHandlerList()
            {
                handlers = new List<WeakHandler>();
            }

            public void AddWeakHandler(TEventSource source, TEventHandler handler)
            {
                WeakHandler handlerSink = new(source, handler);
                handlers.Add(handlerSink);
            }

            public bool RemoveWeakHandler(TEventSource source, TEventHandler handler)
            {
                foreach (var weakHandler in handlers)
                {
                    if (weakHandler.Matches(source, handler))
                    {
                        return handlers.Remove(weakHandler);
                    }
                }

                return false;
            }

            public WeakHandlerList Clone()
            {
                WeakHandlerList newList = new();
                newList.handlers.AddRange(handlers.Where(h => h.IsActive));

                return newList;
            }

            public int Count => handlers.Count;

            public bool IsDeliverActive => deliveries > 0;

            public IDisposable DeliverActive()
            {
                Interlocked.Increment(ref deliveries);
                return new ActionDisposable(() => Interlocked.Decrement(ref deliveries));
            }

            public virtual bool DeliverEvent(object sender, TEventArgs args)
            {
                bool hasStaleEntries = false;

                foreach (var handler in handlers)
                {
                    if (handler.IsActive)
                    {
                        var @delegate = handler.Handler as Delegate;
                        @delegate.DynamicInvoke(sender, args);
                    }
                    else
                    {
                        hasStaleEntries = true;
                    }
                }

                return hasStaleEntries;
            }

            public void Purge()
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    if (!handlers[i].IsActive)
                    {
                        handlers.RemoveAt(i);
                    }
                }
            }
        }

        private class ActionDisposable : IDisposable
        {
            private Action action;

            public ActionDisposable(Action action)
            {
                this.action = action;
            }

            public void Dispose()
            {
                Interlocked.Exchange(ref action, null)();
            }
        }
    }
}
