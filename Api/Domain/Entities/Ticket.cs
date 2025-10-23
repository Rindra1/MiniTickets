using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Api.Domain.Entities;

namespace Api.Domain.Entities
{
    public class Ticket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("status")]
        public TicketStatuts Status { get; set; } = TicketStatuts.Open;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}