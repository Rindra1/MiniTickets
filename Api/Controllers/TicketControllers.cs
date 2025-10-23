using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Services;
using Api.Dtos;
using Api.Domain.Entities;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly TicketService _service;

        public TicketsController(TicketService service)
        {
            _service = service;
        }

        /// Liste les tickets (filtre par status, pagination).
        /// GET /api/tickets?status=Open&page=1&pageSize=10
        
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            TicketStatuts? st = null;
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TicketStatuts>(status, true, out var p)) st = p;
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (items, total) = await _service.ListAsync(st, page, pageSize);
            return Ok(new { items, total, page, pageSize });
        }

        /// Récupère un ticket par id.
        
        [HttpGet("{id:length(24)}")]
        public async Task<IActionResult> GetById(string id)
        {
            var t = await _service.GetByIdAsync(id);
            if (t is null) return NotFound();
            return Ok(t);
        }

        /// Crée un ticket.
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TicketCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // double validation serveur (sécurisé)
            if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Length < 3 || dto.Title.Length > 100)
                return BadRequest(new { error = "Title must be 3..100 chars" });

            if (dto.Description?.Length > 500)
                return BadRequest(new { error = "Description max 500 chars" });

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// Met à jour le status d'un ticket.
        
        [HttpPut("{id:length(24)}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] TicketStatusUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.Status == null) return BadRequest(new { error = "Status required" });

            try
            {
                await _service.UpdateStatusAsync(id, dto.Status.Value);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
