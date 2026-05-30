using ManshiBhatnagarPortfolio.Data;
using ManshiBhatnagarPortfolio.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManshiBhatnagarPortfolio.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.Articles.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }
        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Article article, IFormFile image)
        {
            foreach (var error in ModelState)
            {
                Console.WriteLine($"{error.Key} => {string.Join(",", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            if (image != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads");
                string fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                article.ImageUrl = "/uploads/" + fileName;
            }

            article.Slug = article.Title.Replace(" ", "-").ToLower();

            _context.Add(article);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddImage(GalleryImage model, IFormFile file)
        {
            if (file != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                model.ImageUrl = "/uploads/" + fileName;
            }

            _context.GalleryImages.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var article = _context.Articles.Find(id);
            if (article == null)
                return NotFound();

            return View(article);
        }
        [HttpPost]
        public IActionResult Edit(Article model, IFormFile imageFile)
        {

            var article = _context.Articles.Find(model.Id);
            if (article == null)
                return NotFound();

            article.Title = model.Title;
            article.Content = model.Content;
            article.Author = model.Author;
            article.Category = model.Category;
            article.ReadTime = model.ReadTime;

            if (imageFile != null && imageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                article.ImageUrl = "/uploads/" + fileName;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult DeleteArtical(int id)
        {
            var article = _context.Articles.Find(id);
            _context.Articles.Remove(article);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
