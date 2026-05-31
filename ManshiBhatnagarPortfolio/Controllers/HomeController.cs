using System.Diagnostics;
using ManshiBhatnagarPortfolio.Data;
using ManshiBhatnagarPortfolio.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManshiBhatnagarPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles
                             .OrderByDescending(x => x.CreatedDate)
                             .Take(3)
                             .ToList();

            return View(articles);
        }
        public IActionResult CV()
        {
            return View();
        }
        public IActionResult Research()
        {
            return View();
        }
        public IActionResult Gallery()
        {
            var images = _context.GalleryImages
                                 .OrderByDescending(x => x.CreatedDate)
                                 .ToList();

            return View(images);
        }
        public IActionResult Projects()
        {
            return View();
        }
        public IActionResult Blog(string category)
        {
            var articlesQuery = _context.Articles.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                articlesQuery = articlesQuery.Where(x => x.Category == category);
            }

            var articles = articlesQuery
                            .OrderByDescending(x => x.CreatedDate)
                            .ToList();

            var categories = _context.Articles
                .GroupBy(x => x.Category)
                .Select(g => new CategoryViewModel
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var vm = new BlogPageViewModel
            {
                Articles = articles,
                Categories = categories,
                SelectedCategory = category
            };

            return View(vm);
        }
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                _context.ContactMessages.Add(model);
                await _context.SaveChangesAsync();
                TempData["ContactSuccess"] = "Thank you! Your message has been sent successfully.";
                return RedirectToAction("Contact");
            }

            return View(model);
        }
        public IActionResult BlogDetail(int id)
        {
            var article = _context.Articles.FirstOrDefault(x => x.Id == id);

            if (article == null)
                return NotFound();

            return View(article);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
