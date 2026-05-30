namespace ManshiBhatnagarPortfolio.Models
{
    public class GalleryImage
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }   // path of image

        public string Title { get; set; }      // e.g. Keynote Speech

        public string Description { get; set; } // e.g. Global Tech Summit

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
