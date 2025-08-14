

using Microsoft.EntityFrameworkCore;

namespace MinimalApi;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;

    public VeiculoServico(DbContexto contexto)
    {
        this._contexto = contexto;
    }

    public void Apagar(Veiculo veiculo)
    {
        this._contexto.Veiculos.Remove(veiculo);
        this._contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        this._contexto.Veiculos.Update(veiculo);
        this._contexto.SaveChanges();
    }

    public Veiculo? BuscarPorId(int id)
    {
        return this._contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
        this._contexto.Veiculos.Add(veiculo);
        this._contexto.SaveChanges();
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = this._contexto.Veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}%"));
        }

        int itensPorPagina = 10;

        if (pagina != null)
        {
            query = query.Skip(((int) pagina - 1) * itensPorPagina).Take(itensPorPagina);

        }

        return query.ToList();
    }
}
