using System.ComponentModel.DataAnnotations;

namespace Innopixel.Kontrolpanel.Core.DTOs;

public class LoginDTO
{
	[Required(ErrorMessage = "{0} skal udfyldes!")]
	[Display(Name = "Email")]
	public string Email { get; set; } = null!;

	[Required(ErrorMessage = "{0} skal udfyldes!")]
	[Display(Name = "Adgangskode")]
	public string Password { get; set; } = null!;

	[Display(Name = "Forbliv logget ind")]
	public bool RememberMe { get; set; }
}