using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace Weak.Behaviors;

public class PulseAnimationBehavior : Behavior<Border>
{
    private Border _border;
    private bool _isAnimating;

    protected override void OnAttachedTo(Border border)
    {
        base.OnAttachedTo(border);
        _border = border;
        _border.Loaded += OnLoaded;
    }

    protected override void OnDetachingFrom(Border border)
    {
        base.OnDetachingFrom(border);
        _border.Loaded -= OnLoaded;
        _border = null;
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        if (_isAnimating || _border == null)
            return;

        _isAnimating = true;
        await StartPulseAnimation();
    }

    private async Task StartPulseAnimation()
    {
        while (_border != null && _isAnimating)
        {
            try
            {
                // Pulse animation: scale up and fade slightly
                await Task.WhenAll(
                    _border.ScaleToAsync(1.5, 1000, Easing.SinInOut),
                    _border.FadeToAsync(0.3, 1000, Easing.SinInOut)
                );

                // Scale back and fade in
                await Task.WhenAll(
                    _border.ScaleToAsync(1.0, 1000, Easing.SinInOut),
                    _border.FadeToAsync(1.0, 1000, Easing.SinInOut)
                );
            }
            catch
            {
                // Animation was cancelled
                break;
            }
        }
    }
}
