using Innopixel.Kontrolpanel.Blazor.Authentication;
using Innopixel.Kontrolpanel.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.Security.Cryptography.X509Certificates;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton(sp => new HttpClient() { BaseAddress = new Uri(builder.Configuration["API"]) });
builder.Services.AddDataProtection()
	.PersistKeysToFileSystem(new DirectoryInfo(@"d:\web\localuser\faluf.com\DataProtectionKeys"))
	.ProtectKeysWithCertificate(new X509Certificate2(builder.Configuration["X509Cert:FilePath"], builder.Configuration["X509Cert:Password"]));

builder.Services.AddScoped<AuthenticationStateProvider, AppState>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddSingleton<DisplayService>();
builder.Services.AddSingleton<ImageService>();
builder.Services.AddSingleton<VideoService>();
builder.Services.AddSingleton<LogService>();

builder.Services.AddDirectoryBrowser();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseStaticFiles();

RewriteOptions options = new RewriteOptions();
options.AddRedirectToWww();
options.AddRedirectToHttps();
app.UseRewriter(options);

PhysicalFileProvider fileProvider = new(Path.Combine(builder.Environment.WebRootPath, "app"));
string requestPath = "/maui";
FileExtensionContentTypeProvider provider = new();
provider.Mappings[".apk"] = "application/vnd.android.package-archive";
provider.Mappings[".apks"] = "application/vnd.android.package-archive";

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = fileProvider,
	RequestPath = requestPath,
	ContentTypeProvider = provider
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
	FileProvider = fileProvider,
	RequestPath = requestPath
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();