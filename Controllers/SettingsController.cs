using Microsoft.AspNetCore.Mvc;
using MoodMapper.Data;

namespace MoodMapper.Controllers
{
    public class SettingsController : Controller
    {
        private readonly AppDbContext _context;

        public SettingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Settings/Index  
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var theme = HttpContext.Session.GetString("UserTheme") ?? "default";

            ViewBag.UserEmail = user.Email;
            ViewBag.UserAvatar = string.IsNullOrEmpty(user.AvatarPath) ? "default-avatar.png" : user.AvatarPath;
            ViewBag.UserTheme = theme;

            return View();
        }

        // POST: /Settings/UploadAvatar  
        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (avatar == null || avatar.Length == 0)
            {
                ModelState.AddModelError("", "Выберите изображение.");
                return View("Index");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var fileName = $"{user.Id}_{Path.GetFileName(avatar.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            user.AvatarPath = fileName;
            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Аватар успешно загружен!";
            return RedirectToAction("Index");
        }

        // POST: /Settings/ChangeTheme  
        [HttpPost]
        public IActionResult ChangeTheme(string theme)
        {
            if (string.IsNullOrWhiteSpace(theme))
            {
                ModelState.AddModelError("", "Тема не может быть пустой.");
                return View("Index");
            }

            HttpContext.Session.SetString("UserTheme", theme);
            TempData["Message"] = "Тема успешно обновлена!";
            return RedirectToAction("Index");
        }

        // POST: /Settings/ChangeNickname  
        [HttpPost]
        public async Task<IActionResult> ChangeNickname(string nickname)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(nickname))
            {
                ModelState.AddModelError("", "Имя не может быть пустым.");
                return View("Index");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Имя успешно обновлено!";
            return RedirectToAction("Index");
        }
    }
}
