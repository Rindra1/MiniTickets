using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    public class TicketCreateDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}

