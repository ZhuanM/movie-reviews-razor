using System.ComponentModel.DataAnnotations;

namespace NBU.Models
{
  public class ReviewLikes
  {
    public int ID { get; set; }

    [Required]
    public string UserID { get; set; }

    [Required]
    public int ReviewID { get; set; }

    public ReviewRating ReviewRating { get; set; }
  }
}