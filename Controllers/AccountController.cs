using HarmonySound.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using HarmonySound.API.Consumer;
using HarmonySound.Models;

namespace HarmonySound.MVC.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var apiModel = new
            {
                Email = model.Email,
                Password = model.Password
            };

            var json = JsonConvert.SerializeObject(apiModel);

            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://localhost:7120/api/Auth/Login", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", "Error: " + errorContent);
                        return View(model);
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResult = JsonConvert.DeserializeObject<LoginResult>(responseContent);

                    if (string.IsNullOrEmpty(loginResult?.Token))
                    {
                        ModelState.AddModelError("", "No se pudo iniciar sesión. Verifica tu correo y contraseña, y asegúrate de haber confirmado tu email.");
                        return View(model);
                    }

                    // Decodificar el JWT y extraer los claims
                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(loginResult.Token);
                    var claimsList = jwt.Claims.ToList();

                    // Log de claims para depuración
                    System.Diagnostics.Debug.WriteLine("Claims del usuario:");
                    foreach (var claim in claimsList)
                        System.Diagnostics.Debug.WriteLine($"{claim.Type}: {claim.Value}");

                    // Asegurarse de que haya un NameIdentifier
                    if (!claimsList.Any(c => c.Type == ClaimTypes.NameIdentifier))
                    {
                        var sub = claimsList.FirstOrDefault(c => c.Type == "sub")?.Value;
                        if (sub != null)
                            claimsList.Add(new Claim(ClaimTypes.NameIdentifier, sub));
                        else
                            claimsList.Add(new Claim(ClaimTypes.NameIdentifier, model.Email)); // fallback
                    }

                    var claimsIdentity = new ClaimsIdentity(claimsList, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    // Buscar el rol del usuario (ahora solo ClaimTypes.Role si el mapeo está bien)
                    var roleClaim = claimsList.FirstOrDefault(c =>
                        c.Type == ClaimTypes.Role ||
                        c.Type == "role" ||
                        c.Type == "roles" ||
                        c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

                    var role = roleClaim?.Value;

                    if (string.IsNullOrEmpty(role))
                    {
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        ModelState.AddModelError("", "Tu cuenta no tiene un rol asignado. Contacta al administrador.");
                        return View(model);
                    }

                    // Redirigir según el rol
                    if (role.Equals("Client", System.StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction("Home", "Clients");
                    else if (role.Equals("Artist", System.StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction("Home", "Artists");
                    else
                    {
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        ModelState.AddModelError("", $"No tienes permisos para acceder con el rol '{role}'.");
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error de red o de servidor: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Account/Register
        public async Task<ActionResult> Register()
        {
            var roles = await GetRolesFromApi();
            var model = new RegisterViewModel
            {
                Roles = roles
            };
            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            // Elimina el error de Roles antes de validar
            ModelState.Remove(nameof(model.Roles));

            if (!ModelState.IsValid)
            {
                model.Roles = await GetRolesFromApi();
                return View(model);
            }

            var apiModel = new
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            };

            var json = JsonConvert.SerializeObject(apiModel);

            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://localhost:7120/api/Auth/Register", content);

                    var errorContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Redirige a la vista de confirmación de registro
                        return RedirectToAction("RegisterConfirmation", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo registrar el usuario: " + errorContent);
                        model.Roles = await GetRolesFromApi();
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error de red o de servidor: " + ex.Message);
                model.Roles = await GetRolesFromApi();
                return View(model);
            }
        }

        // Método auxiliar para obtener los roles desde la API
        private async Task<List<string>> GetRolesFromApi()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://localhost:7120/api/Roles");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var roles = JsonConvert.DeserializeObject<List<RoleDto>>(content);
                        return roles?.Select(r => r.Name).ToList() ?? new List<string>();
                    }
                }
            }
            catch
            {
                // Opcional: loguear el error
            }
            return new List<string>();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        private string GetRoleFromJwt(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
                return null;

            var payload = parts[1];
            var jsonBytes = Convert.FromBase64String(PadBase64(payload));
            var json = System.Text.Encoding.UTF8.GetString(jsonBytes);

            var payloadData = Newtonsoft.Json.Linq.JObject.Parse(json);

            // Busca varios posibles nombres de claim
            return (payloadData["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ??
                    payloadData["role"] ??
                    payloadData["roles"])?.ToString();
        }

        private string PadBase64(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return base64.Replace('-', '+').Replace('_', '/');
        }
    }

    public class LoginResult
    {
        public string Token { get; set; }
    }
}