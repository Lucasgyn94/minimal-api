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
        /*Inclusão do bcrypt para antes de salvar gerar o ash da senha informada, o segundo parâmetro (work factor) define a 'força' do hash (11 ou 12 é um bom valor) */
        administrador.Senha = BCrypt.Net.BCrypt.HashPassword(administrador.Senha, workFactor: 11);

        this._contexto.Administradores.Add(administrador);
        this._contexto.SaveChanges();
        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        //var adm = this._contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        var adm = this._contexto.Administradores.FirstOrDefault(a => a.Email == loginDTO.Email);

        if (adm == null)
        {
            return null;
        }

        /*Verificando se a senha fornecida no login (loginDTO.Senha) corresponde ao hash salvo no banco (adm.Senha).*/
        if (BCrypt.Net.BCrypt.Verify(loginDTO.Senha, adm.Senha))
        {
            return adm;
        }

        return null;
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
