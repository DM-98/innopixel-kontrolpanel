using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Services;
using System.Net;
using System.Text.Json;

namespace Innopixel.Kontrolpanel.Infrastructure.Services;

public class ImageService : Service<Image>
{
	private readonly HttpClient httpClient;

	public ImageService(HttpClient httpClient) : base(httpClient, "images")
	{
		this.httpClient = httpClient;
	}

	public async IAsyncEnumerable<Image?> SearchByNameAsync(string name)
	{
		string arg = WebUtility.HtmlEncode(name);
		string uri = $"images/name/{arg}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Image>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Image>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Image image in filteredList!)
			{
				yield return image;
			}
		}
		else
		{
			yield return null;
		}
	}

	public async IAsyncEnumerable<Image?> SearchByDescriptionAsync(string description)
	{
		string arg = WebUtility.HtmlEncode(description);
		string uri = $"images/description/{arg}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Image>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Image>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Image image in filteredList!)
			{
				yield return image;
			}
		}
		else
		{
			yield return null;
		}
	}

	public async IAsyncEnumerable<Image?> SearchByDateAsync(int fromDate, int toDate)
	{
		string arg1 = WebUtility.HtmlEncode(fromDate.ToString());
		string arg2 = WebUtility.HtmlEncode(toDate.ToString());
		string uri = $"images/from/{arg1}/to/{arg2}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Image>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Image>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Image image in filteredList!)
			{
				yield return image;
			}
		}
		else
		{
			yield return null;
		}
	}
}