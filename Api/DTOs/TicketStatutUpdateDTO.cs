using System.ComponentModel.DataAnnotations;
using Api.Domain.Entities;

namespace Api.Dtos
{
    public class TicketStatusUpdateDto
    {
        [Required]
        public TicketStatuts? Status { get; set; }
    }
}

