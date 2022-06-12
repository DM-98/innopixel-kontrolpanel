using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Innopixel.Kontrolpanel.Core.Domain;

public class ApplicationUser : IdentityUser
{
	[Timestamp]
	public byte[] RowVersion { get; set; } = null!;

	[DataType(DataType.DateTime)]
	[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
	public DateTime CreatedDate { get; set; }

	[DataType(DataType.DateTime)]
	[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
	public DateTime? UpdatedDate { get; set; }

	public virtual ICollection<Log>? Logs { get; set; }
}