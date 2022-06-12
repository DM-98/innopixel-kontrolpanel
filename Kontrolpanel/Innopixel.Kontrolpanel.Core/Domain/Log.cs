using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innopixel.Kontrolpanel.Core.Domain;

[Table("Logs")]
public class Log : EntityBase
{
	[Required(ErrorMessage = "{0} skal udfyldes!")]
	public string Description { get; set; } = null!;

	public string UserId { get; set; } = null!;
	public virtual ApplicationUser? User { get; set; }
}