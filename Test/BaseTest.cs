using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi;

namespace Test;

[TestClass]
public abstract class BaseTest // deixando a classe como abstrata para impedir que a mesma seja executada como um teste
{
    protected DbContexto _contexto = default!;

    [TestInitialize]
    public void BaseTestInitialize()
    {
        var config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("appsettings.Testing.json", optional: false)
        .Build();

        // Obter a string de configuração a partir de nossa configuração
        var stringDeConexao = config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(stringDeConexao))
        {
            throw new InvalidOperationException("A string de conexão Default não foi encontrada na configuração de teste!");
        }

        // configurar as opções do DbContext
        var options = new DbContextOptionsBuilder<DbContexto>()
        .UseMySql(stringDeConexao, ServerVersion.AutoDetect(stringDeConexao))
        .Options;

        // Criar a instancia do contexto
        _contexto = new DbContexto(options);

        // garantir que o banco de dados esteja limpo antes de cada teste
        _contexto.Database.EnsureDeleted();
        _contexto.Database.EnsureCreated();


    }
    [TestCleanup]
    void BaseTestCleanup()
    {   
        // fazer com que o contexto seja descartado corretamente apos cada teste
        _contexto?.Dispose();
    }

}
