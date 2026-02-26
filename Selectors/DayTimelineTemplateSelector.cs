using Weak.Models;

namespace Weak.Selectors;

public class DayTimelineTemplateSelector : DataTemplateSelector
{
    public DataTemplate TaskTemplate { get; set; } = null!;
    public DataTemplate MarkerTemplate { get; set; } = null!;
    public DataTemplate ListTemplate { get; set; } = null!;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is TimeMarker)
            return MarkerTemplate;
        if (item is TaskList)
            return ListTemplate;
        return TaskTemplate;
    }
}
