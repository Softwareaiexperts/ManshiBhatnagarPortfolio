using System.ComponentModel.DataAnnotations;

namespace ManshiBhatnagarPortfolio.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Slug { get; set; }

        public string Content { get; set; }

        public string ImageUrl { get; set; }

        public string Author { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int ReadTime { get; set; }

        public string Category { get; set; }
    }
}
