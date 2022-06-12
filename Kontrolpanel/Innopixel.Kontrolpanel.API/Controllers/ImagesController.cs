using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Innopixel.Kontrolpanel.API.Controllers;

[Route("images")]
[ApiController]
[Authorize]
public class ImagesController : ControllerBase
{
	private readonly IRepositoryAPI<Image> context;

	public ImagesController(IRepositoryAPI<Image> context)
	{
		this.context = context;
	}

	[HttpGet]
	public async IAsyncEnumerable<Image> GetImagesAsync()
	{
		IAsyncEnumerable<Image> result = context.GetAllAsync();

		await foreach (Image image in result)
		{
			yield return image;
		}
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Image>> GetImageByIdAsync(int id)
	{
		Image? image = await context.GetByIdAsync(id);

		if (image is not null)
		{
			return Ok(image);
		}
		else
		{
			return NotFound();
		}
	}

	[HttpGet("name/{name}")]
	public async IAsyncEnumerable<Image> GetImagesByNameAsync(string name)
	{
		IAsyncEnumerable<Image> filteredList = context.FindAsync(x => x.Name.ToLower().Contains(name.ToLower()));

		await foreach (Image image in filteredList)
		{
			yield return image;
		}
	}

	[HttpGet("description/{description}")]
	public async IAsyncEnumerable<Image> GetImagesByDescriptionAsync(string description)
	{
		IAsyncEnumerable<Image> filteredList = context.FindAsync(x => x.Description!.ToLower().Contains(description.ToLower()));

		await foreach (Image image in filteredList)
		{
			yield return image;
		}
	}

	[HttpGet("from/{fromDate}/to/{toDate}")]
	public async IAsyncEnumerable<Image> GetImagesByDateTimeAsync(int fromDate, int toDate)
	{
		IAsyncEnumerable<Image> filteredList = context.FindAsync(x => x.CreatedDate.Day >= fromDate && x.CreatedDate.Day <= toDate);

		await foreach (Image image in filteredList)
		{
			yield return image;
		}
	}

	[HttpPost]
	public async Task<ActionResult<Image>> PostImageAsync(Image image)
	{
		image.CreatedDate = DateTime.Now;

		await context.CreateAsync(image);

		if (await context.GetByIdAsync(image.Id) is not null)
		{
			return Ok(image);
		}
		else
		{
			return NotFound();
		}
	}

	[HttpPut]
	public async Task<ActionResult<Image>> PutImageAsync(Image image)
	{
		image.UpdatedDate = DateTime.Now;
		Image? updatedImage = await context.UpdateAsync(image);

		if (updatedImage is not null)
		{
			return Ok(updatedImage);
		}
		else
		{
			Image? concurrencyImage = await context.GetByIdAsync(image.Id);

			return Conflict(concurrencyImage);
		}
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteImageAsync(int id)
	{
		bool deleteSuccess = await context.DeleteAsync(id);

		return deleteSuccess ? NoContent() : NotFound();
	}
}