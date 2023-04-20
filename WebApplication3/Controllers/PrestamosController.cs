using cuentasPorCobrar.Models;
using CuentasPorCobrar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cuentasPorCobrar.Controllers
{
    public class PrestamosController : Controller
    {
        ProyectoContext proyectoContext = new ProyectoContext();
        // GET: HomeController1
        public ActionResult Index()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetPrestamos());
        }

        // GET: PrestamosController/Details/5
        public ActionResult Details(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDetallePrestamo(id));
        }

        // GET: PrestamosController/Create
        public ActionResult Create()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View();
        }

        // POST: PrestamosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Prestamo prestamo)
        {
            try
            {
                prestamo.estado = 2;
                Operacion operacion = proyectoContext.CreatePrestamos(prestamo);
                if (!operacion.esValida)
                {
                    TempData["OperacionError"] = operacion.Mensaje;
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PrestamosController/Edit/5
        public ActionResult Edit(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDetallePrestamo(id));
        }

        // POST: PrestamosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Prestamo prestamo)
        {
            try
            {
                Operacion operacion = proyectoContext.UpdatePrestamos(prestamo);
                if (!operacion.esValida)
                {
                    TempData["OperacionError"] = operacion.Mensaje;
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PrestamosController/Delete/5
        public ActionResult Delete(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDetallePrestamo(id));
        }

        // POST: PrestamosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                Operacion operacion = proyectoContext.DeletePrestamos(id);
                if (!operacion.esValida)
                {
                    TempData["OperacionError"] = operacion.Mensaje;
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
