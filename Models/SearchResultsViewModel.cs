using HarmonySound.Models;

namespace HarmonySound.MVC.Models
{
    public class SearchResultsViewModel
    {
        public string Query { get; set; }
        public List<User> Artists { get; set; } = new();
        public List<Content> Contents { get; set; } = new();
        public ProfileEditViewModel Profile { get; set; }
    }
}