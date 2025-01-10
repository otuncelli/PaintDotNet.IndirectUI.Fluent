// Copyright 2025 Osman Tunçelli. All rights reserved.
// Use of this source code is governed by GNU General Public License (GPL-2.0) that can be found in the COPYING file.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PaintDotNet.Rendering;
using PaintDotNet.PropertySystem.Extensions;

namespace PaintDotNet.PropertySystem;

public sealed class FluentPropertyCollection : ICollection<Property>, IReadOnlyList<Property>, ICloneable
{
    #region Fields

    private readonly KeyedPropertyCollection props = new();

    private readonly List<PropertyCollectionRule> rules = [];

    #endregion

    #region Properties

    public IReadOnlyList<PropertyCollectionRule> Rules => rules;

    public IReadOnlyList<Property> Properties => props;

    public IEnumerable<PropertyName> PropertyNames => props.Select(p => (PropertyName)p.Name);

    public Property this[PropertyName propertyName] => props[propertyName];

    public int Count => props.Count;

    public bool IsReadOnly => throw new NotImplementedException();

    Property IReadOnlyList<Property>.this[int index] => GetPropertyAt(index);

    #endregion

    #region Constructors

    public FluentPropertyCollection(IEnumerable<Property>? props = null, IEnumerable<PropertyCollectionRule>? rules = null)
    {
        if (props == null) { return; }

        foreach (Property prop in props)
        {
            this.props.Add(prop.Clone());
        }

        if (rules != null)
        {
            foreach (PropertyCollectionRule rule in rules)
            {
                this.rules.Add(rule.Clone());
            }
        }
    }

    #endregion

    #region Int32Property

    /// <summary>
    /// Adds <see cref="Int32Property" />
    /// </summary>
    /// <remarks>
    /// <strong>Supported Controls</strong>: 
    /// <see cref="PropertyControlType.Slider" />, 
    /// <see cref="PropertyControlType.ColorWheel" />,
    /// <see cref="PropertyControlType.IncrementButton" />
    /// <br/>
    /// <strong>Default Control</strong>: 
    /// <see cref="PropertyControlType.Slider" />
    /// </remarks>
    public FluentPropertyCollection AddInt32(PropertyName name)
    {
        return AddInternal(new Int32Property(name));
    }

    /// <inheritdoc cref="AddInt32(PropertyName)" />
    public FluentPropertyCollection AddInt32(PropertyName name, int defaultValue, int minValue, int maxValue, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new Int32Property(name, defaultValue, minValue, maxValue, readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddInt32(PropertyName)" />
    public FluentPropertyCollection AddInt32(PropertyName name, int defaultValue, int minValue, int maxValue, bool readOnly)
    {
        return AddInternal(new Int32Property(name, defaultValue, minValue, maxValue, readOnly));
    }

    /// <inheritdoc cref="AddInt32(PropertyName)" />
    public FluentPropertyCollection AddInt32(PropertyName name, int defaultValue, int minValue, int maxValue)
    {
        return AddInternal(new Int32Property(name, defaultValue, minValue, maxValue));
    }

    /// <inheritdoc cref="AddInt32(PropertyName)" />
    public FluentPropertyCollection AddInt32(PropertyName name, int defaultValue)
    {
        return AddInternal(new Int32Property(name, defaultValue));
    }

    /// <inheritdoc cref="AddInt32(PropertyName)" />
    public FluentPropertyCollection AddInt32(PropertyName name, ColorBgra defaultValue, bool readOnly, bool alpha = false)
    {
        GetColorWheelValues(defaultValue, alpha, out int min, out int max, out int def);
        return AddInt32(name, def, min, max, readOnly);
    }

    /// <inheritdoc cref="AddInt32(PropertyName, int)" />
    public FluentPropertyCollection AddInt32(PropertyName name, ColorBgra defaultValue, bool alpha = false)
    {
        GetColorWheelValues(defaultValue, alpha, out int min, out int max, out int def);
        return AddInt32(name, def, min, max);
    }

    private static void GetColorWheelValues(ColorBgra defaultValue, bool alpha, out int min, out int max, out int def)
    {
        if (alpha)
        {
            min = int.MinValue;
            max = int.MaxValue;
            def = (int)defaultValue.Bgra;
        }
        else
        {
            min = 0;
            max = (1 << 24) - 1;
            def = ColorBgra.ToOpaqueInt32(defaultValue);
        }
    }

    #endregion

    #region DoubleProperty

    /// <summary>
    /// Adds <see cref="DoubleProperty" />
    /// </summary>
    /// <remarks>
    /// <strong>Supported Controls</strong>:
    /// <see cref="PropertyControlType.Slider" />, 
    /// <see cref="PropertyControlType.AngleChooser" />
    /// <br/>
    /// <strong>Default Control</strong>: 
    /// <see cref="PropertyControlType.Slider" />
    /// <br />
    /// <strong>Default Minimum</strong>: -32768
    /// <br />
    /// <strong>Default Maximum</strong>: 32767
    /// </remarks>
    public FluentPropertyCollection AddDouble(PropertyName name)
    {
        return AddInternal(new DoubleProperty(name));
    }

    /// <inheritdoc cref="AddDouble(PropertyName)" />
    public FluentPropertyCollection AddDouble(PropertyName name, double defaultValue, double minValue, double maxValue, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new DoubleProperty(name, defaultValue, minValue, maxValue, readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddDouble(PropertyName)" />
    public FluentPropertyCollection AddDouble(PropertyName name, double defaultValue, double minValue, double maxValue, bool readOnly)
    {
        return AddInternal(new DoubleProperty(name, defaultValue, minValue, maxValue, readOnly));
    }

    /// <inheritdoc cref="AddDouble(PropertyName)" />
    public FluentPropertyCollection AddDouble(PropertyName name, double defaultValue, double minValue, double maxValue)
    {
        return AddInternal(new DoubleProperty(name, defaultValue, minValue, maxValue));
    }

    /// <inheritdoc cref="AddDouble(PropertyName)" />
    public FluentPropertyCollection AddDouble(PropertyName name, double defaultValue)
    {
        return AddInternal(new DoubleProperty(name, defaultValue));
    }

    #endregion

    #region BooleanProperty

    /// <summary>
    /// Adds <see cref="BooleanProperty" />
    /// </summary>
    /// <remarks>
    /// <strong>Supported Controls</strong>:
    /// <see cref="PropertyControlType.CheckBox" />
    /// </remarks>
    public FluentPropertyCollection AddBoolean(PropertyName name)
    {
        return AddInternal(new BooleanProperty(name));
    }

    /// <inheritdoc cref="AddBoolean(PropertyName)" />
    public FluentPropertyCollection AddBoolean(PropertyName name, bool defaultValue, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new BooleanProperty(name, defaultValue, readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddBoolean(PropertyName)" />
    public FluentPropertyCollection AddBoolean(PropertyName name, bool defaultValue, bool readOnly)
    {
        return AddInternal(new BooleanProperty(name, defaultValue, readOnly));
    }

    /// <inheritdoc cref="AddBoolean(PropertyName)" />
    public FluentPropertyCollection AddBoolean(PropertyName name, bool defaultValue)
    {
        return AddInternal(new BooleanProperty(name, defaultValue));
    }

    #endregion

    #region DoubleVectorProperty

    private static readonly Pair<double, double> DoubleVectorDefaultMinValue = Pair.Create<double, double>(short.MinValue, short.MinValue);
    private static readonly Pair<double, double> DoubleVectorDefaultMaxValue = Pair.Create<double, double>(short.MaxValue, short.MaxValue);
    private static readonly Pair<double, double> DoubleVectorDefaultValue = Pair.Create<double, double>(0, 0);

    /// <summary>
    /// Adds <see cref="DoubleVectorProperty" />
    /// </summary>
    /// <remarks>
    /// <strong>Supported Controls</strong>:
    /// <see cref="PropertyControlType.PanAndSlider" />
    /// <see cref="PropertyControlType.Slider" />,
    /// <br/>
    /// <strong>Default Control</strong>: 
    /// <see cref="PropertyControlType.PanAndSlider" />
    /// </remarks>
    public FluentPropertyCollection AddDoubleVector(PropertyName name)
    {
        return AddInternal(new DoubleVectorProperty(name, Pair.Create(0.0, 0.0), DoubleVectorDefaultMinValue, DoubleVectorDefaultMaxValue));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, Pair<double, double> defaultValues, Pair<double, double> minValues, Pair<double, double> maxValues, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new DoubleVectorProperty(name, defaultValues, minValues, maxValues, readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, double defaultX, double defaultY, double minValueX, double minValueY, double maxValueX, double maxValueY, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new DoubleVectorProperty(name, Pair.Create(defaultX, defaultY), Pair.Create(minValueX, minValueY), Pair.Create(maxValueX, maxValueY), readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, Pair<double, double> defaultValues, Pair<double, double> minValues, Pair<double, double> maxValues, bool readOnly)
    {
        return AddInternal(new DoubleVectorProperty(name, defaultValues, minValues, maxValues, readOnly));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, double defaultX, double defaultY, double minValueX, double minValueY, double maxValueX, double maxValueY, bool readOnly)
    {
        return AddInternal(new DoubleVectorProperty(name, Pair.Create(defaultX, defaultY), Pair.Create(minValueX, minValueY), Pair.Create(maxValueX, maxValueY), readOnly));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, Pair<double, double> defaultValues, Pair<double, double> minValues, Pair<double, double> maxValues)
    {
        return AddInternal(new DoubleVectorProperty(name, defaultValues, minValues, maxValues));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, double defaultX, double defaultY, double minValueX, double minValueY, double maxValueX, double maxValueY)
    {
        return AddInternal(new DoubleVectorProperty(name, Pair.Create(defaultX, defaultY), Pair.Create(minValueX, minValueY), Pair.Create(maxValueX, maxValueY)));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, Pair<double, double> defaultValues)
    {
        return AddInternal(new DoubleVectorProperty(name, defaultValues, DoubleVectorDefaultMinValue, DoubleVectorDefaultMaxValue));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, double defaultX, double defaultY)
    {
        return AddInternal(new DoubleVectorProperty(name, Pair.Create(defaultX, defaultY), DoubleVectorDefaultMinValue, DoubleVectorDefaultMaxValue));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, Vector2Double defaultValues, Vector2Double minValues, Vector2Double maxValues, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new DoubleVectorProperty(name, defaultValues, minValues, maxValues, readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, Vector2Double defaultValues, Vector2Double minValues, Vector2Double maxValues, bool readOnly)
    {
        return AddInternal(new DoubleVectorProperty(name, defaultValues, minValues, maxValues, readOnly));
    }

    /// <inheritdoc cref="AddDoubleVector(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector(PropertyName name, Vector2Double defaultValues, Vector2Double minValues, Vector2Double maxValues)
    {
        return AddInternal(new DoubleVectorProperty(name, defaultValues, minValues, maxValues));
    }

    #endregion

    #region DoubleVector3Property

    private static readonly Tuple<double, double, double> DoubleVector3DefaultMinValue = Tuple.Create<double, double, double>(short.MinValue, short.MinValue, short.MinValue);
    private static readonly Tuple<double, double, double> DoubleVector3DefaultMaxValue = Tuple.Create<double, double, double>(short.MaxValue, short.MaxValue, short.MaxValue);
    private static readonly Tuple<double, double, double> DoubleVector3DefaultValue = Tuple.Create<double, double, double>(0, 0, 0);

    /// <summary>
    /// Adds <see cref="DoubleVector3Property" />
    /// </summary>
    /// <remarks>
    /// <strong>Supported Controls</strong>:
    /// <see cref="PropertyControlType.Slider" />,
    /// <see cref="PropertyControlType.RollBallAndSliders" />
    /// <br/>
    /// <strong>Default Control</strong>: 
    /// <see cref="PropertyControlType.Slider" />
    /// </remarks>
    public FluentPropertyCollection AddDoubleVector3(PropertyName name)
    {
        return AddInternal(new DoubleVector3Property(name, DoubleVector3DefaultValue, DoubleVector3DefaultMinValue, DoubleVector3DefaultMaxValue));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, Tuple<double, double, double> defaultValues, Tuple<double, double, double> minValues, Tuple<double, double, double> maxValues, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new DoubleVector3Property(name, defaultValues, minValues, maxValues, readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, double defaultX, double defaultY, double defaultZ, double minValueX, double minValueY, double minValueZ, double maxValueX, double maxValueY, double maxValueZ, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new DoubleVector3Property(name, Tuple.Create(defaultX, defaultY, defaultZ), Tuple.Create(minValueX, minValueY, minValueZ), Tuple.Create(maxValueX, maxValueY, maxValueZ), readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, Tuple<double, double, double> defaultValues, Tuple<double, double, double> minValues, Tuple<double, double, double> maxValues, bool readOnly)
    {
        return AddInternal(new DoubleVector3Property(name, defaultValues, minValues, maxValues, readOnly));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, double defaultX, double defaultY, double defaultZ, double minValueX, double minValueY, double minValueZ, double maxValueX, double maxValueY, double maxValueZ, bool readOnly)
    {
        return AddInternal(new DoubleVector3Property(name, Tuple.Create(defaultX, defaultY, defaultZ), Tuple.Create(minValueX, minValueY, minValueZ), Tuple.Create(maxValueX, maxValueY, maxValueZ), readOnly));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, Tuple<double, double, double> defaultValues, Tuple<double, double, double> minValues, Tuple<double, double, double> maxValues)
    {
        return AddInternal(new DoubleVector3Property(name, defaultValues, minValues, maxValues));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, double defaultX, double defaultY, double defaultZ, double minValueX, double minValueY, double minValueZ, double maxValueX, double maxValueY, double maxValueZ)
    {
        return AddInternal(new DoubleVector3Property(name, Tuple.Create(defaultX, defaultY, defaultZ), Tuple.Create(minValueX, minValueY, minValueZ), Tuple.Create(maxValueX, maxValueY, maxValueZ)));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, Tuple<double, double, double> defaultValues)
    {
        return AddInternal(new DoubleVector3Property(name, defaultValues, DoubleVector3DefaultMinValue, DoubleVector3DefaultMaxValue));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, double defaultX, double defaultY, double defaultZ)
    {
        return AddInternal(new DoubleVector3Property(name, Tuple.Create(defaultX, defaultY, defaultZ), DoubleVector3DefaultMinValue, DoubleVector3DefaultMaxValue));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, Vector3Double defaultValues, Vector3Double minValues, Vector3Double maxValues, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new DoubleVector3Property(name, defaultValues, minValues, maxValues, readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, Vector3Double defaultValues, Vector3Double minValues, Vector3Double maxValues, bool readOnly)
    {
        return AddInternal(new DoubleVector3Property(name, defaultValues, minValues, maxValues, readOnly));
    }

    /// <inheritdoc cref="AddDoubleVector3(PropertyName)" />
    public FluentPropertyCollection AddDoubleVector3(PropertyName name, Vector3Double defaultValues, Vector3Double minValues, Vector3Double maxValues)
    {
        return AddInternal(new DoubleVector3Property(name, defaultValues, minValues, maxValues));
    }

    #endregion

    #region StringProperty

    /// <summary>
    /// Adds <see cref="StringProperty" />
    /// </summary>
    /// <remarks>
    /// <strong>Supported Controls</strong>:
    /// <see cref="PropertyControlType.TextBox" />
    /// <see cref="PropertyControlType.FileChooser"/>
    /// <br/>
    /// <strong>Default Control</strong>: 
    /// <see cref="PropertyControlType.TextBox" />
    /// </remarks>
    public FluentPropertyCollection AddString(PropertyName name)
    {
        return AddInternal(new StringProperty(name));
    }

    /// <inheritdoc cref="AddString(PropertyName)" />
    public FluentPropertyCollection AddString(PropertyName name, string defaultValue, int maxLength, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new StringProperty(name, defaultValue, maxLength, readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddString(PropertyName)" />
    public FluentPropertyCollection AddString(PropertyName name, string defaultValue, int maxLength, bool readOnly)
    {
        return AddInternal(new StringProperty(name, defaultValue, maxLength, readOnly));
    }

    /// <inheritdoc cref="AddString(PropertyName)" />
    public FluentPropertyCollection AddString(PropertyName name, string defaultValue, int maxLength)
    {
        return AddInternal(new StringProperty(name, defaultValue, maxLength));
    }

    /// <inheritdoc cref="AddString(PropertyName)" />
    public FluentPropertyCollection AddString(PropertyName name, string defaultValue)
    {
        return AddInternal(new StringProperty(name, defaultValue));
    }

    #endregion

    #region UriProperty

    /// <summary>
    /// Adds <see cref="UriProperty" />
    /// </summary>
    /// <remarks>
    /// <strong>Supported Controls</strong>:
    /// <see cref="PropertyControlType.LinkLabel" />
    /// </remarks>
    public FluentPropertyCollection AddUri(PropertyName name)
    {
        return AddInternal(new UriProperty(name));
    }

    /// <inheritdoc cref="AddUri(PropertyName)" />
    public FluentPropertyCollection AddUri(PropertyName name, Uri defaultValue, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new UriProperty(name, defaultValue, readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddUri(PropertyName)" />
    public FluentPropertyCollection AddUri(PropertyName name, Uri defaultValue, bool readOnly)
    {
        return AddInternal(new UriProperty(name, defaultValue, readOnly));
    }

    /// <inheritdoc cref="AddUri(PropertyName)" />
    public FluentPropertyCollection AddUri(PropertyName name, Uri defaultValue)
    {
        return AddInternal(new UriProperty(name, defaultValue));
    }

    #endregion

    #region StaticListChoiceProperty

    /// <summary>
    /// Adds <see cref="StaticListChoiceProperty" />
    /// </summary>
    /// <remarks>
    /// <strong>Supported Controls</strong>:
    /// <see cref="PropertyControlType.DropDown" />
    /// <see cref="PropertyControlType.RadioButton" />,
    /// <br/>
    /// <strong>Default Control</strong>: 
    /// <see cref="PropertyControlType.DropDown" />
    /// </remarks>
    public FluentPropertyCollection AddStaticListChoice(PropertyName name, IEnumerable valueChoices)
    {
        return AddInternal(new StaticListChoiceProperty(name, valueChoices.Cast<object>().ToArray()));
    }

    /// <inheritdoc cref="AddStaticListChoice(PropertyName, IEnumerable)" />
    public FluentPropertyCollection AddStaticListChoice(PropertyName name, IEnumerable valueChoices, int defaultChoiceIndex, bool readOnly, ValueValidationFailureResult vvfResult)
    {
        return AddInternal(new StaticListChoiceProperty(name, valueChoices.Cast<object>().ToArray(), defaultChoiceIndex, readOnly, vvfResult));
    }

    /// <inheritdoc cref="AddStaticListChoice(PropertyName, IEnumerable)" />
    public FluentPropertyCollection AddStaticListChoice(PropertyName name, IEnumerable valueChoices, int defaultChoiceIndex, bool readOnly)
    {
        return AddInternal(new StaticListChoiceProperty(name, valueChoices.Cast<object>().ToArray(), defaultChoiceIndex, readOnly));
    }

    /// <inheritdoc cref="AddStaticListChoice(PropertyName, IEnumerable)" />
    public FluentPropertyCollection AddStaticListChoice(PropertyName name, IEnumerable valueChoices, int defaultChoiceIndex)
    {
        return AddInternal(new StaticListChoiceProperty(name, valueChoices.Cast<object>().ToArray(), defaultChoiceIndex));
    }

    /// <inheritdoc cref="AddStaticListChoice(PropertyName, IEnumerable)" />
    public FluentPropertyCollection AddStaticListChoice<TEnum>(PropertyName name, TEnum defaultValue, IComparer? comparer = null) where TEnum : Enum
    {
        return AddStaticListChoice(name, defaultValue, readOnly: false, comparer: comparer);
    }

    /// <inheritdoc cref="AddStaticListChoice(PropertyName, IEnumerable)" />
    public FluentPropertyCollection AddStaticListChoice<TEnum>(PropertyName name, TEnum defaultValue, bool readOnly, IComparer? comparer = null) where TEnum : Enum
    {
        return AddStaticListChoice(typeof(TEnum), name, defaultValue, readOnly, comparer: comparer);
    }

    /// <inheritdoc cref="AddStaticListChoice(PropertyName, IEnumerable)" />
    public FluentPropertyCollection AddStaticListChoice(Type enumType, PropertyName name, object defaultValue, IComparer? comparer = null)
    {
        return AddStaticListChoice(enumType, name, defaultValue, readOnly: false, comparer: comparer);
    }

    /// <inheritdoc cref="AddStaticListChoice(PropertyName, IEnumerable)" />
    public FluentPropertyCollection AddStaticListChoice(Type enumType, PropertyName name, object defaultValue, bool readOnly, IComparer? comparer = null)
    {
        if (enumType.GetCustomAttributes(typeof(FlagsAttribute), inherit: true).Length != 0)
        {
            throw new ArgumentException($"Enums with `{nameof(FlagsAttribute)}` are not supported");
        }

        if (!Enum.IsDefined(enumType, defaultValue))
        {
            throw new ArgumentOutOfRangeException($"{nameof(defaultValue)} `{defaultValue}` is not a valid enum value for `{enumType.FullName}`");
        }

        Array values = Enum.GetValues(enumType);

        if (comparer != null)
        {
            Array.Sort(values, comparer);
        }

        int defaultValueIndex = Array.IndexOf(values, defaultValue);

        object[] array = new object[values.Length];
        values.CopyTo(array, 0);

        Property property = new StaticListChoiceProperty(name, array, defaultValueIndex, readOnly);
        return AddInternal(property);
    }

    #endregion

    #region WithRule

    public FluentPropertyCollection WithRule(PropertyCollectionRule rule)
    {
        rules.Add(rule.Clone());
        return this;
    }

    #endregion

    #region WithReadOnlyRule/ReadOnlyBoundToBooleanRule

    public FluentPropertyCollection WithReadOnlyRule(PropertyName targetPropertyName, PropertyName sourceBooleanPropertyName, bool inverse = false)
    {
        ReadOnlyBoundToBooleanRule rule = new(targetPropertyName, sourceBooleanPropertyName, inverse);
        return WithRule(rule);
    }

    public FluentPropertyCollection WithReadOnlyRule(Property targetProperty, BooleanProperty sourceProperty, bool inverse = false)
    {
        ReadOnlyBoundToBooleanRule rule = new(targetProperty, sourceProperty, inverse);
        return WithRule(rule);
    }

    #region Multi-target

    public FluentPropertyCollection WithReadOnlyRule(PropertyName[] targetPropertyNames, PropertyName sourceBooleanPropertyName, bool inverse = false)
    {
        Array.ForEach(targetPropertyNames, tpn => WithReadOnlyRule(tpn, sourceBooleanPropertyName, inverse));
        return this;
    }

    public FluentPropertyCollection WithReadOnlyRule(Property[] targetProperties, BooleanProperty sourceProperty, bool inverse = false)
    {
        Array.ForEach(targetProperties, tp => WithReadOnlyRule(tp, sourceProperty, inverse));
        return this;
    }

    #endregion

    #endregion

    #region WithReadOnlyRule/ReadOnlyBoundToValueRule

    public FluentPropertyCollection WithReadOnlyRule<TValue, TSourceProperty>(PropertyName targetPropertyName, PropertyName sourcePropertyName, TValue[] valuesForReadOnly, bool inverse = false) where TSourceProperty : Property<TValue>
    {
        ReadOnlyBoundToValueRule<TValue, TSourceProperty> rule = new(targetPropertyName, sourcePropertyName, valuesForReadOnly, inverse);
        return WithRule(rule);
    }

    public FluentPropertyCollection WithReadOnlyRule(PropertyName targetPropertyName, PropertyName sourcePropertyName, object valueForReadOnly, bool inverse = false)
    {
        ArgumentNullException.ThrowIfNull(valueForReadOnly);

        Property sourceProperty = props[sourcePropertyName];
        Type genericType = typeof(ReadOnlyBoundToValueRule<,>).MakeGenericType(sourceProperty.ValueType, sourceProperty.GetType());
        PropertyCollectionRule? rule = (PropertyCollectionRule?)Activator.CreateInstance(genericType, targetPropertyName, sourcePropertyName, valueForReadOnly, inverse);
        return rule is null
            ? throw new InvalidOperationException($"Cannot create an instance of `{nameof(PropertyCollectionRule)}`")
            : WithRule(rule);
    }

    public FluentPropertyCollection WithReadOnlyRule(PropertyName targetPropertyName, PropertyName sourcePropertyName, IEnumerable valuesForReadOnly, bool inverse = false)
    {
        ArgumentNullException.ThrowIfNull(valuesForReadOnly);
        object[] array = valuesForReadOnly.Cast<object>().ToArray();
        if (array.Length == 0)
        {
            throw new ArgumentException("Collection is empty.", nameof(valuesForReadOnly));
        }

        Property sourceProperty = props[sourcePropertyName];
        Type genericType = typeof(ReadOnlyBoundToValueRule<,>).MakeGenericType(sourceProperty.ValueType, sourceProperty.GetType());
        PropertyCollectionRule? rule = (PropertyCollectionRule?)Activator.CreateInstance(genericType, targetPropertyName, sourcePropertyName, array, inverse);
        return rule is null
            ? throw new InvalidOperationException($"Cannot create an instance of `{nameof(PropertyCollectionRule)}`")
            : WithRule(rule);
    }

    public FluentPropertyCollection WithReadOnlyRule<TValue, TSourceProperty>(Property targetProperty, TSourceProperty sourceProperty, TValue valueForReadOnly, bool inverse = false) where TSourceProperty : Property<TValue>
    {
        ReadOnlyBoundToValueRule<TValue, TSourceProperty> rule = new(targetProperty, sourceProperty, valueForReadOnly, inverse);
        return WithRule(rule);
    }

    public FluentPropertyCollection WithReadOnlyRule<TValue, TSourceProperty>(Property targetProperty, TSourceProperty sourceProperty, TValue[] valuesForReadOnly, bool inverse = false) where TSourceProperty : Property<TValue>
    {
        ReadOnlyBoundToValueRule<TValue, TSourceProperty> rule = new(targetProperty, sourceProperty, valuesForReadOnly, inverse);
        return WithRule(rule);
    }

    #region Multi-target

    public FluentPropertyCollection WithReadOnlyRule<TValue, TSourceProperty>(PropertyName[] targetPropertyNames, PropertyName sourcePropertyName, TValue[] valuesForReadOnly, bool inverse = false) where TSourceProperty : Property<TValue>
    {
        Array.ForEach(targetPropertyNames, tpn => WithReadOnlyRule(tpn, sourcePropertyName, valuesForReadOnly, inverse));
        return this;
    }

    public FluentPropertyCollection WithReadOnlyRule(PropertyName[] targetPropertyNames, PropertyName sourcePropertyName, object valueForReadOnly, bool inverse = false)
    {
        Array.ForEach(targetPropertyNames, tpn => WithReadOnlyRule(tpn, sourcePropertyName, valueForReadOnly, inverse));
        return this;
    }

    public FluentPropertyCollection WithReadOnlyRule(PropertyName[] targetPropertyNames, PropertyName sourcePropertyName, IEnumerable valuesForReadOnly, bool inverse = false)
    {
        Array.ForEach(targetPropertyNames, tpn => WithReadOnlyRule(tpn, sourcePropertyName, valuesForReadOnly, inverse));
        return this;
    }

    public FluentPropertyCollection WithReadOnlyRule<TValue, TSourceProperty>(Property[] targetProperties, TSourceProperty sourceProperty, TValue valueForReadOnly, bool inverse = false) where TSourceProperty : Property<TValue>
    {
        Array.ForEach(targetProperties, tp => WithReadOnlyRule(tp, sourceProperty, valueForReadOnly, inverse));
        return this;
    }

    public FluentPropertyCollection WithReadOnlyRule<TValue, TSourceProperty>(Property[] targetProperties, TSourceProperty sourceProperty, TValue[] valuesForReadOnly, bool inverse = false) where TSourceProperty : Property<TValue>
    {
        Array.ForEach(targetProperties, tp => WithReadOnlyRule(tp, sourceProperty, valuesForReadOnly, inverse));
        return this;
    }

    #endregion

    #endregion

    #region WithReadOnlyRule/ReadOnlyBoundToNameValuesRule

    public FluentPropertyCollection WithReadOnlyRule(Property targetProperty, ValueTuple<object, object?>[] sourcePropertyNameValuePairs, bool inverse = false)
    {
        ReadOnlyBoundToNameValuesRule rule = new(targetProperty, inverse, sourcePropertyNameValuePairs);
        return WithRule(rule);
    }

    public FluentPropertyCollection WithReadOnlyRule(Property targetProperty, IDictionary sourcePropertyNameValuePairs, bool inverse = false)
    {
        ReadOnlyBoundToNameValuesRule rule = new(targetProperty, inverse, FromDictionary(sourcePropertyNameValuePairs));
        return WithRule(rule);
    }

    public FluentPropertyCollection WithReadOnlyRule(PropertyName targetPropertyName, ValueTuple<object, object?>[] sourcePropertyNameValuePairs, bool inverse = false)
    {
        ReadOnlyBoundToNameValuesRule rule = new(targetPropertyName, inverse, sourcePropertyNameValuePairs);
        return WithRule(rule);
    }

    public FluentPropertyCollection WithReadOnlyRule(PropertyName targetPropertyName, IDictionary sourcePropertyNameValuePairs, bool inverse = false)
    {
        ReadOnlyBoundToNameValuesRule rule = new(targetPropertyName, inverse, FromDictionary(sourcePropertyNameValuePairs));
        return WithRule(rule);
    }

    #region Multi-target

    public FluentPropertyCollection WithReadOnlyRule(Property[] targetProperties, ValueTuple<object, object?>[] sourcePropertyNameValuePairs, bool inverse = false)
    {
        Array.ForEach(targetProperties, tp => WithReadOnlyRule(tp, sourcePropertyNameValuePairs, inverse));
        return this;
    }

    public FluentPropertyCollection WithReadOnlyRule(Property[] targetProperties, IDictionary sourcePropertyNameValuePairs, bool inverse = false)
    {
        Array.ForEach(targetProperties, tp => WithReadOnlyRule(tp, sourcePropertyNameValuePairs, inverse));
        return this;
    }

    public FluentPropertyCollection WithReadOnlyRule(PropertyName[] targetPropertyNames, ValueTuple<object, object?>[] sourcePropertyNameValuePairs, bool inverse = false)
    {
        Array.ForEach(targetPropertyNames, tpn => WithReadOnlyRule(tpn, sourcePropertyNameValuePairs, inverse));
        return this;
    }

    public FluentPropertyCollection WithReadOnlyRule(PropertyName[] targetPropertyNames, IDictionary sourcePropertyNameValuePairs, bool inverse = false)
    {
        Array.ForEach(targetPropertyNames, tpn => WithReadOnlyRule(tpn, sourcePropertyNameValuePairs, inverse));
        return this;
    }

    #endregion

    private static ValueTuple<object, object?>[] FromDictionary(IDictionary rules)
    {
        List<ValueTuple<object, object?>> result = new(rules.Count);
        foreach (DictionaryEntry pair in rules)
        {
            if (pair.Value is IEnumerable iterable)
            {
                foreach (object value in iterable)
                {
                    result.Add(ValueTuple.Create(pair.Key, value));
                }
            }
            else
            {
                result.Add(ValueTuple.Create(pair.Key, pair.Value));
            }
        }
        return [.. result];
    }

    #endregion

    #region LinkValuesBasedOnBoolean

    public FluentPropertyCollection LinkValuesBasedOnBooleanRule<TValue, TProperty>(IEnumerable<TProperty> targetProperties, BooleanProperty sourceProperty, bool inverse = false)
        where TValue : struct, IComparable<TValue>
        where TProperty : ScalarProperty<TValue>
    {
        LinkValuesBasedOnBooleanRule<TValue, TProperty> rule = new(targetProperties, sourceProperty, inverse);
        return WithRule(rule);
    }

    public FluentPropertyCollection LinkValuesBasedOnBooleanRule<TValue, TProperty>(PropertyName[] targetPropertyNames, PropertyName sourceBooleanPropertyName, bool inverse = false)
        where TValue : struct, IComparable<TValue>
        where TProperty : ScalarProperty<TValue>
    {
        LinkValuesBasedOnBooleanRule<TValue, TProperty> rule = new(targetPropertyNames, sourceBooleanPropertyName, inverse);
        return WithRule(rule);
    }

    public FluentPropertyCollection LinkValuesBasedOnBooleanRule(Property[] targetProperties, BooleanProperty sourceProperty, bool inverse = false)
    {
        ArgumentNullException.ThrowIfNull(targetProperties);
        if (targetProperties.Length < 2)
        {
            throw new ArgumentException("Collection must have at least 2 items.", nameof(targetProperties));
        }

        Property targetProperty = targetProperties[0];
        Type genericType = typeof(LinkValuesBasedOnBooleanRule<,>).MakeGenericType(targetProperty.ValueType, targetProperty.GetType());
        PropertyCollectionRule? rule = (PropertyCollectionRule?)Activator.CreateInstance(genericType, targetProperties, sourceProperty, inverse);
        return rule is null
            ? throw new InvalidOperationException($"Cannot create an instance of `{nameof(PropertyCollectionRule)}`")
            : WithRule(rule);
    }

    public FluentPropertyCollection LinkValuesBasedOnBooleanRule(PropertyName[] targetPropertyNames, PropertyName sourceBooleanPropertyName, bool inverse = false)
    {
        ArgumentNullException.ThrowIfNull(targetPropertyNames);
        if (targetPropertyNames.Length < 2)
        {
            throw new ArgumentException("Collection must have at least 2 items.", nameof(targetPropertyNames));
        }

        Property targetProperty = props[targetPropertyNames[0]];
        Type genericType = typeof(LinkValuesBasedOnBooleanRule<,>).MakeGenericType(targetProperty.ValueType, targetProperty.GetType());
        PropertyCollectionRule? rule = (PropertyCollectionRule?)Activator.CreateInstance(genericType, targetPropertyNames, sourceBooleanPropertyName, inverse);
        return rule is null
            ? throw new InvalidOperationException($"Cannot create an instance of `{nameof(PropertyCollectionRule)}`")
            : WithRule(rule);
    }

    #endregion

    #region SoftMutuallyBoundMinMaxRule

    public FluentPropertyCollection SoftMutuallyBoundMinMaxRule<TValue, TProperty>(ScalarProperty<TValue> minProperty, ScalarProperty<TValue> maxProperty)
        where TValue : struct, IComparable<TValue>
        where TProperty : ScalarProperty<TValue>
    {
        SoftMutuallyBoundMinMaxRule<TValue, TProperty> rule = new(minProperty, maxProperty);
        return WithRule(rule);
    }

    public FluentPropertyCollection SoftMutuallyBoundMinMaxRule<TValue, TProperty>(PropertyName minPropertyName, PropertyName maxPropertyName)
        where TValue : struct, IComparable<TValue>
        where TProperty : ScalarProperty<TValue>
    {
        SoftMutuallyBoundMinMaxRule<TValue, TProperty> rule = new(minPropertyName, maxPropertyName);
        return WithRule(rule);
    }

    public FluentPropertyCollection SoftMutuallyBoundMinMaxRule<TValue>(ScalarProperty<TValue> minProperty, ScalarProperty<TValue> maxProperty)
        where TValue : struct, IComparable<TValue>
    {
        Type genericType = typeof(SoftMutuallyBoundMinMaxRule<,>).MakeGenericType(minProperty.ValueType, minProperty.GetType());
        PropertyCollectionRule? rule = (PropertyCollectionRule?)Activator.CreateInstance(genericType, minProperty, maxProperty);
        return rule is null
            ? throw new InvalidOperationException($"Cannot create an instance of `{nameof(PropertyCollectionRule)}`")
            : WithRule(rule);
    }

    public FluentPropertyCollection SoftMutuallyBoundMinMaxRule(PropertyName minPropertyName, PropertyName maxPropertyName)
    {
        Property minProperty = props[minPropertyName];
        Type genericType = typeof(SoftMutuallyBoundMinMaxRule<,>).MakeGenericType(minProperty.ValueType, minProperty.GetType());
        PropertyCollectionRule? rule = (PropertyCollectionRule?)Activator.CreateInstance(genericType, minPropertyName, maxPropertyName);
        return rule is null
            ? throw new InvalidOperationException($"Cannot create an instance of `{nameof(PropertyCollectionRule)}`")
            : WithRule(rule);
    }

    #endregion

    #region Methods

    private FluentPropertyCollection AddInternal(Property property)
    {
        props.Add(property);
        return this;
    }

    public FluentPropertyCollection Add(Property property)
    {
        return AddInternal(property.Clone());
    }

    public Property GetPropertyAt(int index)
    {
        return ((Collection<Property>)props)[index];
    }

    public object? GetPropertyValue(PropertyName propertyName)
    {
        return this[propertyName].Value;
    }

    public T GetPropertyValue<T>(PropertyName propertyName) where T : unmanaged
    {
        return this[propertyName].GetValue<T>();
    }

    public Dictionary<PropertyName, object?> ToDictionary()
    {
        return props.ToDictionary(p => (PropertyName)p.Name, p => p.Value, EqualityComparer<PropertyName>.Default);
    }

    public Pair<string, object?>[] ToPairs()
    {
        return props.Select(p => Pair.Create(p.Name, p.Value)).ToArray();
    }

    #endregion

    #region Implicit operators

    public static implicit operator PropertyCollection(FluentPropertyCollection list)
    {
        return new(list, list.Rules);
    }

    public static implicit operator FluentPropertyCollection(PropertyCollection collection)
    {
        return new(collection, collection.Rules);
    }

    #endregion

    #region ICloneable

    public object Clone()
    {
        return new FluentPropertyCollection(this, Rules);
    }

    #endregion

    #region IEnumerable

    public IEnumerator<Property> GetEnumerator()
    {
        return props.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region ICollection

    void ICollection<Property>.Add(Property item)
    {
        Add(item);
    }

    public void Clear()
    {
        rules.Clear();
        props.Clear();
    }

    public bool Contains(Property item)
    {
        return props.Contains(item);
    }

    public void CopyTo(Property[] array, int arrayIndex)
    {
        props.CopyTo(array, arrayIndex);
    }

    public bool Remove(Property item)
    {
        return props.Remove(item);
    }

    #endregion

    #region KeyedPropertyCollection

    private sealed class KeyedPropertyCollection : KeyedCollection<PropertyName, Property>
    {
        protected override PropertyName GetKeyForItem(Property item)
        {
            return (PropertyName)item.Name;
        }
    }

    #endregion
}