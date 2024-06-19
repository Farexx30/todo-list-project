using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models;
using ToDoList.Models.Dtos;
using ToDoList.Models.Entities;

namespace ToDoList.Services.Repositories
{
    public interface ICategoryRepositoryService
    {
        List<CategoryDto> GetCategories(Guid userId);
        void UpdateCategory(CategoryDto updatedCategoryDto);
        void DeleteCategory(int categoryId);
    }

    public class CategoryRepositoryService(ToDoListDbContext dbContext, IMapper mapper) : ICategoryRepositoryService
    {
        private readonly ToDoListDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public List<CategoryDto> GetCategories(Guid userId)
        {
            var categories = _dbContext.Categories
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .ToList();

            var categoriesDtos = _mapper.Map<List<CategoryDto>>(categories);
            return categoriesDtos;
        }
        public void UpdateCategory(CategoryDto updatedCategoryDto)
        {
            _dbContext.Categories
                .Where(c => c.Id == updatedCategoryDto.Id)
                .ExecuteUpdate(p =>
                 p.SetProperty(c => c.Name, updatedCategoryDto.Name));
        }

        public void DeleteCategory(int categoryId)
        {
            _dbContext.Categories
                .Where(c => c.Id == categoryId)
                .ExecuteDelete();
        }
    }
}
