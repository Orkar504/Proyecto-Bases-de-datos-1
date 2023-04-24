using cuentasPorCobrar.Models;
using CuentasPorCobrar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;

namespace cuentasPorCobrar.Controllers
{
    public class ComiteController : Controller
    {
        ProyectoContext proyectoContext = new ProyectoContext();
        // GET: ComiteController
        public ActionResult Index()
        {
            return View(proyectoContext.GetComite());
        }

        // GET: ComiteController/Details/5
        public ActionResult Details(int id)
        {
            return View(proyectoContext.GetDetalleComite(id));
        }

        // GET: ComiteController/Create
        public ActionResult Create()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View();
        }

        // POST: ComiteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                //comite.estado_solicitud = 'En revisión';
                //Operacion operacion = proyectoContext.CreateComite(comite);
                //if (!operacion.esValida)
                //{
                //    TempData["OperacionError"] = operacion.Mensaje;
                //}
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ComiteController/Edit/5
        public ActionResult Edit(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDetalleComite(id));
        }

        // POST: ComiteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                //Operacion operacion = proyectoContext.UpdateComite(comite);
                //if (!operacion.esValida)
                //{
                //    TempData["OperacionError"] = operacion.Mensaje;
                //}
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ComiteController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ComiteController/Delete/5
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
