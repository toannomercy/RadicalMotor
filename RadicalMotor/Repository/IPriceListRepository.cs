using RadicalMotor.Models;

namespace RadicalMotor.Repository
{
    public interface IPriceListRepository
    {
        Task<PriceList> GetPriceListByIdAsync(string priceListId);
        Task<IEnumerable<PriceList>> GetAllPriceListsAsync();
        Task<PriceList> AddPriceListAsync(PriceList priceList);
        Task UpdatePriceListAsync(PriceList priceList);
        Task DeletePriceListAsync(string priceListId);
    }
}
