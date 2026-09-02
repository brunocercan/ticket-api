namespace TicketAPI.CustomExceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException() 
            : base("Dados não encontrados para o filtro selecionado.", 404) {}
            
        public NotFoundException(string entidade) 
            : base($"{entidade} não encontrado(a) para o filtro selecionado.", 404) {}
    }

    public class CannotDeleteException : BaseException
    {
        public CannotDeleteException(string mensagem) 
            : base(mensagem, 409) {}
    }

    public class CannotCreateException : BaseException
    {
        public CannotCreateException(string mensagem) 
            : base(mensagem, 400) {} 
    }
}