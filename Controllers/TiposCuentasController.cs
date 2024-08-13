using APP_Presupuesto.Interfaces.Repositorios;
using APP_Presupuesto.Interfaces.Servicios;
using APP_Presupuesto.Models;
using APP_Presupuesto.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace APP_Presupuesto.Controllers
{
    public class TiposCuentasController : Controller

    {   private readonly IRepositorioTipoCuenta repositorioTipoCuenta;
        private readonly IServicioUsuarios servicioUsuarios;

        public TiposCuentasController(IRepositorioTipoCuenta repositorioTipoCuenta, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTipoCuenta = repositorioTipoCuenta;
            this.servicioUsuarios = servicioUsuarios;
        }

        public IActionResult Crear()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Tipocuenta tipocuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipocuenta);
            }

            tipocuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioID();

            var existe = await repositorioTipoCuenta.ValidarExistencia(tipocuenta.Nombre, tipocuenta.UsuarioId);
            if (existe)
            {
                ModelState.AddModelError(nameof(tipocuenta.Nombre),
                    $"El nombre {tipocuenta.Nombre} ya existe.");
                return View(tipocuenta);
            }
            await repositorioTipoCuenta.Crear(tipocuenta);

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Index()
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();
            var tiposCuentasLista = await repositorioTipoCuenta.Obtener(usuarioid);
            return View(tiposCuentasLista);

        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();
            var tiposCuenta = await repositorioTipoCuenta.obtenerPorId(id,usuarioid);

            if(tiposCuenta is null) { 
                
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tiposCuenta);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(Tipocuenta tipocuenta)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();
            var existe = await repositorioTipoCuenta.ValidarExistencia(tipocuenta.Nombre, usuarioid);
            var tiposCuentaExiste = await repositorioTipoCuenta.obtenerPorId(tipocuenta.id, usuarioid);
            
            if (existe)
            {
                ModelState.AddModelError(nameof(tipocuenta.Nombre),
                    $"El nombre {tipocuenta.Nombre} ya existe.");
                return View(tipocuenta);
            }
            if (tiposCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            
            await repositorioTipoCuenta.Actualizar(tipocuenta);

            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Borrar (int id)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();
            var tipocuenta = await repositorioTipoCuenta.obtenerPorId(id, usuarioid);

            if (tipocuenta is null)
            {
             return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipocuenta);
        }


        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();
            var tipocuenta = await repositorioTipoCuenta.obtenerPorId(id, usuarioid);

            if (tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            
            if (await repositorioTipoCuenta.TieneDependencias(id))
            {

            

                // Redirigir a la acción ContieneDependencias
                return RedirectToAction("ContieneDependencias", new { id });
            }

            await repositorioTipoCuenta.Borrar(id);
            return RedirectToAction("Index","TiposCuentas");    
        }

        public async Task<IActionResult> ContieneDependencias(int id)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();
            var tipoCuenta = await repositorioTipoCuenta.obtenerPorId(id, usuarioid);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            // Muestra la vista ContieneDependencias
            return View(tipoCuenta);
        }

        [HttpPost]
        // metodo para ordenar la tabla de tipos cuentas
        public async Task <IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();

            var tiposCuentas = await repositorioTipoCuenta.Obtener(usuarioid);

            var idsTiposCuentas = tiposCuentas.Select(Cuenta => Cuenta.id);
           
            // validacion de los ids de los tipos cuentas del front con el backend
            var idsTiposCuentasNoPertenecenAlusuario=ids.Except(idsTiposCuentas).ToList();
            if (idsTiposCuentasNoPertenecenAlusuario.Count > 0)
            {
                return Forbid();
            }
            var TipoCuentasOrdenados= ids.Select((valor,indice)
                => new Tipocuenta() { id=valor, Orden= indice +1}).AsEnumerable();
            await repositorioTipoCuenta.OrdenarCuentas(TipoCuentasOrdenados);

            return Ok();
        }



    }
}
