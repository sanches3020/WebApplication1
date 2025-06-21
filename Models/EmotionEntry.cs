public class EmotionEntry
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public DateTime Date { get; set; }
    public string Emotion { get; set; } = string.Empty;

    public User User { get; set; }
}
