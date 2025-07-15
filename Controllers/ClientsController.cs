using HarmonySound.API.Consumer;
using HarmonySound.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HarmonySound.MVC.Models;
namespace HarmonySound.MVC.Controllers
{
    [Authorize(Roles = "client")]
    public class ClientsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            Crud<Content>.EndPoint = "https://localhost:7120/api/Contents";
            var contenidos = Crud<Content>.GetAll();
            return View(contenidos);
        }
        public async Task<IActionResult> EditProfile()
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://localhost:7120/api/Users/profile/{userId}");
                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var json = await response.Content.ReadAsStringAsync();
                var dto = System.Text.Json.JsonSerializer.Deserialize<ProfileEditViewModel>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    Console.WriteLine(error.ErrorMessage);

                return View(model);
            }

            // Si NO se sube una nueva imagen, recupera la URL actual antes de actualizar
            if (model.ProfileImageFile == null || model.ProfileImageFile.Length == 0)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7120/api/Users/profile/{model.Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var dto = System.Text.Json.JsonSerializer.Deserialize<ProfileEditViewModel>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        model.ProfileImageUrl = dto?.ProfileImageUrl;
                    }
                }
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var form = new MultipartFormDataContent();
                    form.Add(new StringContent(model.Id.ToString()), "UserId");
                    form.Add(new StreamContent(model.ProfileImageFile.OpenReadStream()), "file", model.ProfileImageFile.FileName);

                    var response = await client.PostAsync("https://localhost:7120/api/Users/upload-profile-image", form);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = System.Text.Json.JsonDocument.Parse(json);
                        if (result.RootElement.TryGetProperty("ProfileImageUrl", out var urlProp) ||
                            result.RootElement.TryGetProperty("profileImageUrl", out urlProp))
                        {
                            model.ProfileImageUrl = urlProp.GetString();
                        }
                        else
                        {
                            ModelState.AddModelError("", "No se recibió la URL de la imagen del servidor.");
                            return View(model);
                        }
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error uploading profile image: {error}");
                        return View(model);
                    }
                }
            }

            // Actualizar el resto del perfil
            var userUpdate = new
            {
                Id = model.Id,
                Name = model.Name,
                Biography = model.Biography,
                ProfileImageUrl = model.ProfileImageUrl,
                Email = model.Email
            };

            using (var client = new HttpClient())
            {
                var json = System.Text.Json.JsonSerializer.Serialize(userUpdate);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"https://localhost:7120/api/Users/profile/{model.Id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error updating profile: {error}");
                    return View(model);
                }
            }
            return RedirectToAction("Home");
        }

        public async Task<IActionResult> Home(string query)
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var model = new SearchResultsViewModel { Query = query };

            // Obtén el perfil del usuario
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://localhost:7120/api/Users/profile/{userId}");
                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var json = await response.Content.ReadAsStringAsync();
                model.Profile = System.Text.Json.JsonSerializer.Deserialize<ProfileEditViewModel>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // Si hay búsqueda, consulta artistas y contenidos
            if (!string.IsNullOrWhiteSpace(query))
            {
                using (var client = new HttpClient())
                {
                    // Buscar artistas
                    var artistsResponse = await client.GetAsync($"https://localhost:7120/api/Users/search?query={Uri.EscapeDataString(query)}");
                    if (artistsResponse.IsSuccessStatusCode)
                    {
                        var artistsJson = await artistsResponse.Content.ReadAsStringAsync();
                        model.Artists = System.Text.Json.JsonSerializer.Deserialize<List<User>>(artistsJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }

                    // Buscar contenidos
                    var contentsResponse = await client.GetAsync($"https://localhost:7120/api/Contents/search?query={Uri.EscapeDataString(query)}");
                    if (contentsResponse.IsSuccessStatusCode)
                    {
                        var contentsJson = await contentsResponse.Content.ReadAsStringAsync();
                        model.Contents = System.Text.Json.JsonSerializer.Deserialize<List<Content>>(contentsJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }
            }

            ViewBag.Success = TempData["Success"];
            return View(model);
        }
    }
}
