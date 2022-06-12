using System.ComponentModel.DataAnnotations;

namespace Innopixel.Kontrolpanel.Core.DTOs;

public class RegisterDTO
{
	[Required(ErrorMessage = "{0} skal udfyldes!")]
	[Display(Name = "Brugernavn")]
	public string Username { get; set; } = null!;

	[Required(ErrorMessage = "{0} skal udfyldes!")]
	[EmailAddress]
	[Display(Name = "Email")]
	public string Email { get; set; } = null!;

	[Required(ErrorMessage = "{0} skal udfyldes!")]
	[DataType(DataType.Password)]
	[Display(Name = "Adgangskode")]
	public string Password { get; set; } = null!;
}