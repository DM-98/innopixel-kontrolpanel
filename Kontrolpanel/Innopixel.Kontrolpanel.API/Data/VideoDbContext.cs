using Innopixel.Kontrolpanel.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Innopixel.Kontrolpanel.API.Data;

public class VideoDbContext : DbContext
{
	public DbSet<Video>? Videos { get; set; }

	public VideoDbContext(DbContextOptions<VideoDbContext> options) : base(options)
	{
	}
}