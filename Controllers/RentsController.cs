using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookRental.Data;
using BookRental.Models;

namespace BookRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentsController : ControllerBase
    {
        private readonly Library _context;

        public RentsController(Library context)
        {
            _context = context;
        }

        // Create a new rent
        [HttpPost]
        public async Task<ActionResult<Rent>> PostRent(RentRequest request)
        {
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null || book.IsBorrowed)
            {
                return BadRequest("Book is not available for rent.");
            }

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            book.IsBorrowed = true;
            var rent = new Rent
            {
                BookId = request.BookId,
                Book = book,
                UserId = request.UserId,
                User = user,
                BorrowedAt = DateTime.Now
            };
            _context.Rents.Add(rent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRent), new { id = rent.Id }, rent);
        }

        // Get details of a rent by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Rent>> GetRent(int id)
        {
            var rent = await _context.Rents.Include(l => l.Book).Include(l => l.User).FirstOrDefaultAsync(l => l.Id == id);

            if (rent == null)
            {
                return NotFound();
            }

            return rent;
        }

        // Confirm the return of a borrowed book
        [HttpPut("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var rent = await _context.Rents.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
            if (rent == null || rent.ReturnedAt != null)
            {
                return BadRequest("Invalid rent record or book already returned.");
            }

            rent.ReturnedAt = DateTime.Now;
            rent.Book.IsBorrowed = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
