using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Services;
using System.Net;
using System.Text.Json;

namespace Innopixel.Kontrolpanel.Infrastructure.Services;

public class VideoService : Service<Video>
{
	private readonly HttpClient httpClient;

	public VideoService(HttpClient httpClient) : base(httpClient, "videos")
	{
		this.httpClient = httpClient;
	}

	public async IAsyncEnumerable<Video> SearchByNameAsync(string name)
	{
		string arg = WebUtility.HtmlEncode(name);
		string uri = $"videos/name/{arg}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Video>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Video>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Video video in filteredList!)
			{
				yield return video;
			}
		}
		else
		{
			yield return null!;
		}
	}

	public async IAsyncEnumerable<Video> SearchByDescriptionAsync(string description)
	{
		string arg = WebUtility.HtmlEncode(description);
		string uri = $"videos/description/{arg}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Video>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Video>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Video video in filteredList!)
			{
				yield return video;
			}
		}
		else
		{
			yield return null!;
		}
	}

	public async IAsyncEnumerable<Video> SearchByDateAsync(int fromDate, int toDate)
	{
		string arg1 = WebUtility.HtmlEncode(fromDate.ToString());
		string arg2 = WebUtility.HtmlEncode(toDate.ToString());
		string uri = $"videos/from/{arg1}/to/{arg2}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Video>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Video>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Video video in filteredList!)
			{
				yield return video;
			}
		}
		else
		{
			yield return null!;
		}
	}
}