using System.Collections.Generic;
using System.Linq;
using ContosoPizza.Models;

namespace ContosoPizza.Services;

public static class PizzaService
{
    private static List<Pizza> _pizzas = new()
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
    private static int _nextId = 11;

    public static List<Pizza> GetAll()
    {
        return _pizzas;
    }

    public static Pizza? Get(int id)
    {
        return _pizzas.FirstOrDefault(p => p.Id == id);
    }

    public static void Add(Pizza pizza)
    {
        pizza.Id = _nextId++;
        _pizzas.Add(pizza);
    }

    public static void Update(Pizza pizza)
    {
        var index = _pizzas.FindIndex(p => p.Id == pizza.Id);
        if (index != -1)
        {
            _pizzas[index] = pizza;
        }
    }

    public static void Delete(int id)
    {
        var pizza = Get(id);
        if (pizza != null)
        {
            _pizzas.Remove(pizza);
        }
    }
}
