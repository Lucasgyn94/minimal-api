using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi;
using DotNetEnv;

Env.Load(); // carregando arquivo .env com as credenciais do banco de dados

#region Builder
var builder = WebApplication.CreateBuilder(args);

// injetando serviço AdministradorServico
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
// injetando serviço de veículos
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

// configuração do swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// acrescentando serviço do mysql
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
var dbDatabase = Environment.GetEnvironmentVariable("DB_DATABASE");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var stringConexaoDB = $"Server={dbServer};Database={dbDatabase};Uid={dbUser};Pwd={dbPassword};";

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        stringConexaoDB,
        ServerVersion.AutoDetect(stringConexaoDB)
    );
});

/* 
builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});
*/

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
    // if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
    // {
    //     return Results.Ok("Login com sucesso!");
    // }
    // else
    // {
    //     return Results.Unauthorized();
    // }
}).WithTags("Administradores");


app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);

    foreach (var adm in administradores)
    {

        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil

        });
    }

    return Results.Ok(adms);
}).WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorId(id);

    if (administrador == null) return Results.NotFound();

    return Results.Ok(new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil

    });
}).WithTags("Administradores");


app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("Email não pode ser vazio!");
    if (string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagens.Add("Senha não pode ser vazia!");
    if (administradorDTO.Perfil == null)
        validacao.Mensagens.Add("Perfil não pode ser vazio!");

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    administradorServico.Incluir(administrador);

    return Results.Created($"administrador/{administrador.Id}", new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });

}).WithTags("Administradores");

#endregion

#region Veiculos
// Método de validar dto veículos 
ErrosDeValidacao ValidaDTO(VeiculoDTO veiculoDTO)
{
    ErrosDeValidacao validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("Nome de veículo não pode ser em branco!");
    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("Marca de veículo não pode ser em branco!");
    if (veiculoDTO.Ano < 1950)
    {
        validacao.Mensagens.Add("Ano inválido! Informe um ano de veículo igual ou superior a 1950.");
    }
    ;

    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = ValidaDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);

    if (veiculo == null) return Results.NotFound();
    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapPut("veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);
    if (veiculo == null) return Results.NotFound();

    var validacao = ValidaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
    {
        Results.BadRequest(validacao);
    }


    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculos");

#endregion

app.MapDelete("veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);

    if (veiculo == null) return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).WithTags("Veiculos");

#region App
// instanciando o swagger
app.UseSwagger();
app.UseSwaggerUI(); // instanciando a interface do swagger ui

app.Run();
#endregion

/*PAREI NA AULA: criando endpoints para admnistrador*/
