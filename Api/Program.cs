using DotNetEnv;
using MinimalApi;

Env.Load(); // carregando arquivo .env com as credenciais do banco de dados

IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
}

CreateHostBuilder(args).Build().Run();