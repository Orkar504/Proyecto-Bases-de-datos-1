using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cuentasPorCobrar.Controllers
{
    public class EstadoCivilController : Controller
    {
        // GET: EstadoCivilController
        public ActionResult Index()
        {
            return View();
        }

        // GET: EstadoCivilController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EstadoCivilController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstadoCivilController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EstadoCivilController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EstadoCivilController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EstadoCivilController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EstadoCivilController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
