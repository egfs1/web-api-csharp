namespace WebAPI.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception error)
            {

                var response = context.Response;

                response.ContentType = "application/json";

                switch (error)
                {
                    case Errors.AppException:
                        response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                        break;

                    default:
                        response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                        break;
                }

                var result = System.Text.Json.JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }

    }
}
