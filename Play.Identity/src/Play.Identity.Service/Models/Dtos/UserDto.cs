using System;

namespace Play.Identity.Service.Models.Dtos
{
    public record UserDto
    (
        Guid Id,
        string UserName,
        string Email,
        decimal Gil,
        DateTimeOffset CreatedDate
    );
}
