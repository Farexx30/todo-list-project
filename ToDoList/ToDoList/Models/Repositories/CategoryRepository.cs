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

namespace ToDoList.Models.Repositories
{
    public interface ICategoryRepository
    {
        CategoryDto GetBuiltInCategory();
        List<CategoryDto> GetCategories(Guid userId);
        CategoryDto? AddCategory(CategoryDto newCategoryDto, Guid userId);
        void UpdateCategory(CategoryDto updatedCategoryDto);
        void DeleteCategory(int categoryId);
    }

    public class CategoryRepository(ToDoListDbContext dbContext, IMapper mapper) : ICategoryRepository
    {
        private readonly ToDoListDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public CategoryDto GetBuiltInCategory()
        {
            var builtInCategory = _dbContext.Categories
                .AsNoTracking()
                .First(c => c.IsBuiltIn == true);

            var builtInCategoryDto = _mapper.Map<CategoryDto>(builtInCategory);
            return builtInCategoryDto;
        }

        public List<CategoryDto> GetCategories(Guid userId)
        {
            var categories = _dbContext.Categories
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .ToList();

            var categoriesDtos = _mapper.Map<List<CategoryDto>>(categories);
            return categoriesDtos;
        }

        public CategoryDto? AddCategory(CategoryDto newCategoryDto, Guid userId)
        {
            bool isCategoryExist = _dbContext.Categories
                .Any(c => c.Name == newCategoryDto.Name);

            if (isCategoryExist)
            {
                return null;
            }

            var newCategory = _mapper.Map<Category>(newCategoryDto);
            newCategory.UserId = userId;

            _dbContext.Categories.Add(newCategory);
            _dbContext.SaveChanges();

            var justAddedCategoryDto = _mapper.Map<CategoryDto>(newCategory);
            return justAddedCategoryDto;
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
