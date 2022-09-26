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

    public async Task<IActionResult> IndexReview()
    {
      return View(await _context.Review.ToListAsync());
    }

    [Authorize]
    public async Task<IActionResult> Details(int? id)
    {
      // Check if id is null
      if (id == null)
      {
        return NotFound();
      }

      // Gets review by id
      var review = await _context.Review
          .FirstOrDefaultAsync(m => m.ID == id);

      // Check if review exists
      if (review == null)
      {
        return NotFound();
      }

      return View(review);
    }

    public IActionResult Create()
    {
      return View();
    }


    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID, Likes, Creator, Movie, Comment")] Review review)
    {
      // Get creator from logged user
      var creator = User.FindFirstValue(ClaimTypes.Email);
      review.Creator = creator;

      // Check if data can be posted to server (if incoming values from the request can be binded to the model correctly)
      if (!ModelState.IsValid)
      {
        return View(review);
      }

      // Increments review like count
      review.Likes = 1;

      // Saves changes to db
      _context.Add(review);
      await _context.SaveChangesAsync();

      // return RedirectToAction(nameof(Index));
      return RedirectToAction(nameof(IndexReview));
    }


    [Authorize]
    public async Task<IActionResult> Like(int? id)
    {
      // Check if id is null
      if (id == null)
      {
        return NotFound();
      }

      // Gets review by id
      var review = await _context.Review
          .FirstOrDefaultAsync(m => m.ID == id);

      // Check if review exists
      if (review == null)
      {
        return NotFound();
      }

      // Get logger user id
      var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

      // Gets rating object
      ReviewLikes? reviewLikes = await _context.ReviewsLikes
          .Where(u => u.UserID == userID)
          .Where(p => p.ReviewID == review.ID)
          .FirstOrDefaultAsync();

      // Check if rating object exists otherwise create a new one and save to db
      if (reviewLikes == null)
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

      return RedirectToAction("Details", "Review", new { id = review?.ID });
    }

    [Authorize]
    public async Task<IActionResult> Dislike(int? id)
    {
      // Check if id is null
      if (id == null)
      {
        return NotFound();
      }

      // Gets review by id
      var review = await _context.Review
          .FirstOrDefaultAsync(m => m.ID == id);

      // Check if review exists
      if (review == null)
      {
        return NotFound();
      }

      // Get logger user id
      var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

      // Gets rating object
      ReviewLikes? reviewLikes = await _context.ReviewsLikes
          .Where(u => u.UserID == userID)
          .Where(p => p.ReviewID == review.ID)
          .FirstOrDefaultAsync();

      // Check if rating object exists otherwise create a new one and save to db
      if (reviewLikes == null)
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