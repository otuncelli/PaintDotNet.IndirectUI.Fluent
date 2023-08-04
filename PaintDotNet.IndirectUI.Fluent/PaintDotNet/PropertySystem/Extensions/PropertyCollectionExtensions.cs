// Copyright 2023 Osman Tunçelli. All rights reserved.
// Use of this source code is governed by GNU General Public License (GPL-2.0) that can be found in the COPYING file.

namespace PaintDotNet.PropertySystem.Extensions;

public static class PropertyCollectionExtensions
{
    public static T GetPropertyValue<T>(this PropertyCollection collection, PropertyName propertyName)
        => collection[propertyName].GetValue<T>();

    public static void GetPropertyValue<T>(this PropertyCollection collection, PropertyName propertyName, out T propertyValue)
        => propertyValue = collection[propertyName].GetValue<T>();

    public static object GetPropertyValue(this PropertyCollection collection, PropertyName propertyName)
        => collection[propertyName].Value;

    public static void GetPropertyValue(this PropertyCollection collection, PropertyName propertyName, out object propertyValue)
        => propertyValue = collection[propertyName].Value;
}
