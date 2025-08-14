namespace MinimalApi;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);

}
