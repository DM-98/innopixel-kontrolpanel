using Innopixel.Kontrolpanel.Core.DTOs;
using Innopixel.Kontrolpanel.Core.Utilities;
using Innopixel.Kontrolpanel.MAUI.Views.Authentication;
using Innopixel.Kontrolpanel.MAUI.Views.Home;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Plugin.Fingerprint.Abstractions;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Innopixel.Kontrolpanel.MAUI.ViewModels.Authentication;

[INotifyPropertyChanged]
public partial class LoginViewModel
{
	private readonly HttpClient httpClient;
	private readonly IFingerprint fingerprint;

	[ObservableProperty]
	private bool hasNoConnection;

	[ObservableProperty]
	private LoginDTO loginModel = new();

	[ObservableProperty]
	private string response;

	public LoginViewModel(HttpClient httpClient, IFingerprint fingerprint)
	{
		Connectivity.Current.ConnectivityChanged += Connectivity_ConnectivityChanged;
		this.httpClient = httpClient;
		this.fingerprint = fingerprint;
	}

	private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		HasNoConnection = (e.NetworkAccess != NetworkAccess.Internet) ? true : false;

		if (Shell.Current.CurrentPage is LoginPage)
		{
			if (e.NetworkAccess != NetworkAccess.Internet)
			{
				await Shell.Current.DisplayAlert("Forbindelse mistet!", "Du har mistet forbindelsen til internettet...", "Ok");
			}
			else
			{
				await Shell.Current.DisplayAlert("Forbindelse genoprettet!", "Du har nu internet, og kan nu logge ind...", "Ok");
			}
		}
	}

	[ICommand]
	private async Task LoginAsync()
	{
		if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
		{
			HasNoConnection = true;
			return;
		}

		if (string.IsNullOrWhiteSpace(LoginModel.Email) || string.IsNullOrWhiteSpace(LoginModel.Password))
		{
			Response = "Udfyld venligst alle felter";
			return;
		}

		AuthenticationRequestConfiguration request = new("Log ind på Innopixel Kontrolpanel", "Bevis, at du ikke er en robot");
		FingerprintAuthenticationResult result = await fingerprint.AuthenticateAsync(request);

		if (result.Authenticated)
		{
			StringContent content = new(JsonSerializer.Serialize(LoginModel), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await httpClient.PostAsync("/auth/login", content);

			if (response.IsSuccessStatusCode)
			{
				string token = await response.Content.ReadAsStringAsync();
				string role = JwtParser.ParseClaimsFromJwt(token).Where(x => x.Type == ClaimTypes.Role).FirstOrDefault()?.Value.ToLower();
				string userName = JwtParser.ParseClaimsFromJwt(token).Where(x => x.Type == ClaimTypes.Name).FirstOrDefault().Value;

				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				Preferences.Default.Set("JWT", token);
				MessagingCenter.Send<LoginViewModel>(this, role);
				await Shell.Current.GoToAsync($"//{nameof(HomePage)}?username={userName}");

				LoginModel = new();
				Response = String.Empty;
			}
			else
			{
				Response = response.StatusCode + " - " + await response.Content.ReadAsStringAsync();
			}
		}
		else
		{
			Response = "Biometri ikke godkendt, prøv igen senere.";
			await Shell.Current.DisplayAlert("Biometri ikke godkendt!", "Adgang nægtet", "Luk");
		}
	}

	[ICommand]
	private void Tapped(CheckBox checkBox)
	{
		if (checkBox.IsChecked)
		{
			LoginModel.RememberMe = false;
			checkBox.IsChecked = false;
		}
		else
		{
			LoginModel.RememberMe = true;
			checkBox.IsChecked = true;
		}
	}
}