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
            CreateMap<Tarea, TareaDTO>()
                // Esto se debe realizar así ya que PasosTotal, y PasosRealizados no pertenecen a TareaDTO, entonces AutoMapper no puede encontrarlos.
                // Se mapea del DTO.PasosTotal, para obtener la cantidad de elementos de los pasos para así tener el número de pasos que tiene cada Tarea, y cuáles están marcados como realizados.
                .ForMember(dto => dto.PasosTotal, ent => ent.MapFrom(x => x.Pasos.Count())) 
                .ForMember(dto => dto.PasosRealizados, ent => ent.MapFrom(x => x.Pasos.Where(p => p.Realizado).Count()));
        }
    }
}
