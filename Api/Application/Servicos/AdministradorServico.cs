

namespace MinimalApi;

public class AdministradorServico : IAdministradorServico
{

    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto contexto)
    {
        this._contexto = contexto;
    }

    public Administrador? BuscarPorId(int id)
    {
        return this._contexto.Administradores.Where(a => a.Id == id).FirstOrDefault();
    }

    public Administrador Incluir(Administrador administrador)
    {
        this._contexto.Administradores.Add(administrador);
        this._contexto.SaveChanges();
        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = this._contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = this._contexto.Administradores.AsQueryable();

        int itensPorPagina = 10;

        if (pagina != null)
        {
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
        return query.ToList();
    }
}
