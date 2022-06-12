using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Innopixel.Kontrolpanel.MAUI.ViewModels.Home;

[INotifyPropertyChanged]
[QueryProperty(nameof(UserName), "username")]
public partial class HomeViewModel
{
	[ObservableProperty]
	private string userName;
}