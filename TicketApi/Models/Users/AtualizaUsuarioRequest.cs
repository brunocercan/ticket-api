using System.ComponentModel.DataAnnotations;

namespace TicketAPI.Models.Users
{
    public class AtualizaUsuarioRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = "";

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = "";

        [StringLength(500)]
        public string? Senha { get; set; }

        [Required]
        [StringLength(30)]
        public string Funcao { get; set; } = "";

        public DateTime DataCriacao { get; set; }
    }
}