using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;
using TareasMVC.Models;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/tareas")]
    public class TareasController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IMapper mapper;

        public TareasController(ApplicationDbContext context,
            IServicioUsuarios servicioUsuarios,
            IMapper mapper)
        {
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<TareaDTO>> Get()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tareas = await context.Tareas.Where(t => t.UsuarioCreacionId == usuarioId)
                .OrderBy(t => t.Orden)
/*                .Select(t => new TareaDTO
                { // Se puede asignar a las propiedades Id y Titulo de TareasDTO de la siguiente manera: Id = t.Id, Titulo = t.Titulo, sin embargo, no es tan general, porque si son 40 columnas sería un proceso tedioso, por eso se utiliza Automapper, para automatizar este proceso. 
                    Id = t.Id,
                    Titulo = t.Titulo
                }) // Si se usara como un listado de tipo anónimo (new {}), su retorno no sería de tipo Tarea. Por eso se crea TareaDTO (DTO: DataTransferObject)*/
                .ProjectTo<TareaDTO>(mapper.ConfigurationProvider) // Usando AutoMapper para proyectar las entidades Tarea a TareaDTO.
                .ToListAsync(); // Obtener todas las tareas de la base de datos.
            return tareas; // Retorna un 200 (STATUS CODE) en el body, por lo que se serializa como un JSON.
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tarea>> Get(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas
                .Include(t => t.Pasos.OrderBy(p => p.Orden))
                .FirstOrDefaultAsync(t => t.Id == id && 
                t.UsuarioCreacionId == usuarioId);

            if (tarea is null)
            {
                return NotFound();
            }

            return tarea;
        }


        [HttpPost]
        public async Task<ActionResult<Tarea>> Post([FromBody] string titulo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var existenTareas = await context.Tareas.AnyAsync(t => t.UsuarioCreacionId == usuarioId);

            var ordenMayor = 0;
            if (existenTareas)
            {
                ordenMayor = await context.Tareas
                    .Where(t => t.UsuarioCreacionId == usuarioId)
                    .Select(t => t.Orden).MaxAsync(); // Obtener el orden mayor de las tareas del usuario actual
            }

            var tarea = new Tarea
            {
                Titulo = titulo,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow,
                Orden = ordenMayor + 1,
            }; // Crear una nueva tarea con el título proporcionado, el ID del usuario actual, la fecha de creación y un orden basado en las tareas existentes del usuario.

            context.Add(tarea); // Agregar la tarea al contexto de la base de datos.
            await context.SaveChangesAsync(); // Guardar los cambios en la base de datos.

            return tarea; // Retornar la tarea creada.
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditarTarea(int id, 
            [FromBody] TareaEditarDTO tareaEditarDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas.
                FirstOrDefaultAsync(t => t.Id == id && 
                t.UsuarioCreacionId == usuarioId);

            if (tarea is null)
            {
                return NotFound();
            }

            tarea.Titulo = tareaEditarDTO.Titulo;
            tarea.Descripcion = tareaEditarDTO.Descripcion;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas
                .FirstOrDefaultAsync(t => t.Id == id &&
                t.UsuarioCreacionId == usuarioId);

            if (tarea is null)
            {
                return NotFound();
            }

            context.Remove(tarea);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("ordenar")]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tareas = await context.Tareas
                .Where(t => t.UsuarioCreacionId == usuarioId).ToListAsync();

            var tareasId = tareas.Select(t => t.Id);

            var idsTareasNoPertenecenAlUsuario = ids.Except(tareasId).ToList();

            if (idsTareasNoPertenecenAlUsuario.Any())
            {
                return Forbid();
            }

            var tareasDiccionario = tareas.ToDictionary(x => x.Id);

            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var tarea = tareasDiccionario[id];
                tarea.Orden = i + 1;
            }

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
