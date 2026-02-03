using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeForMoney.Api.Data;
using TimeForMoney.Api.Models;

namespace TimeForMoney.Api.Controllers;

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
}