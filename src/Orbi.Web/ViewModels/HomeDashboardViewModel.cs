namespace Orbi.Web.ViewModels;

public record TableCountViewModel(string TableName, int Count, string? ControllerName, string Icon);

public class HomeDashboardViewModel
{
    public HomeDashboardViewModel(IReadOnlyList<TableCountViewModel> tableCounts)
    {
        TableCounts = tableCounts;
        TotalRecords = tableCounts.Sum(item => item.Count);
        MaxRecords = tableCounts.Count == 0 ? 0 : tableCounts.Max(item => item.Count);
    }

    public IReadOnlyList<TableCountViewModel> TableCounts { get; }

    public int TotalRecords { get; }

    public int MaxRecords { get; }
}
