namespace MinimalApi;

public static class HomeEndpoints
{
    public static void MapHomeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", (HttpContext httpContext) =>
        {
            var requisicao = httpContext.Request;
            var urlBase = $"{requisicao.Scheme}://{requisicao.Host}{requisicao.PathBase}";

            var resposta = new
            {
                Titulo = "Minimal API de Gerenciamento de Veículos",
                Mensagem = "A documentação da api pode ser encontrada no link abaixo.",
                Versao = "1.0.0",
                Documentacao = $"{urlBase}/swagger"
            };
            return Results.Json(resposta);
        }).AllowAnonymous().WithTags("Home");
    }
}
