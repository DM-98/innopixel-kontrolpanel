using Innopixel.Kontrolpanel.MAUI.ViewModels.Media;

namespace Innopixel.Kontrolpanel.MAUI.Views.Media;

public partial class UploadVideoPage : ContentPage
{
	public UploadVideoPage(UploadVideoViewModel uploadVideoViewModel)
	{
		InitializeComponent();
		BindingContext = uploadVideoViewModel;
	}

	protected override async void OnAppearing()
	{
		await ((UploadVideoViewModel)BindingContext).ChooseVideoAsync();
	}
}