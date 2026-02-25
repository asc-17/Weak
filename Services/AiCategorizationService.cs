namespace Weak.Services;

public class AiCategorizationService
{
    public record Suggestion(string? Subject, string? Category, int? Effort);

    private static readonly Dictionary<string, string> SubjectKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        // Math
        ["calculus"] = "Math",
        ["integral"] = "Math",
        ["algebra"] = "Math",
        ["statistics"] = "Math",
        ["geometry"] = "Math",
        ["differential"] = "Math",
        ["linear algebra"] = "Math",
        ["probability"] = "Math",

        // Science
        ["physics"] = "Physics",
        ["chemistry"] = "Chemistry",
        ["biology"] = "Biology",
        ["lab report"] = "Science",
        ["experiment"] = "Science",

        // English / Writing
        ["essay"] = "English",
        ["thesis"] = "English",
        ["literature"] = "English",
        ["writing"] = "English",
        ["composition"] = "English",
        ["reading"] = "English",
        ["grammar"] = "English",

        // Computer Science
        ["programming"] = "Computer Science",
        ["algorithm"] = "Computer Science",
        ["data structure"] = "Computer Science",
        ["coding"] = "Computer Science",
        ["software"] = "Computer Science",
        ["database"] = "Computer Science",

        // History
        ["history"] = "History",
        ["civilization"] = "History",
        ["war"] = "History",

        // Economics
        ["economics"] = "Economics",
        ["microeconomics"] = "Economics",
        ["macroeconomics"] = "Economics",
        ["finance"] = "Economics",

        // Psychology
        ["psychology"] = "Psychology",
        ["behavioral"] = "Psychology",
        ["cognitive"] = "Psychology",
    };

    private static readonly Dictionary<string, string> CategoryKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        // Exam
        ["exam"] = "Exam",
        ["midterm"] = "Exam",
        ["final"] = "Exam",
        ["test"] = "Exam",
        ["quiz"] = "Exam",

        // Homework
        ["assignment"] = "Homework",
        ["homework"] = "Homework",
        ["problem set"] = "Homework",
        ["worksheet"] = "Homework",
        ["exercise"] = "Homework",

        // Lab
        ["lab"] = "Lab",
        ["report"] = "Lab",
        ["experiment"] = "Lab",

        // Project
        ["project"] = "Project",
        ["presentation"] = "Project",
        ["group work"] = "Project",
        ["poster"] = "Project",

        // Reading
        ["reading"] = "Reading",
        ["chapter"] = "Reading",
        ["textbook"] = "Reading",
    };

    public Suggestion Suggest(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return new Suggestion(null, null, null);

        var subject = MatchKeyword(title, SubjectKeywords);
        var category = MatchKeyword(title, CategoryKeywords);
        var effort = EstimateEffort(title);

        return new Suggestion(subject, category, effort);
    }

    private static string? MatchKeyword(string title, Dictionary<string, string> keywords)
    {
        // Try multi-word keywords first (longer = more specific)
        foreach (var kvp in keywords.OrderByDescending(k => k.Key.Length))
        {
            if (title.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }
        return null;
    }

    private static int? EstimateEffort(string title)
    {
        var wordCount = title.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        // Longer, more descriptive titles suggest more complex tasks
        if (wordCount >= 8)
            return 7;
        else if (wordCount >= 5)
            return 5;
        else if (wordCount >= 3)
            return 3;

        return null; // Not enough signal
    }
}
