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
    public interface IToDoDataService
    {
        List<CategoryDto> GetCategories(int userId);
        void DeleteCategory(int categoryId);

        List<AssignmentDto> GetAssignments(int categoryId);
        void AddAssignment(AssignmentDto newAssignmentDto);
        void UpdateAssignment(AssignmentDto updatedAssignmentDto);
        void DeleteAssignment(int assignmentId);

        List<AssignmentStepDto> GetAssignmentSteps(int assignmentId);
        void AddAssignmentStep(AssignmentStepDto newAssignmentStepDto, int currentAssignmentId);
        void UpdateAssignmentStep(AssignmentStepDto updatedAssignmentStepDto, int currentAssignmentId);
        void DeleteAssignmentStep(int currentAssignmentId, int assignmentStepId);

        void SaveChanges();
    }

    public class ToDoDataService : IToDoDataService
    {
        private readonly ToDoListDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly ICurrentDataService _currentDataService;

        private readonly List<Assignment> _currentCategoryDataCache = [];

        public ToDoDataService(ToDoListDbContext dbContext, IMapper mapper, IUserContextService userContextService, ICurrentDataService currentDataService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
            _currentDataService = currentDataService;

            //_currentCategoryDataCache = _mapper.Map<List<Assignment>>(_currentDataService.CurrentCategory?.Assignments);
        }

        public List<CategoryDto> GetCategories(int userId)
        {
            var categories = _dbContext.Categories
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .ToList();

            var categoriesDtos = _mapper.Map<List<CategoryDto>>(categories);
            return categoriesDtos;
        }

        public void DeleteCategory(int categoryId)
        {
            _ = _dbContext.Categories
                .Where(c => c.Id == categoryId)
                .ExecuteDelete();
        }

        //Assignments:
        public List<AssignmentDto> GetAssignments(int categoryId)
        {
            var assignments = _dbContext.Assignments
                .AsNoTracking()
                .Where(a => a.UserId == _userContextService.CurrentUserDto!.Id)
                .Join(_dbContext.CategoryAssignments,
                      assignment => assignment.Id,
                      categoryAssignment => categoryAssignment.AssignmentId,
                      (assignment, categoryAssignment) => new { Assignment = assignment, CategoryAssignment = categoryAssignment })
                        .Where(result => result.CategoryAssignment.CategoryId == categoryId)
                        .Select(result => result.Assignment)
                .ToList();

            var assignmentsDtos = _mapper.Map<List<AssignmentDto>>(assignments);
            return assignmentsDtos;
        }

        public void AddAssignment(AssignmentDto newAssignmentDto)
        {
            var newAssignment = _mapper.Map<Assignment>(newAssignmentDto);
            _currentCategoryDataCache.Add(newAssignment);

            _dbContext.Entry(newAssignment).State = EntityState.Added;
        }

        public void UpdateAssignment(AssignmentDto updatedAssignmentDto)
        {
            var assignmentToUpdate = _currentCategoryDataCache
                .First(a => a.Id == updatedAssignmentDto.Id);

            assignmentToUpdate.Name = updatedAssignmentDto.Name;
            assignmentToUpdate.Deadline = updatedAssignmentDto.Deadline;
            assignmentToUpdate.IsChecked = updatedAssignmentDto.IsChecked;
            assignmentToUpdate.IsImportant = updatedAssignmentDto.IsImportant;
        }

        public void DeleteAssignment(int assignmentId)
        {
            var assignmentToDelete = _currentCategoryDataCache
                .First(a => a.Id == assignmentId);

            _currentCategoryDataCache.Remove(assignmentToDelete);
        }

        //AssignmentStep:
        public List<AssignmentStepDto> GetAssignmentSteps(int assignmentId)
        {
            var assignmentSteps = _dbContext.AssignmentSteps
                .AsNoTracking()
                .Where(aStep => aStep.AssignmentId == assignmentId)
                .ToList();

            var assignmentStepsDtos = _mapper.Map<List<AssignmentStepDto>>(assignmentSteps);
            return assignmentStepsDtos;
        }

        public void AddAssignmentStep(AssignmentStepDto newAssignmentStepDto, int currentAssignmentId)
        {
            var currentAssignment = _currentCategoryDataCache
                .First(a => a.Id == currentAssignmentId);

            var newAssignmentStep = _mapper.Map<AssignmentStep>(newAssignmentStepDto);
            currentAssignment.AssignmentSteps.Add(newAssignmentStep);

            _dbContext.Entry(newAssignmentStep).State = EntityState.Added;
        }

        public void UpdateAssignmentStep(AssignmentStepDto updatedAssignmentStepDto, int currentAssignmentId)
        {
            var currentAssignment = _currentCategoryDataCache
                .First(a => a.Id == currentAssignmentId);

            var assignmentStepToUpdate = currentAssignment.AssignmentSteps
                .First(aStep => aStep.Id == updatedAssignmentStepDto.Id);

            assignmentStepToUpdate.Name = updatedAssignmentStepDto.Name;
        }

        public void DeleteAssignmentStep(int currentAssignmentId, int assignmentStepId)
        {
            var currentAssignment = _currentCategoryDataCache
                .First(a => a.Id == currentAssignmentId);

            var assignmentStepToDelete = currentAssignment.AssignmentSteps
                .First(aStep => aStep.Id == assignmentStepId);

            currentAssignment.AssignmentSteps.Remove(assignmentStepToDelete);
        }

        //Zapis zmian do bazy danych.
        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
