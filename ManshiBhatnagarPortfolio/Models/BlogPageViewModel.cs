namespace ManshiBhatnagarPortfolio.Models
{
    public class BlogPageViewModel
    {
        public List<Article> Articles { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public string SelectedCategory { get; set; }
    }
}
