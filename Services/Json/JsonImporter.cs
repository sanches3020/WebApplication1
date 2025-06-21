using System.Text.Json;
using MoodMapper.Models;

namespace MoodMapper.Services.Json
{
    public class JsonImporter
    {
        /// <summary>
        /// Импортирует данные из указанного JSON-файла и преобразует их в объекты домена.
        /// </summary>
        /// <param name="path">Путь к JSON-файлу с экспортированными данными.</param>
        /// <returns>Кортеж, содержащий объект пользователя, список настроений и список заметок.</returns>
        public async Task<(User user, List<Mood> moods, List<Note> notes)> ImportAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Файл не найден: {path}");

            var jsonContent = await File.ReadAllTextAsync(path);

            var exportData = JsonSerializer.Deserialize<ExportData>(jsonContent);
            if (exportData == null)
                throw new Exception("Ошибка при десериализации JSON");

            var user = new User
            {
                Email = exportData.user.Email,
                RegisteredAt = DateTime.UtcNow
            };

            var moods = exportData.moodEntries.Select(m => new Mood
            {
                Date = DateTime.Parse(m.date),
                Emotion = m.emotion,
                SubmittedAt = DateTime.UtcNow,
                User = user
            }).ToList();

            var notes = exportData.notes.Select(n => new Note
            {
                CreatedAt = DateTime.Parse(n.date),
                Content = n.content,
                User = user
            }).ToList();

            return (user, moods, notes);
        }

        private class ExportData
        {
            public required ExportedUser user { get; set; }
            public required List<ExportedMood> moodEntries { get; set; }
            public required List<ExportedNote> notes { get; set; }
        }

        private class ExportedUser
        {
            public required string UserName { get; set; }
            public required string Email { get; set; }
        }

        private class ExportedMood
        {
            public required string date { get; set; }
            public required string emotion { get; set; }
        }

        private class ExportedNote
        {
            public required string date { get; set; }
            public required string content { get; set; }
        }
    }
}
