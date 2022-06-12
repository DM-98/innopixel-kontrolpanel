using System.ComponentModel.DataAnnotations;

namespace Innopixel.Kontrolpanel.Core.Domain;

public class EntityBase
{
	[Key]
	public int Id { get; set; }

	[Timestamp]
	public byte[]? RowVersion { get; set; }

	[DataType(DataType.DateTime)]
	[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
	public DateTime CreatedDate { get; set; }

	[DataType(DataType.DateTime)]
	[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
	public DateTime? UpdatedDate { get; set; }
}