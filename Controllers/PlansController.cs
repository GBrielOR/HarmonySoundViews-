using HarmonySound.API.Consumer;
using HarmonySound.Models;
using HarmonySound.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace HarmonySound.MVC.Controllers
{
    
    public class PlansController : Controller
    {
        // GET: PlansController
        private readonly HttpClient _httpClient;

        public PlansController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [Authorize(Roles = "client")]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("https://localhost:7120/api/Plans");
            var json = await response.Content.ReadAsStringAsync();
            var plans = JsonSerializer.Deserialize<List<Plan>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var premiumPlans = plans.Where(p => p.Price > 0).ToList();

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userPlanResponse = await _httpClient.GetAsync($"https://localhost:7120/api/UserPlans/user/{userId}");

            UserPlan? userPlan = null;
            if (userPlanResponse.IsSuccessStatusCode)
            {
                var userPlanJson = await userPlanResponse.Content.ReadAsStringAsync();
                userPlan = JsonSerializer.Deserialize<UserPlan>(userPlanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            var viewModel = new SubscriptionViewModel
            {
                CurrentPlan = userPlan?.Plan,
                PremiumPlans = premiumPlans
            };

            return View(viewModel);
        }

        [Authorize(Roles = "client")]
        [HttpPost]
        public async Task<IActionResult> Subscribe(int planId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var payload = new { UserId = userId, PlanId = planId };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7120/api/UserPlans/subscribe", content);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Subscription successful!";
            else
                TempData["Error"] = "Subscription failed.";

            return RedirectToAction("Index");
        }

        // GET: PlansController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Plan>.GetById(id);
            return View(data);
        }

        // GET: PlansController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlansController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Plan data)
        {
            try
            {
                Crud<Plan>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }

        // GET: PlansController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Plan>.GetById(id);
            return View(data);
        }

        // POST: PlansController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Plan data)
        {
            try
            {
                Crud<Plan>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }

        // GET: PlansController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Plan>.GetById(id);
            return View(data);
        }

        // POST: PlansController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Plan data)
        {
            try
            {
                Crud<Plan>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }
    }
}
