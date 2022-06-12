using Innopixel.Kontrolpanel.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Innopixel.Kontrolpanel.API.Controllers;

[Route("users")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
	private readonly UserManager<ApplicationUser> userManager;

	public UsersController(UserManager<ApplicationUser> userManager)
	{
		this.userManager = userManager;
	}

	[HttpGet]
	public async IAsyncEnumerable<ApplicationUser> GetUsersAsync()
	{
		await foreach (ApplicationUser user in userManager.Users.AsAsyncEnumerable())
		{
			yield return user;
		}
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<ApplicationUser>> GetUserByIdAsync(string id)
	{
		ApplicationUser? user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);

		if (user is not null)
		{
			return Ok(user);
		}
		else
		{
			return NotFound();
		}
	}

	[HttpGet("email/{email}")]
	public async IAsyncEnumerable<ApplicationUser> GetUserByEmailAsync(string email)
	{
		IAsyncEnumerable<ApplicationUser> filteredList = userManager.Users.Where(x => x.Email.ToLower().Contains(email.ToLower())).AsAsyncEnumerable();

		await foreach (ApplicationUser user in filteredList)
		{
			yield return user;
		}
	}

	[HttpGet("from/{fromDate}/to/{toDate}")]
	public async IAsyncEnumerable<ApplicationUser> GetUserByDateTimeAsync(int fromDate, int toDate)
	{
		IAsyncEnumerable<ApplicationUser> filteredList = userManager.Users.Where(x => x.CreatedDate.Day >= fromDate && x.CreatedDate.Day <= toDate).AsAsyncEnumerable();

		await foreach (ApplicationUser user in filteredList)
		{
			yield return user;
		}
	}

	[HttpPut]
	public async Task<ActionResult<ApplicationUser>> PutUserAsync(ApplicationUser userToUpdate)
	{
		userToUpdate.UpdatedDate = DateTime.Now;
		IdentityResult result = await userManager.UpdateAsync(userToUpdate);

		try
		{
			await userManager.UpdateAsync(userToUpdate);
		}
		catch (DbUpdateConcurrencyException)
		{
			ApplicationUser? databaseUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userToUpdate.Id);

			if (databaseUser is null)
			{
				return NotFound();
			}
			else
			{
				return Conflict(databaseUser);
			}
		}

		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteUserAsync(string id)
	{
		ApplicationUser? user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);

		if (user is null)
		{
			return NotFound();
		}

		await userManager.DeleteAsync(user);

		return NoContent();
	}
}