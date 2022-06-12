namespace Innopixel.Kontrolpanel.Core.Interfaces;

public interface IRepository<T> where T : class
{
	IAsyncEnumerable<T> GetAllAsync();

	Task<T?> GetByIdAsync(int entityId);

	Task<T> CreateAsync(T entity);

	Task<T?> UpdateAsync(T entity);

	Task<bool> DeleteAsync(int entityId);
}