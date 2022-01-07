// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Microsoft.Toolkit.Uwp.UI.Controls;

/// <summary>
/// RangeSelector is a "double slider" control for range values.
/// </summary>
public partial class RangeSelector : Control
{
    private double DragWidth()
    {
        return _containerCanvas.ActualWidth - _thumbs[_thumbs.Length - 1].Width;
    }

    private double GetPositionFromValue(double value)
    {
        var position = (value - Minimum) / (Maximum - Minimum);
        return position * DragWidth();
    }

    private double GetValueFromPosition(double position)
    {
        var value = position / DragWidth();
        return Minimum + (value * (Maximum - Minimum));
    }

    private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        OnThumbDragStarted(e);
        StartDragThumb(Array.IndexOf(_thumbs, sender));
    }

    private void StartDragThumb(int thumb)
    {
        _oldValue = GetRange(thumb);
        _absolutePosition = Canvas.GetLeft(_thumbs[thumb]);

        for (var i = 0; i < _thumbs.Length; i++)
        {
            Canvas.SetZIndex(_thumbs[i], i == thumb ? 10 : 0);
        }

        if (_toolTipText != null && _toolTip != null)
        {
            _toolTip.Visibility = Visibility.Visible;
            var thumbCenter = _absolutePosition + (_thumbs[thumb].Width / 2);
            _toolTip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var ttWidth = _toolTip.ActualWidth / 2;
            Canvas.SetLeft(_toolTip, thumbCenter - ttWidth);

            UpdateToolTipText(_oldValue);
        }

        VisualStateManager.GoToState(this, ThumbsInfo[thumb].PressedVisualState, true);
    }

    private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        var thumb = Array.IndexOf(_thumbs, sender);

        _absolutePosition += e.HorizontalChange;

        SetRange(thumb, DragThumb((Thumb)sender, 0, DragWidth(), _absolutePosition));

        UpdateToolTipText(GetRange(thumb));
    }

    private double DragThumb(Thumb thumb, double min, double max, double nextPos)
    {
        nextPos = Math.Max(min, nextPos);
        nextPos = Math.Min(max, nextPos);

        Canvas.SetLeft(thumb, nextPos);

        if (_toolTipText != null && _toolTip != null)
        {
            var thumbCenter = nextPos + (thumb.Width / 2);
            _toolTip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var ttWidth = _toolTip.ActualWidth / 2;

            Canvas.SetLeft(_toolTip, thumbCenter - ttWidth);
        }

        return GetValueFromPosition(nextPos);
    }

    private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        OnThumbDragCompleted(e);

        var thumb = Array.IndexOf(_thumbs, sender);

        OnValueChanged(new(_oldValue, GetRange(thumb), thumb));

        SyncThumbs();

        if (_toolTip != null)
        {
            _toolTip.Visibility = Visibility.Collapsed;
        }

        VisualStateManager.GoToState(this, "Normal", true);
    }
}
