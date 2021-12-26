using AutoMapper;
using TodoApi.Models;

namespace TodoApi.AutoMapperConfiguration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TodoItem, TodoItemDTO>();
        }
    }
}
