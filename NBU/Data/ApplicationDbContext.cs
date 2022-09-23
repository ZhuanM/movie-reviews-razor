using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NBU.Models;

namespace NBU.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<ReviewLikes> ReviewsLikes { get; set; }
}
