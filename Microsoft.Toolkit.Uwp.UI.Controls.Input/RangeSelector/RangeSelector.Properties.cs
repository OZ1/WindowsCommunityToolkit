// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls;

/// <summary>
/// RangeSelector is a "double slider" control for range values.
/// </summary>
public partial class RangeSelector : Control
{
    /// <summary>
    /// Identifies the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            nameof(Minimum),
            typeof(double),
            typeof(RangeSelector),
            new PropertyMetadata(DefaultMinimum, MinimumChangedCallback));

    /// <summary>
    /// Identifies the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(double),
            typeof(RangeSelector),
            new PropertyMetadata(DefaultMaximum, MaximumChangedCallback));

    /// <summary>
    /// Identifies the <see cref="RangeMin"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeMinProperty =
        RegisterRangeProperty(nameof(RangeMin), typeof(RangeSelector), DefaultMinimum);

    /// <summary>
    /// Identifies the <see cref="RangeMax"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeMaxProperty =
        RegisterRangeProperty(nameof(RangeMax), typeof(RangeSelector), DefaultMaximum);

    /// <summary>
    /// Identifies the <see cref="StepFrequency"/> property.
    /// </summary>
    public static readonly DependencyProperty StepFrequencyProperty =
        DependencyProperty.Register(
            nameof(StepFrequency),
            typeof(double),
            typeof(RangeSelector),
            new PropertyMetadata(DefaultStepFrequency));

    /// <summary>
    /// Gets or sets the absolute minimum value of the range.
    /// </summary>
    /// <value>
    /// The minimum.
    /// </value>
    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Gets or sets the absolute maximum value of the range.
    /// </summary>
    /// <value>
    /// The maximum.
    /// </value>
    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Gets or sets the current selected lower limit value of the range, modifiable by the user.
    /// </summary>
    /// <value>
    /// The current lower limit.
    /// </value>
    public double RangeMin
    {
        get => (double)GetValue(RangeMinProperty);
        set => SetValue(RangeMinProperty, value);
    }

    /// <summary>
    /// Gets or sets the current selected upper limit value of the range, modifiable by the user.
    /// </summary>
    /// <value>
    /// The current upper limit.
    /// </value>
    public double RangeMax
    {
        get => (double)GetValue(RangeMaxProperty);
        set => SetValue(RangeMaxProperty, value);
    }

    /// <summary>
    /// Gets or sets the value part of a value range that steps should be created for.
    /// </summary>
    /// <value>
    /// The value part of a value range that steps should be created for.
    /// </value>
    public double StepFrequency
    {
        get => (double)GetValue(StepFrequencyProperty);
        set => SetValue(StepFrequencyProperty, value);
    }

    {

    private static void MinimumChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RangeSelector rangeSelector || !rangeSelector._valuesAssigned)
        {
            return;
        }

        var newValue = (double)e.NewValue;
        var oldValue = (double)e.OldValue;

        if (rangeSelector.Maximum < newValue)
        {
            rangeSelector.Maximum = newValue + Epsilon;
        }

        for (var i = 0; i < rangeSelector._thumbs.Length; i++)
        {
            if (rangeSelector.GetRange(i) < newValue)
            {
                rangeSelector.SetRange(i, newValue);
            }
        }

        if (newValue != oldValue)
        {
            rangeSelector.SyncThumbs();
        }
    }

    private static void MaximumChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RangeSelector rangeSelector || !rangeSelector._valuesAssigned)
        {
            return;
        }

        var newValue = (double)e.NewValue;
        var oldValue = (double)e.OldValue;

        if (rangeSelector.Minimum > newValue)
        {
            rangeSelector.Minimum = newValue - Epsilon;
        }

        for (var i = rangeSelector._thumbs.Length - 1; i >= 0; i--)
        {
            if (rangeSelector.GetRange(i) > newValue)
            {
                rangeSelector.SetRange(i, newValue);
            }
        }

        if (newValue != oldValue)
        {
            rangeSelector.SyncThumbs();
        }
    }

    protected static void RangeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RangeSelector rangeSelector)
        {
            return;
        }

        var thumb = Array.FindIndex(rangeSelector.ThumbsInfo, x => x.RangeProperty == e.Property);

        rangeSelector._rangeSet |= 1u << thumb;

        if (!rangeSelector._valuesAssigned)
        {
            return;
        }

        var newValue = (double)e.NewValue;
        rangeSelector.SetRange(thumb, rangeSelector.MoveToStepFrequency(rangeSelector.GetRange(thumb)));

        if (rangeSelector._valuesAssigned)
        {
            if (newValue < rangeSelector.Minimum)
            {
                rangeSelector.SetRange(thumb, rangeSelector.Minimum);
            }
            else if (newValue > rangeSelector.Maximum)
            {
                rangeSelector.SetRange(thumb, rangeSelector.Maximum);
            }

            rangeSelector.SyncActiveRectangle();

            for (var i = rangeSelector._thumbs.Length - 1; i > thumb; i--)
            {
                if (rangeSelector.GetRange(i) < newValue)
                {
                    rangeSelector.SetRange(i, newValue);
                }
            }

            for (var i = 0; i < thumb; i++)
            {
                if (rangeSelector.GetRange(i) > newValue)
                {
                    rangeSelector.SetRange(i, newValue);
                }
            }
        }

        rangeSelector.SyncThumbs();
    }

    {
        {
            return;
        }

        {
    }

            {
    {
        }

        {
        }
    }
}
