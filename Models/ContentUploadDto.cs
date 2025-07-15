namespace HarmonySound.MVC.Models
{
    public class ContentUploadDto
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int ArtistId { get; set; }
    }
}
