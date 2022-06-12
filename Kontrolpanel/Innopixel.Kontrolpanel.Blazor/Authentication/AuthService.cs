using Innopixel.Kontrolpanel.Core.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Innopixel.Kontrolpanel.Blazor.Authentication;

public class AuthService
{
	private readonly HttpClient httpClient;
	private readonly AuthenticationStateProvider authStateProvider;
	private readonly ProtectedLocalStorage protectedLocalStorage;
	private readonly IConfiguration configuration;

	public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ProtectedLocalStorage protectedLocalStorage, IConfiguration configuration)
	{
		this.httpClient = httpClient;
		this.authStateProvider = authStateProvider;
		this.protectedLocalStorage = protectedLocalStorage;
		this.configuration = configuration;
	}

	public async Task<bool> Login(LoginDTO loginModel)
	{
		HttpResponseMessage response = await httpClient.PostAsync(configuration["API"] + "/auth/login", new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json"));

		if (response.IsSuccessStatusCode)
		{
			string token = await response.Content.ReadAsStringAsync();

			await protectedLocalStorage.SetAsync("authToken", token);
			await protectedLocalStorage.SetAsync("authVersion", typeof(Program).Assembly.GetName().Version?.ToString()!);
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			((AppState)authStateProvider).NotifyUserAuthentication(token);

			return true;
		}

		return false;
	}

	public async Task Logout()
	{
		await protectedLocalStorage.DeleteAsync("authToken");
		httpClient.DefaultRequestHeaders.Authorization = null;
		((AppState)authStateProvider).NotifyUserLogout();
	}
}