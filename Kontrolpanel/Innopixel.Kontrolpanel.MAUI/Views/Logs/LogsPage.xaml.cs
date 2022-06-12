using Innopixel.Kontrolpanel.MAUI.ViewModels.Logs;

namespace Innopixel.Kontrolpanel.MAUI.Views.Logs;

public partial class LogsPage : ContentPage
{
	private bool tapped;

	public LogsPage(LogsViewModel logsViewModel)
	{
		InitializeComponent();
		BindingContext = logsViewModel;
	}

	protected override async void OnAppearing()
	{
		await ((LogsViewModel)BindingContext).LoadLogsAsync();
	}

	private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
	{
		if (!tapped)
		{
			((Label)sender).LineBreakMode = LineBreakMode.WordWrap;
			tapped = true;
		}
		else
		{
			((Label)sender).LineBreakMode = LineBreakMode.TailTruncation;
			tapped = false;
		}
	}
}