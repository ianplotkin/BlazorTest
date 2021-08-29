
using BlazorTest.Data.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTest.Data
{
    public class CategoryService
    {
        private readonly GroceryContext _context;

        public CategoryService(GroceryContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Category.AsNoTracking().ToListAsync();
        }

        public Task<Category> CreateCategoryAsync(Category objCategory)
        {
            _context.Category.Add(objCategory);
            _context.SaveChanges();
            return Task.FromResult(objCategory);
        }

        public Task<bool> UpdateCategoryAsync(Category objCategory)
        {
            var ExistingCategory = _context.Category.Where(x => x.Id == objCategory.Id).FirstOrDefault();
            if (ExistingCategory != null)
            {
                ExistingCategory.Name = objCategory.Name;
                _context.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public Task<bool> DeleteCategoryAsync(Category objCategory)
        {
            var ExistingCategory = _context.Category.Where(x => x.Id == objCategory.Id).FirstOrDefault();
            if (ExistingCategory != null)
            {
                _context.Category.Remove(ExistingCategory);
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
