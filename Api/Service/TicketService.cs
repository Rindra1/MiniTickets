using System;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Domain.Entities;
using Api.Domain.Interfaces;

namespace Api.Services
{
    public class TicketService
    {
        private readonly ITicketRepositorie _repo;

        public TicketService(ITicketRepositorie repo)
        {
            _repo = repo;
        }

        public async Task<Ticket> CreateAsync(TicketCreateDto dto)
        {
            // validation beyond attributes if needed
            var now = DateTime.UtcNow;
            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = TicketStatuts.Open,
                CreatedAt = now,
                UpdatedAt = now
            };
            return await _repo.CreateAsync(ticket);
        }

        public Task<Ticket?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);

        public Task<(System.Collections.Generic.IEnumerable<Ticket> Items, long Total)> ListAsync(TicketStatuts? status, int page, int pageSize)
            => _repo.ListAsync(status, page, pageSize);

        public async Task UpdateStatusAsync(string id, TicketStatuts status)
        {
            await _repo.UpdateStatusAsync(id, status);
        }
    }
}
