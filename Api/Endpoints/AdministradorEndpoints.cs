using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace MinimalApi;

public static class AdministradorEndpoints
{
    public static void MapAdministradorEndpoints(this IEndpointRouteBuilder app)
    {
        var administradorGrupo = app.MapGroup("/administradores").WithTags("Administradores");

        administradorGrupo.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico, ITokenServico tokenService) =>
        {
            Administrador? administrador = administradorServico.Login(loginDTO);

            if (administrador is not null)
            {
                string token = tokenService.GerarToken(administrador);
                return Results.Ok(new AdministradorLogado
                {
                    Email = administrador.Email,
                    Perfil = administrador.Perfil,
                    Token = token
                });
            }
            else
            {
                return Results.Unauthorized();
            }
        }).AllowAnonymous();


        administradorGrupo.MapGet("/", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
        {
            List<Administrador>? administradores = administradorServico.Todos(pagina);

            IEnumerable<AdministradorModelView>? administadorModelView = administradores.Select(administrador => new AdministradorModelView
            {
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil
            });
            return Results.Ok(administadorModelView);
        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm"
        });

        administradorGrupo.MapGet("{id}", (int id, IAdministradorServico administradorServico) =>
        {
            Administrador? administrador = administradorServico.BuscarPorId(id);

            if (administrador is null)
            {
                return Results.NotFound();
            }
            else
            {
                return Results.Ok(new AdministradorModelView
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
            }
        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm"
        });

        administradorGrupo.MapPost("/", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
        {
        Administrador administrador = new Administrador
        {
            Email = administradorDTO.Email,
            Senha = administradorDTO.Senha,
            Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
        };
        Administrador administradorIncluido = administradorServico.Incluir(administrador);
        var modeloDeVizualizacao = new AdministradorModelView
        {
            Id = administradorIncluido.Id,
            Email = administradorIncluido.Email,
            Perfil = administradorIncluido.Perfil
        };

        return Results.Created($"/administrador/{administradorIncluido.Id}", modeloDeVizualizacao);

        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm"
        });

    }
}
