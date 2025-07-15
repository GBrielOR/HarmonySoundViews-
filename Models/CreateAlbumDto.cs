
using System.ComponentModel.DataAnnotations;

namespace HarmonySound.API.DTOs
{
    public class CreateAlbumDto
    {
        [Required] 
        public string Title { get; set; }

        public int ArtistId { get; set; }
    }
}
