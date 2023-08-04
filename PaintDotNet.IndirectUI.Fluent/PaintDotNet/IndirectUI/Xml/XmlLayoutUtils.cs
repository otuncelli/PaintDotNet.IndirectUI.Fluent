// Copyright 2023 Osman Tunçelli. All rights reserved.
// Use of this source code is governed by GNU General Public License (GPL-2.0) that can be found in the COPYING file.

using PaintDotNet.IndirectUI.Extensions;
using PaintDotNet.IndirectUI.Xml.Schema;
using PaintDotNet.PropertySystem;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace PaintDotNet.IndirectUI.Xml
{
    public static class XmlLayoutHelper
    {
        #region Stage #1 - LoadXml

        private static Container LoadXml(string xml)
        {
            XmlSerializer serializer = new(typeof(Container));
            using (var reader = new StringReader(xml))
            {
                return (Container)serializer.Deserialize(reader);
            }
        }

        #endregion

        #region Stage #2 - Add Properties

        private static void AddProperties(Container container, FluentPropertyCollection properties)
        {
            ValueValidationFailureResult DefaultValueValidationFailureResult = Property.DefaultValueValidationFailureResult;

            foreach (Panel panel in container.Panel)
            {
                foreach (object item in panel.Controls.Items)
                {
                    switch (item)
                    {
                        case IntSlider a:
                            a.Value = a.ValueSpecified ? a.Value : ClampUtil.Clamp(a.Value, a.MinValue, a.MaxValue);
                            properties.AddInt32(a.Name, a.Value, a.MinValue, a.MaxValue, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case ColorWheel a:
                            ColorBgra color = ColorBgra.ParseHexString(a.Value.Substring(1));
                            properties.AddInt32(a.Name, color, a.ReadOnly, a.Alpha);
                            break;
                        case IncrementButton a:
                            a.Value = a.ValueSpecified ? a.Value : ClampUtil.Clamp(a.Value, a.MinValue, a.MaxValue);
                            properties.AddInt32(a.Name, a.Value, a.MinValue, a.MaxValue, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case DoubleSlider a:
                            a.Value = a.ValueSpecified ? a.Value : ClampUtil.Clamp(a.Value, a.MinValue, a.MaxValue);
                            properties.AddDouble(a.Name, a.Value, a.MinValue, a.MaxValue, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case AngleChooser a:
                            a.Value = a.ValueSpecified ? a.Value : ClampUtil.Clamp(a.Value, a.MinValue, a.MaxValue);
                            properties.AddDouble(a.Name, a.Value, a.MinValue, a.MaxValue, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case CheckBox a:
                            properties.AddBoolean(a.Name, a.Value, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case DoubleVectorSlider a:
                            a.xValue = a.xValueSpecified ? a.xValue : ClampUtil.Clamp(a.xValue, a.xMinValue, a.xMaxValue);
                            a.yValue = a.yValueSpecified ? a.yValue : ClampUtil.Clamp(a.yValue, a.yMinValue, a.yMaxValue);
                            properties.AddDoubleVector(a.Name, 
                                Pair.Create(a.xValue, a.yValue),
                                Pair.Create(a.xMinValue, a.yMinValue), 
                                Pair.Create(a.xMaxValue, a.yMaxValue),
                                a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case DoubleVectorPanAndSlider a:
                            a.xValue = a.xValueSpecified ? a.xValue : ClampUtil.Clamp(a.xValue, a.xMinValue, a.xMaxValue);
                            a.yValue = a.yValueSpecified ? a.yValue : ClampUtil.Clamp(a.yValue, a.yMinValue, a.yMaxValue);
                            properties.AddDoubleVector(a.Name, 
                                Pair.Create(a.xValue, a.yValue),
                                Pair.Create(a.xMinValue, a.yMinValue), 
                                Pair.Create(a.xMaxValue, a.yMaxValue),
                                a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case DoubleVector3Slider a:
                            a.xValue = a.xValueSpecified ? a.xValue : ClampUtil.Clamp(a.xValue, a.xMinValue, a.xMaxValue);
                            a.yValue = a.yValueSpecified ? a.yValue : ClampUtil.Clamp(a.yValue, a.yMinValue, a.yMaxValue);
                            a.zValue = a.zValueSpecified ? a.zValue : ClampUtil.Clamp(a.zValue, a.zMinValue, a.zMaxValue);
                            properties.AddDoubleVector3(a.Name, 
                                Tuple.Create(a.xValue, a.yValue, a.zValue),
                                Tuple.Create(a.xMinValue, a.yMinValue, a.zMinValue), 
                                Tuple.Create(a.xMaxValue, a.yMaxValue, a.zMaxValue),
                                a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case DoubleVector3RollBallAndSlider a:
                            a.xValue = a.xValueSpecified ? a.xValue : ClampUtil.Clamp(a.xValue, a.xMinValue, a.xMaxValue);
                            a.yValue = a.yValueSpecified ? a.yValue : ClampUtil.Clamp(a.yValue, a.yMinValue, a.yMaxValue);
                            a.zValue = a.zValueSpecified ? a.zValue : ClampUtil.Clamp(a.zValue, a.zMinValue, a.zMaxValue);
                            properties.AddDoubleVector3(a.Name, 
                                Tuple.Create(a.xValue, a.yValue, a.zValue),
                                Tuple.Create(a.xMinValue, a.yMinValue, a.zMinValue),
                                Tuple.Create(a.xMaxValue, a.yMaxValue, a.zMaxValue),
                                a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case TextBox a:
                            properties.AddString(a.Name, a.Value, a.MaxLength, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case FileChooser a:
                            properties.AddString(a.Name, a.Path, StringProperty.MaxMaxLength, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case FolderChooser a:
                            properties.AddString(a.Name, a.Path, StringProperty.MaxMaxLength, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case LinkLabel a:
                            Uri uri = new(a.Value);
                            properties.AddUri(a.Name, uri, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case DropDown a:
                            properties.AddStaticListChoice(a.Name, a.Items1, 0, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case RadioButton a:
                            properties.AddStaticListChoice(a.Name, a.Items1, 0, a.ReadOnly, a.OnValidationFailure.ConvertTo(DefaultValueValidationFailureResult));
                            break;
                        case Container c:
                            AddProperties(c, properties);
                            break;
                        default:
                            throw new NotSupportedException($"{item.GetType()} isn't a supported control type.");
                    }
                }
            }
        }

        #endregion

        #region Stage #3 - Add ReadOnly Rules

        private static void AddRules(Container container, FluentPropertyCollection properties)
        {
            if (container?.Panel == null) { return; }

            foreach (Panel panel in container.Panel)
            {
                if (panel.Controls?.Items == null) { continue; }

                foreach (object item in panel.Controls.Items)
                {
                    if (item is BaseControl bc)
                    {
                        if (bc.Items == null) { continue; }

                        foreach (object rule in bc.Items)
                        {
                            switch (rule)
                            {
                                case ReadOnlyBoundToBoolean a:
                                    properties.WithReadOnlyRule(bc.Name, a.SourceControl, inverse: a.Inverse);
                                    break;
                                case ReadOnlyBoundToValues a:
                                    object[] values = ParseCommaSeparatedValues(properties[bc.Name].ValueType, a.CommaSeparatedValues);
                                    properties.WithReadOnlyRule(bc.Name, a.SourceControl, values, inverse: a.Inverse);
                                    break;
                                case ReadOnlyBoundToNameValues a:
                                    IDictionary dict = a.Pair.ToDictionary(p => p.SourceControl, p => ParseCommaSeparatedValues(properties[bc.Name].ValueType, p.Value)[0]);
                                    properties.WithReadOnlyRule(bc.Name, dict);
                                    break;
                            }
                        }
                    }
                    else if (item is Container c)
                    {
                        AddRules(c, properties);
                    }
                }
            }
        }

        #endregion

        #region Stage #4 - Build Controls

        private static ControlInfo BuildControls(Container container, PropertyControlInfoCollection pcic)
        {
            const SliderControlStyle DefaultSliderControlStyle = SliderControlStyle.Default;

            static ColorBgra[] ConvertColors(string[] hexStrings)
            {
                if (hexStrings == null) { return null; }
                return hexStrings.Select(s => ColorBgra.ParseHexString(s.Substring(1))).ToArray();
            }

            ControlInfo root = container.Panel.Length > 1
                ? new TabContainerControlInfo()
                : null;
            foreach (Panel panel in container.Panel)
            {
                ControlInfo child = root is TabContainerControlInfo
                    ? new TabPageControlInfo { Text = panel.Text, ToolTipText = panel.ToolTipText }
                    : new PanelControlInfo();
                foreach (object item in panel.Controls.Items)
                {
                    switch (item)
                    {
                        case IntSlider a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.Slider)
                                    .ControlColors(ConvertColors(a.ControlColors?.Color))
                                    .ControlStyle(a.ControlStyle.ConvertTo(DefaultSliderControlStyle))
                                    .RangeWraps(a.RangeWraps)
                                    .ShowResetButton(a.ShowResetButton)
                                    .SliderLargeChange(a.SliderLargeChange)
                                    .SliderShowTickMarks(a.SliderShowTickMarks)
                                    .SliderSmallChange(a.SliderSmallChange)
                                    .UpDownIncrement(a.UpDownIncrement));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case ColorWheel a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.ColorWheel)
                                    .ShowResetButton(a.ShowResetButton));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case IncrementButton a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.IncrementButton)
                                    .ButtonText(a.ButtonText ?? "+"));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case DoubleSlider a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.Slider)
                                    .ControlColors(ConvertColors(a.ControlColors?.Color))
                                    .ControlStyle(a.ControlStyle.ConvertTo(DefaultSliderControlStyle))
                                    .DecimalPlaces(a.DecimalPlaces)
                                    .RangeWraps(a.RangeWraps)
                                    .ShowResetButton(a.ShowResetButton)
                                    .SliderLargeChange(a.SliderLargeChange)
                                    .SliderSmallChange(a.SliderSmallChange)
                                    .UpDownIncrement(a.UpDownIncrement)
                                    .UseExponentialScale(a.UseExponentialScale));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case AngleChooser a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.AngleChooser)
                                    .ShowResetButton(a.ShowResetButton)
                                    .UpDownIncrement(a.UpDownIncrement));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case CheckBox a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .Footnote(a.Footnote));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case DoubleVectorSlider a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.Slider)
                                    .DecimalPlaces(a.DecimalPlaces)
                                    .ShowResetButton(a.ShowResetButton)
                                    .SliderLargeChangeX(a.SliderLargeChangeX)
                                    .SliderLargeChangeY(a.SliderLargeChangeY)
                                    .SliderShowTickMarksX(a.SliderShowTickMarksX)
                                    .SliderShowTickMarksY(a.SliderShowTickMarksY)
                                    .SliderSmallChangeX(a.SliderSmallChangeX)
                                    .SliderSmallChangeY(a.SliderSmallChangeY)
                                    .UpDownIncrementX(a.UpDownIncrementX)
                                    .UpDownIncrementY(a.UpDownIncrementY)
                                    .UseExponentialScale(a.UseExponentialScale));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case DoubleVectorPanAndSlider a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.PanAndSlider)
                                    .ShowResetButton(a.ShowResetButton)
                                    .SliderLargeChangeX(a.SliderLargeChangeX)
                                    .SliderLargeChangeY(a.SliderLargeChangeY)
                                    .SliderShowTickMarksX(a.SliderShowTickMarksX)
                                    .SliderShowTickMarksY(a.SliderShowTickMarksY)
                                    .SliderSmallChangeX(a.SliderSmallChangeX)
                                    .SliderSmallChangeY(a.SliderSmallChangeY)
                                    .StaticImageUnderlay(a.StaticImageUnderlay)
                                    .UpDownIncrementX(a.UpDownIncrementX)
                                    .UpDownIncrementY(a.UpDownIncrementY));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case DoubleVector3Slider a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.Slider)
                                    .ShowResetButton(a.ShowResetButton)
                                    .SliderLargeChangeX(a.SliderLargeChangeX)
                                    .SliderLargeChangeY(a.SliderLargeChangeY)
                                    .SliderLargeChangeZ(a.SliderLargeChangeZ)
                                    .SliderShowTickMarksX(a.SliderShowTickMarksX)
                                    .SliderShowTickMarksY(a.SliderShowTickMarksY)
                                    .SliderShowTickMarksZ(a.SliderShowTickMarksZ)
                                    .SliderSmallChangeX(a.SliderSmallChangeX)
                                    .SliderSmallChangeY(a.SliderSmallChangeY)
                                    .SliderSmallChangeZ(a.SliderSmallChangeZ)
                                    .UpDownIncrementX(a.UpDownIncrementX)
                                    .UpDownIncrementY(a.UpDownIncrementY)
                                    .UpDownIncrementZ(a.UpDownIncrementZ));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case DoubleVector3RollBallAndSlider a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.RollBallAndSliders)
                                    .ShowResetButton(a.ShowResetButton)
                                    .SliderLargeChangeX(a.SliderLargeChangeX)
                                    .SliderLargeChangeY(a.SliderLargeChangeY)
                                    .SliderLargeChangeZ(a.SliderLargeChangeZ)
                                    .SliderShowTickMarksX(a.SliderShowTickMarksX)
                                    .SliderShowTickMarksY(a.SliderShowTickMarksY)
                                    .SliderShowTickMarksZ(a.SliderShowTickMarksZ)
                                    .SliderSmallChangeX(a.SliderSmallChangeX)
                                    .SliderSmallChangeY(a.SliderSmallChangeY)
                                    .SliderSmallChangeZ(a.SliderSmallChangeZ)
                                    .UpDownIncrementX(a.UpDownIncrementX)
                                    .UpDownIncrementY(a.UpDownIncrementY)
                                    .UpDownIncrementZ(a.UpDownIncrementZ));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case TextBox a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.TextBox)
                                    .Multiline(a.Multiline));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case FileChooser a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.FileChooser)
                                    .AllowAllFiles(a.AllowAllFiles)
                                    .FileTypes(a.FileTypes));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case FolderChooser a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.FolderChooser));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case LinkLabel a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description);
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case DropDown a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.DropDown)
                                    .ValueDisplayNameCallback<ChoiceType>(GetDisplayName));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case RadioButton a:
                            pcic.Configure(a.Name, a.DisplayName ?? a.Name, a.Description, p => p
                                    .ControlType(PropertyControlType.RadioButton)
                                    .ValueDisplayNameCallback<ChoiceType>(GetDisplayName));
                            child.AddChildControl(pcic[a.Name]);
                            break;
                        case Container a:
                            child.AddChildControl(BuildControls(a, pcic));
                            break;
                    }
                }

                if (child is TabPageControlInfo tabPageControlInfo)
                {
                    ((TabContainerControlInfo)root).AddTab(tabPageControlInfo);
                }
                else
                {
                    root = child;
                    break;
                }
            }
            return root;
        }

        #endregion

        #region Stage #5 - Public Static Methods

        private static TControlInfo AddChildControl<TOwnerControlInfo, TControlInfo>(this TOwnerControlInfo ownerControlInfo, TControlInfo controlInfo) 
            where TOwnerControlInfo : ControlInfo
            where TControlInfo : ControlInfo
        {
            if (ownerControlInfo is PanelControlInfo panelControlInfo)
            {
                return panelControlInfo.AddChildControl(controlInfo);
            }
            if (ownerControlInfo is TabPageControlInfo tabPageControlInfo)
            {
                return tabPageControlInfo.AddChildControl(controlInfo);
            }
            throw new InvalidOperationException();
        }

        public static PropertyCollection CreatePropertyCollectionFromXml(string xml)
            => CreatePropertyCollection(LoadXml(xml));

        public static ControlInfo CreateConfigUIFromXml(this PropertyCollection properties, string xml)
            => CreateConfigUI(LoadXml(xml), properties);

        #endregion

        #region Private Helper Methods

        private static TTargetEnum ConvertTo<TSourceEnum, TTargetEnum>(this TSourceEnum src, TTargetEnum def = default)
            where TTargetEnum : struct, Enum
            where TSourceEnum : struct, Enum
            => Enum.TryParse(Enum.GetName(src.GetType(), src), out TTargetEnum result) ? result : def;

        private static object[] ParseCommaSeparatedValues(Type valueType, string csv)
        {
            string[] strlist = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return valueType.IsEnum
                ? strlist.Select(s => Enum.Parse(valueType, s)).ToArray()
                : strlist.Select(s => Convert.ChangeType(s, valueType)).ToArray();
        }

        private static PropertyCollection CreatePropertyCollection(Container root)
        {
            FluentPropertyCollection properties = new();
            AddProperties(root, properties);
            AddRules(root, properties);
            return properties;
        }

        private static ControlInfo CreateConfigUI(Container root, PropertyCollection properties)
        {
            PropertyControlInfoCollection pcic = new(properties);
            return BuildControls(root, pcic);
        }

        private static string GetDisplayName(ChoiceType c) => c.Value1 ?? c.Value;

        #endregion

        #region ClampUtil

        private static class ClampUtil
        {
            public static int Clamp(int value, int min, int max)
            {
                return Math.Clamp(value, min, max);
            }

            public static double Clamp(double value, double min, double max)
            {
                return Math.Clamp(value, min, max);
            }
        }

        #endregion
    }
}
