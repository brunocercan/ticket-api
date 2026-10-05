using FluentValidation;
using TicketAPI.Models.Tickets;
using TicketAPI.Models.TicketComments;

namespace TicketAPI.Validators
{
    public class CadastraTicketRequestValidator : AbstractValidator<CadastraTicketRequest>
    {
        public CadastraTicketRequestValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("Título é obrigatório")
                .MaximumLength(200).WithMessage("Título deve ter no máximo 200 caracteres");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("Descrição é obrigatória")
                .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");

            RuleFor(x => x.Prioridade)
                .NotEmpty().WithMessage("Prioridade é obrigatória")
                .Must(BeValidPriority).WithMessage("Prioridade deve ser 'Low', 'Medium', 'High' ou 'Critical'");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status é obrigatório")
                .Must(BeValidStatus).WithMessage("Status deve ser 'Open', 'InProgress' ou 'Resolved'");

            RuleFor(x => x.IdCategoria)
                .GreaterThan(0).WithMessage("Categoria é obrigatória");

            RuleFor(x => x.IdSolicitante)
                .GreaterThan(0).WithMessage("Solicitante é obrigatório");

            RuleFor(x => x.IdVinculado)
                .GreaterThan(0).WithMessage("ID do responsável deve ser maior que zero")
                .When(x => x.IdVinculado.HasValue);
        }

        private bool BeValidPriority(string priority)
        {
            return priority is "Low" or "Medium" or "High" or "Critical";
        }

        private bool BeValidStatus(string status)
        {
            return status is "Open" or "InProgress" or "Resolved";
        }
    }

    public class AtualizaTicketRequestValidator : AbstractValidator<AtualizaTicketRequest>
    {
        public AtualizaTicketRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID deve ser maior que zero");

            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("Título é obrigatório")
                .MaximumLength(200).WithMessage("Título deve ter no máximo 200 caracteres");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("Descrição é obrigatória")
                .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");

            RuleFor(x => x.Prioridade)
                .NotEmpty().WithMessage("Prioridade é obrigatória")
                .Must(BeValidPriority).WithMessage("Prioridade deve ser 'Low', 'Medium', 'High' ou 'Critical'");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status é obrigatório")
                .Must(BeValidStatus).WithMessage("Status deve ser 'Open', 'InProgress' ou 'Resolved'");

            RuleFor(x => x.IdCategoria)
                .GreaterThan(0).WithMessage("Categoria é obrigatória");

            RuleFor(x => x.IdSolicitante)
                .GreaterThan(0).WithMessage("Solicitante é obrigatório");

            RuleFor(x => x.IdVinculado)
                .GreaterThan(0).WithMessage("ID do responsável deve ser maior que zero");

            RuleFor(x => x.DataCriacao)
                .NotEmpty().WithMessage("Data de criação é obrigatória");
        }

        private bool BeValidPriority(string priority)
        {
            return priority is "Low" or "Medium" or "High" or "Critical";
        }

        private bool BeValidStatus(string status)
        {
            return status is "Open" or "InProgress" or "Resolved";
        }
    }

    public class CadastraComentarioTicketValidator : AbstractValidator<CadastraComentarioTicket>
    {
        public CadastraComentarioTicketValidator()
        {
            RuleFor(x => x.TicketId)
                .GreaterThan(0).WithMessage("Ticket ID é obrigatório");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID é obrigatório");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Conteúdo do comentário é obrigatório")
                .MaximumLength(500).WithMessage("Comentário deve ter no máximo 500 caracteres");
        }
    }
}