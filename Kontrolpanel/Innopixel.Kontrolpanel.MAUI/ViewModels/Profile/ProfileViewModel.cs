using Innopixel.Kontrolpanel.Core.Utilities;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Security.Claims;

namespace Innopixel.Kontrolpanel.MAUI.ViewModels.Profile;

[INotifyPropertyChanged]
public partial class ProfileViewModel
{
	[ObservableProperty]
	private string userName;

	[ObservableProperty]
	private string email;

	[ObservableProperty]
	private string role;

	private bool isDarkModeEnabled;

	public bool IsDarkModeEnabled
	{
		get => isDarkModeEnabled;
		set { isDarkModeEnabled = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsDarkModeEnabledText)); AppThemeChanged(value); }
	}

	public string IsDarkModeEnabledText => (IsDarkModeEnabled) ? "Dark mode: Tændt" : "Dark mode: Slukket";

	public ProfileViewModel()
	{
		UserName = JwtParser.ParseClaimsFromJwt(Preferences.Default.Get("JWT", "")).Where(x => x.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
		Email = JwtParser.ParseClaimsFromJwt(Preferences.Default.Get("JWT", "")).Where(x => x.Type == ClaimTypes.Email).FirstOrDefault()?.Value;
		Role = JwtParser.ParseClaimsFromJwt(Preferences.Default.Get("JWT", "")).Where(x => x.Type == ClaimTypes.Role).FirstOrDefault()?.Value;
		IsDarkModeEnabled = (Application.Current.UserAppTheme == AppTheme.Dark) ? true : false;
	}

	private void AppThemeChanged(bool darkModeEnabled)
	{
		Application.Current.UserAppTheme = (darkModeEnabled) ? AppTheme.Dark : AppTheme.Light;
		Preferences.Default.Set("IsDarkModeEnabled", darkModeEnabled.ToString().ToLower());
	}
}