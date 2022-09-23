using System.ComponentModel.DataAnnotations;

namespace NBU.Models
{
  public class Review
  {
    public int ID { get; set; }

    public int Likes { get; set; }

    [Required]
    public string Creator { get; set; }

    [Required]
    [StringLength(200, ErrorMessage = "Movie length can't be more than 200 characters long.")]
    public string Movie { get; set; }

    [Required]
    [StringLength(450, ErrorMessage = "Comment length can't be more than 450 characters long.")]
    public string Comment { get; set; }
  }
}
