using APP_Presupuesto.Interfaces.Repositorios;
using APP_Presupuesto.Interfaces.Servicios;
using APP_Presupuesto.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace APP_Presupuesto.Controllers
{
    public class CuentasController:Controller
    {
        private readonly IRepositorioTipoCuenta repositorioTipoCuenta;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IMapper mapper;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioReportes servicioReportes;

        public CuentasController( IRepositorioTipoCuenta repositorioTipoCuenta,  IServicioUsuarios servicioUsuarios, 
            IRepositorioCuentas repositorioCuentas, IMapper mapper, IRepositorioTransacciones repositorioTransacciones,
            IServicioReportes servicioReportes )
        {
            this.repositorioTipoCuenta = repositorioTipoCuenta;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper ;
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioReportes = servicioReportes;
        }

        
        [HttpGet]
        public async Task<IActionResult> Crear() {

            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            
            var modelo = new CuentasVM();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tipoCuentas = await repositorioTipoCuenta.Obtener(usuarioId);
            //texto                Valor html
            return tipoCuentas.Select(x => new SelectListItem(x.Nombre.ToString(), x.id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentasVM cuentas)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var tipoCuenta = await repositorioTipoCuenta.obtenerPorId(cuentas.TipoCuentaId,usuarioId);

            if(tipoCuenta is null) {

                return RedirectToAction("NoEncontrado", "Home");

            }

            if (!ModelState.IsValid)
            {
                cuentas.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuentas);
            }
            await repositorioCuentas.Crear(cuentas);
            return RedirectToAction("Index");
        }

        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Obtiene el ID del usuario utilizando el servicio "servicioUsuarios".
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();

            // Llama al método "Buscar" del repositorio "repositorioCuentas" de forma asíncrona,
            // pasando el ID del usuario obtenido. Esto devuelve una lista de cuentas asociadas
            // a ese usuario.
            var Join_CuentasConTipoCuentas = await repositorioCuentas.Buscar(usuarioid);

            // Agrupa las cuentas obtenidas por su tipo (TipoCuenta) y luego proyecta el resultado
            // en una nueva lista de objetos del tipo "IndiceCuentaVM". Cada objeto de esta lista
            // contiene un tipo de cuenta y las cuentas asociadas a ese tipo.
            var modelo = Join_CuentasConTipoCuentas
                .GroupBy(cuenta => cuenta.TipoCuenta) // Agrupa las cuentas por TipoCuenta.
                .Select(Agrupar => new IndiceCuentaVM
                {
                    TipoCuenta = Agrupar.Key,           // Asigna el tipo de cuenta al campo TipoCuenta.
                    Cuentas = Agrupar.AsEnumerable()   // Asigna las cuentas agrupadas al campo Cuentas.
                }).ToList();                           // Convierte el resultado en una lista.

            // Devuelve la vista "Index" pasando el modelo creado (la lista de "IndiceCuentaVM").
            return View(modelo);
        }


    
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();
            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(id, usuarioid);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<CuentasVM>(cuenta);

            //var modelo = new CuentasVM()
            //{
            //    Id = cuenta.Id,
            //    Nombre = cuenta.Nombre,
            //    TipoCuentaId = cuenta.Id,
            //    Descripcion = cuenta.Descripcion,
            //    Balance = cuenta.Balance,
            //};
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioid);
            return View(modelo);

        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentasVM cuentaVm)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();
            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(cuentaVm.Id, usuarioid);

            if(cuenta is null)
            {

                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipocuenta = await repositorioTipoCuenta.obtenerPorId(cuentaVm.TipoCuentaId, usuarioid);
            if(tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");

            }

            await repositorioCuentas.Actualizar(cuentaVm);
            return RedirectToAction("Index");

        }
        
        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();
            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(id, usuarioid);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
              
            }

            return View(cuenta);

        }
        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();
            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(id, usuarioid);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");

            }
         

            await repositorioCuentas.Borrar(id);
            return RedirectToAction("Index");

        }



        public async Task<IActionResult> DetalleCuenta(int id,int mes,int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = await servicioReportes
                .ObtenerReporteTransaccionesDetalladasPorCuenta(usuarioId, id, mes, año, ViewBag);

            return View(modelo);
        }
    }
}
