using Innopixel.Kontrolpanel.Core.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Innopixel.Kontrolpanel.API.Data;

public class UserDbContext : IdentityDbContext<ApplicationUser>
{
	public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
	{
	}
}