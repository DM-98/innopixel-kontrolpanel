using Innopixel.Kontrolpanel.MAUI.Extensions;

namespace Innopixel.Kontrolpanel.MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		MauiAppBuilder builder = MauiApp.CreateBuilder();

		builder.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureViewModels()
			.ConfigurePages()
			.ConfigureServices();

		return builder.Build();
	}
}