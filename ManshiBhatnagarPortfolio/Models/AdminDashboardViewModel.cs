using System.Collections.Generic;

namespace ManshiBhatnagarPortfolio.Models
{
    public class AdminDashboardViewModel
    {
        public List<Article> Articles { get; set; }
        public List<GalleryImage> GalleryImages { get; set; }
        public List<ContactMessage> ContactMessages { get; set; }
        public int UnreadMessagesCount { get; set; }
    }
}
