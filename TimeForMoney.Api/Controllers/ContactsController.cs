namespace TimeForMoney.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeForMoney.Api.Data;
using TimeForMoney.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase {
    private readonly AppDbContext _context;

    public ContactsController(AppDbContext context) {
        _context = context;
    }

    // GET: api/contacts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contact>>> GetContacts() {
        return await _context.Contacts.Include(c => c.Client).ToListAsync();
    }

    // GET: api/contacts/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Contact>> GetContact(int id) {
        var contact = await _context.Contacts.Include(c => c.Client).FirstOrDefaultAsync(c => c.Id == id);

        if (contact == null) {
            return NotFound("Contact with this ID was not found.");
        }

        return contact;
    }

    // POST: api/contacts
    [HttpPost]
    public async Task<ActionResult<Contact>> PostContact(Contact contact) {
        if (!await _context.Clients.AnyAsync(c => c.Id == contact.ClientId)) {
            return BadRequest($"Client with ID {contact.ClientId} does not exist.");
        }

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();

        var createdContact = await _context.Contacts
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == contact.Id);

        return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, createdContact);
    }

    // PUT: api/contacts/1
    [HttpPut("{id}")]
    public async Task<IActionResult> PutContact(int id, Contact contact) {
        if (id != contact.Id) {
            return BadRequest("ID in URL does not match ID in request body.");
        }
        
        if (!await _context.Clients.AnyAsync(c => c.Id == contact.ClientId)) {
            return BadRequest($"Client with ID {contact.ClientId} does not exist.");
        }

        _context.Entry(contact).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            if (!await _context.Contacts.AnyAsync(c => c.Id == id)) {
                return NotFound("Contact with this ID was not found.");
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/contacts/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(int id) {
        var contact = await _context.Contacts.FindAsync(id);

        if (contact == null) {
            return NotFound("Contact with this ID was not found.");
        }

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();

        return NoContent(); 
    }
}