using System.Net;
using System.Text.Json;
using TicketAPI.CustomExceptions;

namespace TicketAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro não tratado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 1. Verifica se a resposta já começou a ser enviada ao cliente
            if (context.Response.HasStarted)
            {
                // Se já começou, não podemos mudar Headers/StatusCode. 
                // Apenas registramos o erro no log e interrompemos o fluxo.
                return;
            }

            context.Response.ContentType = "application/json";

            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Erro Interno do Servidor";
            string detail = "Ocorreu um erro inesperado no sistema. Tente novamente mais tarde.";

            if (exception is BaseException baseException)
            {
                statusCode = baseException.StatusCode;
                title = "Violação de Regra de Negócio";
                detail = baseException.Message;
            }

            context.Response.StatusCode = statusCode;

            var response = new
            {
                status = statusCode,
                title,
                detail
            };

            var json = JsonSerializer.Serialize(response);
            
            // 2. Usamos await diretamente aqui para garantir a escrita assíncrona correta
            await context.Response.WriteAsync(json);
        }
    }
}
