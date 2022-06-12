using Innopixel.Kontrolpanel.MAUI.ViewModels.Media;

namespace Innopixel.Kontrolpanel.MAUI.Views.Media;

public partial class UploadImagePage : ContentPage
{
	public UploadImagePage(UploadImageViewModel uploadImageViewModel)
	{
		InitializeComponent();
		BindingContext = uploadImageViewModel;
	}

	protected override async void OnAppearing()
	{
		await ((UploadImageViewModel)BindingContext).ChooseImageAsync();
	}
}