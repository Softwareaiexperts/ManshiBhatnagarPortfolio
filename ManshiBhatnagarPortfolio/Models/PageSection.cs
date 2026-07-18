using System.ComponentModel.DataAnnotations;

namespace ManshiBhatnagarPortfolio.Models
{
    public class PageSection
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PageName { get; set; } = string.Empty; // "About", "CV", "Research"

        [Required]
        [StringLength(100)]
        public string SectionKey { get; set; } = string.Empty; // e.g. "hero_title"

        [StringLength(50)]
        public string SectionType { get; set; } = "text"; // "text", "html", "image", "percentage"

        [StringLength(200)]
        public string? Title { get; set; } // Display label in admin

        public string Content { get; set; } = string.Empty; // The actual value

        public int SortOrder { get; set; } = 0;
    }
}
