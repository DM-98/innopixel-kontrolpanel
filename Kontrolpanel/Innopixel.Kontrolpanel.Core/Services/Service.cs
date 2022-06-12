using Innopixel.Kontrolpanel.Core.Interfaces;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Innopixel.Kontrolpanel.Core.Services;

public class Service<T> : IRepository<T> where T : class
{
	private readonly HttpClient httpClient;
	private readonly string controllerName;

	public Service(HttpClient httpClient, string controllerName)
	{
		this.httpClient = httpClient;
		this.controllerName = controllerName;
	}

	public async Task<T> CreateAsync(T entity)
	{
		HttpResponseMessage result = await httpClient.PostAsync(controllerName, new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json"));

		if (result.IsSuccessStatusCode)
		{
			T? response = JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })!;

			return response;
		}
		else
		{
			return null!;
		}
	}

	public async Task<bool> DeleteAsync(int entityId)
	{
		string uri = controllerName + "/" + WebUtility.UrlEncode(entityId.ToString());
		HttpResponseMessage result = await httpClient.DeleteAsync(uri);

		return result.IsSuccessStatusCode;
	}

	public async IAsyncEnumerable<T> GetAllAsync()
	{
		HttpResponseMessage result = await httpClient.GetAsync(controllerName);

		if (result.IsSuccessStatusCode)
		{
			IAsyncEnumerable<T>? response = await JsonSerializer.DeserializeAsync<IAsyncEnumerable<T>>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })!;

			await foreach (T entity in response!)
			{
				yield return entity;
			}
		}
		else
		{
			yield return null!;
		}
	}

	public async Task<T?> GetByIdAsync(int entityId)
	{
		string url = controllerName + "/" + WebUtility.UrlEncode(entityId.ToString());
		HttpResponseMessage result = await httpClient.GetAsync(url);

		if (result.IsSuccessStatusCode)
		{
			T? response = JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })!;

			return response;
		}
		else
		{
			return null;
		}
	}

	public async Task<T?> UpdateAsync(T entity)
	{
		HttpResponseMessage result = await httpClient.PutAsync(controllerName, new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json"));
		T? response = JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })!;

		return result.IsSuccessStatusCode ? response : result.StatusCode == HttpStatusCode.Conflict ? response : null;
	}
}