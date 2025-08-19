
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;
using MinimalApi;

namespace Test;

[TestClass]
public class AdministradorRequestTest
{
    private static HttpClient _client = default!;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
        _client = Setup.client;
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    #region Metodo Auxiliar
    // Método auxiliar para realizar login e retornar token JWT
    private async Task<string> LoginAssincronoEPegarToken(string email, string senha)
    {
        var loginDto = new LoginDTO
        {
            Email = email,
            Senha = senha
        };
        var content = JsonContent.Create(loginDto);

        var response = await _client.PostAsync("administradores/login", content);

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var adminLogado = JsonSerializer.Deserialize<AdministradorLogado>(responseBody, _jsonOptions);

        Assert.IsNotNull(adminLogado?.Token, "O token não deveria ser nulo!");
        return adminLogado.Token;
    }
    #endregion

    #region Testes de Login (Post /administradores/login)
    [TestMethod]
    public async Task LoginComCredenciaisValidasRetornaOkComToken()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };
        //var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var content = JsonContent.Create(loginDto);
        // Act
        var response = await _client.PostAsync("/administradores/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(responseBody, _jsonOptions);

        Assert.IsNotNull(admLogado);
        Assert.IsFalse(string.IsNullOrEmpty(admLogado.Token));
        Assert.AreEqual("adm@teste.com", admLogado.Email);
    }


    [TestMethod]
    public async Task LoginComCredenciaisInvalidasDeveRetornarUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            Email = "adm@teste_invalido.com",
            Senha = "123456"
        };
        //var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var content = JsonContent.Create(loginDto);

        // Act
        var response = await _client.PostAsync("/administradores/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    #endregion

    #region Testes de criar Adminitradores (POST /administradores)
    [TestMethod]
    public async Task CriarAdministradorComDadosValidosEAutorizacaoDeveRetornarCreated()
    {
        //Arrange
        var token = await LoginAssincronoEPegarToken("adm@teste.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var novoAdmDto = new AdministradorDTO
        {
            Email = "novo_adm@teste.com",
            Senha = "123456",
            Perfil = 0

        };

        //var content = new StringContent(JsonSerializer.Serialize(novoAdmDto), Encoding.UTF8, "application/json");
        var content = JsonContent.Create(novoAdmDto);

        //Act
        var response = await _client.PostAsync("/administradores", content);

        //Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(response.Headers.Location, "O cabeçalho não foi retornado");

    }

    [TestMethod]
    public async Task CriarAdministradorSemAutorizacaoDeveRetornarUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;
        var novoAdmDto = new AdministradorDTO
        {
            Email = "adm@teste.com.br",
            Senha = "123456",
        };

        //var content = new StringContent(JsonSerializer.Serialize(novoAdmDto), Encoding.UTF8, "application/json");
        var content = JsonContent.Create(novoAdmDto);

        // Act
        var response = await _client.PostAsync("/administradores", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task CriarAdministradorComAutorizacaoDeEditorDeveRetornarForbidden()
    {
        // arrange
        var token = await LoginAssincronoEPegarToken("editor@teste.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var novoAdmDto = new AdministradorDTO
        {
            Email = "adm@teste.com",
            Senha = "123456",
            Perfil = 0
        };

        //var content = new StringContent(JsonSerializer.Serialize(novoAdmDto), Encoding.UTF8, "application/json");
        var content = JsonContent.Create(novoAdmDto);

        // act
        var response = await _client.PostAsync("/administradores", content);

        // assert
        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Testes de Listar Administradores (GET /administradores)
    [TestMethod]
    public async Task ObterTodosOsAdministradoresComAutorizacaoDeveRetornarOkComLista()
    {
        // Arrange
        var token = await LoginAssincronoEPegarToken("adm@teste.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/administradores");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        var administradores = JsonSerializer.Deserialize<List<AdministradorModelView>>(responseBody, _jsonOptions);

        Assert.IsNotNull(administradores);
        Assert.IsTrue(administradores.Count > 0, "Lista de administradores vazia!");
    }
    #endregion

    #region Testes de buscar por ID (GET /administradores/{id})
    [TestMethod]
    public async Task ObterAdministradorPorIdComIdValidoEAutorizacaoDeveRetornaOk()
    {
        // Arrange
        var token = await LoginAssincronoEPegarToken("adm@teste.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const int idParaBuscar = 1;

        // Act
        var response = await _client.GetAsync($"administradores/{idParaBuscar}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        var adminitrador = JsonSerializer.Deserialize<AdministradorModelView>(responseBody, _jsonOptions);

        Assert.IsNotNull(adminitrador);
        Assert.AreEqual(idParaBuscar, adminitrador.Id);
    }

    [TestMethod]
    public async Task ObterAdministradorPorIdComIdInvalidoDeveRetornarNotFound()
    {
        // arrange
        var token = await LoginAssincronoEPegarToken("adm@teste.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        const int idParaBuscar = 999;

        // act
        var response = await _client.GetAsync($"/administradores/{idParaBuscar}");

        // assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion


}

