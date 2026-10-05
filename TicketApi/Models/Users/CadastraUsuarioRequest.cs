using System.ComponentModel.DataAnnotations;

namespace TicketAPI.Models.Users
{
    public class CadastraUsuarioRequest
    {
        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = "";

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(500)]
        public string Senha { get; set; } = "";

        [Required]
        [StringLength(30)]
        public string Funcao { get; set; } = "";
    }
}