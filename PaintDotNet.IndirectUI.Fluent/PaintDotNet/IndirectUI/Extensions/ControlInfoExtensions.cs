// Copyright 2025 Osman Tunçelli. All rights reserved.
// Use of this source code is governed by GNU General Public License (GPL-2.0) that can be found in the COPYING file.

using PaintDotNet.PropertySystem;
using System;

namespace PaintDotNet.IndirectUI.Extensions;

internal static class ControlInfoExtensions
{
    public static ControlInfo Configure(this ControlInfo info, PropertyName propertyName, Func<PropertyControlInfo, PropertyControlInfo> selector, bool throwOnError = true)
    {
        ArgumentNullException.ThrowIfNull(info);
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(selector);

        PropertyControlInfo? pci = info.FindControlForPropertyName(propertyName);
        if (pci is null)
        {
            return throwOnError
                ? throw new ArgumentException($"Cannot find control for property name `{propertyName.Name}`", nameof(propertyName))
                : info;
        }
        _ = selector(pci);
        return info;
    }

    public static ControlInfo Configure(this ControlInfo info, PropertyName propertyName, string displayName, Func<PropertyControlInfo, PropertyControlInfo>? selector = null, bool throwOnError = true)
    {
        return info.Configure(propertyName, p =>
        {
            PropertyControlInfo pci = p.DisplayName(displayName);
            return selector is not null ? selector(pci) : pci;
        }, throwOnError);
    }

    public static ControlInfo Configure(this ControlInfo info, PropertyName propertyName, string displayName, string description, Func<PropertyControlInfo, PropertyControlInfo>? selector = null, bool throwOnError = true)
    {
        return info.Configure(propertyName, displayName, p =>
        {
            PropertyControlInfo pci = p.Description(description);
            return selector is not null ? selector(pci) : pci;
        }, throwOnError);
    }
}