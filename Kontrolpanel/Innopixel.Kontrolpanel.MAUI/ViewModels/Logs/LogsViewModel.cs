using Acr.UserDialogs;
using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Services;
using Innopixel.Kontrolpanel.MAUI.Views.Logs;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Innopixel.Kontrolpanel.MAUI.ViewModels.Logs;

[INotifyPropertyChanged]
public partial class LogsViewModel
{
	private readonly Service<Log> logService;

	[ObservableProperty]
	private bool hasNoConnection;

	[ObservableProperty]
	private string loadTime;

	[ObservableProperty]
	private string loadTimeFirstLog;

	[ObservableProperty]
	private bool isRefreshing;

	[ObservableProperty]
	private ObservableCollection<Log> logs = new();

	[ObservableProperty]
	private bool isEmpty;

	public LogsViewModel(Service<Log> logService)
	{
		Connectivity.Current.ConnectivityChanged += Connectivity_ConnectivityChanged;
		this.logService = logService;
	}

	private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		HasNoConnection = (e.NetworkAccess != NetworkAccess.Internet) ? true : false;

		if (Shell.Current.CurrentPage is LogsPage)
		{
			if (e.NetworkAccess != NetworkAccess.Internet)
			{
				await Shell.Current.DisplayAlert("Forbindelse mistet!", "Du har mistet forbindelsen til internettet...", "Ok");
			}
			else
			{
				await Shell.Current.DisplayAlert("Forbindelse genoprettet!", "Du har nu internet, og logs bliver nu genindlæst...", "Ok");
				await LoadLogsAsync();
			}
		}
	}

	[ICommand]
	internal async Task LoadLogsAsync()
	{
		if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
		{
			HasNoConnection = true;
			IsRefreshing = false;
			return;
		}

#if ANDROID || IOS || tvOS || Tizen
		UserDialogs.Instance.ShowLoading("Henter logs fra databasen...");
#endif

		if (Logs.Count is not 0)
		{
			Logs.Clear();
		}

		Stopwatch stopWatch = new();
		Stopwatch stopWatchFirstLog = new();
		LoadTime = String.Empty;
		LoadTimeFirstLog = String.Empty;
		stopWatch.Start();
		stopWatchFirstLog.Start();

		try
		{
			await foreach (Log log in logService.GetAllAsync().OrderByDescending(x => x.CreatedDate))
			{
				Logs.Add(log);

				if (Logs.Count is 1)
				{
					stopWatchFirstLog.Stop();
					LoadTimeFirstLog = stopWatchFirstLog.ElapsedMilliseconds + " millisekunder for at loade den første log";
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		finally
		{
			stopWatch.Stop();
			LoadTime = stopWatch.ElapsedMilliseconds + " millisekunder for at loade alle logs";

			if (Logs.Count is 0)
			{
				IsEmpty = true;
			}
			else
			{
				IsEmpty = false;
			}

			IsRefreshing = false;
#if ANDROID || IOS || tvOS
			UserDialogs.Instance.HideLoading();
#endif
		}
	}
}