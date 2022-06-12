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
public partial class UploadVideoViewModel
{
	private readonly Service<Video> videoService;
	private readonly Service<Log> logService;
	private readonly HttpClient httpClient;

	private FileResult result;
	private FileResult resultThumbnail;
	private string thumbnailPath;

	[ObservableProperty]
	private bool hasNoConnection;

	[ObservableProperty]
	private string fileType;

	[ObservableProperty]
	private string videoName;

	[ObservableProperty]
	private string videoDescription;

	[ObservableProperty]
	private string uploadErrorStatus;

	[ObservableProperty]
	private ImageSource imageSource;

	public UploadVideoViewModel(Service<Video> videoService, Service<Log> logService, HttpClient httpClient)
	{
		Connectivity.Current.ConnectivityChanged += Connectivity_ConnectivityChanged;
		this.videoService = videoService;
		this.logService = logService;
		this.httpClient = httpClient;
	}

	private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		HasNoConnection = (e.NetworkAccess != NetworkAccess.Internet) ? true : false;

		if (Shell.Current.CurrentPage is UploadVideoPage)
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
	internal async Task ChooseVideoAsync()
	{
		UploadErrorStatus = null;
		VideoName = null;

		try
		{
			result = await FilePicker.PickAsync(PickOptions.Default);

			if (result is not null)
			{
				VideoName = result.FileName;

				if (result.FileName.EndsWith("mp4", StringComparison.OrdinalIgnoreCase))
				{
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
	private async Task AddThumbnailAsync()
	{
		if (Connectivity.Current.NetworkAccess == NetworkAccess.None)
		{
			HasNoConnection = true;
			return;
		}

		ImageSource = null;
		UploadErrorStatus = null;

		try
		{
			resultThumbnail = await FilePicker.PickAsync(PickOptions.Default);

			if (resultThumbnail is not null)
			{
				if (resultThumbnail.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) || resultThumbnail.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) || resultThumbnail.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
				{
					Stream imageStream = await resultThumbnail.OpenReadAsync();
					ImageSource = ImageSource.FromStream(() => imageStream);

					MultipartFormDataContent formData = new();

					Stream imageStreamToUpload = await resultThumbnail.OpenReadAsync();
					StreamContent streamContent = new(imageStreamToUpload);
					streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
					{
						Name = "FormFile",
						FileName = resultThumbnail.FileName
					};
					formData.Add(streamContent);

					HttpResponseMessage httpResponse = await httpClient.PostAsync("/upload", formData);

					if (httpResponse.IsSuccessStatusCode)
					{
						thumbnailPath = await httpResponse.Content.ReadAsStringAsync();
					}
					else
					{
						UploadErrorStatus = $"Thumbnail upload fejlede ({httpResponse.StatusCode}). Prøv igen.";
					}
				}
				else
				{
					UploadErrorStatus = "Kun jpg, jpeg og png filer er tilladte for thumbnails.";
				}
			}
			else
			{
				UploadErrorStatus = "Ingen fil valgt.";
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
			UploadErrorStatus = "Ingen videoer valgt.";
			return;
		}

		if (FileType != "video/mp4")
		{
			UploadErrorStatus = "Kun .mp4 filer er tilladt.";
			return;
		}

		MultipartFormDataContent formData = new();

		Stream videoStream = await result.OpenReadAsync();
		StreamContent streamContent = new(videoStream);
		streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		{
			Name = "FormFile",
			FileName = result.FileName
		};
		formData.Add(streamContent);

		HttpResponseMessage httpResponse = await httpClient.PostAsync("/upload", formData);

		if (!httpResponse.IsSuccessStatusCode)
		{
			UploadErrorStatus = $"Fejl ved upload af video ({httpResponse.StatusCode}). Prøv igen.";
			return;
		}

		string responseString = await httpResponse.Content.ReadAsStringAsync();

		Video video = new()
		{
			Name = VideoName,
			Description = VideoDescription,
			FilePath = responseString,
			FileType = FileType,
			ThumbnailPath = thumbnailPath
		};

		await videoService.CreateAsync(video);
		await logService.CreateAsync(new Log()
		{
			Description = $"{JwtParser.ParseClaimsFromJwt(Preferences.Default.Get("JWT", "")).Where(x => x.Type == ClaimTypes.Name).FirstOrDefault()?.Value} uploadede billede: {VideoName}",
			UserId = JwtParser.ParseClaimsFromJwt(Preferences.Default.Get("JWT", "")).Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value
		});
		await Shell.Current.GoToAsync("..");
	}
}