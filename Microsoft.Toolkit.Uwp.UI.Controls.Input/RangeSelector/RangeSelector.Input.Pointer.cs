// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Microsoft.Toolkit.Uwp.UI.Controls;

/// <summary>
/// RangeSelector is a "double slider" control for range values.
/// </summary>
public partial class RangeSelector : Control
{
    private void ContainerCanvas_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "PointerOver", false);
    }

    private void ContainerCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (_pointerManipulating == _pointerManipulatingMax)
        {
            DragStop(e.GetCurrentPoint(_containerCanvas).Position.X, false);
        }

        VisualStateManager.GoToState(this, "Normal", false);
    }

    private void ContainerCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (_pointerManipulating == _pointerManipulatingMax)
        {
            DragStop(e.GetCurrentPoint(_containerCanvas).Position.X, true);
        }
    }

    private void ContainerCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var position = e.GetCurrentPoint(_containerCanvas).Position.X;

        if (_pointerManipulating != _pointerManipulatingMax)
        {
            if (position < _pointerPressPosition)
            {
                _pointerManipulatingMax = _pointerManipulating;
            }
            else
            {
                _pointerManipulating = _pointerManipulatingMax;
            }

            DragStart(position);
        }

        SetRange(_pointerManipulating, DragThumb(_thumbs[_pointerManipulating], 0, DragWidth(), position));
        UpdateToolTipText(GetRange(_pointerManipulating));
    }

    private void ContainerCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var position = e.GetCurrentPoint(_containerCanvas).Position.X;

        _pointerPressPosition = position;
        var minValueDiff = double.MaxValue;
        for (var i = 0; i < _thumbs.Length; i++)
        {
            var valueDiff = Math.Abs(GetPositionFromValue(GetRange(i)) - position);
            if (minValueDiff > valueDiff)
            {
                minValueDiff = valueDiff;
                _pointerManipulating = i;
                _pointerManipulatingMax = i;
            }
            else if (minValueDiff == valueDiff)
            {
                _pointerManipulatingMax = i;
            }
        }

        if (_pointerManipulating == _pointerManipulatingMax)
        {
            DragStart(position);
        }
    }

    private void DragStart(double position)
    {
        var normalizedPosition = GetValueFromPosition(position);

        SetRange(_pointerManipulating, normalizedPosition);
        StartDragThumb(_pointerManipulating);

        SyncThumbs();
    }

    private void DragStop(double position, bool syncThumbs)
    {
        var normalizedPosition = GetValueFromPosition(position);

        var thumb = _pointerManipulating;
        _pointerManipulating = -1;
        _containerCanvas.IsHitTestVisible = true;
        OnValueChanged(new(GetRange(thumb), normalizedPosition, thumb));

        if (syncThumbs)
        {
            SyncThumbs();
        }

        if (_toolTip != null)
        {
            _toolTip.Visibility = Visibility.Collapsed;
        }
    }
}
