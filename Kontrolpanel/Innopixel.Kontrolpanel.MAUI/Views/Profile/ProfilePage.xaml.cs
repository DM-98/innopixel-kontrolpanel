using Innopixel.Kontrolpanel.MAUI.ViewModels.Profile;

namespace Innopixel.Kontrolpanel.MAUI.Views.Profile;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfileViewModel profileViewModel)
	{
		InitializeComponent();
		BindingContext = profileViewModel;
	}
}