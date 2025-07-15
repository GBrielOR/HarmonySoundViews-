using HarmonySound.API.Consumer;
using HarmonySound.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HarmonySound.MVC.Controllers
{
    [Authorize(Roles = "artist")]
    public class ContentAlbumsController : Controller
    {
        // GET: ContentAlbumsController/Create?albumId=5
        public ActionResult Create(int albumId)
        {
            ViewBag.AlbumId = albumId;
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var mySongs = Crud<Content>.GetAll().Where(c => c.ArtistId == userId).ToList();
            ViewBag.MySongs = mySongs;
            return View();
        }

        // POST: ContentAlbumsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContentAlbum data)
        {
            try
            {
                Crud<ContentAlbum>.Create(data);
                return RedirectToAction("Details", "Albums", new { id = data.AlbumId });
            }
            catch
            {
                return View(data);
            }
        }

        // POST: ContentAlbumsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, int albumId)
        {
            try
            {
                Crud<ContentAlbum>.Delete(id);
                return RedirectToAction("Details", "Albums", new { id = albumId });
            }
            catch
            {
                return RedirectToAction("Details", "Albums", new { id = albumId });
            }
        }
    }
}
