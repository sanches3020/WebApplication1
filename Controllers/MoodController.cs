using Microsoft.AspNetCore.Mvc;
using MoodMapper.Data;
using MoodMapper.Models;
using MoodMapper.Services.Json;
using Microsoft.EntityFrameworkCore;

namespace MoodMapper.Controllers
{
    public class MoodController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmotionService _emotionService;

        public MoodController(AppDbContext context, EmotionService emotionService)
        {
            _context = context;
            _emotionService = emotionService;
        }

        // GET: /Mood/Index
        public IActionResult Index(int? year, int? month)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var reference = DateTime.Today;
            if (year.HasValue && month.HasValue)
            {
                try
                {
                    reference = new DateTime(year.Value, month.Value, 1);
                }
                catch
                {
                    reference = DateTime.Today;
                }
            }

            var today = DateTime.Today;
            var firstDay = new DateTime(reference.Year, reference.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(reference.Year, reference.Month);
            var firstDayOfWeek = (int)firstDay.DayOfWeek;
            if (firstDayOfWeek == 0) firstDayOfWeek = 7;

            var moods = _context.Moods
                .Where(m => m.UserId == userId &&
                            m.Date.Year == reference.Year &&
                            m.Date.Month == reference.Month)
                .ToList();

            var saved = moods
                .GroupBy(m => m.Date.Day)
                .ToDictionary(g => g.Key, g => g.First().Emotion);

            var viewModel = new CalendarViewModel
            {
                Year = reference.Year,
                Month = reference.Month,
                DaysInMonth = daysInMonth,
                FirstDayOfWeek = firstDayOfWeek,
                TodayDay = (reference.Year == today.Year && reference.Month == today.Month) ? today.Day : 0,
                SavedEmotions = saved
            };

            return View(viewModel);
        }

        // GET: /Mood/GetEmotionsModal
        [HttpGet]
        public async Task<IActionResult> GetEmotionsModal()
        {
            var emotions = await _emotionService.GetEmotionsAsync();
            return PartialView("_EmotionsModal", emotions);
        }

        // POST: /Mood/SaveMood
        [HttpPost]
        public async Task<IActionResult> SaveMood(string emotion, string date)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(emotion) || string.IsNullOrEmpty(date))
                return BadRequest("Отсутствует эмоция или дата.");

            if (!DateTime.TryParse(date, out DateTime moodDate))
                return BadRequest("Неверный формат даты.");

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
                return Unauthorized();

            var existingMood = await _context.Moods
                .FirstOrDefaultAsync(m => m.UserId == user.Id && m.Date.Date == moodDate.Date);

            if (existingMood != null)
            {
                existingMood.Emotion = emotion;
                existingMood.SubmittedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Moods.Add(new Mood
                {
                    Emotion = emotion,
                    Date = moodDate,
                    SubmittedAt = DateTime.UtcNow,
                    UserId = user.Id,
                    User = user
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
