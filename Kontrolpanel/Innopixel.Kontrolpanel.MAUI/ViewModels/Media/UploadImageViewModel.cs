using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Services;
using Innopixel.Kontrolpanel.Core.Utilities;
using Innopixel.Kontrolpanel.MAUI.Views.Media;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Innopixel.Kontrolpanel.MAUI.ViewModels.Media;

[INotifyPropertyChanged]
public partial class UploadImageViewModel
{
	private readonly Service<Core.Domain.Image> imageService;
	private readonly Service<Log> logService;
	private readonly HttpClient httpClient;

	private FileResult result;

	[ObservableProperty]
	private bool hasNoConnection;

	[ObservableProperty]
	private string fileType;

	[ObservableProperty]
	private ImageSource imageSource;

	[ObservableProperty]
	private string imageName;

	[ObservableProperty]
	private string imageDescription;

	[ObservableProperty]
	private string uploadErrorStatus;

	public UploadImageViewModel(Service<Core.Domain.Image> imageService, Service<Log> logService, HttpClient httpClient)
	{
		Connectivity.Current.ConnectivityChanged += Connectivity_ConnectivityChanged;
		this.imageService = imageService;
		this.logService = logService;
		this.httpClient = httpClient;
	}

	private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		HasNoConnection = (e.NetworkAccess != NetworkAccess.Internet) ? true : false;

		if (Shell.Current.CurrentPage is UploadImagePage)
		{
			if (e.NetworkAccess != NetworkAccess.Internet)
			{
				await Shell.Current.DisplayAlert("Forbindelse mistet!", "Du har mistet forbindelsen til internettet...", "Ok");
			}
			else
			{
				await Shell.Current.DisplayAlert("Forbindelse genoprettet!", "Du har nu internet og kan fortsætte arbejdet...", "Ok");
			}
		}
	}

	[ICommand]
	internal async Task ChooseImageAsync()
	{
		ImageSource = null;
		UploadErrorStatus = null;
		ImageName = null;

		try
		{
			result = await FilePicker.PickAsync(PickOptions.Default);

			if (result is not null)
			{
				ImageName = result.FileName;

				if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) || result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) || result.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
				{
					Stream imageStream = await result.OpenReadAsync();
					ImageSource = ImageSource.FromStream(() => imageStream);
					FileType = result.ContentType;
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	[ICommand]
	private async Task UploadAsync()
	{
		if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
		{
			HasNoConnection = true;
			return;
		}

		if (result is null)
		{
			UploadErrorStatus = "Ingen billeder valgt.";
			return;
		}

		if (FileType != "image/jpg" && FileType != "image/png" && FileType != "image/jpeg")
		{
			UploadErrorStatus = $"Filen({result.FileName}) skal være et af typerne: .jpg, .png eller .jpeg!";
			return;
		}

		MultipartFormDataContent formData = new();

		Stream imageStream = await result.OpenReadAsync();
		StreamContent streamContent = new(imageStream);
		streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		{
			Name = "FormFile",
			FileName = result.FileName
		};
		formData.Add(streamContent);

		HttpResponseMessage httpResponse = await httpClient.PostAsync("/upload", formData);

		if (httpResponse.IsSuccessStatusCode)
		{
			string responseString = await httpResponse.Content.ReadAsStringAsync();

			Core.Domain.Image image = new()
			{
				Name = ImageName,
				Description = ImageDescription,
				FilePath = responseString,
				FileType = FileType
			};

			await imageService.CreateAsync(image);
			await logService.CreateAsync(new Log()
			{
				Description = $"{JwtParser.ParseClaimsFromJwt(Preferences.Default.Get("JWT", "")).Where(x => x.Type == ClaimTypes.Name).FirstOrDefault()?.Value} uploadede billede: {ImageName}",
				UserId = JwtParser.ParseClaimsFromJwt(Preferences.Default.Get("JWT", "")).Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value
			});
			await Shell.Current.GoToAsync("..");
		}
		else
		{
			UploadErrorStatus = $"Upload af billede fejlede ({httpResponse.StatusCode}).";
		}
	}
}