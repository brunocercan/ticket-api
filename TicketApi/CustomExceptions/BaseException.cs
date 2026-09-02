namespace TicketAPI.CustomExceptions
{
    public abstract class BaseException : Exception
    {
        public int StatusCode {get; set;}
        protected BaseException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

    }
}