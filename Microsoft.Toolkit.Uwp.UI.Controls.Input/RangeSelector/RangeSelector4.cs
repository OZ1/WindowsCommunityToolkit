// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;

namespace Microsoft.Toolkit.Uwp.UI.Controls;

/// <summary>
/// RangeSelector4 is a "quad slider" control for range values.
/// </summary>
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MinPressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "LoPressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "HiPressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MaxPressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplatePart(Name = "OutOfRangeContentContainer", Type = typeof(Border))]
[TemplatePart(Name = "ActiveRectangle", Type = typeof(Rectangle))]
[TemplatePart(Name = "MinThumb", Type = typeof(Thumb))]
[TemplatePart(Name = "LoThumb", Type = typeof(Thumb))]
[TemplatePart(Name = "HiThumb", Type = typeof(Thumb))]
[TemplatePart(Name = "MaxThumb", Type = typeof(Thumb))]
[TemplatePart(Name = "ContainerCanvas", Type = typeof(Canvas))]
[TemplatePart(Name = "ControlGrid", Type = typeof(Grid))]
[TemplatePart(Name = "ToolTip", Type = typeof(Grid))]
[TemplatePart(Name = "ToolTipText", Type = typeof(TextBlock))]

public partial class RangeSelector4 : RangeSelector
{
    private static readonly ThumbInfo[] _TI = new ThumbInfo[]
    {
            new("MinThumb", RangeMinProperty, "MinPressed"),
            new("LoThumb", RangeLoProperty, "LoPressed"),
            new("HiThumb", RangeHiProperty, "HiPressed"),
            new("MaxThumb", RangeMaxProperty, "MaxPressed"),
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeSelector4"/> class.
    /// Create a default range selector control.
    /// </summary>
    public RangeSelector4()
    {
        ThumbsInfo = _TI;
        DefaultStyleKey = typeof(RangeSelector4);
    }

    /// <summary>
    /// Identifies the <see cref="RangeLo"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeLoProperty =
        RegisterRangeProperty(nameof(RangeLo), typeof(RangeSelector4), 1.0 / 3);

    /// <summary>
    /// Identifies the <see cref="RangeHi"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeHiProperty =
        RegisterRangeProperty(nameof(RangeHi), typeof(RangeSelector4), 2.0 / 3);

    /// <summary>
    /// Gets or sets the current selected lower value of the range, modifiable by the user.
    /// </summary>
    /// <value>
    /// The current lower value.
    /// </value>
    public double RangeLo
    {
        get => (double)GetValue(RangeLoProperty);
        set => SetValue(RangeLoProperty, value);
    }

    /// <summary>
    /// Gets or sets the current selected upper value of the range, modifiable by the user.
    /// </summary>
    /// <value>
    /// The current upper value.
    /// </value>
    public double RangeHi
    {
        get => (double)GetValue(RangeHiProperty);
        set => SetValue(RangeHiProperty, value);
    }
}
