using Innopixel.Kontrolpanel.MAUI.ViewModels.Media;

namespace Innopixel.Kontrolpanel.MAUI.Views.Media;

public partial class ImagesPage : ContentPage
{
	public ImagesPage(ImagesViewModel imagesViewModel)
	{
		InitializeComponent();
		BindingContext = imagesViewModel;
	}

	protected override async void OnAppearing()
	{
		await ((ImagesViewModel)BindingContext).LoadImagesAsync();
	}
}