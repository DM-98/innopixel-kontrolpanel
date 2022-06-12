using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Services;
using System.Net;
using System.Text.Json;

namespace Innopixel.Kontrolpanel.Infrastructure.Services;

public class DisplayService : Service<Display>
{
	private readonly HttpClient httpClient;

	public DisplayService(HttpClient httpClient) : base(httpClient, "displays")
	{
		this.httpClient = httpClient;
	}

	public async IAsyncEnumerable<Display?> SearchByNameAsync(string name)
	{
		string arg = WebUtility.HtmlEncode(name);
		string uri = $"displays/name/{arg}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Display>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Display>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Display display in filteredList!)
			{
				yield return display;
			}
		}
		else
		{
			yield return null;
		}
	}

	public async IAsyncEnumerable<Display?> SearchByDescriptionAsync(string description)
	{
		string arg = WebUtility.HtmlEncode(description);
		string uri = $"displays/description/{arg}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Display>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Display>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Display display in filteredList!)
			{
				yield return display;
			}
		}
		else
		{
			yield return null;
		}
	}

	public async IAsyncEnumerable<Display?> SearchByDateAsync(int fromDate, int toDate)
	{
		string arg1 = WebUtility.HtmlEncode(fromDate.ToString());
		string arg2 = WebUtility.HtmlEncode(toDate.ToString());
		string uri = $"displays/from/{arg1}/to/{arg2}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Display>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Display>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Display display in filteredList!)
			{
				yield return display;
			}
		}
		else
		{
			yield return null;
		}
	}
}