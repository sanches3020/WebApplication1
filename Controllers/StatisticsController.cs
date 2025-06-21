using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoodMapper.Data;
using MoodMapper.ViewModels;

namespace MoodMapper.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly AppDbContext _context;

        public StatisticsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Statistics/Index
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var stats = await _context.Moods
                .Where(m => m.UserId == userId)
                .GroupBy(m => m.Emotion)
                .Select(g => new EmotionStatisticsViewModel
                {
                    Emotion = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            if (stats == null || stats.Count == 0)
            {
                TempData["Message"] = "Данные для статистики отсутствуют.";
                return View(new List<EmotionStatisticsViewModel>());
            }

            return View(stats);
        }

        // GET: /Statistics/GetMoodStatistics
        [HttpGet]
        public IActionResult GetMoodStatistics(DateTime? startDate, DateTime? endDate)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var query = _context.Moods
                .Where(m => m.UserId == userId.Value);

            if (startDate.HasValue)
                query = query.Where(m => m.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.Date <= endDate.Value);

            var grouped = query
                .GroupBy(m => m.Emotion)
                .Select(g => new EmotionStatisticsViewModel
                {
                    Emotion = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(e => e.Count)
                .ToList();

            var topEmotion = grouped.FirstOrDefault()?.Emotion;

            return Json(new
            {
                Data = grouped,
                TopEmotion = topEmotion
            });
        }
    }
}
