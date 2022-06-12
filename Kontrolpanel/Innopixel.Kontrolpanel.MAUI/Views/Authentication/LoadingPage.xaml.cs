using Innopixel.Kontrolpanel.MAUI.ViewModels.Authentication;

namespace Innopixel.Kontrolpanel.MAUI.Views.Authentication;

public partial class LoadingPage : ContentPage
{
	public LoadingPage(LoadingPageViewModel loadingPageViewModel)
	{
		InitializeComponent();
		BindingContext = loadingPageViewModel;
	}
}