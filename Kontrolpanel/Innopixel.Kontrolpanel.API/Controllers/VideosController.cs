using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Innopixel.Kontrolpanel.API.Controllers;

[Route("videos")]
[ApiController]
[Authorize]
public class VideosController : ControllerBase
{
	private readonly IRepositoryAPI<Video> context;

	public VideosController(IRepositoryAPI<Video> context)
	{
		this.context = context;
	}

	[HttpGet]
	public async IAsyncEnumerable<Video> GetVideosAsync()
	{
		IAsyncEnumerable<Video> result = context.GetAllAsync();

		await foreach (Video video in result)
		{
			yield return video;
		}
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Video>> GetVideoByIdAsync(int id)
	{
		Video? video = await context.GetByIdAsync(id);

		if (video is not null)
		{
			return Ok(video);
		}
		else
		{
			return NotFound();
		}
	}

	[HttpGet("name/{name}")]
	public async IAsyncEnumerable<Video> GetVideosByNameAsync(string name)
	{
		IAsyncEnumerable<Video> filteredList = context.FindAsync(x => x.Name.ToLower().Contains(name.ToLower()));

		await foreach (Video video in filteredList)
		{
			yield return video;
		}
	}

	[HttpGet("description/{description}")]
	public async IAsyncEnumerable<Video> GetVideosByDescriptionAsync(string description)
	{
		IAsyncEnumerable<Video> filteredList = context.FindAsync(x => x.Description!.ToLower().Contains(description.ToLower()));

		await foreach (Video video in filteredList)
		{
			yield return video;
		}
	}

	[HttpGet("from/{fromDate}/to/{toDate}")]
	public async IAsyncEnumerable<Video> GetVideosByDateTimeAsync(int fromDate, int toDate)
	{
		IAsyncEnumerable<Video> filteredList = context.FindAsync(x => x.CreatedDate.Day >= fromDate && x.CreatedDate.Day <= toDate);

		await foreach (Video video in filteredList)
		{
			yield return video;
		}
	}

	[HttpPost]
	public async Task<ActionResult<Video>> PostVideoAsync(Video video)
	{
		video.CreatedDate = DateTime.Now;

		await context.CreateAsync(video);

		if (await context.GetByIdAsync(video.Id) is not null)
		{
			return Ok(video);
		}
		else
		{
			return NotFound();
		}
	}

	[HttpPut]
	public async Task<ActionResult<Video>> PutVideoAsync(Video video)
	{
		video.UpdatedDate = DateTime.Now;
		Video? updatedVideo = await context.UpdateAsync(video);

		if (updatedVideo is not null)
		{
			return Ok(updatedVideo);
		}
		else
		{
			Video? concurrencyVideo = await context.GetByIdAsync(video.Id);

			return Conflict(concurrencyVideo);
		}
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteVideoAsync(int id)
	{
		bool deleteSuccess = await context.DeleteAsync(id);

		return deleteSuccess ? NoContent() : NotFound();
	}
}