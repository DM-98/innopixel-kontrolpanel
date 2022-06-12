using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Innopixel.Kontrolpanel.API.Controllers;

[Route("logs")]
[ApiController]
[Authorize(Roles = "Admin")]
public class LogsController : ControllerBase
{
	private readonly IRepositoryAPI<Log> context;

	public LogsController(IRepositoryAPI<Log> context)
	{
		this.context = context;
	}

	[HttpGet]
	public async IAsyncEnumerable<Log> GetLogsAsync()
	{
		IAsyncEnumerable<Log> result = context.GetAllAsync();

		await foreach (Log log in result)
		{
			yield return log;
		}
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Log>> GetLogByIdAsync(int id)
	{
		Log? log = await context.GetByIdAsync(id);

		if (log is not null)
		{
			return Ok(log);
		}
		else
		{
			return NotFound();
		}
	}

	[HttpGet("description/{description}")]
	public async IAsyncEnumerable<Log> GetLogsByDescriptionAsync(string description)
	{
		IAsyncEnumerable<Log> filteredList = context.FindAsync(x => x.Description.ToLower().Contains(description.ToLower()));

		await foreach (Log log in filteredList)
		{
			yield return log;
		}
	}

	[HttpGet("from/{fromDate}/to/{toDate}")]
	public async IAsyncEnumerable<Log> GetLogsByDateTimeAsync(int fromDate, int toDate)
	{
		IAsyncEnumerable<Log> filteredList = context.FindAsync(x => x.CreatedDate.Day >= fromDate && x.CreatedDate.Day <= toDate);

		await foreach (Log log in filteredList)
		{
			yield return log;
		}
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<ActionResult<Log>> PostLogAsync(Log log)
	{
		log.CreatedDate = DateTime.Now;

		await context.CreateAsync(log);

		if (await context.GetByIdAsync(log.Id) is not null)
		{
			return Ok(log);
		}
		else
		{
			return NotFound();
		}
	}

	[HttpPut]
	public async Task<ActionResult<Log>> PutLogAsync(Log log)
	{
		log.UpdatedDate = DateTime.Now;
		Log? updatedLog = await context.UpdateAsync(log);

		if (updatedLog is not null)
		{
			return Ok(updatedLog);
		}
		else
		{
			Log? concurrencyLog = await context.GetByIdAsync(log.Id);

			return Conflict(concurrencyLog);
		}
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteLogAsync(int id)
	{
		bool deleteSuccess = await context.DeleteAsync(id);

		return deleteSuccess ? NoContent() : NotFound();
	}
}