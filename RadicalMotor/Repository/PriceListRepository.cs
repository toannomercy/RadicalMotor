using Microsoft.EntityFrameworkCore;
using RadicalMotor.Models;
namespace RadicalMotor.Repository
{
    public class PriceListRepository : IPriceListRepository
    {
        private readonly ApplicationDbContext _context;

        public PriceListRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PriceList> GetPriceListByIdAsync(string priceListId)
        {
            return await _context.PriceLists
                                 .FirstOrDefaultAsync(p => p.PriceListId == priceListId);
        }

        public async Task<IEnumerable<PriceList>> GetAllPriceListsAsync()
        {
            return await _context.PriceLists.ToListAsync();
        }

        public async Task<PriceList> AddPriceListAsync(PriceList priceList)
        {
            _context.PriceLists.Add(priceList);
            await _context.SaveChangesAsync();
            return priceList;
        }

        public async Task UpdatePriceListAsync(PriceList priceList)
        {
            _context.PriceLists.Update(priceList);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePriceListAsync(string priceListId)
        {
            var priceList = await _context.PriceLists
                                       .FirstOrDefaultAsync(p => p.PriceListId == priceListId);
            if (priceList != null)
            {
                _context.PriceLists.Remove(priceList);
                await _context.SaveChangesAsync();
            }
        }
    }
}
