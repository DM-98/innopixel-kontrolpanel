using Innopixel.Kontrolpanel.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Innopixel.Kontrolpanel.API.Data;

public class LogDbContext : DbContext
{
	public DbSet<Log>? Logs { get; set; }

	public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<Log>(b => b.HasOne(e => e.User).WithMany(a => a.Logs).HasForeignKey(x => x.UserId).HasConstraintName("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired());
	}
}