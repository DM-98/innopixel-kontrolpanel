using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Services;
using Innopixel.Kontrolpanel.Infrastructure.Services;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Image = Innopixel.Kontrolpanel.Core.Domain.Image;

namespace Innopixel.Kontrolpanel.MAUI.Extensions;

public static class Services
{
	public static MauiAppBuilder ConfigureServices(this MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<Service<Display>, DisplayService>();
		builder.Services.AddSingleton<Service<Video>, VideoService>();
		builder.Services.AddSingleton<Service<Image>, ImageService>();
		builder.Services.AddSingleton<Service<Log>, LogService>();

		builder.Services.AddSingleton<IFingerprint>(CrossFingerprint.Current);
		builder.Services.AddSingleton(sp => new HttpClient() { BaseAddress = new Uri("https://api.faluf.com") });

		return builder;
	}
}