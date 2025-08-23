using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Sprache;

namespace MinimalApi;

public static class VeiculoEndpoints
{
    public static void MapVeiculoEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder? grupoVeiculos = app.MapGroup("/veiculos").WithTags("Veiculos");

        ErrosDeValidacao ValidaDTO(VeiculoDTO veiculoDTO)
        {
            ErrosDeValidacao validacao = new ErrosDeValidacao
            {
                Mensagens = new List<string>()
            };

            if (string.IsNullOrEmpty(veiculoDTO.Nome))
            {
                validacao.Mensagens.Add("Nome não pode ser em branco.");
            }
            if (string.IsNullOrEmpty(veiculoDTO.Marca))
            {
                validacao.Mensagens.Add("Marca de veículo não pode ser em branco.");
            }
            if (veiculoDTO.Ano < 1950)
            {
                validacao.Mensagens.Add("Ano de veículo deve ser superior/igual a 1950.");
            }
            return validacao;
        }

        grupoVeiculos.MapGet("/", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
        {
            return Results.Ok(veiculoServico.Todos(pagina));
        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm,Editor"
        });

        grupoVeiculos.MapGet("/{id}", (int id, IVeiculoServico veiculoServico) =>
        {
            Veiculo? veiculo = veiculoServico.BuscarPorId(id);

            if (veiculo is null)
            {
                return Results.NotFound();
            }
            else
            {
                return Results.Ok(veiculo);
            }
        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm,Editor"
        });

        grupoVeiculos.MapPost("/", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
        {
            ErrosDeValidacao validacao = ValidaDTO(veiculoDTO);

            if (validacao.Mensagens.Count > 0)
            {
                return Results.BadRequest(validacao);
            }

            Veiculo? veiculo = new Veiculo
            {
                Nome = veiculoDTO.Nome,
                Marca = veiculoDTO.Marca,
                Ano = veiculoDTO.Ano
            };
            veiculoServico.Incluir(veiculo);

            return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm,Editor"
        });

        grupoVeiculos.MapPut("/{id}", (int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
        {
            Veiculo? veiculo = veiculoServico.BuscarPorId(id);

            if (veiculo is null)
            {
                return Results.NotFound();
            }

            ErrosDeValidacao validacao = ValidaDTO(veiculoDTO);

            if (validacao.Mensagens.Count > 0)
            {
                return Results.BadRequest(validacao);
            }

            veiculo.Nome = veiculoDTO.Nome;
            veiculo.Marca = veiculoDTO.Marca;
            veiculo.Ano = veiculoDTO.Ano;

            veiculoServico.Atualizar(veiculo);

            return Results.Ok(veiculo);

        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm"
        });

        grupoVeiculos.MapDelete("/{id}", (int id, IVeiculoServico veiculoServico) =>
        {
            Veiculo? veiculo = veiculoServico.BuscarPorId(id);

            if (veiculo is null)
            {
                return Results.NotFound();
            }

            veiculoServico.Apagar(veiculo);

            return Results.NoContent();
        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm"
        });
    }
}

