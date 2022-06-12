using Innopixel.Kontrolpanel.MAUI.ViewModels.Home;

namespace Innopixel.Kontrolpanel.MAUI.Views.Home;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel homeViewModel)
	{
		InitializeComponent();
		BindingContext = homeViewModel;
	}
}