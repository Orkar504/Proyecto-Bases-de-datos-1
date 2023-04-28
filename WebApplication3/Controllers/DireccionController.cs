using cuentasPorCobrar.Models;
using CuentasPorCobrar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Mysqlx.Crud.Order.Types;

namespace cuentasPorCobrar.Controllers
{
    public class DireccionController : Controller
    {
        ProyectoContext proyectoContext = new ProyectoContext();
        // GET: DireccioController
        public ActionResult Index(int id)
        {
            TempData["clienteId"] = id;
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDireccionesPorCliente(id));
        }

        // GET: DireccioController/Details/5
        public ActionResult Details(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            Direccion direccion = proyectoContext.GetDireccion(id);
            direccion.clienteId = id;
            return View(proyectoContext.GetDireccion(id));
        }

        // GET: DireccioController/Create
        public ActionResult Create(int id)
        {
            TempData["clienteId"] = id;
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View();
        }

        // POST: DireccioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, Direccion direccion)
        {
            try
            {
                TempData["clienteId"] = id;
                direccion.clienteId = id;
                Operacion operacion = proyectoContext.CreateDirecciones(direccion);
                if (!operacion.esValida)
                {
                    TempData["OperacionError"] = operacion.Mensaje;
                }
                return Redirect("/Direccion/Index/"+ direccion.clienteId);
            }
            catch
            {
                return View();
            }
        }

        // GET: DireccioController/Edit/5
        public ActionResult Edit(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDireccionEdit(id));
        }

        // POST: DireccioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Direccion direccion)
        {
            try
            {
                TempData["clienteId"] = id;
                Operacion operacion = proyectoContext.UpdateDirecciones(direccion);
                if (!operacion.esValida)
                {
                    TempData["OperacionError"] = operacion.Mensaje;
                }
                Direccion dir = proyectoContext.GetDireccionEdit(id);
                return Redirect("/"+proyectoContext.getSitio()+"/Direccion/Index/" + dir.clienteId);

                
            }
            catch
            {
                return View();
            }
        }

        // GET: DireccioController/Delete/5
        public ActionResult Delete(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDireccion(id));
        }

        // POST: DireccioController/Delete/5
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
