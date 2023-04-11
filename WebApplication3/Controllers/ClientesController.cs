using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CuentasPorCobrar.Models;

namespace CuentasPorCobrar.Controllers
{
    public class ClientesController : Controller
    {
        ProyectoContext proyectoContext = new ProyectoContext();
        // GET: ClientesController
        public ActionResult Index()
        {
            var clientes = proyectoContext.GetClientes(); 
            return View(clientes);
        }

        // GET: ClientesController/Details/5
        public ActionResult Details(int id)
        {
            Cliente cliente = proyectoContext.GetCliente(id);
            return View(cliente);
        }

        // GET: ClientesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente)
        {
            try
            {
                Cliente clienteAgregado = proyectoContext.agregarCliente(cliente);
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
            return View(new Cliente()
            {
                Id = 1,
                Nombres = "",
                Email = "Correo Electronico"
            });
        }

        // POST: ClientesController/Edit/5
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

        // GET: ClientesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ClientesController/Delete/5
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
