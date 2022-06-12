using Innopixel.Kontrolpanel.MAUI.ViewModels.Authentication;
using Innopixel.Kontrolpanel.MAUI.Views.Authentication;
using Innopixel.Kontrolpanel.MAUI.Views.Displays;
using Innopixel.Kontrolpanel.MAUI.Views.Home;
using Innopixel.Kontrolpanel.MAUI.Views.Logs;
using Innopixel.Kontrolpanel.MAUI.Views.Media;
using Innopixel.Kontrolpanel.MAUI.Views.Profile;
using Microsoft.Toolkit.Mvvm.Input;

namespace Innopixel.Kontrolpanel.MAUI;

public partial class AppShell : Shell
{
	private bool isAdmin;

	public bool IsAdmin
	{
		get { return isAdmin; }
		set { isAdmin = value; OnPropertyChanged(nameof(IsAdmin)); }
	}

	public AppShell()
	{
		if (Preferences.ContainsKey("IsDarkModeEnabled"))
		{
			if (Preferences.Get("IsDarkModeEnabled", "") == "true")
			{
				App.Current.UserAppTheme = AppTheme.Dark;
			}
			else
			{
				App.Current.UserAppTheme = AppTheme.Light;
			}
		}

		MessagingCenter.Subscribe<LoginViewModel>(this, "admin", (sender) => { IsAdmin = true; });
		MessagingCenter.Subscribe<LoginViewModel>(this, "bruger", (sender) => { IsAdmin = false; });
		MessagingCenter.Subscribe<LoadingPageViewModel>(this, "admin", (sender) => { IsAdmin = true; });
		MessagingCenter.Subscribe<LoadingPageViewModel>(this, "bruger", (sender) => { IsAdmin = false; });

		InitializeComponent();
		BindingContext = this;

		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
		Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
		Routing.RegisterRoute(nameof(DisplaysPage), typeof(DisplaysPage));
		Routing.RegisterRoute(nameof(VideosPage), typeof(VideosPage));
		Routing.RegisterRoute(nameof(ImagesPage), typeof(ImagesPage));
		Routing.RegisterRoute(nameof(UploadImagePage), typeof(UploadImagePage));
		Routing.RegisterRoute(nameof(UploadVideoPage), typeof(UploadVideoPage));
		Routing.RegisterRoute(nameof(LogsPage), typeof(LogsPage));
		Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
	}

	[ICommand]
	private async Task LogOutAsync()
	{
		if (Preferences.Default.ContainsKey("JWT"))
		{
			Preferences.Default.Remove("JWT");
		}

		await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
	}
}