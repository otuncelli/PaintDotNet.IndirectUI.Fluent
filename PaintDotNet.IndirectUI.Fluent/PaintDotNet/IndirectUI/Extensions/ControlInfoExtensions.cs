// Copyright 2023 Osman Tunçelli. All rights reserved.
// Use of this source code is governed by GNU General Public License (GPL-2.0) that can be found in the COPYING file.

using PaintDotNet.PropertySystem;
using System;

namespace PaintDotNet.IndirectUI.Extensions;

internal static class ControlInfoExtensions
{
    public static ControlInfo Configure(this ControlInfo info, PropertyName propertyName, Func<PropertyControlInfo, PropertyControlInfo> selector, bool throwOnError = true)
    {
        PropertyControlInfo pci = info.FindControlForPropertyName(propertyName);
        if (pci == null)
        {
            return throwOnError
                ? throw new ArgumentException($"Can not find control for property: {propertyName}.", nameof(propertyName))
                : info;
        }
        _ = selector(pci);
        return info;
    }

    public static ControlInfo Configure(this ControlInfo info, PropertyName propertyName, string displayName, Func<PropertyControlInfo, PropertyControlInfo> selector = null, bool throwOnError = true)
    {
        return info.Configure(propertyName, p =>
        {
            PropertyControlInfo pci = p.DisplayName(displayName);
            return selector != null ? selector(pci) : pci;
        }, throwOnError);
    }

    public static ControlInfo Configure(this ControlInfo info, PropertyName propertyName, string displayName, string description, Func<PropertyControlInfo, PropertyControlInfo> selector = null, bool throwOnError = true)
    {
        return info.Configure(propertyName, displayName, p =>
        {
            PropertyControlInfo pci = p.Description(description);
            return selector != null ? selector(pci) : pci;
        }, throwOnError);
    }
}