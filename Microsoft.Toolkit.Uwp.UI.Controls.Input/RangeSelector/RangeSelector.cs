// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;

namespace Microsoft.Toolkit.Uwp.UI.Controls;

/// <summary>
/// RangeSelector is a "double slider" control for range values.
/// </summary>
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MinPressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MaxPressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplatePart(Name = "OutOfRangeContentContainer", Type = typeof(Border))]
[TemplatePart(Name = "ActiveRectangle", Type = typeof(Rectangle))]
[TemplatePart(Name = "MinThumb", Type = typeof(Thumb))]
[TemplatePart(Name = "MaxThumb", Type = typeof(Thumb))]
[TemplatePart(Name = "ContainerCanvas", Type = typeof(Canvas))]
[TemplatePart(Name = "ControlGrid", Type = typeof(Grid))]
[TemplatePart(Name = "ToolTip", Type = typeof(Grid))]
[TemplatePart(Name = "ToolTipText", Type = typeof(TextBlock))]

public partial class RangeSelector : Control
{
    private const double Epsilon = 0.01;
    private const double DefaultMinimum = 0.0;
    private const double DefaultMaximum = 1.0;
    private const double DefaultStepFrequency = 1;
    private static readonly TimeSpan TimeToHideToolTipOnKeyUp = TimeSpan.FromSeconds(1);

    private static readonly ThumbInfo[] _TI = new ThumbInfo[]
    {
        new("MinThumb", RangeMinProperty, "MinPressed"),
        new("MaxThumb", RangeMaxProperty, "MaxPressed"),
    };

    protected ThumbInfo[] ThumbsInfo { get; set; } = _TI;

    private Thumb[] _thumbs;
    private uint _rangeSet;
    private double _absolutePosition;
    private Border _outOfRangeContentContainer;
    private Rectangle _activeRectangle;
    private Canvas _containerCanvas;
    private Grid _controlGrid;
    private double _oldValue;
    private bool _valuesAssigned;
    private int _pointerManipulating;
    private int _pointerManipulatingMax;
    private double _pointerPressPosition;
    private Grid _toolTip;
    private TextBlock _toolTipText;

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeSelector"/> class.
    /// Create a default range selector control.
    /// </summary>
    public RangeSelector()
    {
        DefaultStyleKey = typeof(RangeSelector);
        _thumbs = new Thumb[ThumbsInfo.Length];
    }

    /// <summary>
    /// Update the visual state of the control when its template is changed.
    /// </summary>
    protected override void OnApplyTemplate()
    {
        IsEnabledChanged -= RangeSelector_IsEnabledChanged;

        if (_containerCanvas != null)
        {
            _containerCanvas.SizeChanged -= ContainerCanvas_SizeChanged;
            _containerCanvas.PointerEntered -= ContainerCanvas_PointerEntered;
            _containerCanvas.PointerPressed -= ContainerCanvas_PointerPressed;
            _containerCanvas.PointerMoved -= ContainerCanvas_PointerMoved;
            _containerCanvas.PointerReleased -= ContainerCanvas_PointerReleased;
            _containerCanvas.PointerExited -= ContainerCanvas_PointerExited;
        }

        foreach (var thumb in _thumbs)
        {
            if (thumb != null)
            {
                thumb.DragCompleted -= Thumb_DragCompleted;
                thumb.DragDelta -= Thumb_DragDelta;
                thumb.DragStarted -= Thumb_DragStarted;
                thumb.KeyDown -= Thumb_KeyDown;
                thumb.KeyUp -= Thumb_KeyUp;
            }
        }

        // Need to make sure the values can be set in XAML and don't overwrite each other
        VerifyValues();
        _valuesAssigned = true;

        _outOfRangeContentContainer = GetTemplateChild("OutOfRangeContentContainer") as Border;
        _activeRectangle = GetTemplateChild("ActiveRectangle") as Rectangle;
        _containerCanvas = GetTemplateChild("ContainerCanvas") as Canvas;
        _controlGrid = GetTemplateChild("ControlGrid") as Grid;
        _toolTip = GetTemplateChild("ToolTip") as Grid;
        _toolTipText = GetTemplateChild("ToolTipText") as TextBlock;
        for (var i = 0; i < ThumbsInfo.Length; i++)
        {
            _thumbs[i] = GetTemplateChild(ThumbsInfo[i].TemplateName) as Thumb;
        }

        foreach (var thumb in _thumbs)
        {
            if (thumb != null)
            {
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
                thumb.DragStarted += Thumb_DragStarted;
                thumb.KeyDown += Thumb_KeyDown;
                thumb.KeyUp += Thumb_KeyUp;
            }
        }

        if (_containerCanvas != null)
        {
            _containerCanvas.SizeChanged += ContainerCanvas_SizeChanged;
            _containerCanvas.PointerEntered += ContainerCanvas_PointerEntered;
            _containerCanvas.PointerPressed += ContainerCanvas_PointerPressed;
            _containerCanvas.PointerMoved += ContainerCanvas_PointerMoved;
            _containerCanvas.PointerReleased += ContainerCanvas_PointerReleased;
            _containerCanvas.PointerExited += ContainerCanvas_PointerExited;
        }

        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", false);

        IsEnabledChanged += RangeSelector_IsEnabledChanged;

        // Measure our min/max text longest value so we can avoid the length of the scrolling reason shifting in size during use.
        var tb = new TextBlock { Text = Maximum.ToString() };
        tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        base.OnApplyTemplate();
    }

    private void UpdateToolTipText(double newValue)
    {
        if (_toolTipText != null)
        {
            _toolTipText.Text = string.Format("{0:0.##}", newValue);
        }
    }

    private void ContainerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SyncThumbs();
    }

    private void VerifyValues()
    {
        if (Minimum > Maximum)
        {
            Minimum = Maximum;
            Maximum = Maximum;
        }

        if (Minimum == Maximum)
        {
            Maximum += Epsilon;
        }

        if ((_rangeSet >> (_thumbs.Length - 1)) == 0)
        {
            RangeMax = Maximum;
        }

        if ((_rangeSet & 1u) == 0)
        {
            RangeMin = Minimum;
        }

        for (int lo, hi = _thumbs.Length - 2; hi > 1; hi--)
        {
            if ((_rangeSet & (1u << hi)) != 0)
            {
                continue;
            }

            for (lo = hi; lo > 1; lo--)
            {
                if ((_rangeSet & (1u << (lo - 1))) != 0)
                {
                    break;
                }
            }

            var n = hi - lo + 2;
            var max = GetRange(hi + 1);
            var min = GetRange(lo - 1);
            for (var i = 1; i < n; i++)
            {
                SetRange(lo + i - 1, (min * (n - i) / n) + (max * i / n));
            }

            hi = lo - 1;
        }

        for (var i = 0; i < _thumbs.Length; i++)
        {
            var value = GetRange(i);

            if (value < Minimum)
            {
                SetRange(i, Minimum);
            }

            if (value > Maximum)
            {
                SetRange(i, Maximum);
            }
        }

        var rangeNext = GetRange(_thumbs.Length - 1);
        for (var i = _thumbs.Length - 2; i >= 0; i--)
        {
            var rangePrev = GetRange(i);
            if (rangePrev > rangeNext)
            {
                rangePrev = rangeNext;
                SetRange(i, rangeNext);
            }

            rangeNext = rangePrev;
        }
    }

    private double MoveToStepFrequency(double rangeValue)
    {
        double newValue = Minimum + (((int)Math.Round((rangeValue - Minimum) / StepFrequency)) * StepFrequency);

        if (newValue < Minimum)
        {
            return Minimum;
        }

        if (newValue > Maximum || Maximum - newValue < StepFrequency)
        {
            return Maximum;
        }

        return newValue;
    }

    private void SyncThumbs(int fromKeyDown = -1)
    {
        if (_containerCanvas == null)
        {
            return;
        }

        for (var i = 0; i < _thumbs.Length; i++)
        {
            Canvas.SetLeft(_thumbs[i], GetPositionFromValue(GetRange(i)));
        }

        if (fromKeyDown != -1)
        {
            var min = fromKeyDown < 1 ? 0 :
                Canvas.GetLeft(_thumbs[fromKeyDown - 1]);

            var max = fromKeyDown >= _thumbs.Length - 1 ? DragWidth() :
                Canvas.GetLeft(_thumbs[fromKeyDown + 1]);

            DragThumb(_thumbs[fromKeyDown], min, max, GetPositionFromValue(GetRange(fromKeyDown)));

            UpdateToolTipText(GetRange(fromKeyDown));
        }

        SyncActiveRectangle();
    }

    private void SyncActiveRectangle()
    {
        if (_containerCanvas is null)
        {
            return;
        }

        var minThumb = _thumbs[0];
        var maxThumb = _thumbs[_thumbs.Length - 1];

        if (minThumb is null)
        {
            return;
        }

        if (maxThumb is null)
        {
            return;
        }

        var relativeLeft = Canvas.GetLeft(minThumb);
        Canvas.SetLeft(_activeRectangle, relativeLeft);
        Canvas.SetTop(_activeRectangle, (_containerCanvas.ActualHeight - _activeRectangle.ActualHeight) / 2);
        _activeRectangle.Width = Math.Max(0, Canvas.GetLeft(maxThumb) - Canvas.GetLeft(minThumb));
    }

    private void RangeSelector_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
    }

    protected struct ThumbInfo
    {
        public string TemplateName;
        public DependencyProperty RangeProperty;
        public string PressedVisualState;

        public ThumbInfo(string templateName, DependencyProperty rangeProperty, string pressedVisualState)
        {
            TemplateName = templateName;
            RangeProperty = rangeProperty;
            PressedVisualState = pressedVisualState;
        }
    }
}
