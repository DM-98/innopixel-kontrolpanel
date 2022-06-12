using Innopixel.Kontrolpanel.Core.Utilities;
using Innopixel.Kontrolpanel.MAUI.Views.Authentication;
using Innopixel.Kontrolpanel.MAUI.Views.Home;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Innopixel.Kontrolpanel.MAUI.ViewModels.Authentication;

public partial class LoadingPageViewModel
{
	private readonly HttpClient httpClient;

	public LoadingPageViewModel(HttpClient httpClient)
	{
		this.httpClient = httpClient;
		CheckJWTCache();
	}

	private async void CheckJWTCache()
	{
		await Task.Delay(2000);
		string token = Preferences.Default.Get("JWT", "");

		if (string.IsNullOrWhiteSpace(token))
		{
			await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
		}
		else
		{
			string role = JwtParser.ParseClaimsFromJwt(token).Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value.ToLower();
			string userName = JwtParser.ParseClaimsFromJwt(token).Where(x => x.Type == ClaimTypes.Name).FirstOrDefault().Value;

			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			MessagingCenter.Send<LoadingPageViewModel>(this, role);
			await Shell.Current.GoToAsync($"//{nameof(HomePage)}?username={userName}");
		}
	}
}