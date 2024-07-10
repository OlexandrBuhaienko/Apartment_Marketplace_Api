using Apartment_Marketplace_API.Data;
using Apartment_Marketplace_API.Models.Domain;
using Apartment_Marketplace_API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Apartment_Marketplace_API.Repositories.Implementation
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly ApiDbContext _dbContext;

        public ApartmentRepository(ApiDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Apartment> CreateAsync(Apartment apartment)
        {
            await _dbContext.Apartments.AddAsync(apartment);
            await _dbContext.SaveChangesAsync();
            return apartment;
        }

        public async Task<IEnumerable<Apartment>> GetAllAsync()
        {
            return await _dbContext.Apartments.ToListAsync();
        }

        public async Task<Apartment?> GetById(Guid id)
        {
            return await _dbContext.Apartments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Apartment?> UpdateAsync(Apartment apartment)
        {
            var existingApartment = await _dbContext.Apartments.FirstOrDefaultAsync(x => x.Id == apartment.Id);
            if (existingApartment != null)
            {
                _dbContext.Entry(existingApartment).CurrentValues.SetValues(apartment);
                await _dbContext.SaveChangesAsync();
                return apartment;
            }
            return null;
        }
        public async Task<Apartment?> DeleteAsync(Guid id)
        {
            var existingApartment = await _dbContext.Apartments.FirstOrDefaultAsync(x => x.Id == id);
            if (existingApartment is null)
            {
                return null;
            }
            _dbContext.Remove(existingApartment);
            await _dbContext.SaveChangesAsync();
            return existingApartment;
        }
    }
}
