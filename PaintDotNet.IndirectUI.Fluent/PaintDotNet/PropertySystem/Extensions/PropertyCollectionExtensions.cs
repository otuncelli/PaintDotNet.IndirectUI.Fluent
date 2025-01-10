// Copyright 2025 Osman Tunçelli. All rights reserved.
// Use of this source code is governed by GNU General Public License (GPL-2.0) that can be found in the COPYING file.

using System;

namespace PaintDotNet.PropertySystem.Extensions;

public static class PropertyCollectionExtensions
{
    public static T GetPropertyValue<T>(this PropertyCollection collection, PropertyName propertyName)
    {
        return GetProperty(collection, propertyName).GetValue<T>();
    }

    public static void GetPropertyValue<T>(this PropertyCollection collection, PropertyName propertyName, out T propertyValue)
    {
        propertyValue = GetProperty(collection, propertyName).GetValue<T>();
    }

    public static object? GetPropertyValue(this PropertyCollection collection, PropertyName propertyName)
    {
        return GetProperty(collection, propertyName).Value;
    }

    public static void GetPropertyValue(this PropertyCollection collection, PropertyName propertyName, out object? propertyValue)
    {
        propertyValue = GetProperty(collection, propertyName).Value;
    }

    private static Property GetProperty(this PropertyCollection collection, PropertyName propertyName)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(propertyName);

        Property? prop = collection[propertyName];
        return prop ?? throw new ArgumentException($"Cannot find property with name `{propertyName.Name}`", nameof(propertyName));
    }
}
