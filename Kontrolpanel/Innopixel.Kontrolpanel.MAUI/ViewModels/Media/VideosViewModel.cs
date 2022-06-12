using Acr.UserDialogs;
using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Services;
using Innopixel.Kontrolpanel.MAUI.Views.Media;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Innopixel.Kontrolpanel.MAUI.ViewModels.Media;

[INotifyPropertyChanged]
public partial class VideosViewModel
{
	private readonly Service<Video> videoService;

	[ObservableProperty]
	private bool hasNoConnection;

	[ObservableProperty]
	private ObservableCollection<Video> videos = new();

	[ObservableProperty]
	private bool isEmpty;

	[ObservableProperty]
	private bool isRefreshing;

	public VideosViewModel(Service<Video> videoService)
	{
		Connectivity.Current.ConnectivityChanged += Connectivity_ConnectivityChanged;
		this.videoService = videoService;
	}

	private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		HasNoConnection = (e.NetworkAccess != NetworkAccess.Internet) ? true : false;

		if (Shell.Current.CurrentPage is VideosPage)
		{
			if (e.NetworkAccess != NetworkAccess.Internet)
			{
				await Shell.Current.DisplayAlert("Forbindelse mistet!", "Du har mistet forbindelsen til internettet...", "Ok");
			}
			else
			{
				await Shell.Current.DisplayAlert("Forbindelse genoprettet!", "Du har nu internet, og videoer bliver nu genindlæst...", "Ok");
				await LoadVideosAsync();
			}
		}
	}

	[ICommand]
	internal async Task LoadVideosAsync()
	{
		if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
		{
			HasNoConnection = true;
			IsRefreshing = false;
			return;
		}

#if ANDROID || IOS || tvOS || Tizen
		UserDialogs.Instance.ShowLoading("Henter videoer fra databasen...");
#endif

		if (Videos.Count is not 0)
		{
			Videos.Clear();
		}

		try
		{
			await foreach (Video video in videoService.GetAllAsync().OrderBy(x => x.Id))
			{
				Videos.Add(video);
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		finally
		{
			if (Videos.Count is 0)
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
	private async Task UploadVideoAsync()
	{
		await Shell.Current.GoToAsync($"//{nameof(VideosPage)}/{nameof(UploadVideoPage)}");
	}
}