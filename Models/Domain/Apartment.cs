using System.ComponentModel.DataAnnotations;

namespace Apartment_Marketplace_API.Models.Domain
{
    public class Apartment
    {
        [Key]
        public Guid Id { get; set; }
        public uint Rooms { get; set; }
        public string Name { get; set; }
        public uint Price { get; set; }
        public string Description { get; set; }
    }
}
