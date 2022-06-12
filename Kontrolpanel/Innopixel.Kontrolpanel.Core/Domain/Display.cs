using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innopixel.Kontrolpanel.Core.Domain;

[Table("Displays")]
public class Display : EntityBase
{
	[Required(ErrorMessage = "{0} skal udfyldes!")]
	public string Name { get; set; } = null!;

	public bool IsOn { get; set; }

	public string? Description { get; set; }

	public virtual ICollection<Video>? Videos { get; set; }

	public virtual ICollection<Image>? Images { get; set; }
}