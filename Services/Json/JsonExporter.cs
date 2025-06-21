using System.Text.Json;
using MoodMapper.Models; 

namespace MoodMapper.Services.Json
{
    public class JsonExporter
    {

        private readonly string _path = "mood_export.json";

        /// <summary>
        /// Асинхронный метод для экспорта данных пользователя.
        /// </summary>
        /// <param name="user">Объект пользователя (ApplicationUser)</param>
        /// <param name="moods">Список записей настроений</param>
        /// <param name="notes">Список заметок пользователя</param>
        public async Task ExportAsync(User user, List<Mood> moods, List<Note> notes)
        {
     
            var exportData = new
            {
                user = new
                {
                    user.Email
                },

                moodEntries = moods.Select(m => new
                {
                    date = m.Date.ToString("yyyy-MM-dd"), 
                    emotion = m.Emotion
                }),

                notes = notes.Select(n => new
                {
                    date = n.CreatedAt.ToString("yyyy-MM-dd"),
                    content = n.Content
                })
            };

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });

            await File.WriteAllTextAsync(_path, json);
        }
    }
}
