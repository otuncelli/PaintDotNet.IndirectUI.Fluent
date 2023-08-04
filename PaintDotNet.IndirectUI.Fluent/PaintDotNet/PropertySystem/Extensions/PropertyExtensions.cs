using PaintDotNet.Rendering;
using System;
using System.Threading;

namespace PaintDotNet.PropertySystem.Extensions;

public static class PropertyExtensions
{
    public static T GetValue<T>(this Property property)
    {
        Type type = typeof(T);
        object o = property.Value;
        switch (o)
        {
            case int val when type == typeof(ColorBgra):
                return (T)(object)ColorBgra.FromUInt32((uint)val);
            case Tuple<double, double, double> val when type == typeof(Vector3Double):
                return (T)(object)new Vector3Double(val.Item1, val.Item2, val.Item3);
            case Pair<double, double> val when type == typeof(Vector2Double):
                return (T)(object)new Vector2Double(val.First, val.Second);
            case IConvertible val:
                return (T)Convert.ChangeType(val, typeof(T));
            default:
                throw new ArgumentException($"Can't convert from '{o.GetType()}' to '{type}'.", nameof(T));
        }
    }

    public static IDisposable UseAsWritable(this Property property)
    {
        if (!property.ReadOnly)
        {
            return Disposable.NoOp;
        }
        property.ReadOnly = false;
        return Disposable.FromAction(() => property.ReadOnly = true);
    }

    private static class Disposable
    {
        public static readonly IDisposable NoOp = new NoOpDisposable();

        public static IDisposable FromAction(Action onDispose) => new ActionDisposable(onDispose);

        private sealed class ActionDisposable : IDisposable
        {
            private Action action;

            public ActionDisposable(Action action)
            {
                if (this.action == null)
                {
                    throw new ArgumentNullException(nameof(action));
                }
                this.action = action;
            }

            public void Dispose()
            {
                Interlocked.Exchange(ref action, null)();
            }
        }

        private sealed class NoOpDisposable : IDisposable
        {
            public NoOpDisposable()
            {
                GC.SuppressFinalize(this);
            }

            public void Dispose()
            {
            }
        }
    }
}