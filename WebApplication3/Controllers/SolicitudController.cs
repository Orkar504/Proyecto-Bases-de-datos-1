using cuentasPorCobrar.Models;
using CuentasPorCobrar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cuentasPorCobrar.Controllers
{
    public class SolicitudController : Controller
    {
        ProyectoContext proyectoContext = new ProyectoContext();
        // GET: HomeController1
        public ActionResult Index()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            TempData["userId"] = HttpContext.Request.Cookies["UserId"];
            return View(proyectoContext.GetSolicitud(2));
        }

        // GET: SolicitudController/Details/5
        public ActionResult Details(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDetalleSolicitud(id));
        }

        // GET: SolicitudController/Create
        public ActionResult Create()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View();
        }

        // POST: SolicitudController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Solicitud solicitud)
        {
            try
            {
                solicitud.estado_solicitud = 2;
                Operacion operacion = proyectoContext.CreateSolicitud(solicitud);
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

        // GET: SolicitudController/Edit/5
        public ActionResult Edit(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDetalleSolicitud(id));
        }

        // POST: SolicitudController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Solicitud solicitud)
        {
            try
            {
                Operacion operacion = proyectoContext.UpdateSolicitud(solicitud);
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
        // 1 denegadas, 2 por aprobar, 3 aprobadas

        // GET: SolicitudController/Delete/5
        public ActionResult Delete(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            TempData["id"] = id;
            return View(proyectoContext.GetDetalleSolicitud(id));  
        }

        // POST: SolicitudController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Denegar(int id, IFormCollection collection)
        {
            try
            {
                String idStr = TempData["id"].ToString();
                int idInt = Convert.ToInt32(idStr);
                Operacion operacion = proyectoContext.UpdateEstadoSolicitud(idInt, 1);
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
        // POST: SolicitudController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aprobar(int id, IFormCollection collection)
        {
            try
            {
                String idStr = TempData["id"].ToString();
                int idInt = Convert.ToInt32(idStr);
                Operacion operacion = proyectoContext.UpdateEstadoSolicitud(idInt, 3);
                
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
