using Microsoft.AspNetCore.Mvc;
using NBU.Models;
using NBU.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace NBU.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reviews
        //[Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reviews.ToListAsync());
        }

        // GET: Reviews/Details/5
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

        // GET: Reviews/Create
        //[Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Review/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID, Movie, Comment, Creator, Likes")] Review review)
        {
            var creator =  User.FindFirstValue(ClaimTypes.Email);
            review.Creator = creator;

            if (!ModelState.IsValid)
            {
                return View(review);
            }
            review.Likes = 1;
            _context.Add(review);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        // GET: Reviews/Like/5
        [Authorize]
        public async Task<IActionResult> Like(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.ID == id);
            
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            ReviewLikes? reviewLikes = await _context.ReviewsLikes
                .Where(u => u.UserID == userID)
                .Where(p => p.ReviewID == review.ID)
                .FirstOrDefaultAsync();

            if(reviewLikes == null)
            {
                ReviewLikes reviewRating = new ReviewLikes();
                review.Likes += 1;
                _context.Update(review);
                await _context.SaveChangesAsync();

                reviewRating.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                reviewRating.ReviewID = review.ID;
                reviewRating.ReviewRating = ReviewRating.Like;

                _context.Add(reviewRating);
                await _context.SaveChangesAsync();
            }
            //to here

            return RedirectToAction("Details", "Review", new { id = review?.ID });
        }

        // GET: Reviews/Dislike/5
        [Authorize]
        public async Task<IActionResult> Dislike(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.ID == id);
            
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ReviewLikes? reviewLikes = await _context.ReviewsLikes
                .Where(u => u.UserID == userID)
                .Where(p => p.ReviewID == review.ID)
                .FirstOrDefaultAsync();

            if(reviewLikes == null)
            {
                ReviewLikes reviewRating = new ReviewLikes();
                review.Likes -= 1;
                _context.Update(review);
                await _context.SaveChangesAsync();

                reviewRating.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                reviewRating.ReviewID = review.ID;
                reviewRating.ReviewRating = ReviewRating.Dislike;

                _context.Add(reviewRating);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Review", new { id = review.ID });
        }
    }
}