namespace Orbi.Web.ViewModels;

public record PaginationViewModel(
    int PageIndex,
    int TotalPages,
    string? SearchField,
    string? SearchTerm);
