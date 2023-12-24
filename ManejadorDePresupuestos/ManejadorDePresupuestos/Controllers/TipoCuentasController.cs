using ManejadorDePresupuestos.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManejadorDePresupuestos.Controllers
{
    public class TipoCuentasController : Controller
    {
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(TipoCuenta tipoCuenta)
        {
            if(!ModelState.IsValid) { return  View(tipoCuenta); }
            return View();
        }

    }
}
