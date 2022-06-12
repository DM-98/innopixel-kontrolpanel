using System.Linq.Expressions;

namespace Innopixel.Kontrolpanel.Core.Interfaces;

public interface IRepositoryAPI<T> : IRepository<T> where T : class
{
	IAsyncEnumerable<T> FindAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "");
}