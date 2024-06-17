using AutoMapper;
using ToDoList.Models.Dtos;
using ToDoList.Models.Entities;

namespace ToDoList.MappingProfiles
{
    public class ToDoListMappingProfile : Profile
    {
        public ToDoListMappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterUserDto, User>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            CreateMap<Assignment, AssignmentDto>();
            CreateMap<AssignmentDto, Assignment>();

            CreateMap<AssignmentStep, AssignmentStepDto>();
            CreateMap<AssignmentStepDto, AssignmentStep>();
        }
    }
}
