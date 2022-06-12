using Innopixel.Kontrolpanel.MAUI.ViewModels.Authentication;

namespace Innopixel.Kontrolpanel.MAUI.Views.Authentication;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel loginViewModel)
	{
		InitializeComponent();
		BindingContext = loginViewModel;
	}
}