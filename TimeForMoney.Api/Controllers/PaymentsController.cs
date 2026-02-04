namespace TimeForMoney.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeForMoney.Api.Data;
using TimeForMoney.Api.Models;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase {
    private readonly AppDbContext _context;

    public PaymentsController(AppDbContext context) {
        _context = context;
    }

    // GET: api/payments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPayments() {
        return await _context.Payments.Include(p => p.Client).ToListAsync();
    }

    // GET: api/payments/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetPayment(int id) {
        var payment = await _context.Payments.Include(p => p.Client).FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null) {
            return NotFound();
        }

        return payment;
    }

    // POST: api/payments
    [HttpPost]
    public async Task<ActionResult<Payment>> PostPayment(Payment payment) {
        if (!await _context.Clients.AnyAsync(c => c.Id == payment.ClientId)) {
            return BadRequest($"Klient s ID {payment.ClientId} neexistuje.");
        }

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        var createdPayment = await _context.Payments
            .Include(p => p.Client)
            .FirstOrDefaultAsync(p => p.Id == payment.Id);

        return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, createdPayment);
    }

    // PUT: api/payments/1
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPayment(int id, Payment payment) {
        if (id != payment.Id) {
            return BadRequest("ID v URL neodpovídá ID v těle požadavku.");
        }

        if (!await _context.Clients.AnyAsync(c => c.Id == payment.ClientId)) {
            return BadRequest($"Klient s ID {payment.ClientId} neexistuje.");
        }

        _context.Entry(payment).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            if (!await _context.Payments.AnyAsync(p => p.Id == id)) {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/payments/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayment(int id) {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null) {
            return NotFound();
        }

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
