using Innopixel.Kontrolpanel.MAUI.ViewModels.Authentication;
using Innopixel.Kontrolpanel.MAUI.ViewModels.Displays;
using Innopixel.Kontrolpanel.MAUI.ViewModels.Home;
using Innopixel.Kontrolpanel.MAUI.ViewModels.Logs;
using Innopixel.Kontrolpanel.MAUI.ViewModels.Media;
using Innopixel.Kontrolpanel.MAUI.ViewModels.Profile;

namespace Innopixel.Kontrolpanel.MAUI.Extensions;

public static class ViewModels
{
	public static MauiAppBuilder ConfigureViewModels(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<LoadingPageViewModel>();
		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<DisplaysViewModel>();
		builder.Services.AddTransient<VideosViewModel>();
		builder.Services.AddTransient<ImagesViewModel>();
		builder.Services.AddTransient<UploadImageViewModel>();
		builder.Services.AddTransient<UploadVideoViewModel>();
		builder.Services.AddTransient<LogsViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();

		return builder;
	}
}