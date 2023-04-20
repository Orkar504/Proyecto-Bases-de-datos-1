using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CuentasPorCobrar.Models;
using cuentasPorCobrar.Models;

namespace CuentasPorCobrar.Controllers
{
    public class ClientesController : Controller
    {
        ProyectoContext proyectoContext = new ProyectoContext();

        // GET: ClientesController
        public ActionResult Index()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return  Redirect("/Usuarios/Login");
            Operacion operacion = proyectoContext.GetClientesList();
            if (!operacion.esValida)
            {
                TempData["OperacionError"] = operacion.Mensaje;
            }
            var clientes = operacion.resultado; 

            return View(clientes);
        }

        // GET: ClientesController/Details/5
        public ActionResult Details(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDetalleClientesList(id).resultado);
        }

        // GET: ClientesController/Create
        public ActionResult Create()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View();
        }

        // POST: EstadoCivilController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente)
        {
            try
            {
                cliente.estado_RegistroClientesId = 1;
                Operacion operacion = proyectoContext.CreateClientes(cliente);
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


        // GET: ClientesController/Edit/5
        public ActionResult Edit(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");

            return View(proyectoContext.GetDetalleClientes(id).resultado);
        }

        // POST: ClientesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Cliente cliente)
        {
            try
            {
                Operacion operacion = proyectoContext.UpdateClientes(cliente);
                if(!operacion.esValida)
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

        // GET: ClientesController/Delete/5
        public ActionResult Delete(int id)
        {
            if (!HttpContext.Request.Cookies.ContainsKey("UserId"))
                return Redirect("/Usuarios/Login");
            return View(proyectoContext.GetDetalleClientesList(id).resultado);
        }

        // POST: ClientesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                Operacion operacion = proyectoContext.DeleteClientes(id);
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
