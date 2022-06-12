using Innopixel.Kontrolpanel.API.Data;
using Innopixel.Kontrolpanel.Core.Domain;
using Innopixel.Kontrolpanel.Core.Interfaces;
using Innopixel.Kontrolpanel.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("UserDbContext")));
builder.Services.AddDbContext<DisplayDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DisplayDbContext")));
builder.Services.AddDbContext<VideoDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("VideoDbContext")));
builder.Services.AddDbContext<ImageDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("ImageDbContext")));
builder.Services.AddDbContext<LogDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("LogDbContext")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	options.Password.RequiredLength = 1;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireDigit = false;
	options.User.RequireUniqueEmail = true;
	options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ1234567890@._- ";
	options.SignIn.RequireConfirmedAccount = true;
}).AddEntityFrameworkStores<UserDbContext>();

builder.Services.AddDataProtection()
	.PersistKeysToFileSystem(new DirectoryInfo(@"d:\web\localuser\faluf.com\DataProtectionKeys"))
	.ProtectKeysWithCertificate(new X509Certificate2(builder.Configuration["X509Cert:FilePath"], builder.Configuration["X509Cert:Password"]));

// Generic Repository Pattern - Kan nemt skifte fra EFCore(EFRepository<T, TDbContext>) til noget andet. Godt for Unit Testing.
builder.Services.AddTransient<IRepositoryAPI<Display>, EFRepository<Display, DisplayDbContext>>();
builder.Services.AddTransient<IRepositoryAPI<Video>, EFRepository<Video, VideoDbContext>>();
builder.Services.AddTransient<IRepositoryAPI<Image>, EFRepository<Image, ImageDbContext>>();
builder.Services.AddTransient<IRepositoryAPI<Log>, EFRepository<Log, LogDbContext>>();

X509Certificate2 certificate = new(builder.Configuration["X509Cert:FilePath"], builder.Configuration["X509Cert:Password"]);
X509SecurityKey securityKey = new(certificate);

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters.ValidateIssuerSigningKey = true;
	options.TokenValidationParameters.ValidateIssuer = false;
	options.TokenValidationParameters.ValidateAudience = false;
	options.TokenValidationParameters.IssuerSigningKey = securityKey;
	options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
});

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
	// Include 'SecurityScheme' to use JWT Authentication
	OpenApiSecurityScheme jwtSecurityScheme = new()
	{
		Scheme = "bearer",
		BearerFormat = "JWT",
		Name = "JWT Authentication",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

		Reference = new OpenApiReference
		{
			Id = JwtBearerDefaults.AuthenticationScheme,
			Type = ReferenceType.SecurityScheme
		}
	};

	setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

	setup.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{ jwtSecurityScheme, Array.Empty<string>() }
	});
});

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();