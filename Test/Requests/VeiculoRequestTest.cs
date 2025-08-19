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

    [TestMethod]
    public async Task CriarVeiculoComDadosInvalidosDeveRetornarBadRequest()
    {
        // arrange
        var token = await LoginAssincronoEPegarToken("editor@teste.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var veiculoDto = new VeiculoDTO
        {
            Nome = "Fusca",
            Marca = "VW",
            Ano = 1920
        };
        var content = JsonContent.Create(veiculoDto);

        // act
        var response = await _client.PostAsync("/veiculos", content);

        // assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

    }
    #endregion

    #region Testes de Obter Veículos (GET /veiculos e GET /veiculos/{id})

    [TestMethod]
    public async Task ObterVeiculoPorIdComIdValidoEAutorizacaoDeveRetornarOk()
    {
        // Arrange
        var token = await LoginAssincronoEPegarToken("editor@teste.com", "123456");
        var veiculoCriado = await CriarVeiculoParaTesteAsync(token);

        // Act
        var response = await _client.GetAsync($"/veiculos/{veiculoCriado.Id}");

        // Assert        
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var veiculoRetornado = await response.Content.ReadFromJsonAsync<Veiculo>(_jsonOptions);
        Assert.IsNotNull(veiculoRetornado);
        Assert.AreEqual(veiculoCriado.Nome, veiculoCriado.Nome);
    }

    [TestMethod]
    public async Task ObterVeiculoPorIdComIdInvalidoDeveRetornarNotFound()
    {
        // Arrange
        var token = await LoginAssincronoEPegarToken(
            "editor@teste.com",
            "123456"
        );
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

        // Act
        var response = await _client.GetAsync("/veiculos/999"); // passando id invalido

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion

    #region Testes de Atualizar Veículo {PUT /veiculos/{id}}

    [TestMethod]
    public async Task AtualizarVeiculoComDadosValidosEAutorizacaoAdminDeveRetornarOk()
    {
        // Arrange
        var tokenAdmin = await LoginAssincronoEPegarToken(
            "adm@teste.com",
            "123456"
        );

        var veiculoCriado = await CriarVeiculoParaTesteAsync(tokenAdmin);

        var veiculoAtualizadoDto = new VeiculoDTO
        {
            Nome = "C-200",
            Marca = "Mercedez",
            Ano = 2025
        };
        var content = JsonContent.Create(veiculoAtualizadoDto);

        // Act
        var response = await _client.PutAsync($"/veiculos/{veiculoCriado.Id}", content);

        // Assert        
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var veiculoRetornado = await response.Content.ReadFromJsonAsync<Veiculo>(_jsonOptions);
        Assert.IsNotNull(veiculoRetornado);
        Assert.AreEqual("C-200", veiculoRetornado.Nome);
    }

    [TestMethod]
    public async Task AtualizarVeiculoComAutorizacaoEditorDeveRetornarForbidden()
    {
        // Arrange
        var tokenAdmin = await LoginAssincronoEPegarToken(
            "adm@teste.com",
            "123456"
        );
        var veiculoCriado = await CriarVeiculoParaTesteAsync(tokenAdmin);

        var tokenEditor = await LoginAssincronoEPegarToken(
            "editor@teste.com",
            "123456"
        );
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", tokenEditor);

        var veiculoAtualizadoDto = new VeiculoDTO
        {
            Nome = "C-200",
            Marca = "Mercedez",
            Ano = 2024
        };
        var content = JsonContent.Create(veiculoAtualizadoDto);

        // Act
        var response = await _client.PutAsync($"/veiculos/{veiculoCriado}", content);

        // Assert        
        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Testes de Apagar Veículo (DELETE / veiculos/{id})

    [TestMethod]
    public async Task ApagarVeiculoComIdValidoEAutorizacaoAdminDeveRetornarNoContent()
    {
        // Arrange
        var tokenAdmin = await LoginAssincronoEPegarToken(
            "adm@teste.com",
            "123456"
        );
        var veiculoCriado = await CriarVeiculoParaTesteAsync(tokenAdmin);

        // Act
        var response = await _client.DeleteAsync($"/veiculos/{veiculoCriado.Id}");

        // Assert 
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);       

    }

    [TestMethod]
    public async Task ApagarVeiculoComAutorizacaoEditorDeveRetornarForbidden()
    {
        // Arrange
        var tokenAdmin = await LoginAssincronoEPegarToken(
            "adm@teste.com",
            "123456"
        );
        var veiculoCriado = await CriarVeiculoParaTesteAsync(tokenAdmin);

        var tokenEditor = await LoginAssincronoEPegarToken(
            "editor@teste.com",
            "123456"
        );
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", tokenEditor);

        // Act
        var response = await _client.DeleteAsync($"veiculos/{veiculoCriado.Id}");

        // Assert

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion
}

