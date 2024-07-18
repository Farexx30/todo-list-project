using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        UpdatingCategoryResult UpdateCategory(CategoryDto updatedCategoryDto, Guid userId);
        void DeleteCategory(int categoryId, List<int> connectedAssignmentsToDeleteDto);
    }

    public class CategoryRepository(ToDoListDbContext dbContext, IMapper mapper) : ICategoryRepository
    {
        private readonly ToDoListDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;


        //Get:
        public CategoryDto GetBuiltInCategory()
        {
            try
            {
                var builtInCategory = _dbContext.Categories
                    .AsNoTracking()
                    .First(c => c.IsBuiltIn == true);

                var builtInCategoryDto = _mapper.Map<CategoryDto>(builtInCategory);
                return builtInCategoryDto;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occured: {ex.Message}.\n\nThe application will shutdown.");
                Application.Current.Shutdown();
                return null!;
            }
        }

        public List<CategoryDto> GetCategories(Guid userId)
        {
            try
            {
                var categories = _dbContext.Categories
                    .Where(u => u.UserId == userId)
                    .ToList();

                var categoriesDtos = _mapper.Map<List<CategoryDto>>(categories);
                return categoriesDtos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occured: {ex.Message}.\n\nThe application will shutdown.");
                Application.Current.Shutdown();
                return null!;
            }
        }


        //Add:
        public CategoryDto? AddCategory(CategoryDto newCategoryDto, Guid userId)
        {
            try
            {
                if (IsCategoryExist(newCategoryDto.Name, userId))
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
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occured: {ex.Message}.\n\nThe application will shutdown.");
                Application.Current.Shutdown();
                return null;
            }
        }


        //Update:
        public UpdatingCategoryResult UpdateCategory(CategoryDto updatedCategoryDto, Guid userId)
        {
            try
            {
                if (IsCategoryExist(updatedCategoryDto.Name, userId, updatedCategoryDto.Id))
                {
                    return UpdatingCategoryResult.Failed;
                }

                var categoryToUpdate = _dbContext.Categories
                    .First(c => c.Id == updatedCategoryDto.Id);

                categoryToUpdate.Name = updatedCategoryDto.Name;

                _dbContext.SaveChanges();

                return UpdatingCategoryResult.Success;
            }           
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occured: {ex.Message}.\n\nThe application will shutdown.");
                Application.Current.Shutdown();
                return UpdatingCategoryResult.Failed;
            }
        }


        //Delete:
        public void DeleteCategory(int categoryId, List<int> connectedAssignmentsToDeleteIds)
        {
            try
            {
                var categoryToDelete = _dbContext.Categories
                    .First(c => c.Id == categoryId);

                var connectedAssignmentsToDelete = _dbContext.Assignments
                    .Where(a => connectedAssignmentsToDeleteIds.Contains(a.Id))
                    .ToList();

                _dbContext.Categories.Remove(categoryToDelete);
                _dbContext.Assignments.RemoveRange(connectedAssignmentsToDelete);

                _dbContext.SaveChanges();
            }           
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occured: {ex.Message}.\n\nThe application will shutdown.");
                Application.Current.Shutdown();                
            }
        }

        private bool IsCategoryExist(string name, Guid userId, int? categoryId = null) 
            => _dbContext.Categories                
                 .Any(c => c.Name == name 
                 && (categoryId == null || c.Id != categoryId)
                 && c.UserId == userId);
    }
}
