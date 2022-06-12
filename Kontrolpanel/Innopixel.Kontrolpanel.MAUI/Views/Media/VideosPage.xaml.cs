using Innopixel.Kontrolpanel.MAUI.ViewModels.Media;

namespace Innopixel.Kontrolpanel.MAUI.Views.Media;

public partial class VideosPage : ContentPage
{
	public VideosPage(VideosViewModel videosViewModel)
	{
		InitializeComponent();
		BindingContext = videosViewModel;
	}

	protected override async void OnAppearing()
	{
		await ((VideosViewModel)BindingContext).LoadVideosAsync();
	}
}