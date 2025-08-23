using MinimalApi;

namespace Test;

public class AdministradorServicoMock : IAdministradorServico
{
    private static List<Administrador> _administradores = new List<Administrador>()
    {
        new Administrador{
            Id = 1,
            Email = "adm@teste.com",
            Senha = "123456",
            Perfil = "Adm"
        },
        new Administrador{
            Id = 2,
            Email = "editor@teste.com",
            Senha = "123456",
            Perfil = "Editor"
        }
    };

    public Administrador? BuscarPorId(int id)
    {
        return _administradores.Find(a => a.Id == id);
    }

    public Administrador Incluir(Administrador administrador)
    {
        administrador.Id = _administradores.Count() + 1;
        _administradores.Add(administrador);
        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        return _administradores.Find(a => a.Email == loginDTO.Email);
    }

    public List<Administrador> Todos(int? pagina)
    {
        return _administradores;
    }
}
