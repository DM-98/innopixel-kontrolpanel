using System.ComponentModel.DataAnnotations.Schema;

namespace Innopixel.Kontrolpanel.Core.Domain;

[Table("Images")]
public class Image : Media
{
	public int? DisplayId { get; set; }

	[ForeignKey("DisplayId")]
	public virtual Display? Display { get; set; }
}