using MinimalApi;

namespace Test;

public class VeiculoServicoMock : IVeiculoServico
{

    private static List<Veiculo> _veiculos = new List<Veiculo>()
    {
        new Veiculo{
            Nome = "Gol",
            Marca = "VW",
            Ano = 2010
        },
        new Veiculo{
            Nome = "Jetta",
            Marca = "VW",
            Ano = 2020
        }
    };

    public void Apagar(Veiculo veiculo)
    {
        var veiculoParaApagar = _veiculos.Find(v => v.Id == veiculo.Id);
        if (veiculoParaApagar != null)
        {
            _veiculos.Remove(veiculoParaApagar);
        }
    }

    public void Atualizar(Veiculo veiculo)
    {
        // Encontrando indice do veículo na lista
        var index = _veiculos.FindIndex(v => v.Id == veiculo.Id);

        if (index != -1)
        {
            _veiculos[index] = veiculo;
        }
    }

    public Veiculo? BuscarPorId(int id)
    {
        return _veiculos.Find(v => v.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
        if (veiculo != null)
        {
            _veiculos.Add(veiculo);
        }
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        IEnumerable<Veiculo> consulta = _veiculos;

        // aplicando filtro de nome quando fornecido
        if (!string.IsNullOrEmpty(nome))
        {
            consulta = consulta.Where(v => v.Nome.ToLower() == nome.ToLower());
        }

        // aplicando filtro de nome quando fornecido
        if (!string.IsNullOrEmpty(marca))
        {
            consulta = consulta.Where(v => v.Marca.ToLower() == marca.ToLower());
        }

        const int itensPorPagina = 10;
        if (pagina.HasValue && pagina > 0)
        {
            consulta = consulta.Skip((pagina.Value - 1) * itensPorPagina).Take(itensPorPagina);
        }

        return consulta.ToList();
    }
}
