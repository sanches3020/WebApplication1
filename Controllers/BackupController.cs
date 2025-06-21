using Microsoft.AspNetCore.Mvc;
using MoodMapper.Data;
using MoodMapper.Models;
using System.Text.Json;

namespace MoodMapper.Controllers
{
    public class BackupController : Controller
    {
        private readonly AppDbContext _context;

        public BackupController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Export()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var notes = _context.Notes
                .Where(n => n.UserId == userId)
                .Select(n => new { Content = n.Content })
                .ToList();

            var emotions = _context.EmotionEntries
                .Where(e => e.UserId == userId)
                .Select(e => new { e.Date, e.Emotion })
                .ToList();

            var theme = HttpContext.Session.GetString("Theme") ?? "light";

            var backup = new
            {
                notes,
                emotions,
                settings = new { theme }
            };

            var json = JsonSerializer.Serialize(backup, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", "MoodMapper-backup.json");
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || file == null || file.Length == 0) return BadRequest();

            using var stream = new StreamReader(file.OpenReadStream());
            var json = await stream.ReadToEndAsync();

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Импорт заметок
                if (root.TryGetProperty("notes", out var notesElement))
                {
                    foreach (var note in notesElement.EnumerateArray())
                    {
                        var content = note.GetProperty("Content").GetString();
                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            _context.Notes.Add(new Note
                            {
                                UserId = userId.Value,
                                Content = content,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }

                // Импорт эмоций
                if (root.TryGetProperty("emotions", out var emotionsElement))
                {
                    foreach (var entry in emotionsElement.EnumerateArray())
                    {
                        var date = entry.GetProperty("Date").GetDateTime();
                        var emoji = entry.GetProperty("Emotion").GetString();

                        _context.EmotionEntries.Add(new EmotionEntry
                        {
                            UserId = userId.Value,
                            Date = date,
                            Emotion = emoji ?? "😶"
                        });
                    }
                }

                // Импорт темы
                if (root.TryGetProperty("settings", out var settingsElement) &&
                    settingsElement.TryGetProperty("theme", out var themeElement))
                {
                    var theme = themeElement.GetString();
                    if (!string.IsNullOrEmpty(theme))
                        HttpContext.Session.SetString("Theme", theme);
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Импорт не удался: " + ex.Message);
                return BadRequest("Файл повреждён или содержит недопустимую структуру.");
            }
        }
    }
}
