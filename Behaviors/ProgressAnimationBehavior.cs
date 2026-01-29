using Microsoft.Maui.Layouts;

namespace Weak.Behaviors;

public class ProgressAnimationBehavior : Behavior<BoxView>
{
    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(nameof(Progress), typeof(double), typeof(ProgressAnimationBehavior), 0.0, propertyChanged: OnProgressChanged);

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var behavior = (ProgressAnimationBehavior)bindable;
        if (behavior.AssociatedObject is BoxView box && (double)newValue != (double)oldValue)
        {
            var newProgress = (double)newValue;
            var oldProgress = (double)oldValue;

            // Create animation
            var animation = new Animation(v => 
            {
                // Assuming standard layout: X=0, Y=0.5, W=v, H=12
                // Flags are PositionProportional, WidthProportional.
                // Height is Absolute 12 (matching XAML container).
                AbsoluteLayout.SetLayoutBounds(box, new Rect(0, 0.5, v, 12));
            }, oldProgress, newProgress);

            animation.Commit(box, "ProgressAnimation", 16, 500, Easing.CubicOut);
        }
    }

    public BoxView AssociatedObject { get; private set; }

    protected override void OnAttachedTo(BoxView bindable)
    {
        base.OnAttachedTo(bindable);
        AssociatedObject = bindable;
        // Initial set without animation
        AbsoluteLayout.SetLayoutBounds(bindable, new Rect(0, 0.5, Progress, 12));
    }

    protected override void OnDetachingFrom(BoxView bindable)
    {
        base.OnDetachingFrom(bindable);
        AssociatedObject = null;
    }
}
