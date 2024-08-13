using APP_Presupuesto.Interfaces.Repositorios;
using APP_Presupuesto.Interfaces.Servicios;
using APP_Presupuesto.Models;
using APP_Presupuesto.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace APP_Presupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServicioUsuarios servicioUsuarios;

        public CategoriasController(IRepositorioCategorias repositorioCategorias,IServicioUsuarios servicioUsuarios) 
        {
            this.repositorioCategorias = repositorioCategorias;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crear(Categorias categorias)
        {
            if (!ModelState.IsValid)
            {
                return View(categorias);
            }
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            categorias.UsuarioId= usuarioId;
            await repositorioCategorias.Crear(categorias);
                    
            return RedirectToAction("Index");
        }


        [HttpGet]   
        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var categorias = await repositorioCategorias.Obtener(usuarioId); 
            return View(categorias);    

        }

        [HttpPost]
        // metodo para ordenar 
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioID();

            var categorias = await repositorioCategorias.Obtener(usuarioid);

            var idCategorias = categorias.Select(x => x.Id);

            // validacion de los ids de los tipos cuentas del front con el backend
            var idCategoriasNoPertenecenAlusuario = ids.Except(idCategorias).ToList();
            if (idCategoriasNoPertenecenAlusuario.Count > 0)
            {
                return Forbid();
            }

            var CategoriasOrdenados = ids.Select((valor, indice)
                => new Categorias() { Id = valor, Orden = indice + 1 }).AsEnumerable();
            await repositorioCategorias.OrdenarCategorias(CategoriasOrdenados);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var categoria= await repositorioCategorias.ObtenerCategoriaPorID(id, usuarioId);  

            if (categoria == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar (Categorias categoria)
        {

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var EditarCategoria = await repositorioCategorias.ObtenerCategoriaPorID(categoria.Id, usuarioId);

            if (EditarCategoria == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            categoria.UsuarioId = usuarioId;

            await repositorioCategorias.ActualizarCategoria(categoria);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var categoria = await repositorioCategorias.ObtenerCategoriaPorID(id, usuarioId);

            if (categoria == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id)
        {

            
            var usuarioId = servicioUsuarios.ObtenerUsuarioID();
            var BorrarCategoria = await repositorioCategorias.ObtenerCategoriaPorID(id, usuarioId);

            if (BorrarCategoria == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCategorias.BorrarCategoria(id);
            return RedirectToAction("Index");
        }

    }
}
