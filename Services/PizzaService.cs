using ContosoPizza.Data;
using ContosoPizza.Models;

namespace ContosoPizza.Services;

public class PizzaService
{
    private readonly AppDbContext _context;

    public PizzaService(AppDbContext context)
    {
        _context = context;

        // Garantir que existam pizzas no banco (apenas se a tabela estiver vazia)
        if (!_context.Pizzas.Any())
        {
            _context.Pizzas.AddRange(new List<Pizza>
            {
                new Pizza { Id = 1, Name = "Margherita", Price = 9.99M, IsVegetarian = true },
                new Pizza { Id = 2, Name = "Pepperoni", Price = 12.99M, IsVegetarian = false },
                new Pizza { Id = 3, Name = "Quattro Formaggi", Price = 14.99M, IsVegetarian = true },
                new Pizza { Id = 4, Name = "Calabresa", Price = 11.99M, IsVegetarian = false },
                new Pizza { Id = 5, Name = "Portuguesa", Price = 13.99M, IsVegetarian = false },
                new Pizza { Id = 6, Name = "Frango com Catupiry", Price = 14.99M, IsVegetarian = false },
                new Pizza { Id = 7, Name = "Vegetariana Especial", Price = 15.99M, IsVegetarian = true },
                new Pizza { Id = 8, Name = "Napolitana", Price = 12.99M, IsVegetarian = true },
                new Pizza { Id = 9, Name = "Mexicana", Price = 13.99M, IsVegetarian = false },
                new Pizza { Id = 10, Name = "Chocolate", Price = 18.99M, IsVegetarian = true }
            });
            _context.SaveChanges();
        }
    }

    public List<Pizza> GetAll() => _context.Pizzas.ToList();
    public Pizza? Get(int id) => _context.Pizzas.Find(id);
    public void Add(Pizza pizza) { _context.Pizzas.Add(pizza); _context.SaveChanges(); }
    public void Update(Pizza pizza) { _context.Pizzas.Update(pizza); _context.SaveChanges(); }
    public void Delete(int id) { var p = Get(id); if (p != null) { _context.Pizzas.Remove(p); _context.SaveChanges(); } }
}
