using System;
using Play.Catalog.Service.Models.Dtos;
using Play.Catalog.Service.Models.Entities;

namespace Play.Catalog.Service.Extensions
{
    public static class ItemExtension
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto
            (
                item.Id,
                item.Name,
                item.Description,
                item.Price,
                item.CreatedDate
            );
        }
    }
}
