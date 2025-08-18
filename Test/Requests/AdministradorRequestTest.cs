
using System.Net;
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

    // Método auxiliar para realizar login e retornar token JWT
    private async Task<string> LoginAssincronoEPegarToken(string email, string senha)
    {
        var loginDto = new LoginDTO
        {
            Email = email,
            Senha = senha
        };
        var content = new StringContent(JsonSerializer.Serialize(loginDto));

        var response = await _client.PostAsync("/administradores/login", content);

        var responseBody = await response.Content.ReadAsStringAsync();
        var adminLogado = JsonSerializer.Deserialize<AdministradorLogado>(responseBody, _jsonOptions);

        Assert.IsNotNull(adminLogado?.Token, "O token não deveria ser nulo!");
        return adminLogado.Token;
    }

    // Testes de Login (Post / administradores/login)
    [TestMethod]
    public async Task LoginComCredenciaisValidasRetornaOkComToken()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };
        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

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
        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/administradores/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

}
