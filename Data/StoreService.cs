
using BlazorTest.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTest.Data
{
    public class StoreService
    {
        private readonly GroceryContext _context;

        public StoreService(GroceryContext context)
        {
            _context = context;
        }

        public async Task<List<Store>> GetStoresAsync()
        {
            return await _context.Store.OrderBy(s => s.Name).AsNoTracking().ToListAsync();
        }

        public Task<Store> CreateStoreAsync(Store objStore)
        {
            _context.Store.Add(objStore);
            _context.SaveChanges();
            return Task.FromResult(objStore);
        }

        public Task<bool> UpdateStoresync(Store objStore)
        {
            var ExistinStore = _context.Grocery.Where(x => x.Id == objStore.Id).FirstOrDefault();
            if (ExistinStore != null)
            {
                ExistinStore.Name = objStore.Name;
                _context.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public Task<bool> DeleteStoreAsync(Store objStore)
        {
            var ExistingStore = _context.Store.Where(x => x.Id == objStore.Id).FirstOrDefault();
            if (ExistingStore != null)
            {
                _context.Store.Remove(ExistingStore);
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
