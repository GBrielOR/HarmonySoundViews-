using HarmonySound.API.Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using HarmonySound.Models;
using HarmonySound.MVC.Models;
namespace HarmonySound.MVC.Controllers
{
    public class ContentsController : Controller
    {
        // GET: ContentsController
        public ActionResult Index()
        {
            var data = Crud<Content>.GetAll();
            return View(data);
        }

        // GET: ContentsController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Content>.GetById(id);
            return View(data);
        }

        // GET: ContentsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContentUploadDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using (var client = new HttpClient())
                {
                    var form = new MultipartFormDataContent();
                    form.Add(new StringContent(model.Title ?? ""), "Title");
                    form.Add(new StringContent(model.Type ?? ""), "Type");
                    form.Add(new StringContent(model.ArtistId.ToString()), "ArtistId");

                    if (model.File != null && model.File.Length > 0)
                    {
                        var streamContent = new StreamContent(model.File.OpenReadStream());
                        form.Add(streamContent, "File", model.File.FileName);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Debes seleccionar un archivo de audio.");
                        return View(model);
                    }

                    var response = await client.PostAsync("https://localhost:7120/api/Contents/upload", form);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", "Error al subir: " + error);
                        return View(model);
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(model);
            }
        }

        // GET: ContentsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Content>.GetById(id);
            return View(data);
        }

        // POST: ContentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Content data)
        {
            try
            {
                Crud<Content>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }

        // GET: ContentsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Content>.GetById(id);
            return View(data);
        }

        // POST: ContentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Content data)
        {
            try
            {
                Crud<Content>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }
    }
}
