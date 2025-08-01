using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;

namespace TareasMVC
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<Tarea>().Property(t => t.Titulo)
            //     .HasMaxLength(250)
            //     .IsRequired(); // Configuración del campo Titulo de la entidad Tarea
        }

        public DbSet<Tarea> Tareas { get; set; } // Creación de la tabla Tareas
        public DbSet<Paso> Pasos { get; set; } // Creación de la tabla Pasos
        public DbSet<ArchivoAdjunto> ArchivosAdjuntos { get; set; } // Creación de la tabla ArchivosAdjuntos

        protected ApplicationDbContext()
        {
        }
    }
}
