namespace TicketAPI.Models.TicketComments
{
    public class CadastraComentarioTicket
    {
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = "";
    }
}