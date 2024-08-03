using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required(ErrorMessage = "User is required.")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot be longer than 1000 characters.")]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
