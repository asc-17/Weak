using System.Runtime.CompilerServices;

namespace Weak.Behaviors;

public class ExpandBehavior : Behavior<View>
{
    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(ExpandBehavior), false, propertyChanged: OnIsExpandedChanged);

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    private static void OnIsExpandedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var behavior = (ExpandBehavior)bindable;
        if (behavior.AssociatedObject is View view)
        {
            var isExpanded = (bool)newValue;
            if (isExpanded)
            {
                view.IsVisible = true;
                view.Opacity = 0;
                view.FadeToAsync(1, 250, Easing.CubicOut);
                // Simple height animation is difficult with Auto, so we stick to Fade. 
                // If we want height, we need to know the target height.
            }
            else
            {
                view.FadeToAsync(0, 250, Easing.CubicOut).ContinueWith((t) => 
                {
                    MainThread.BeginInvokeOnMainThread(() => 
                    {
                        view.IsVisible = false;
                    });
                });
            }
        }
    }

    public View AssociatedObject { get; private set; }

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);
        AssociatedObject = bindable;
        bindable.IsVisible = IsExpanded;
    }

    protected override void OnDetachingFrom(View bindable)
    {
        base.OnDetachingFrom(bindable);
        AssociatedObject = null;
    }
}
