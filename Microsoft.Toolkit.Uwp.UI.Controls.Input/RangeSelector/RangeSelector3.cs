// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;

namespace Microsoft.Toolkit.Uwp.UI.Controls;

/// <summary>
/// RangeSelector3 is a "tripple slider" control for range values.
/// </summary>
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MinPressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MidPressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MaxPressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplatePart(Name = "OutOfRangeContentContainer", Type = typeof(Border))]
[TemplatePart(Name = "ActiveRectangle", Type = typeof(Rectangle))]
[TemplatePart(Name = "MinThumb", Type = typeof(Thumb))]
[TemplatePart(Name = "MidThumb", Type = typeof(Thumb))]
[TemplatePart(Name = "MaxThumb", Type = typeof(Thumb))]
[TemplatePart(Name = "ContainerCanvas", Type = typeof(Canvas))]
[TemplatePart(Name = "ControlGrid", Type = typeof(Grid))]
[TemplatePart(Name = "ToolTip", Type = typeof(Grid))]
[TemplatePart(Name = "ToolTipText", Type = typeof(TextBlock))]

public partial class RangeSelector3 : RangeSelector
{
    private static readonly ThumbInfo[] _TI = new ThumbInfo[]
    {
            new("MinThumb", RangeMinProperty, "MinPressed"),
            new("MidThumb", RangeMidProperty, "MidPressed"),
            new("MaxThumb", RangeMaxProperty, "MaxPressed"),
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeSelector3"/> class.
    /// Create a default range selector control.
    /// </summary>
    public RangeSelector3()
    {
        ThumbsInfo = _TI;
        DefaultStyleKey = typeof(RangeSelector3);
    }

    /// <summary>
    /// Identifies the <see cref="RangeMid"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeMidProperty =
        RegisterRangeProperty(nameof(RangeMid), typeof(RangeSelector3), 1.0 / 2);

    /// <summary>
    /// Gets or sets the current selected middle value of the range, modifiable by the user.
    /// </summary>
    /// <value>
    /// The current middle.
    /// </value>
    public double RangeMid
    {
        get => (double)GetValue(RangeMidProperty);
        set => SetValue(RangeMidProperty, value);
    }
}
