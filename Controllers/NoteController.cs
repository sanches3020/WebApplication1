using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoodMapper.Data;
using MoodMapper.Models;

namespace MoodMapper.Controllers
{
    public class NoteController : Controller
    {
        private readonly AppDbContext _context;

        public NoteController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Note/Index
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notes);
        }

        // POST: /Note/SaveNote
        [HttpPost]
        public async Task<IActionResult> SaveNote(int? id, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userName = HttpContext.Session.GetString("UserName");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Message"] = "Заметка не может быть пустой.";
                return RedirectToAction("Index");
            }

            if (id.HasValue)
            {
                var note = await _context.Notes.FindAsync(id.Value);
                if (note != null && note.UserId == userId)
                {
                    note.Content = content;
                    _context.Update(note);
                }
            }
            else
            {
                var note = new Note
                {
                    Content = content,
                    UserId = userId.Value,
 
                    CreatedAt = DateTime.UtcNow
                };
                _context.Notes.Add(note);
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Заметка сохранена!";
            return RedirectToAction("Index");
        }

        // POST: /Note/DeleteNote
        [HttpPost]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var note = await _context.Notes.FindAsync(id);
            if (note == null || note.UserId != userId)
                return NotFound();

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Заметка удалена.";
            return RedirectToAction("Index");
        }
    }
}
