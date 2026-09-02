using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketAPI.DataTransferObjects;

[Table("TicketComments")]
public class TicketCommentsDto
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TicketId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(500)]
    public string Content { get; set; } = "";

    [Required]
    public DateTime? CreatedAt { get; set; }
}