using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Models;

namespace ContosoPizza.Services;

public class PizzaService
{
    private readonly AppDbContext _context;

    public PizzaService(AppDbContext context)
    {
        _context = context;
    }

    public List<Pizza> GetAll()
    {
        return _context.Pizzas.ToList();
    }

    public Pizza? Get(int id)
    {
        return _context.Pizzas.Find(id);
    }

    public void Add(Pizza pizza)
    {
        pizza.Id = 0;
        _context.Pizzas.Add(pizza);
        _context.SaveChanges();
    }

    public void Update(Pizza pizza)
    {
        _context.Pizzas.Update(pizza);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var pizza = Get(id);
        if (pizza != null)
        {
            _context.Pizzas.Remove(pizza);
            _context.SaveChanges();
        }
    }
}
