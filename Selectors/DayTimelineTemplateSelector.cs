using Weak.Models;

namespace Weak.Selectors;

public class DayTimelineTemplateSelector : DataTemplateSelector
{
    public DataTemplate TaskTemplate { get; set; } = null!;
    public DataTemplate MarkerTemplate { get; set; } = null!;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        => item is TimeMarker ? MarkerTemplate : TaskTemplate;
}
