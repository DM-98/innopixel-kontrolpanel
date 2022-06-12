using System.ComponentModel.DataAnnotations.Schema;

namespace Innopixel.Kontrolpanel.Core.Domain;

[Table("Videos")]
public class Video : Media
{
	public int? Frames { get; set; }

	public string? SubtitlesPath { get; set; }

	public string? ThumbnailPath { get; set; }

	public int? DisplayId { get; set; }

	[ForeignKey("DisplayId")]
	public virtual Display? Display { get; set; }
}