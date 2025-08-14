
namespace MinimalApi;

public class AdministradorServico : IAdministradorServico
{

    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto contexto)
    {
        this._contexto = contexto;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = this._contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }
}
