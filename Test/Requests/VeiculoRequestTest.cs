using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MinimalApi;

namespace Test;

[TestClass]
public class VeiculoRequestTest
{
    private static HttpClient _client = default!;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

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

    #region Métodos Auxiliares
    // fazer login e pegar token
    private async Task<string> LoginAssincronoEPegarToken(string email, string senha)
    {
        var loginDto = new LoginDTO
        {
            Email = email,
            Senha = senha
        };
        var content = JsonContent.Create(loginDto);
        var response = await _client.PostAsync("/administradores/login", content);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var adminLogado = JsonSerializer.Deserialize<AdministradorLogado>(responseBody, _jsonOptions);
        Assert.IsNotNull(adminLogado?.Token, "Token não pode ser nulo");
        return adminLogado.Token;
    }

    private async Task<Veiculo> CriarVeiculoParaTesteAsync(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var veiculoDto = new VeiculoDTO
        {
            Nome = "Eclipse",
            Marca = "Mitsubishi",
            Ano = 2022
        };
        var content = JsonContent.Create(veiculoDto);
        var response = await _client.PostAsync("/veiculos", content);
        response.EnsureSuccessStatusCode();
        var veiculoCriado = await response.Content.ReadFromJsonAsync<Veiculo>(_jsonOptions);
        Assert.IsNotNull(veiculoCriado);
        return veiculoCriado;
    }
    #endregion

    #region Testes de criar Veículo (POST /veiculos)

    [TestMethod]
    public async Task CriarVeiculoComDadosValidosEAutorizacaoEditorDeveRetornarCreated()
    {
        // Arrange
        var token = await LoginAssincronoEPegarToken("editor@teste.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var veiculoDto = new VeiculoDTO
        {
            Nome = "Jetta",
            Marca = "VW",
            Ano = 2020
        };
        var content = JsonContent.Create(veiculoDto);

        // Act
        var response = await _client.PostAsync("/veiculos", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(response.Headers.Location, "O cabeçalho Location não foi localizado");
    }

    [TestMethod]
    public async Task CriarVeiculoSemAutorizacaoDeveRetornarUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null; // garante que não ha ngm logado
        var veiculoDto = new VeiculoDTO
        {
            Nome = "Jetta",
            Marca = "VW",
            Ano = 2020
        };
        var content = JsonContent.Create(veiculoDto);
        // Act
        var response = await _client.PostAsync("/veiculos", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    #endregion
}

        // Arrange

        // Act

        // Assert