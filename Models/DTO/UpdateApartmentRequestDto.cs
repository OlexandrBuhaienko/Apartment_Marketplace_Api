﻿namespace Apartment_Marketplace_API.Models.DTO
{
    public class UpdateApartmentRequestDto
    {
        public string Name { get; set; }
        public uint Rooms { get; set; }
        public uint Price { get; set; }
        public string Description { get; set; }
    }
}