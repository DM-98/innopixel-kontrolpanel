using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Innopixel.Kontrolpanel.API.Controllers
{
	[Authorize]
	[Route("upload")]
	[ApiController]
	public class UploadController : ControllerBase
	{
		private readonly IWebHostEnvironment webHostEnvironment;

		public record UploadResult(IFormFile FormFile);

		public UploadController(IWebHostEnvironment webHostEnvironment)
		{
			this.webHostEnvironment = webHostEnvironment;
		}

		[HttpPost]
		public async Task<ActionResult<string>> UploadFile([FromForm] UploadResult file)
		{
			try
			{
				if (file.FormFile.Length > 0)
				{
					if (!Directory.Exists(webHostEnvironment.WebRootPath + "\\web\\localuser\\faluf.com\\public_html\\wwwroot\\app\\"))
					{
						Directory.CreateDirectory(webHostEnvironment.WebRootPath + "\\web\\localuser\\faluf.com\\public_html\\wwwroot\\app\\");
					}
					using (FileStream fileStream = System.IO.File.Create(webHostEnvironment.WebRootPath + "\\web\\localuser\\faluf.com\\public_html\\wwwroot\\app\\" + file.FormFile.FileName))
					{
						await file.FormFile.CopyToAsync(fileStream);
						await fileStream.FlushAsync();

						return @"https://www.faluf.com/maui/" + file.FormFile.FileName;
					}
				}
				else
				{
					return BadRequest("Ingen filer valgt.");
				}
			}
			catch (Exception ex)
			{
				return ex.Message.ToString();
			}
		}
	}
}