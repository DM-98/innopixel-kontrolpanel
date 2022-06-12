using Acr.UserDialogs;
using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Services;
using Innopixel.Kontrolpanel.MAUI.Views.Displays;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Innopixel.Kontrolpanel.MAUI.ViewModels.Displays;

[INotifyPropertyChanged]
public partial class DisplaysViewModel
{
	private readonly Service<Display> displayService;

	[ObservableProperty]
	private bool hasNoConnection;

	[ObservableProperty]
	private ObservableCollection<Display> displays = new();

	[ObservableProperty]
	private bool isEmpty;

	[ObservableProperty]
	private bool isRefreshing;

	public DisplaysViewModel(Service<Display> displayService)
	{
		Connectivity.Current.ConnectivityChanged += Connectivity_ConnectivityChanged;
		this.displayService = displayService;
	}

	private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		HasNoConnection = (e.NetworkAccess != NetworkAccess.Internet) ? true : false;

		if (Shell.Current.CurrentPage is DisplaysPage)
		{
			if (e.NetworkAccess != NetworkAccess.Internet)
			{
				await Shell.Current.DisplayAlert("Forbindelse mistet!", "Du har mistet forbindelsen til internettet...", "Ok");
			}
			else
			{
				await Shell.Current.DisplayAlert("Forbindelse genoprettet!", "Du har nu internet, og displays bliver nu genindlæst...", "Ok");
				await LoadDisplaysAsync();
			}
		}
	}

	[ICommand]
	internal async Task LoadDisplaysAsync()
	{
		if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
		{
			HasNoConnection = true;
			IsRefreshing = false;
			return;
		}

#if ANDROID || IOS || tvOS || Tizen
		UserDialogs.Instance.ShowLoading("Henter displays fra databasen...");
#endif

		if (Displays.Count is not 0)
		{
			Displays.Clear();
		}

		try
		{
			await foreach (Display display in displayService.GetAllAsync().OrderBy(x => x.Id))
			{
				Displays.Add(display);
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		finally
		{
			if (Displays.Count is 0)
			{
				IsEmpty = true;
			}
			else
			{
				IsEmpty = false;
			}

			IsRefreshing = false;
#if ANDROID || IOS || tvOS || Tizen
			UserDialogs.Instance.HideLoading();
#endif
		}
	}

	[ICommand]
	private async Task UploadDisplayAsync()
	{
		await Shell.Current.DisplayAlert("Tilføj et nyt display", "Under opbygning - kommer senere!", "OK");
	}
}