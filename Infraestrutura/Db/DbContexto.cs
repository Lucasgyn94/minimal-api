using Microsoft.EntityFrameworkCore;

namespace MinimalApi;

public class DbContexto : DbContext
{

    // injeção do objeto do tipo IConfiguration para configuração da conexão
    //    private readonly IConfiguration _configuracaoAppSettings;


    // criando construtor para pegar string de conexão

    public DbContexto(DbContextOptions<DbContexto> opcoes) : base(opcoes)
    {
        
    }
    /* 
    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        this._configuracaoAppSettings = configuracaoAppSettings;

    }
    */

    // mapeando a entidade Adminitrador
    public DbSet<Administrador> Administradores { get; set; } = default!;

    // mapeando a entidade Veículo
    public DbSet<Veiculo> Veiculos { get; set; } = default!;


    // criando um seed(função) para cadastro do administrador inicial
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "administrador@teste.com.br",
                Senha = "123456",
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
