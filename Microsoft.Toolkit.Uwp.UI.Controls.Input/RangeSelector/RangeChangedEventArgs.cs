// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Uwp.UI.Controls;

/// <summary>
/// Event args for a value changing event
/// </summary>
public class RangeChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the old value.
    /// </summary>
    public double OldValue { get; private set; }

    /// <summary>
    /// Gets the new value.
    /// </summary>
    public double NewValue { get; private set; }

    /// <summary>
    /// Gets the range property that triggered the event
    /// </summary>
    public int ChangedRangeProperty { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldValue">The old value</param>
    /// <param name="newValue">The new value</param>
    /// <param name="changedRangeProperty">The changed range property</param>
    public RangeChangedEventArgs(double oldValue, double newValue, int changedRangeProperty)
    {
        OldValue = oldValue;
        NewValue = newValue;
        ChangedRangeProperty = changedRangeProperty;
    }
}
