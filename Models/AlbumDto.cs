namespace HarmonySound.API.DTOs
{
    public class AlbumDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset CreationDate { get; set; }

        public string ArtistName { get; set; }
        public List<ContentDto> Contents { get; set; }
    }
}

