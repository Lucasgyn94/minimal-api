using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi;

namespace Test;

[TestClass]
public class AdministradorServicoTest : BaseTest
{
    private AdministradorServico _administradorServico = default!;

    [TestInitialize]
    public void Configuracao()
    {
        this._administradorServico = new AdministradorServico(this._contexto);
    }


    [TestMethod]
    public void IncluirSalvarNovoAdministradorComSucesso()
    {
        // Arrange
        var adm = new Administrador();
        adm.Email = "teste@teste.com.br";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        // Act
        this._administradorServico.Incluir(adm);

        // Assert
        var admSalvo = this._administradorServico.BuscarPorId(adm.Id);
        Assert.IsNotNull(admSalvo);
        Assert.AreEqual(2, this._administradorServico.Todos(1).Count);

    }

    [TestMethod]
    public void BuscarPorIdQuandoIdExistirRetornarAdministrador()
    {
        // Arrange
        var adm = new Administrador();
        adm.Email = "teste@teste.com.br";
        adm.Senha = "teste";
        adm.Perfil = "Adm";
        this._administradorServico.Incluir(adm);

        // Act
        var admDoBanco = this._administradorServico.BuscarPorId(adm.Id);

        // Assert
        Assert.IsNotNull(admDoBanco);
        Assert.AreEqual(adm.Email, admDoBanco.Email);
    }

    [TestMethod]
    public void BuscarPorIdQuandoIdNaoExistirRetornarNulo()
    {
        // Arrange
        /*Não iremos inserir ninguém */

        // Act
        var admDoBanco = this._administradorServico.BuscarPorId(999);

        // Assert
        Assert.IsNull(admDoBanco);
    }

    [TestMethod]
    public void LoginComCredenciaisValidasRetornaAdministrador()
    {
        // Arrange
        var admParaSalvar = new Administrador();
        admParaSalvar.Email = "teste@valido.com.br";
        admParaSalvar.Senha = "123456";
        admParaSalvar.Perfil = "Admin";

        this._administradorServico.Incluir(admParaSalvar);

        var loginDto = new LoginDTO();
        loginDto.Email = "teste@valido.com.br";
        loginDto.Senha = "123456";

        // Act

        var admLogado = this._administradorServico.Login(loginDto);

        // Assert
        Assert.IsNotNull(admLogado);
        Assert.AreEqual("teste@valido.com.br", admLogado.Email);
    }
    [TestMethod]
    public void LoginComCredenciaisInvalidasRetornaNulo()
    {
        // Arrange
        var loginDto = new LoginDTO();
        loginDto.Email = "teste@invalido.com";
        loginDto.Senha = "123456";

        // Act

        var resultado = this._administradorServico.Login(loginDto);

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void TodosQuandoExistemAdministradoresRetornaListaComTodos()
    {
        // Arrange
        var adm1 = new Administrador();
        adm1.Email = "adm1@teste.com";
        adm1.Senha = "123456";
        adm1.Perfil = "Adm";

        var adm2 = new Administrador();
        adm2.Email = "adm2@teste.com";
        adm2.Senha = "123456";
        adm2.Perfil = "Adm";

        this._administradorServico.Incluir(adm1);
        this._administradorServico.Incluir(adm2);


        // Act
        var listaDeAdms = this._administradorServico.Todos(null);

        // Assert
        Assert.IsNotNull(listaDeAdms);
        Assert.AreEqual(3, listaDeAdms.Count);
    }

    [TestMethod]
    public void TodosQuandoNaoExistemAdministradoresRetornaVazia()
    {
        // Arrange
        // Deixamos o banco vazio

        // Act
        var listaDeAdms = this._administradorServico.Todos(null);

        // Assert
        Assert.IsNotNull(listaDeAdms);
        Assert.AreEqual(1, listaDeAdms.Count);
    }
}
