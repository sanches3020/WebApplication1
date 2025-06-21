using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Mood
{
    public int Id { get; set; }

    [Required]
    public string Emotion { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime Date { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    // связь с пользователем
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
