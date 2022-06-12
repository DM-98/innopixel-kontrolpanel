using Innopixel.Kontrolpanel.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Innopixel.Kontrolpanel.Infrastructure.Repositories;

public class EFRepository<T, TDbContext> : IRepositoryAPI<T> where T : class where TDbContext : DbContext
{
	private readonly TDbContext context;
	private readonly DbSet<T> table;

	public EFRepository(TDbContext context)
	{
		this.context = context;
		table = context.Set<T>();
	}

	public virtual async IAsyncEnumerable<T> GetAllAsync()
	{
		await foreach (T entity in table.AsAsyncEnumerable())
		{
			yield return entity;
		}
	}

	public virtual async IAsyncEnumerable<T> FindAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "")
	{
		IQueryable<T> query = table;

		if (filter is not null)
		{
			query = query.Where(filter);
		}

		foreach (string includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
		{
			query = query.Include(includeProperty);
		}

		if (orderBy is not null)
		{
			await foreach (T entity in orderBy(query).AsAsyncEnumerable())
			{
				yield return entity;
			}
		}
		else
		{
			await foreach (T entity in query.AsAsyncEnumerable())
			{
				yield return entity;
			}
		}
	}

	public virtual async Task<T?> GetByIdAsync(int entityId)
	{
		return await table.FindAsync(entityId);
	}

	public virtual async Task<T> CreateAsync(T entity)
	{
		await table.AddAsync(entity);
		await context.SaveChangesAsync();

		return entity;
	}

	public virtual async Task<T?> UpdateAsync(T entity)
	{
		context.Entry(entity).State = EntityState.Modified;

		try
		{
			await context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			return null;
		}

		return entity;
	}

	public virtual async Task<bool> DeleteAsync(int entityId)
	{
		T? entityToDelete = await table.FindAsync(entityId);

		if (entityToDelete is not null)
		{
			if (context.Entry(entityToDelete).State is EntityState.Detached)
			{
				table.Attach(entityToDelete);
			}

			table.Remove(entityToDelete);
		}

		return await context.SaveChangesAsync() >= 1;
	}
}