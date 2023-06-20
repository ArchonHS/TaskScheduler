using AutoMapper;
using TaskScheduler.Data;
using TaskScheduler.Models;

namespace TaskScheduler
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<TaskFromApiCreateDTO, TaskFromApiDTO>();
            CreateMap<TaskFromApiUpdateDTO, TaskFromApiDTO>();
        }
    }
}