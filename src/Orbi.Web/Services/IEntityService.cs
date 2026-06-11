namespace Orbi.Web.Services;

public interface IEntityService<T, TViewModel>
    where T : class
    where TViewModel : class
{
    Task<IEnumerable<TViewModel>> GetAllAsync();
    Task<TViewModel?> GetByIdAsync(int id);
    Task CreateAsync(TViewModel viewModel);
    Task UpdateAsync(TViewModel viewModel);
    Task SoftDeleteAsync(int id);
}
