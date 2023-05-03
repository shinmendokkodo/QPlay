using System;

namespace Play.Catalog.Service.Models.Dtos
{
    public record ItemDto
    (
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        DateTimeOffset CreatedDate
    );
}