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

    public async Task<IActionResult> IndexMyReviews()
    {
      // Gets user's reviews
      var myReviews = await _context.Review
          .Where(creator => creator.Creator == User.FindFirstValue(ClaimTypes.Email))
          .ToListAsync();

      // Check if reviews exists
      if (myReviews == null)
      {
        return NotFound();
      }

      return View(myReviews);
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

    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
      // Check if id is null
      if (id == null)
      {
        return NotFound();
      }

      // Gets review by id
      var review = await _context.Review.FindAsync(id);

      // Check if review exists
      if (review == null)
      {
        return NotFound();
      }

      return View(review);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ID, Likes, Creator, Movie, Comment")] Review review)
    {
      // Check if id matches
      if (id != review.ID)
      {
        return NotFound();
      }

      // Check if data can be posted to server (if incoming values from the request can be binded to the model correctly)
      if (!ModelState.IsValid)
      {
        return View(review);
      }

      // Saves changes to db
      _context.Update(review);
      await _context.SaveChangesAsync();

      // return RedirectToAction(nameof(Index));
      return RedirectToAction(nameof(IndexMyReviews));
    }

    public async Task<IActionResult> Delete(int? id)
    {
      // Check if id is null
      if (id == null)
      {
        return NotFound();
      }

      // Gets review by id
      var review = await _context.Review.FindAsync(id);

      // Check if review exists
      if (review == null)
      {
        return NotFound();
      }

      // Deletes review from db
      _context.Review.Remove(review);
      await _context.SaveChangesAsync();

      // return RedirectToAction(nameof(Index));
      return RedirectToAction(nameof(IndexMyReviews));
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

      // Increments review like count
      review.Likes += 1;

      // Saves changes to db
      _context.Update(review);
      await _context.SaveChangesAsync();

      // return View(Index);
      return View(IndexMyReviews);
    }
  }
}