using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Services;
using System.Net;
using System.Text.Json;

namespace Innopixel.Kontrolpanel.Infrastructure.Services;

public class LogService : Service<Log>
{
	private readonly HttpClient httpClient;

	public LogService(HttpClient httpClient) : base(httpClient, "logs")
	{
		this.httpClient = httpClient;
	}

	public async IAsyncEnumerable<Log?> SearchByDescriptionAsync(string description)
	{
		string arg = WebUtility.HtmlEncode(description);
		string uri = $"logs/description/{arg}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Log>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Log>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Log log in filteredList!)
			{
				yield return log;
			}
		}
		else
		{
			yield return null;
		}
	}

	public async IAsyncEnumerable<Log?> SearchByDateAsync(int fromDate, int toDate)
	{
		string arg1 = WebUtility.HtmlEncode(fromDate.ToString());
		string arg2 = WebUtility.HtmlEncode(toDate.ToString());
		string uri = $"logs/from/{arg1}/to/{arg2}";
		HttpResponseMessage result = await httpClient.GetAsync(uri);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<Log>? filteredList = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<Log>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })!;

			await foreach (Log log in filteredList!)
			{
				yield return log;
			}
		}
		else
		{
			yield return null;
		}
	}
}