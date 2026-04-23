using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Models;

namespace ContosoPizza.Services;

public static class PizzaService
{
    private static AppDbContext? _context;

    public static void Initialize(AppDbContext context)
    {
        _context = context;
    }

    public static List<Pizza> GetPizzasPadrao()
    {
        return new List<Pizza>
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
        };
    }

    public static List<Pizza> GetAll()
    {
        if (_context == null) return new List<Pizza>();
        return _context.Pizzas.ToList();
    }

    public static Pizza? Get(int id)
    {
        if (_context == null) return null;
        return _context.Pizzas.Find(id);
    }

    public static void Add(Pizza pizza)
    {
        if (_context == null) return;
        pizza.Id = 0; // Deixa o banco gerar o ID
        _context.Pizzas.Add(pizza);
        _context.SaveChanges();
    }

    public static void Update(Pizza pizza)
    {
        if (_context == null) return;
        _context.Pizzas.Update(pizza);
        _context.SaveChanges();
    }

    public static void Delete(int id)
    {
        if (_context == null) return;
        var pizza = Get(id);
        if (pizza != null)
        {
            _context.Pizzas.Remove(pizza);
            _context.SaveChanges();
        }
    }
}
