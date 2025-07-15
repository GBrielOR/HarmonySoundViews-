namespace HarmonySound.API.DTOs
{
    public class PlaylistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PlaylistSongDto> Songs { get; set; }

    }
}
