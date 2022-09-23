using System.Security.Claims;
using NBU.Data;
using NBU.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NBU.Controllers
{
    public class MyReviewsController : Controller
    {
     private readonly ApplicationDbContext _context;
        public MyReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reviews
        //[Authorize]
        public async Task<IActionResult> Index()
        {
            var myReviews = await _context.Reviews
                .Where(creator => creator.Creator == User.FindFirstValue(ClaimTypes.Email))
                .ToListAsync();

            return View(myReviews);
        }

        // GET: Reviews/Details/{id}
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.ID == id);

            return View(review);
        }

        // GET: Review/Edit/{id}
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Review/Edit/{id}
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317{id}98.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID, Movie, Comment, Creator, Likes")] Review review)
        {
            if (id != review.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(review);
            }

            _context.Update(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/Delete/{id}
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            var review = await _context.Reviews.FindAsync(id);

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/Like/{id}
        [Authorize]
        public async Task<IActionResult> Like(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.ID == id);
            
            review.Likes += 1;
            _context.Update(review);
            await _context.SaveChangesAsync();

            return View(Index);
        }
    }
}