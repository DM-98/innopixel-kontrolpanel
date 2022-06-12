using Innopixel.Kontrolpanel.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Innopixel.Kontrolpanel.API.Data;

public class ImageDbContext : DbContext
{
	public DbSet<Image>? Images { get; set; }

	public ImageDbContext(DbContextOptions<ImageDbContext> options) : base(options)
	{
	}
}