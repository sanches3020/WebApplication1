using System.ComponentModel.DataAnnotations;

namespace MoodMapper.Models
{
    public class Settings
    {
        public User User { get; set; }

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Theme { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}