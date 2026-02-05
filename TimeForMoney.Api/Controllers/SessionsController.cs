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
    public async Task<ActionResult<Session>> GetSession([FromRoute] int id) {
        var session = await _context.Sessions.Include(s => s.Client).FirstOrDefaultAsync(s => s.Id == id);

        if (session == null) {
            return NotFound("Session with this ID was not found.");
        }

        return session;
    }

    // POST: api/sessions
    [HttpPost]
    public async Task<ActionResult<Session>> PostSession([FromBody] Session session) {
        if (!await _context.Clients.AnyAsync(c => c.Id == session.ClientId)) {
            return BadRequest($"Client with ID {session.ClientId} does not exist.");
        }

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        var createdSession = await _context.Sessions
            .Include(s => s.Client)
            .FirstOrDefaultAsync(s => s.Id == session.Id);

        return CreatedAtAction(nameof(GetSession), new { id = session.Id }, createdSession);
    }

    // PUT: api/sessions/1
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSession([FromRoute] int id, [FromBody] Session session) {
        if (id != session.Id) {
            return BadRequest("ID in URL does not match ID in request body.");
        }

        if (!await _context.Clients.AnyAsync(c => c.Id == session.ClientId)) {
            return BadRequest($"Client with ID {session.ClientId} does not exist.");
        }

        _context.Entry(session).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            if (!await _context.Sessions.AnyAsync(s => s.Id == id)) {
                return NotFound("Session with this ID was not found.");
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/sessions/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSession([FromRoute] int id) {
        var session = await _context.Sessions.FindAsync(id);

        if (session == null) {
            return NotFound("Session with this ID was not found.");
        }

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
