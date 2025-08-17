namespace MinimalApi;

public record class AdministradorLogado
{
    public string Email { get; set; } = default!;
    public String Perfil { get; set; } = default!;
    public string Token { get; set; } = default!;
}
