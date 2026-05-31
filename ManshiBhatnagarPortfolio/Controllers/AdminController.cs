using ManshiBhatnagarPortfolio.Data;
using ManshiBhatnagarPortfolio.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Linq;

namespace ManshiBhatnagarPortfolio.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                Articles = _context.Articles.ToList(),
                GalleryImages = _context.GalleryImages.ToList(),
                ContactMessages = _context.ContactMessages.OrderByDescending(x => x.CreatedDate).ToList(),
                UnreadMessagesCount = _context.ContactMessages.Count(x => !x.IsRead)
            };
            return View(viewModel);
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

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            var actionDescriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;

            var bypassActions = new[] { "Login", "ProcessGoogleLogin", "DevLogin", "Logout" };

            if (actionName != null && !bypassActions.Contains(actionName))
            {
                var adminEmail = HttpContext.Session.GetString("AdminEmail");
                if (string.IsNullOrEmpty(adminEmail))
                {
                    context.Result = RedirectToAction("Login");
                }
            }

            base.OnActionExecuting(context);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = _configuration["Authentication:Google:ClientId"];
            return View();
        }

        [HttpPost]
        public IActionResult ProcessGoogleLogin([FromForm] string credential)
        {
            if (string.IsNullOrEmpty(credential))
            {
                TempData["LoginError"] = "Google sign-in credential was empty.";
                return RedirectToAction("Login");
            }

            try
            {
                var tokenParts = credential.Split('.');
                if (tokenParts.Length < 2)
                {
                    TempData["LoginError"] = "Invalid Google credential token format.";
                    return RedirectToAction("Login");
                }

                var payload = tokenParts[1];
                var padLength = 4 - (payload.Length % 4);
                if (padLength < 4) payload = payload.PadRight(payload.Length + padLength, '=');
                
                var jsonBytes = Convert.FromBase64String(payload);
                var jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

                using (var doc = JsonDocument.Parse(jsonString))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("email", out var emailProp))
                    {
                        string email = emailProp.GetString() ?? "";
                        
                        var allowedAdminsStr = _configuration["AllowedAdmins"] ?? "";
                        var allowedAdmins = allowedAdminsStr.Split('|', StringSplitOptions.RemoveEmptyEntries)
                                                            .Select(x => x.Trim().ToLower())
                                                            .ToList();

                        if (allowedAdmins.Contains(email.ToLower()))
                        {
                            HttpContext.Session.SetString("AdminEmail", email);
                            HttpContext.Session.SetString("AdminName", root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "Admin" : "Admin");
                            HttpContext.Session.SetString("AdminAvatar", root.TryGetProperty("picture", out var picProp) ? picProp.GetString() ?? "" : "");
                            
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["LoginError"] = $"Access Denied: '{email}' is not authorized in appsettings.json.";
                            return RedirectToAction("Login");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["LoginError"] = $"Authentication failed: {ex.Message}";
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult DevLogin(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["LoginError"] = "Email cannot be empty.";
                return RedirectToAction("Login");
            }

            var allowedAdminsStr = _configuration["AllowedAdmins"] ?? "";
            var allowedAdmins = allowedAdminsStr.Split('|', StringSplitOptions.RemoveEmptyEntries)
                                                .Select(x => x.Trim().ToLower())
                                                .ToList();

            if (allowedAdmins.Contains(email.ToLower()))
            {
                HttpContext.Session.SetString("AdminEmail", email);
                HttpContext.Session.SetString("AdminName", "Developer Mode");
                HttpContext.Session.SetString("AdminAvatar", "");
                
                return RedirectToAction("Index");
            }
            else
            {
                TempData["LoginError"] = $"Access Denied: '{email}' is not whitelisted in appsettings.json.";
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
