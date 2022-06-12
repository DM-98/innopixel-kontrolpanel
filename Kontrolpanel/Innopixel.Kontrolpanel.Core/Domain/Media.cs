using System.ComponentModel.DataAnnotations;

namespace Innopixel.Kontrolpanel.Core.Domain;

public class Media : EntityBase
{
	[Required(ErrorMessage = "Navn skal udfyldes!")]
	public string Name { get; set; } = null!;

	public string? Description { get; set; }

	[Required(ErrorMessage = "Filtype mangler!")]
	public string FileType { get; set; } = null!;

	[Required(ErrorMessage = "Filsti mangler!")]
	public string FilePath { get; set; } = null!;
}