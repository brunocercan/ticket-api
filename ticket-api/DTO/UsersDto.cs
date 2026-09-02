using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObjects;

[Table("Users")]
public class UsersDto
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(255)]
    public string Email { get; set; } = "";

    [Required]
    [StringLength(500)]
    public string PasswordHash { get; set; } = "";

    [Required]
    [StringLength(30)]
    public string Role { get; set; } = "";

    [Required]
    public DateTime? CreatedAt { get; set; }
}