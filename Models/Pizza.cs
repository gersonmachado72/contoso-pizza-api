using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public class Pizza
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da pizza é obrigatório")]
    [MinLength(3, ErrorMessage = "O nome deve ter pelo menos 3 caracteres")]
    public string? Name { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal Price { get; set; }

    public bool IsVegetarian { get; set; }
}
