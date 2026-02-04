namespace TimeForMoney.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeForMoney.Api.Data;
using TimeForMoney.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase {
    private readonly AppDbContext _context;

    public SessionsController(AppDbContext context) {
        _context = context;
    }

    // GET: api/sessions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Session>>> GetSessions() {
        return await _context.Sessions.Include(s => s.Client).ToListAsync();
    }

    // GET: api/sessions/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Session>> GetSession(int id) {
        var session = await _context.Sessions.Include(s => s.Client).FirstOrDefaultAsync(s => s.Id == id);

        if (session == null) {
            return NotFound();
        }

        return session;
    }

    // POST: api/sessions
    [HttpPost]
    public async Task<ActionResult<Session>> PostSession(Session session) {
        if (!await _context.Clients.AnyAsync(c => c.Id == session.ClientId)) {
            return BadRequest($"Klient s ID {session.ClientId} neexistuje.");
        }

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
    }

}
