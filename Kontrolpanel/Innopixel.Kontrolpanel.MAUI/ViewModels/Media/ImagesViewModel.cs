using Acr.UserDialogs;
using Innopixel.Kontrolpanel.Core.Services;
using Innopixel.Kontrolpanel.MAUI.Views.Media;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Image = Innopixel.Kontrolpanel.Core.Domain.Image;

namespace Innopixel.Kontrolpanel.MAUI.ViewModels.Media;

[INotifyPropertyChanged]
public partial class ImagesViewModel
{
	private readonly Service<Image> imageService;

	[ObservableProperty]
	private bool hasNoConnection;

	[ObservableProperty]
	private ObservableCollection<Image> images = new();

	[ObservableProperty]
	private bool isEmpty;

	[ObservableProperty]
	private bool isRefreshing;

	public ImagesViewModel(Service<Image> imageService)
	{
		Connectivity.Current.ConnectivityChanged += Connectivity_ConnectivityChanged;
		this.imageService = imageService;
	}

	private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		HasNoConnection = (e.NetworkAccess != NetworkAccess.Internet) ? true : false;

		if (Shell.Current.CurrentPage is ImagesPage)
		{
			if (e.NetworkAccess != NetworkAccess.Internet)
			{
				await Shell.Current.DisplayAlert("Forbindelse mistet!", "Du har mistet forbindelsen til internettet...", "Ok");
			}
			else
			{
				await Shell.Current.DisplayAlert("Forbindelse genoprettet!", "Du har nu internet, og billeder bliver nu genindlæst...", "Ok");
				await LoadImagesAsync();
			}
		}
	}

	[ICommand]
	internal async Task LoadImagesAsync()
	{
		if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
		{
			HasNoConnection = true;
			IsRefreshing = false;
			return;
		}

#if ANDROID || IOS || tvOS || Tizen
		UserDialogs.Instance.ShowLoading("Henter billeder fra databasen...");
#endif

		if (Images.Count is not 0)
		{
			Images.Clear();
		}

		try
		{
			await foreach (Image image in imageService.GetAllAsync().OrderBy(x => x.Id))
			{
				Images.Add(image);
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		finally
		{
			if (Images.Count is 0)
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
	private async Task UploadImageAsync()
	{
		await Shell.Current.GoToAsync($"//{nameof(ImagesPage)}/{nameof(UploadImagePage)}");
	}
}