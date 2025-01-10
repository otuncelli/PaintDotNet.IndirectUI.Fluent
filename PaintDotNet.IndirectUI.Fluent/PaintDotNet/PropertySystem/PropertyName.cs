// Copyright 2025 Osman Tunçelli. All rights reserved.
// Use of this source code is governed by GNU General Public License (GPL-2.0) that can be found in the COPYING file.

using System;

namespace PaintDotNet.PropertySystem;

public sealed class PropertyName(string name) : IEquatable<PropertyName>
{
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    public bool Equals(PropertyName? other)
    {
        if (other is null) { return false; }
        if (ReferenceEquals(this, other)) { return true; }
        return Name.Equals(other.Name, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PropertyName);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public override string ToString()
    {
        return Name;
    }

    public TEnum? ToEnum<TEnum>(bool ignoreCase = false) where TEnum : struct, Enum
    {
        return Enum.TryParse(Name, ignoreCase, out TEnum @enum) ? (TEnum?)@enum : null;
    }

    #region Operators

    public static implicit operator PropertyName(Enum @enum)
    {
        ArgumentNullException.ThrowIfNull(@enum);

        string? name = Enum.GetName(@enum.GetType(), @enum);
        return name is null
            ? throw new InvalidOperationException("The specified enum constant could not be found.")
            : new PropertyName(name);
    }

    public static implicit operator PropertyName(string s)
    {
        return new(s);
    }

    public static implicit operator string(PropertyName pn)
    {
        return pn.Name;
    }

    public static bool operator ==(PropertyName pn, Enum @enum)
    {
        if (@enum is null) { return false; }

        string? name = Enum.GetName(@enum.GetType(), @enum);
        return name is not null && name.Equals(pn.Name, StringComparison.Ordinal);
    }

    public static bool operator !=(PropertyName pn, Enum @enum)
    {
        return !(pn == @enum);
    }

    #endregion
}
