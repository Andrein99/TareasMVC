using AutoMapper;
using TareasMVC.Entidades;
using TareasMVC.Models;

namespace TareasMVC.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Aquí puedes definir tus mapeos entre modelos y DTOs
            // Por ejemplo:
            // CreateMap<SourceModel, DestinationModel>();
            CreateMap<Tarea, TareaDTO>();
        }
    }
}
