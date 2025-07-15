using HarmonySound.Models;

namespace HarmonySound.MVC.Models
{
    public class AlbumDetailsViewModel
    {
        public Album Album { get; set; }
        public List<Content> AllArtistSongs { get; set; }
        public List<int> SelectedSongIds { get; set; }
    }
}
