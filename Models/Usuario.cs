using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Models;

public class Usuario : IdentityUser
{
    public string? NomeCompleto { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public bool IsAdmin { get; set; } = true;
}
