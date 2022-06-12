#if ANDROID

using Android.Content.Res;
using Android.Graphics.Drawables;

#endif

using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Innopixel.Kontrolpanel.MAUI;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

#if ANDROID29_0_OR_GREATER
		EntryHandler.Mapper.AppendToMapping("Entry", (handler, view) =>
		{
			GradientDrawable gradientDrawable = new();
			gradientDrawable.SetCornerRadius(10f);
			gradientDrawable.SetStroke(3, Color.FromArgb("#202020").ToPlatform());
			gradientDrawable.SetColor(Color.FromArgb("#214354").ToPlatform());
			gradientDrawable.SetPadding(20, 15, 20, 15);
			handler.PlatformView.SetBackground(gradientDrawable);
			handler.PlatformView.SetHighlightColor(Color.FromArgb("#3254A8").ToPlatform());
			handler.PlatformView.SetTextColor(Colors.White.ToPlatform());
			handler.PlatformView.UpdatePlaceholderColor(Color.FromArgb("#BEBEBE"));
		});
#endif

		MainPage = new AppShell();
	}
}