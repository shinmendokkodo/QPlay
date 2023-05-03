using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service.Models.Dtos
{
    public record UpdateUserDto
    (
        [Required]
        [EmailAddress]
        string Email,
        [Range(0, 1000000)]
        decimal Gil
    );
}
