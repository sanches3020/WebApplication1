using MoodMapper.Models;
using System.ComponentModel.DataAnnotations;


public class User
{
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string AvatarPath { get; set; } = "default-avatar.png";

    public DateTime RegisteredAt { get; set; }

    public List<Mood> Moods { get; set; } = new();
    public List<Note> Notes { get; set; } = new();
}
