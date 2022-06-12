using Innopixel.Kontrolpanel.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Innopixel.Kontrolpanel.API.Data;

public class DisplayDbContext : DbContext
{
	public DbSet<Display>? Displays { get; set; }

	public DisplayDbContext(DbContextOptions<DisplayDbContext> options) : base(options)
	{
	}
}