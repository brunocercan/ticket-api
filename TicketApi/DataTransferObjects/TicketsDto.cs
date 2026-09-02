using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketAPI.DataTransferObjects;

[Table("Tickets")]
public class TicketsDto
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = "";

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = "";

    [Required]
    [StringLength(20)]
    public string Priority { get; set; } = "";

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "";

    public int CategoryId { get; set; }

    public int RequesterId { get; set; }

    public int? AssignedToId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }
}