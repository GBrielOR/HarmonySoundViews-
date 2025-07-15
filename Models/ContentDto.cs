namespace HarmonySound.API.DTOs
{
    public class ContentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; } // "Canción", "Podcast"
        public TimeSpan Duration { get; set; }
        public DateTimeOffset UploadDate { get; set; }
        public int ArtistId { get; set; }
        public string? ArtistName { get; set; }
        public string? AlbumTitle { get; set; }
    }
}
