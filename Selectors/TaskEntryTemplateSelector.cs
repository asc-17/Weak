using Weak.Models;

namespace Weak.Selectors;

public class TaskEntryTemplateSelector : DataTemplateSelector
{
    public DataTemplate TaskTemplate { get; set; } = null!;
    public DataTemplate ListTemplate { get; set; } = null!;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        => item is TaskList ? ListTemplate : TaskTemplate;
}
