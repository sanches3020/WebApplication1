using Microsoft.AspNetCore.Mvc;
using MoodMapper.Data;
using MoodMapper.ViewModels;

namespace MoodMapper.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u =>
                    u.Email == model.Email &&
                    u.Password == model.Password
                );

                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("UserEmail", user.Email ?? "");
                    HttpContext.Session.SetString("UserName", user.Email?.Split('@')[0] ?? "User");
                    HttpContext.Session.SetString("UserAvatar", user.AvatarPath ?? "/images/default-avatar.png");

                    Console.WriteLine($"✅ Сессия установлена: UserId = {user.Id}");
                    return RedirectToAction("Index", "Mood");
                }

                ModelState.AddModelError("", "Неверный email или пароль.");
            }

            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Этот email уже зарегистрирован.");
                    return View(model);
                }

                var newUser = new User
                {
                    Email = model.Email,
                    Password = model.Password,
                    RegisteredAt = DateTime.UtcNow,
                    AvatarPath = "/images/default-avatar.png"
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("UserId", newUser.Id);
                HttpContext.Session.SetString("UserEmail", newUser.Email);
                HttpContext.Session.SetString("UserName", newUser.Email.Split('@')[0]);
                HttpContext.Session.SetString("UserAvatar", newUser.AvatarPath);

                Console.WriteLine($"🆕 Зарегистрирован и залогинен: UserId = {newUser.Id}");
                return RedirectToAction("Index", "Mood");
            }

            return View(model);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /Account/Profile
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login");

            return View(user);
        }

        // POST: /Account/UpdateProfile
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(IFormFile avatarFile, string newEmail)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToAction("Login");

            if (!string.IsNullOrWhiteSpace(newEmail) && newEmail != user.Email)
            {
                user.Email = newEmail;
                HttpContext.Session.SetString("UserEmail", newEmail);
                HttpContext.Session.SetString("UserName", newEmail.Split('@')[0]);
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

                var fileName = $"avatar_{userId}_{Guid.NewGuid()}{Path.GetExtension(avatarFile.FileName)}";
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                user.AvatarPath = "/uploads/" + fileName;
                HttpContext.Session.SetString("UserAvatar", user.AvatarPath);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: /Account/ChangePassword
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login");

            if (user.Password != oldPassword)
            {
                TempData["PasswordMessage"] = "Старый пароль неверен.";
                return RedirectToAction("Index");
            }

            if (newPassword != confirmPassword)
            {
                TempData["PasswordMessage"] = "Новый пароль и подтверждение не совпадают.";
                return RedirectToAction("Index");
            }

            user.Password = newPassword;
            _context.SaveChanges();

            TempData["PasswordMessage"] = "Пароль успешно изменён.";
            return RedirectToAction("Index");
        }
    }
}
