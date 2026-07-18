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

        // ===== HELPERS =====
        private Dictionary<string, string> GetPageSections(string pageName)
        {
            return _context.PageSections
                .Where(s => s.PageName == pageName)
                .ToDictionary(s => s.SectionKey, s => s.Content);
        }

        private void UpdateSection(string pageName, string sectionKey, string? value)
        {
            if (value == null) return;
            var section = _context.PageSections.FirstOrDefault(s => s.PageName == pageName && s.SectionKey == sectionKey);
            if (section != null)
            {
                section.Content = value;
            }
        }

        private async Task<string?> SaveUploadedFile(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;
            string folder = Path.Combine(_env.WebRootPath, "uploads");
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(folder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "/uploads/" + fileName;
        }

        // ===== DASHBOARD =====
        public IActionResult Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                Articles = _context.Articles.ToList(),
                GalleryImages = _context.GalleryImages.ToList(),
                ContactMessages = _context.ContactMessages.OrderByDescending(x => x.CreatedDate).ToList(),
                UnreadMessagesCount = _context.ContactMessages.Count(x => !x.IsRead)
            };
            ViewBag.UnreadCount = viewModel.UnreadMessagesCount;
            return View(viewModel);
        }

        // ===== BLOG CRUD =====
        public IActionResult Create()
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

        public IActionResult AddImage()
        {
            return View();
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
            if (article != null)
            {
                _context.Articles.Remove(article);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditImage(int id)
        {
            var image = _context.GalleryImages.Find(id);
            if (image == null)
                return NotFound();

            return View(image);
        }

        [HttpPost]
        public async Task<IActionResult> EditImage(GalleryImage model, IFormFile file)
        {
            var image = _context.GalleryImages.Find(model.Id);
            if (image == null)
                return NotFound();

            image.Title = model.Title;
            image.Description = model.Description;

            if (file != null && file.Length > 0)
            {
                // Delete old physical file if it exists
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting old file: {ex.Message}");
                        }
                    }
                }

                // Save new file
                string folder = Path.Combine(_env.WebRootPath, "uploads");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                image.ImageUrl = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult DeleteImage(int id)
        {
            var image = _context.GalleryImages.Find(id);
            if (image == null)
                return NotFound();

            // Delete physical file from filesystem if it exists
            if (!string.IsNullOrEmpty(image.ImageUrl))
            {
                var filePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting file: {ex.Message}");
                    }
                }
            }

            _context.GalleryImages.Remove(image);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // ===== CONTACT MESSAGES =====
        [HttpPost]
        public IActionResult MarkMessageRead(int id)
        {
            var message = _context.ContactMessages.Find(id);
            if (message == null)
                return NotFound();

            message.IsRead = true;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteMessage(int id)
        {
            var message = _context.ContactMessages.Find(id);
            if (message == null)
                return NotFound();

            _context.ContactMessages.Remove(message);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ===== PAGE EDITORS =====

        // --- ABOUT PAGE ---
        [HttpGet]
        public IActionResult EditAbout()
        {
            ViewBag.Sections = GetPageSections("About");
            ViewBag.UnreadCount = _context.ContactMessages.Count(x => !x.IsRead);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveAbout(
            string about_hero_tagline, string about_hero_name, string about_hero_subtitle,
            string about_intro_heading, string about_intro_text,
            string about_stat_number, string about_stat_label,
            string about_mission_heading, string about_mission_subtitle,
            string about_mission_1_title, string about_mission_1_text,
            string about_mission_2_title, string about_mission_2_text,
            string about_mission_3_title, string about_mission_3_text,
            IFormFile? about_hero_image_file, IFormFile? about_intro_image_file,
            IFormFile? about_mission_1_image_file, IFormFile? about_mission_2_image_file,
            IFormFile? about_mission_3_image_file)
        {
            UpdateSection("About", "about_hero_tagline", about_hero_tagline);
            UpdateSection("About", "about_hero_name", about_hero_name);
            UpdateSection("About", "about_hero_subtitle", about_hero_subtitle);
            UpdateSection("About", "about_intro_heading", about_intro_heading);
            UpdateSection("About", "about_intro_text", about_intro_text);
            UpdateSection("About", "about_stat_number", about_stat_number);
            UpdateSection("About", "about_stat_label", about_stat_label);
            UpdateSection("About", "about_mission_heading", about_mission_heading);
            UpdateSection("About", "about_mission_subtitle", about_mission_subtitle);
            UpdateSection("About", "about_mission_1_title", about_mission_1_title);
            UpdateSection("About", "about_mission_1_text", about_mission_1_text);
            UpdateSection("About", "about_mission_2_title", about_mission_2_title);
            UpdateSection("About", "about_mission_2_text", about_mission_2_text);
            UpdateSection("About", "about_mission_3_title", about_mission_3_title);
            UpdateSection("About", "about_mission_3_text", about_mission_3_text);

            // Handle image uploads
            var heroImg = await SaveUploadedFile(about_hero_image_file);
            if (heroImg != null) UpdateSection("About", "about_hero_image", heroImg);

            var introImg = await SaveUploadedFile(about_intro_image_file);
            if (introImg != null) UpdateSection("About", "about_intro_image", introImg);

            var m1Img = await SaveUploadedFile(about_mission_1_image_file);
            if (m1Img != null) UpdateSection("About", "about_mission_1_image", m1Img);

            var m2Img = await SaveUploadedFile(about_mission_2_image_file);
            if (m2Img != null) UpdateSection("About", "about_mission_2_image", m2Img);

            var m3Img = await SaveUploadedFile(about_mission_3_image_file);
            if (m3Img != null) UpdateSection("About", "about_mission_3_image", m3Img);

            await _context.SaveChangesAsync();
            TempData["Success"] = "About page updated successfully!";
            return RedirectToAction("EditAbout");
        }

        // --- CV PAGE ---
        [HttpGet]
        public IActionResult EditCV()
        {
            ViewBag.Sections = GetPageSections("CV");
            ViewBag.UnreadCount = _context.ContactMessages.Count(x => !x.IsRead);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveCV(IFormCollection form)
        {
            var cvSections = _context.PageSections.Where(s => s.PageName == "CV").ToList();
            foreach (var section in cvSections)
            {
                if (form.ContainsKey(section.SectionKey))
                {
                    section.Content = form[section.SectionKey].ToString();
                }
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "CV page updated successfully!";
            return RedirectToAction("EditCV");
        }

        // --- RESEARCH PAGE ---
        [HttpGet]
        public IActionResult EditResearch()
        {
            ViewBag.Sections = GetPageSections("Research");
            ViewBag.UnreadCount = _context.ContactMessages.Count(x => !x.IsRead);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveResearch(
            string research_hero_title,
            string research_1_heading, string research_1_text,
            string research_2_heading, string research_2_text,
            IFormFile? research_1_image_file, IFormFile? research_2_image_file)
        {
            UpdateSection("Research", "research_hero_title", research_hero_title);
            UpdateSection("Research", "research_1_heading", research_1_heading);
            UpdateSection("Research", "research_1_text", research_1_text);
            UpdateSection("Research", "research_2_heading", research_2_heading);
            UpdateSection("Research", "research_2_text", research_2_text);

            var img1 = await SaveUploadedFile(research_1_image_file);
            if (img1 != null) UpdateSection("Research", "research_1_image", img1);

            var img2 = await SaveUploadedFile(research_2_image_file);
            if (img2 != null) UpdateSection("Research", "research_2_image", img2);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Research page updated successfully!";
            return RedirectToAction("EditResearch");
        }

        // --- CONNECT LINKS ---
        [HttpGet]
        public IActionResult EditConnect()
        {
            ViewBag.Sections = GetPageSections("Connect");
            ViewBag.UnreadCount = _context.ContactMessages.Count(x => !x.IsRead);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveConnect(string connect_linkedin, string connect_twitter, string connect_researchgate, string connect_github)
        {
            UpdateSection("Connect", "connect_linkedin", connect_linkedin);
            UpdateSection("Connect", "connect_twitter", connect_twitter);
            UpdateSection("Connect", "connect_researchgate", connect_researchgate);
            UpdateSection("Connect", "connect_github", connect_github);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Connect links updated successfully!";
            return RedirectToAction("EditConnect");
        }
    }
}
