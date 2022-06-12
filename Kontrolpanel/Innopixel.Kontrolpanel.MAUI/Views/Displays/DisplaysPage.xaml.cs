using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Services;
using Innopixel.Kontrolpanel.Core.Utilities;
using Innopixel.Kontrolpanel.MAUI.ViewModels.Displays;
using System.Security.Claims;

namespace Innopixel.Kontrolpanel.MAUI.Views.Displays;

public partial class DisplaysPage : ContentPage
{
	private readonly Service<Display> displayService;
	private readonly Service<Log> logService;

	public DisplaysPage(DisplaysViewModel displaysViewModel, Service<Display> displayService, Service<Log> logService)
	{
		InitializeComponent();
		this.displayService = displayService;
		this.logService = logService;
		BindingContext = displaysViewModel;
	}

	protected override async void OnAppearing()
	{
		await ((DisplaysViewModel)BindingContext).LoadDisplaysAsync();
	}

	private async void Switch_Toggled(object sender, ToggledEventArgs e)
	{
		Switch displaySwitch = sender as Switch;
		Display selectedDisplay = displaySwitch.BindingContext as Display;

		if (selectedDisplay is null)
		{
			return;
		}

		Display displayToUpdate = await displayService.GetByIdAsync(selectedDisplay.Id);

		if (displayToUpdate is null)
		{
			return;
		}

		if (displayToUpdate.IsOn == displaySwitch.IsToggled)
		{
			return;
		}

		displayToUpdate.IsOn = displaySwitch.IsToggled;
		string isOnString = displaySwitch.IsToggled ? "tændt" : "slukket";

		await displayService.UpdateAsync(displayToUpdate);
		await logService.CreateAsync(new Log()
		{
			Description = $"{JwtParser.ParseClaimsFromJwt(Preferences.Default.Get("JWT", "")).Where(x => x.Type == ClaimTypes.Name).FirstOrDefault()?.Value} har {isOnString} {displayToUpdate.Name}.",
			UserId = JwtParser.ParseClaimsFromJwt(Preferences.Default.Get("JWT", "")).Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value
		});
	}
}