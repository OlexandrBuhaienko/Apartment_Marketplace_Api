using Apartment_Marketplace_API.Models.Domain;

namespace Apartment_Marketplace_API.Repositories.Interface
{
    public interface IApartmentRepository
    {
        Task<Apartment> CreateAsync(Apartment apartment);
        Task<IEnumerable<Apartment>> GetAllAsync();
        Task<Apartment?> GetById(Guid id);
        Task<Apartment?> UpdateAsync(Apartment apartment);
        Task<Apartment?> DeleteAsync(Guid id);
    }
}
