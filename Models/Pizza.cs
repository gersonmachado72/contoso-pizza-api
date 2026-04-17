using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public class Pizza
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    public bool IsVegetarian { get; set; }
}
