using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Models.Dtos
{
    public record CreateItemDto
    (
        [Required]
        [MaxLength(30)]
        string Name,
        [Required]
        [MaxLength(100)]
        string Description,
        [Range(0, 1000)]
        decimal Price
    );
}