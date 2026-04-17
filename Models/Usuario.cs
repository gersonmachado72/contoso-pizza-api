namespace ContosoPizza.Models;

public class Usuario
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Senha { get; set; }
    public string? NomeCompleto { get; set; }
    public bool IsAdmin { get; set; } = true;
}
