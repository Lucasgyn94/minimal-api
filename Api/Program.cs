using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi;
using Microsoft.AspNetCore.HttpOverrides; 

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        /* Configuração de Injeção de Dependência
        ## SOBRE
        * Injeção de Dependência permite que uma classe receba as dependências que precisa de fontes externas, em vez de termos que criar internamente.
        */
        #region Injeção de Dependência
        {
            // Adicionando configuração do token jwt ao projeto
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt"]!)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            builder.Services.AddAuthorization();

            // Injeção dos Serviços da Aplicação
            builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
            builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();
            builder.Services.AddScoped<ITokenServico, TokenServico>();

            // Configuração do Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Define as informações gerais da API que aparecerão no topo
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API de Gerenciamento de Veículos",
                    Description = """
                                Uma API RestFul para o gerenciamento de veículos e administradores.
                                
                                Este projeto demonstra as melhores práticas de desenvolvimento de APIs com .NET 8, incluindo:
                                - Autenticação e Autorização com JWT.
                                - Arquitetura Limpa (Clean Architecture).
                                - Testes de Integração e de Serviço.
                                - Padrão de Minimal APIs.
                                """,
                    Contact = new OpenApiContact
                    {
                        Name = "Lucas Ferreira Da Silva",
                        Url = new Uri("https://www.linkedin.com/in/lucas-ferreira-soares-desenvolvedor/"),
                        Email = "https://www.linkedin.com/in/lucas-ferreira-55053412a/"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Licença MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // Configuração de segurança
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira seu token JWT aqui"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference{
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            /*
            ## Configurando o serviço de Forwarded Headers (Cabeçalhos Encaminhados)
            * Serve para informar que a nossa API.NET pode confiar nas informações que o Nginx está enviando sobre a requisição original. 
            */
            // builder.Services.Configure<ForwardedHeadersOptions>(options =>
            // {
            //     options.ForwardedHeaders =
            //         ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            // });

            // configuração do DbContext
            var stringConexaoDB = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<DbContexto>(options =>
            {
                options.UseMySql(stringConexaoDB, ServerVersion.AutoDetect(stringConexaoDB));
            });
        }
        #endregion

        var app = builder.Build();

        /* Configuração do Pipeline HTTP
        ## RESUMO
        Middleware: Componentes que processam requisições HTTP em um pipeline.
        Pipeline de Middleware: Sequência de middleware que uma requisição percorre do início ao fim.
        Funções do Middleware: Podem processar a requisição, passar para o próximo middleware ou gerar uma resposta.
        Exemplo: app.UseSwagger(); e app.UseSwaggerUI(); são exemplos de middleware que geram e exibem a documentação Swagger de uma API.
        */
        #region  Pipeline HTTP
        {
            /*Adicionando o Middleware de Forwarded Headers */
            //app.UseForwardedHeaders();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();
        }
        #endregion

        // Mapeamento dos Endpoints
        #region Endpoints
        {
            app.MapHomeEndpoints();
            app.MapAdministradorEndpoints();
            app.MapVeiculoEndpoints();
        }
        #endregion

        app.Run();
    }
}