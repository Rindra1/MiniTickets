using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Domain.Interfaces
{
    public interface ITicketRepositorie
    {
        Task<Ticket> CreateAsync(Ticket t);
        Task<Ticket?> GetByIdAsync(string id);
        Task<(IEnumerable<Ticket> Items, long Total)> ListAsync(TicketStatuts? status, int page, int pageSize);
        Task UpdateStatusAsync(string id, TicketStatuts status);
        Task EnsureIndexesAsync();
    }
}
