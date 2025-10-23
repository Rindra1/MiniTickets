using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.Interfaces;
using MongoDB.Driver;

namespace Api.Infrastructure.Repositories
{
    public class MongoTicketRepository : ITicketRepositorie
    {
        private readonly IMongoCollection<Ticket> _col;

        public MongoTicketRepository(IMongoDatabase db)
        {
            _col = db.GetCollection<Ticket>("tickets");
        }

        public async Task<Ticket> CreateAsync(Ticket t)
        {
            await _col.InsertOneAsync(t);
            return t;
        }

        public async Task<Ticket?> GetByIdAsync(string id)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.Id, id);
            return await _col.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Ticket> Items, long Total)> ListAsync(TicketStatuts? status, int page, int pageSize)
        {
            var filter = status.HasValue ?
                Builders<Ticket>.Filter.Eq(x => x.Status, status.Value) :
                Builders<Ticket>.Filter.Empty;

            var total = await _col.CountDocumentsAsync(filter);
            var items = await _col.Find(filter)
                                  .SortByDescending(x => x.CreatedAt)
                                  .Skip((page - 1) * pageSize)
                                  .Limit(pageSize)
                                  .ToListAsync();
            return (items, total);
        }

        public async Task UpdateStatusAsync(string id, TicketStatuts status)
        {
            var update = Builders<Ticket>.Update
                .Set(x => x.Status, status)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var res = await _col.UpdateOneAsync(Builders<Ticket>.Filter.Eq(x => x.Id, id), update);

            if (res.MatchedCount == 0)
                throw new KeyNotFoundException($"Ticket {id} not found");
        }

        public async Task EnsureIndexesAsync()
        {
            var keys = Builders<Ticket>.IndexKeys.Ascending(x => x.Status).Descending(x => x.CreatedAt);
            await _col.Indexes.CreateOneAsync(new CreateIndexModel<Ticket>(keys));
        }
    }
}
