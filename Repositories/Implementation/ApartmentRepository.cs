using Apartment_Marketplace_API.Data;
using Apartment_Marketplace_API.Models.Domain;
using Apartment_Marketplace_API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Apartment_Marketplace_API.Repositories.Implementation
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly ApiDbContext _dbContext;
        private (bool, string) ValidateApartments(Apartment apartment)
        {
            const uint maxNameLength = 99;
            const uint maxDescriptionLength = 999;
            const decimal maxPrice = 50000m;
            const uint maxRooms = 10;
            if (apartment.Name.Length > maxNameLength || apartment.Name == null)
            {
                return (false, $"Length of the apartments name cannot be longer than {maxNameLength} or null");
            }
            if (apartment.Description.Length > maxDescriptionLength || apartment.Description == null)
            {
                return (false, $"Length of the apartments description cannot be longer than {maxDescriptionLength} or null");
            }
            if (apartment.Price > maxPrice || apartment.Price < 0)
            {
                return (false, $"Price must be from 0 to {maxPrice}");
            }
            if (apartment.Rooms > maxRooms || apartment.Rooms <= 0)
            {
                return (false, $"Rooms number of the apartments must be from 1, to {maxRooms}");
            }
            return (true, "");
        }

        public ApartmentRepository(ApiDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Apartment> CreateAsync(Apartment apartment)
        {
            (bool, string) isValid = ValidateApartments(apartment);
            if(isValid.Item1 == false)
            {
                throw new BadHttpRequestException(isValid.Item2);
            }
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
            (bool, string) isValid = ValidateApartments(apartment);
            if (isValid.Item1 == false)
            {
                throw new BadHttpRequestException(isValid.Item2);
            }
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
