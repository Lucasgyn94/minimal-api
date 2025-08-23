using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace MinimalApi;

public class DbContexto : DbContext
{

    // injeção do objeto do tipo IConfiguration para configuração da conexão
    //    private readonly IConfiguration _configuracaoAppSettings;

    /* 
    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        this._configuracaoAppSettings = configuracaoAppSettings;

    }
    */

    // criando construtor para pegar string de conexão

    public DbContexto(DbContextOptions<DbContexto> opcoes) : base(opcoes)
    {
        
    }

    // mapeando a entidade Adminitrador
    public DbSet<Administrador> Administradores { get; set; } = default!;

    // mapeando a entidade Veículo
    public DbSet<Veiculo> Veiculos { get; set; } = default!;


    // criando um seed(função) para cadastro do administrador inicial
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /* 
        // Iremos gerar o hash para a senha "123456" e vamos usar no seed.
        // Este valor de hash será sempre o mesmo para a mesma senha se o "sal" for o mesmo,
        // mas o BCrypt gera um "sal" aleatório a cada vez, então deixaremos um valor fixo aqui.
        */
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("123456", workFactor: 11);
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "administrador@teste.com.br",
                Senha = senhaHash,
                Perfil = "Adm",
            }
        );
    }

    /* 
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Instanciando a string de conexão
            var stringConexao = this._configuracaoAppSettings.GetConnectionString("mysql")?.ToString();

            if (!string.IsNullOrEmpty(stringConexao))
            {
                optionsBuilder.UseMySql(
                stringConexao,
                ServerVersion.AutoDetect(stringConexao)
                );

            }
        }
        // optionsBuilder.UseMySql(
        //     "String de conexão",
        //     ServerVersion.AutoDetect("String de conexão")
        // );
    }
    */


}
