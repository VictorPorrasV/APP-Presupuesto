using APP_Presupuesto.Interfaces.Repositorios;
using APP_Presupuesto.Interfaces.Servicios;
using APP_Presupuesto.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.ClientModel.Primitives;
using System.Reflection;

namespace APP_Presupuesto.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositoriocuentas;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IMapper mapper;

        public TransaccionesController(IServicioUsuarios servicioUsuarios,IRepositorioCuentas repositorioCuentas, IRepositorioCategorias repositorioCategorias,IRepositorioTransacciones repositorioTransacciones,IMapper mapper)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositoriocuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioTransacciones = repositorioTransacciones;
            this.mapper = mapper;
        }



        public IActionResult Index()
        {
            return View();  
        }


        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var modelo = new VMTransaccionCreacion();
            // proyeccion del select de get ceuntas a  selectlistitem cuentas-VMTransaccionCreacion
            modelo.Cuentas = await GetCuentas(usuarioId);
            // proyeccion del select de get categorias a  selectlistitem categorias-VMTransaccionCreacion
            modelo.Categorias = await GetCategorias(usuarioId, modelo.TipoOperacionId);

            return View(modelo);

        }

  
        private async Task<IEnumerable<SelectListItem>>GetCuentas(int usuarioId)
        {
            var getCuentas = await repositoriocuentas.Buscar(usuarioId);
                                                                        //Texto        //valor html
            return getCuentas.Select(cuenta => new SelectListItem ( cuenta.Nombre, cuenta.Id.ToString() ));

        }
        


        //js para traer las categorias relacionadas con el tipo de operacion
        [HttpPost]

        public async Task<IActionResult> GetCategorias([FromBody] TipoOperaciones tipoOperaciones)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var categorias = await GetCategorias(usuarioId,tipoOperaciones);
            return Ok(categorias);
        }
        //js para traer las categorias relacionadas con el tipo de operacion
        private async Task<IEnumerable<SelectListItem>> GetCategorias(int usuarioId, TipoOperaciones tipoOperaciones)
        {
            var getCategorias = await repositorioCategorias.Obtener(usuarioId, tipoOperaciones);
                                                                             //Texto        //valor html
            return getCategorias.Select(categoria => new SelectListItem(categoria.Nombre, categoria.Id.ToString()));

        }

        [HttpPost]
        public async Task<IActionResult> Crear(VMTransaccionCreacion vmTransaccion)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();

            if(!ModelState.IsValid)
            {
                vmTransaccion.Cuentas = await GetCuentas(usuarioId);
                // proyeccion del select de get categorias a  selectlistitem categorias-VMTransaccionCreacion
                vmTransaccion.Categorias = await GetCategorias(usuarioId, vmTransaccion.TipoOperacionId);
                return View(vmTransaccion);
            }
            
            
            var cuenta  = await repositoriocuentas.ObtenerCuentaPorID( vmTransaccion.CuentaId, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            
            var categoria = await repositorioCategorias.ObtenerCategoriaPorID(vmTransaccion.CategoriaId,usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            vmTransaccion.UsuarioId = usuarioId;
            if (vmTransaccion.TipoOperacionId == TipoOperaciones.Gasto)
            {
                vmTransaccion.Monto *= -1;
            }
            
            
            await repositorioTransacciones.CrearTransaccion(vmTransaccion); 
            
            
            return RedirectToAction("Index"); 
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id,string urlRetorno=null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var transaccion= await repositorioTransacciones.ObtenerTransaccionPorID(id, usuarioId); 
            if(transaccion is null) 
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<VMTransaccionActualizacion>(transaccion);

            modelo.MontoAnterior = modelo.Monto;

            if (modelo.TipoOperacionId == TipoOperaciones.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1;
            }
            modelo.cuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await GetCategorias(usuarioId, transaccion.TipoOperacionId);
            modelo.Cuentas = await GetCuentas(usuarioId);
            modelo.UrlRetorno= urlRetorno;
            return View(modelo);

        }

        [HttpPost]
       public async Task<IActionResult> Editar(VMTransaccionActualizacion modelo)
        {
            var usuarioId= servicioUsuarios.ObtenerUsuarioID();

            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await GetCuentas(usuarioId);
                modelo.Categorias = await GetCategorias(usuarioId,modelo.TipoOperacionId);
                return View(modelo) ;   

            }
            var cuenta = await repositoriocuentas.ObtenerCuentaPorID(modelo.cuentaAnteriorId, usuarioId);
            if (cuenta == null)
            {

                return RedirectToAction("NoEncontrado", "Home");

            }

            var categoria = await repositorioCategorias.ObtenerCategoriaPorID(modelo.CategoriaId, usuarioId);
            if (categoria == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = mapper.Map<Transacciones>(modelo);

            if(modelo.TipoOperacionId== TipoOperaciones.Gasto)
            {

                transaccion.Monto *= -1;
            }
            await repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, modelo.cuentaAnteriorId);
            if(string.IsNullOrEmpty(modelo.UrlRetorno))
            {
                return RedirectToAction("Index");
            }
            else{
                return LocalRedirect(modelo.UrlRetorno);    
            }
            

        }


        [HttpPost]
        public async Task<IActionResult>Borrar (int id, string urlRetorno = null)
        {

            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var transaccion = await repositorioTransacciones.ObtenerTransaccionPorID(id, usuarioId); 
            if (transaccion == null) {

                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTransacciones.Borrar(id);

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }
            

        }

    }
}
