using Innopixel.Kontrolpanel.Core.Utilities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Innopixel.Kontrolpanel.Blazor.Authentication;

public class AppState : AuthenticationStateProvider
{
	private readonly HttpClient httpClient;
	private readonly ProtectedLocalStorage protectedLocalStorage;

	public AppState(HttpClient httpClient, ProtectedLocalStorage protectedLocalStorage)
	{
		this.httpClient = httpClient;
		this.protectedLocalStorage = protectedLocalStorage;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		ProtectedBrowserStorageResult<string> version = await protectedLocalStorage.GetAsync<string>("authVersion");

		if (version.Success)
		{
			if (typeof(Program).Assembly?.GetName().Version?.ToString() != version.Value)
			{
				await protectedLocalStorage.DeleteAsync("authToken");
			}
		}

		ProtectedBrowserStorageResult<string> token = await protectedLocalStorage.GetAsync<string>("authToken");

		if (token.Success)
		{
			ClaimsIdentity user = new(JwtParser.ParseClaimsFromJwt(token.Value!), "jwtAuthType");
			DateTimeOffset exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(user.FindFirst("exp")!.Value));

			if (exp.ToLocalTime() > DateTime.Now)
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

				return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(user)));
			}
		}

		await protectedLocalStorage.DeleteAsync("authToken");
		httpClient.DefaultRequestHeaders.Authorization = null;
		NotifyUserLogout();

		return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
	}

	public void NotifyUserAuthentication(string token)
	{
		Task<AuthenticationState> authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"))));
		NotifyAuthenticationStateChanged(authState);
	}

	public void NotifyUserLogout()
	{
		Task<AuthenticationState> authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
		NotifyAuthenticationStateChanged(authState);
	}
}