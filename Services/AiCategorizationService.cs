namespace Weak.Services;

public class AiCategorizationService
{
    public record Suggestion(int? Effort);

    public Suggestion Suggest(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return new Suggestion(null);

        var effort = EstimateEffort(title);

        return new Suggestion(effort);
    }

    private static int? EstimateEffort(string title)
    {
        var wordCount = title.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        if (wordCount >= 8)
            return 7;
        else if (wordCount >= 5)
            return 5;
        else if (wordCount >= 3)
            return 3;

        return null;
    }
}
