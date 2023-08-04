// Copyright 2023 Osman Tunçelli. All rights reserved.
// Use of this source code is governed by GNU General Public License (GPL-2.0) that can be found in the COPYING file.

using PaintDotNet.IndirectUI.Extensions;
using PaintDotNet.PropertySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace PaintDotNet.IndirectUI;

public sealed class PropertyControlInfoCollection : IReadOnlyList<PropertyControlInfo>
{
    #region Fields

    private readonly KeyedPropertyControlInfoCollection items;
    private readonly HashSet<PropertyName> addedToPanel;

    #endregion

    #region Properties

    public IEnumerable<Property> Properties => items.Select(pci => pci.Property);

    public IEnumerable<PropertyName> PropertyNames => items.Select(pci => (PropertyName)pci.Property.Name);

    public PropertyControlInfo this[PropertyName propertyName] => items[propertyName];

    public int Count => items.Count;

#pragma warning disable CA1822 // Mark members as static
    public bool IsReadOnly => false;
#pragma warning restore CA1822 // Mark members as static

    PropertyControlInfo IReadOnlyList<PropertyControlInfo>.this[int index] => GetPropertyControlInfoAt(index);

    #endregion

    #region Constructors

    public PropertyControlInfoCollection(IEnumerable<Property> props)
    {
        items = new KeyedPropertyControlInfoCollection();
        addedToPanel = new HashSet<PropertyName>();
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        Regex re = new("(\\B[A-Z])");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        foreach (Property prop in props)
        {
            PropertyControlInfo pci = PropertyControlInfo.CreateFor(prop);
            pci.DisplayName(re.Replace(prop.Name, " $1"));
            items.Add(pci);
        }
    }

    #endregion

    #region Methods

    public PropertyControlInfoCollection Configure(PropertyName propertyName, Func<PropertyControlInfo, PropertyControlInfo> selector)
    {
        PropertyControlInfo pci = items[propertyName];
        _ = selector(pci);
        return this;
    }

    public PropertyControlInfoCollection Configure(PropertyName propertyName, string displayName, Func<PropertyControlInfo, PropertyControlInfo> selector = null)
    {
        return Configure(propertyName, p =>
        {
            PropertyControlInfo pci = p.DisplayName(displayName);
            return selector?.Invoke(pci) ?? pci;
        });
    }

    public PropertyControlInfoCollection Configure(PropertyName propertyName, string displayName, string description, Func<PropertyControlInfo, PropertyControlInfo> selector = null)
    {
        return Configure(propertyName, displayName, p =>
        {
            PropertyControlInfo pci = p.Description(description);
            return selector?.Invoke(pci) ?? pci;
        });
    }

    public bool TryGetControl(PropertyName propertyName, out PropertyControlInfo pci)
    {
        return items.TryGetValue(propertyName, out pci);
    }

    public PropertyControlInfo GetPropertyControlInfoAt(int index)
    {
        return ((Collection<PropertyControlInfo>)items)[index];
    }

    public PanelControlInfo CreatePanel(params PropertyName[] propertyNames)
    {
        if (propertyNames == null || propertyNames.Length == 0)
        {
            propertyNames = PropertyNames.ToArray();
        }
        PanelControlInfo panel = new();
        foreach (PropertyName propertyName in propertyNames)
        {
            if (addedToPanel.Contains(propertyName))
            {
                continue;
            }
            panel.AddChildControl(items[propertyName]);
            addedToPanel.Add(propertyName);
        }
        return panel;
    }

    public TabPageControlInfo CreateTabPage(string text, string toolTipText, params PropertyName[] propertyNames)
    {
        if (propertyNames == null || propertyNames.Length == 0)
        {
            propertyNames = PropertyNames.ToArray();
        }
        TabPageControlInfo tabPage = new()
        {
            Text = text,
            ToolTipText = toolTipText
        };
        foreach (PropertyName propertyName in propertyNames)
        {
            if (addedToPanel.Contains(propertyName))
            {
                continue;
            }
            tabPage.AddChildControl(items[propertyName]);
            addedToPanel.Add(propertyName);
        }
        return tabPage;
    }

    public TabPageControlInfo CreateTabPage(string text, params PropertyName[] propertyNames)
    {
        return CreateTabPage(text, null, propertyNames);
    }

    #endregion

    #region IEnumerable

    public IEnumerator<PropertyControlInfo> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region KeyedPropertyControlInfoCollection

    private sealed class KeyedPropertyControlInfoCollection : KeyedCollection<PropertyName, PropertyControlInfo>
    {
        protected override PropertyName GetKeyForItem(PropertyControlInfo item)
        {
            return (PropertyName)item.Property.Name;
        }
    }

    #endregion
}