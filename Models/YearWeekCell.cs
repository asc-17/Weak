namespace Weak.Models;

public class YearWeekCell
{
    public int WeekNumber { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public double LoadScore { get; set; }
    public string Intensity { get; set; } = "Low";
    public string DateRange => $"{WeekStart:MMM dd} – {WeekEnd:MMM dd}";

    public Color CellColor => Intensity switch
    {
        "Low" => Color.FromArgb("#dcfce7"),
        "Moderate" => Color.FromArgb("#fef9c3"),
        "High" => Color.FromArgb("#ffedd5"),
        "Critical" => Color.FromArgb("#fee2e2"),
        _ => Color.FromArgb("#f3f4f6")
    };
}
