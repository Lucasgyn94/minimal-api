using MinimalApi;

namespace Test;


[TestClass]
public class VeiculoTest
{
    [TestMethod]
    public void TestarGetSetPropriedadesVeiculosComSucesso()
    {
        // Arrange
        var veiculo = new Veiculo();

        // Act
        veiculo.Id = 1;
        veiculo.Nome = "m200";
        veiculo.Marca = "bmw";
        veiculo.Ano = 2025;

        // Assert
        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual("m200", veiculo.Nome);
        Assert.AreEqual("bmw", veiculo.Marca);
        Assert.AreEqual(2025, veiculo.Ano);

    }

}
