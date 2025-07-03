using Microsoft.EntityFrameworkCore;

namespace TareasMVC.Entidades
{
    public class ArchivoAdjunto
    {
        public Guid Id { get; set; }
        public int TareaId { get; set; }
        public Tarea Tarea { get; set; }
        [Unicode(false)] // Se hace, porque las URLs pueden contener caracteres que no son Unicode.
        public string Url { get; set; }
        public string Titulo { get; set; }
        public int Orden { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
