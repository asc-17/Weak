using Weak.Models;

namespace Weak.Selectors;

public class OnboardingTemplateSelector : DataTemplateSelector
{
    public DataTemplate? WelcomeTemplate { get; set; }
    public DataTemplate? NameTemplate { get; set; }
    public DataTemplate? TimesTemplate { get; set; }
    public DataTemplate? WeekStartTemplate { get; set; }
    public DataTemplate? GoogleTemplate { get; set; }
    public DataTemplate? DoneTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is OnboardingStep step)
        {
            return step.StepType switch
            {
                "welcome" => WelcomeTemplate!,
                "name" => NameTemplate!,
                "times" => TimesTemplate!,
                "weekstart" => WeekStartTemplate!,
                "google" => GoogleTemplate!,
                "done" => DoneTemplate!,
                _ => WelcomeTemplate!
            };
        }
        return WelcomeTemplate!;
    }
}
