using APP_Presupuesto.Interfaces.Repositorios;
using APP_Presupuesto.Interfaces.Servicios;
using APP_Presupuesto.Models;
using APP_Presupuesto.Servicios;
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
        private readonly IServicioReportes servicioReportes;

        public TransaccionesController(IServicioUsuarios servicioUsuarios,IRepositorioCuentas repositorioCuentas, IRepositorioCategorias repositorioCategorias,IRepositorioTransacciones repositorioTransacciones,IMapper mapper, IServicioReportes servicioReportes)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositoriocuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioTransacciones = repositorioTransacciones;
            this.mapper = mapper;
            this.servicioReportes = servicioReportes;

        }



        public async Task< IActionResult> Index(int mes,int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var modelo = await servicioReportes
                .ObtenerReporteTransaccionesDetalladas(usuarioId, mes, año, ViewBag);
            return View(modelo);
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



        [HttpGet]
        public async Task<IActionResult> Semanal(int mes, int año)
        {
            // Obtener el ID del usuario desde un servicio
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();

            // Obtener las transacciones semanales para el usuario específico, mes y año
            IEnumerable<ResultadoObtenerPorSemana> transaccionesPorSemana = await servicioReportes.ReporteSemanal(usuarioId, mes, año, ViewBag);

            // Agrupar las transacciones por semana y calcular los ingresos y gastos
            var agrupar = transaccionesPorSemana.GroupBy(x => x.Semana).Select(x => new ResultadoObtenerPorSemana()
            {
                Semana = x.Key,
                Ingresos = x.Where(x => x.TipoOperacionId == TipoOperaciones.Ingreso)
                            .Select(x => x.Monto).FirstOrDefault(),

                Gastos = x.Where(x => x.TipoOperacionId == TipoOperaciones.Gasto)
                          .Select(x => x.Monto).FirstOrDefault(),
            }).ToList();

            // Si el año o mes son 0, usar el año y mes actuales
            if (año == 0 || mes == 0)
            {
                var hoy = DateTime.Today;
                año = hoy.Year;
                mes = hoy.Month;
            }

            // Crear un objeto DateTime para el primer día del mes
            var fechaReferencia = new DateTime(año, mes, 1);

            // Obtener todos los días del mes y dividirlos en semanas
            var diaDelmes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);
            var diasSegmentados = diaDelmes.Chunk(7).ToList(); // Dividir en semanas

            for (int i = 0; i < diasSegmentados.Count; i++)
            {
                var semana = i + 1;
                var fechaInicio = new DateTime(año, mes, diasSegmentados[i].FirstOrDefault());
                var fechaFin = new DateTime(año, mes, diasSegmentados[i].Last());

                // Verificar si la semana ya está en el agrupamiento
                var grupoSemana = agrupar.FirstOrDefault(x => x.Semana == semana);
                if (grupoSemana != null)
                {
                    // Si existe, actualizar las fechas
                    grupoSemana.FechaInicio = fechaInicio;
                    grupoSemana.FechaFin = fechaFin;
                }
                else
                {
                    // Si no existe, agregar una nueva entrada con las fechas
                    agrupar.Add(new ResultadoObtenerPorSemana()
                    {
                        Semana = semana,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin
                    });
                }
            }

            // Ordenar las semanas de forma descendente
            agrupar = agrupar.OrderByDescending(x => x.Semana).ToList();

            // Crear el modelo para la vista
            var modelo = new ReporteSemanalViewModel();
            modelo.TransaccionesPorSemana = agrupar;
            modelo.FechaReferencia = fechaReferencia;

            // Pasar el modelo a la vista
            return View(modelo);
        }




        [HttpGet]
        public IActionResult Mensual()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ExcelReporte()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Calendario()
        {
            return View();
        }
    }
}
