using FluentValidation;
using TicketAPI.Models.Users;

namespace TicketAPI.Validators
{
    public class CadastraUsuarioRequestValidator : AbstractValidator<CadastraUsuarioRequest>
    {
        public CadastraUsuarioRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres")
                .MaximumLength(500).WithMessage("Senha deve ter no máximo 500 caracteres");

            RuleFor(x => x.Funcao)
                .NotEmpty().WithMessage("Função é obrigatória")
                .MaximumLength(30).WithMessage("Função deve ter no máximo 30 caracteres")
                .Must(BeValidRole).WithMessage("Função deve ser 'User', 'Support' ou 'Admin'");
        }

        private bool BeValidRole(string role)
        {
            return role is "User" or "Support" or "Admin";
        }
    }

    public class AtualizaUsuarioRequestValidator : AbstractValidator<AtualizaUsuarioRequest>
    {
        public AtualizaUsuarioRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID deve ser maior que zero");

            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");

            RuleFor(x => x.Senha)
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres")
                .MaximumLength(500).WithMessage("Senha deve ter no máximo 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Senha));

            RuleFor(x => x.Funcao)
                .NotEmpty().WithMessage("Função é obrigatória")
                .MaximumLength(30).WithMessage("Função deve ter no máximo 30 caracteres")
                .Must(BeValidRole).WithMessage("Função deve ser 'User', 'Support' ou 'Admin'");

            RuleFor(x => x.DataCriacao)
                .NotEmpty().WithMessage("Data de criação é obrigatória");
        }

        private bool BeValidRole(string role)
        {
            return role is "User" or "Support" or "Admin";
        }
    }
}