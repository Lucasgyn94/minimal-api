using Microsoft.EntityFrameworkCore;
using MinimalApi;

namespace Test;

[TestClass]
public class VeiculoServicoTest
{
    private DbContexto _contexto = default!;
    private VeiculoServico _veiculoServico = default!;
    private DbContexto CriarContextoDeTeste()
    {
        DotNetEnv.Env.Load();
        var server = Environment.GetEnvironmentVariable("DB_SERVER");
        var database = Environment.GetEnvironmentVariable("DB_DATABASE");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var stringDeConexao = $"Server={server};Database={database};Uid={user};Pwd={password};";

        var optionsBuilder = new DbContextOptionsBuilder<DbContexto>();
        optionsBuilder.UseMySql(stringDeConexao, ServerVersion.AutoDetect(stringDeConexao));
        return new DbContexto(optionsBuilder.Options);
    }

    [TestInitialize]
    public void Configuracao()
    {
        this._contexto = CriarContextoDeTeste();
        this._contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");
        this._veiculoServico = new VeiculoServico(_contexto);
    }

    [TestMethod]
    public void IncluirVeiculoValidoESalvarNoBanco()
    {
        //Arrange
        var veiculo = new Veiculo();
        veiculo.Nome = "Gol";
        veiculo.Marca = "VW";
        veiculo.Ano = 2020;

        // Act
        this._veiculoServico.Incluir(veiculo);
        var veiculoSalvo = this._veiculoServico.BuscarPorId(veiculo.Id);

        // Assert
        Assert.IsNotNull(veiculoSalvo);
        Assert.AreEqual("Gol", veiculoSalvo.Nome);

    }
    [TestMethod]
    public void BuscarPorIdQuandoIdNaoExistirRetornaNulo()
    {
        // Arrannge
        // banco de dados vazio pois sempre e rodado o método de Configuracao que limpa o mesmo

        // Act
        var veiculo = this._veiculoServico.BuscarPorId(999);

        // Assert
        Assert.IsNull(veiculo);
    }

    [TestMethod]
    public void AtualizarModificarDadosDoVeiculoComSucesso()
    {
        // Arrange
        var veiculoOriginal = new Veiculo { Nome = "Fusca", Marca = "VW", Ano = 1980 };
        this._veiculoServico.Incluir(veiculoOriginal);

        // modificando o objeto antes de mandar atualizar
        veiculoOriginal.Nome = "Brasilia";

        //Act
        this._veiculoServico.Atualizar(veiculoOriginal);

        // Assert
        var veiculoAtualizado = this._veiculoServico.BuscarPorId(veiculoOriginal.Id);
        Assert.IsNotNull(veiculoAtualizado);
        Assert.AreEqual("Brasilia", veiculoAtualizado.Nome);
    }

    [TestMethod]
    public void ApagarRemoverVeiculoDoBancoComSucesso()
    {
        // Arrange
        var veiculoParaApagar = new Veiculo { Nome = "Chevette", Marca = "Chevrolet", Ano = 1990 };
        this._veiculoServico.Incluir(veiculoParaApagar);

        // verificando se veiculo existe antes de apagar
        var veiculoExistente = this._veiculoServico.BuscarPorId(veiculoParaApagar.Id);
        Assert.IsNotNull(veiculoExistente);

        // Act
        this._veiculoServico.Apagar(veiculoParaApagar);

        // Assert
        var veiculoApagado = this._veiculoServico.BuscarPorId(veiculoParaApagar.Id);
        Assert.IsNull(veiculoApagado);
    }

    [TestMethod]
    public void TodosComFiltroPorNomeRetornandoApenasVeiculosCorrespondentes()
    {
        // Arrange
        _veiculoServico.Incluir(new Veiculo { Nome = "Gol", Marca = "VW", Ano = 2020 });
        _veiculoServico.Incluir(new Veiculo { Nome = "Polo", Marca = "VW", Ano = 2021 });
        _veiculoServico.Incluir(new Veiculo { Nome = "Jetta", Marca = "VW", Ano = 2022 });

        // Act
        var filtro = this._veiculoServico.Todos(nome: "ta");

        // Assert
        Assert.IsNotNull(filtro);
        //Assert.AreEqual(2, filtro.Count);
        Assert.AreEqual(1, filtro.Count);
    }
}
