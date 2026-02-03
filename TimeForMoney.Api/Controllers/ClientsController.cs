namespace TimeForMoney.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeForMoney.Api.Data;
using TimeForMoney.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase {
    private readonly AppDbContext _context;

    public ClientsController(AppDbContext context) {
        _context = context;
    }

    // GET: api/clients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetClients() {
        return await _context.Clients.ToListAsync();
    }

    // GET: api/clients/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetClient(int id) {
        var client = await _context.Clients.FindAsync(id);

        if (client == null) {
            return NotFound();
        }

        return client;
    }

    // POST: api/clients
    [HttpPost]
    public async Task<ActionResult<Client>> PostClient(Client client) {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
    }

    // PUT: api/clients/1
    [HttpPut("{id}")]
    public async Task<IActionResult> PutClient(int id, Client client) {
        if (id != client.Id) {
            return BadRequest("ID v URL neodpovídá ID v těle požadavku.");
        }

        _context.Entry(client).State = EntityState.Modified;
        
        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            if (!await _context.Clients.AnyAsync(c => c.Id == id)) {
                return NotFound("Klient s tímto ID nebyl nalezen.");
            }
            throw;
        }
        return NoContent();
    }

    // DELETE: api/clients/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id) {
        var client = await _context.Clients.FindAsync(id);

        if (client == null) {
            return NotFound("Klient s tímto ID nebyl nalezen.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}