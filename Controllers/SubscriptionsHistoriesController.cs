using HarmonySound.API.Consumer;
using HarmonySound.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HarmonySound.MVC.Controllers
{
    public class SubscriptionsHistoriesController : Controller
    {
        // GET: SubscriptionsHistoriesController
        public ActionResult Index()
        {
            var data = Crud<SubscriptionHistory>.GetAll();
            return View(data);
        }

        // GET: SubscriptionsHistoriesController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<SubscriptionHistory>.GetById(id);
            return View(data);
        }

        // GET: SubscriptionsHistoriesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SubscriptionsHistoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubscriptionHistory data)
        {
            try
            {
                Crud<SubscriptionHistory>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }

        // GET: SubscriptionsHistoriesController/Edit/5
        public ActionResult Edit(int id)
        { 
            var data = Crud<SubscriptionHistory>.GetById(id);
            return View(data);
        }

        // POST: SubscriptionsHistoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, SubscriptionHistory data)
        {
            try
            {
                Crud<SubscriptionHistory>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }

        // GET: SubscriptionsHistoriesController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<SubscriptionHistory>.GetById(id);
            return View(data);
        }

        // POST: SubscriptionsHistoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, SubscriptionHistory data)
        {
            try
            {
                Crud<SubscriptionHistory>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }
    }
}
