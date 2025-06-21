using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoodMapper.Services.Json
{
    public class EmotionService
    {
        // Укажите путь к файлу emotions.json (например, в корне проекта)
        private readonly string _filePath = "emotions.json";

        public async Task<List<string>> GetEmotionsAsync()
        {
            if (!File.Exists(_filePath))
                return new List<string>();

            var json = await File.ReadAllTextAsync(_filePath);
            var emotions = JsonSerializer.Deserialize<List<string>>(json);
            return emotions ?? new List<string>();
        }
    }
}
