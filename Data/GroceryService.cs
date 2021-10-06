
using BlazorTest.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTest.Data
{
    public class GroceryService
    {
        private readonly GroceryContext _context;

        public GroceryService(GroceryContext context)
        {
            _context = context;
        }

        public async Task<List<Grocery>> GetGroceriesAsync()
        {
            return await _context.Grocery.Include(g => g.Category).OrderBy(g => g.Name).AsNoTracking().ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Category.OrderBy(g => g.Name).AsNoTracking().ToListAsync();
        }

        public Task<Grocery> CreateGroceryAsync(Grocery objGrocery)
        {
            _context.Grocery.Add(objGrocery);
            _context.SaveChanges();
            return Task.FromResult(objGrocery);
        }

        public Task<bool> UpdateGroceryAsync(Grocery objGrocery)
        {
            var ExistingGrocery = _context.Grocery.Where(x => x.Id == objGrocery.Id).FirstOrDefault();
            if (ExistingGrocery != null)
            {
                ExistingGrocery.Name = objGrocery.Name;
                ExistingGrocery.CategoryId = objGrocery.CategoryId;
                ExistingGrocery.DefaultAmount = objGrocery.DefaultAmount;
                ExistingGrocery.DefaultUnit = objGrocery.DefaultUnit;
                _context.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public Task<bool> DeleteGroceryAsync(Grocery objGrocery)
        {
            var ExistingGrocery = _context.Grocery.Where(x => x.Id == objGrocery.Id).FirstOrDefault();
            if (ExistingGrocery != null)
            {
                _context.Grocery.Remove(ExistingGrocery);
                _context.SaveChanges(); 
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }
}
