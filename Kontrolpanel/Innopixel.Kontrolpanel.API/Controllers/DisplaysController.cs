using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Innopixel.Kontrolpanel.API.Controllers;

[Route("displays")]
[ApiController]
[Authorize]
public class DisplaysController : ControllerBase
{
	private readonly IRepositoryAPI<Display> context;

	public DisplaysController(IRepositoryAPI<Display> context)
	{
		this.context = context;
	}

	[HttpGet]
	public async IAsyncEnumerable<Display> GetDisplaysAsync()
	{
		IAsyncEnumerable<Display> result = context.GetAllAsync();

		await foreach (Display display in result)
		{
			yield return display;
		}
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Display>> GetDisplayByIdAsync(int id)
	{
		Display? display = await context.GetByIdAsync(id);

		if (display is not null)
		{
			return Ok(display);
		}
		else
		{
			return NotFound();
		}
	}

	[HttpGet("name/{name}")]
	public async IAsyncEnumerable<Display> GetDisplaysByNameAsync(string name)
	{
		IAsyncEnumerable<Display> filteredList = context.FindAsync(x => x.Name.ToLower().Contains(name.ToLower()));

		await foreach (Display display in filteredList)
		{
			yield return display;
		}
	}

	[HttpGet("description/{description}")]
	public async IAsyncEnumerable<Display> GetDisplaysByDescriptionAsync(string description)
	{
		IAsyncEnumerable<Display> filteredList = context.FindAsync(x => x.Description!.ToLower().Contains(description.ToLower()));

		await foreach (Display display in filteredList)
		{
			yield return display;
		}
	}

	[HttpGet("from/{fromDate}/to/{toDate}")]
	public async IAsyncEnumerable<Display> GetDisplaysByDateTimeAsync(int fromDate, int toDate)
	{
		IAsyncEnumerable<Display> filteredList = context.FindAsync(x => x.CreatedDate.Day >= fromDate && x.CreatedDate.Day <= toDate);

		await foreach (Display display in filteredList)
		{
			yield return display;
		}
	}

	[HttpPost]
	public async Task<ActionResult<Display>> PostDisplayAsync(Display display)
	{
		display.CreatedDate = DateTime.Now;

		await context.CreateAsync(display);

		if (await context.GetByIdAsync(display.Id) is not null)
		{
			return Ok(display);
		}
		else
		{
			return NotFound();
		}
	}

	[HttpPut]
	public async Task<ActionResult<Display>> PutDisplayAsync(Display display)
	{
		display.UpdatedDate = DateTime.Now;
		Display? updatedDisplay = await context.UpdateAsync(display);

		if (updatedDisplay is not null)
		{
			return Ok(updatedDisplay);
		}
		else
		{
			Display? concurrencyDisplay = await context.GetByIdAsync(display.Id);

			return Conflict(concurrencyDisplay);
		}
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteDisplayAsync(int id)
	{
		bool deleteSuccess = await context.DeleteAsync(id);

		return deleteSuccess ? NoContent() : NotFound();
	}
}