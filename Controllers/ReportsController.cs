using HarmonySound.API.Consumer;
using HarmonySound.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HarmonySound.MVC.Controllers
{
    public class ReportsController : Controller
    {
        // GET: ReportsController
        public ActionResult Index()
        {
            var data = Crud<Report>.GetAll();
            return View(data);
        }

        // GET: ReportsController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Report>.GetById(id);
            return View(data);
        }

        // GET: ReportsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReportsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Report data)
        {
            try
            {
                Crud<Report>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }

        // GET: ReportsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Report>.GetById(id);
            return View(data);
        }

        // POST: ReportsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Report data)
        {
            try
            {
                Crud<Report>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }

        // GET: ReportsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Report>.GetById(id);
            return View(data);
        }

        // POST: ReportsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Report data)
        {
            try
            {
                Crud<Report>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(data);
            }
        }
    }
}
