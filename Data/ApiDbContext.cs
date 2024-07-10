using Apartment_Marketplace_API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Apartment_Marketplace_API.Data
{
    public class ApiDbContext : DbContext
    {
        public virtual DbSet<Apartment> Apartments { get; set; }
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }
    }
}
