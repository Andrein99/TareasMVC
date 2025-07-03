using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Entidades
{
    public class Tarea
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<Paso> Pasos { get; set; } // Propiedad de navegación - Uno a muchos con la Entidad Paso.
        public List<ArchivoAdjunto> ArchivosAdjuntos { get; set; } // Propiedad de navegación - Uno a muchos con la Entidad Tarea (Una tarea va a tener varios archivos adjuntos).
    }
}
