namespace Apartment_Marketplace_API.Models.DTO
{
    public class ApartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Rooms { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
