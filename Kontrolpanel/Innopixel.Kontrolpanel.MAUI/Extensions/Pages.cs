using Innopixel.Kontrolpanel.MAUI.Views.Authentication;
using Innopixel.Kontrolpanel.MAUI.Views.Displays;
using Innopixel.Kontrolpanel.MAUI.Views.Home;
using Innopixel.Kontrolpanel.MAUI.Views.Logs;
using Innopixel.Kontrolpanel.MAUI.Views.Media;
using Innopixel.Kontrolpanel.MAUI.Views.Profile;

namespace Innopixel.Kontrolpanel.MAUI.Extensions;

public static class Pages
{
	public static MauiAppBuilder ConfigurePages(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<LoadingPage>();
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<DisplaysPage>();
		builder.Services.AddTransient<VideosPage>();
		builder.Services.AddTransient<ImagesPage>();
		builder.Services.AddTransient<UploadImagePage>();
		builder.Services.AddTransient<UploadVideoPage>();
		builder.Services.AddTransient<LogsPage>();
		builder.Services.AddTransient<ProfilePage>();

		return builder;
	}
}