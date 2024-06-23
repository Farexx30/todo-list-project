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
    public interface IAssignmentRepository
    {
        List<AssignmentDto> GetAssignments(CategoryMode mode, Guid userId, int? categoryId = null);
        AssignmentDto AddAssignment(AssignmentDto newAssignmentDto, Guid userId, int categoryId);
        void UpdateAssignment(AssignmentDto updatedAssignmentDto);
        void DeleteAssignment(int assignmentId);
        void ShareAssignment(int assignmentId);
    }

    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ToDoListDbContext _dbContext;
        private readonly IMapper _mapper;

        private readonly Dictionary<CategoryMode, Func<Guid, int?, List<Assignment>>> _assignmentsMethods = [];

        public AssignmentRepository(ToDoListDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;

            _assignmentsMethods.Add(CategoryMode.Custom, GetDbAssignments);
            _assignmentsMethods.Add(CategoryMode.MyDay, GetMyDayAssignments);
            _assignmentsMethods.Add(CategoryMode.Planned, GetPlannedAssignments);
            _assignmentsMethods.Add(CategoryMode.Important, GetImportantAssignments);
        }

        public List<AssignmentDto> GetAssignments(CategoryMode mode, Guid userId, int? categoryId = null)
        {
            if(_assignmentsMethods.TryGetValue(mode, out var result))
            {
                var assignments = result(userId, categoryId);
                var assignmentsDtos = _mapper.Map<List<AssignmentDto>>(assignments);
                return assignmentsDtos;
            }
            return [];
        }

        private List<Assignment> GetDbAssignments(Guid userId, int? categoryId)
        {
            var assignments = _dbContext.Assignments
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .Join(_dbContext.CategoryAssignments,
                      assignment => assignment.Id,
                      categoryAssignment => categoryAssignment.AssignmentId,
                      (assignment, categoryAssignment) => new { Assignment = assignment, CategoryAssignment = categoryAssignment })
                        .Where(result => result.CategoryAssignment.CategoryId == categoryId)
                        .Select(result => result.Assignment)
                .ToList();

            return assignments;
        }

        private List<Assignment> GetMyDayAssignments(Guid userId, int? categoryId)
        {
            var assignments = _dbContext.Assignments
                .AsNoTracking()
                .Where(a => a.UserId == userId && a.Deadline == DateOnly.FromDateTime(DateTime.Now))
                .ToList();

            return assignments;
        }

        private List<Assignment> GetPlannedAssignments(Guid userId, int? categoryId)
        {
            var assignments = _dbContext.Assignments
                .AsNoTracking()
                .Where(a => a.UserId == userId && a.Deadline != null)
                .ToList();

            return assignments;
        }

        private List<Assignment> GetImportantAssignments(Guid userId, int? categoryId)
        {
            var assignments = _dbContext.Assignments
                .AsNoTracking()
                .Where(a => a.UserId == userId && a.IsImportant == true)
                .ToList();

            return assignments;
        }

        public AssignmentDto AddAssignment(AssignmentDto newAssignmentDto, Guid userId, int categoryId)
        {
            var newAssignment = _mapper.Map<Assignment>(newAssignmentDto);
            newAssignment.UserId = userId;
            _dbContext.Assignments.Add(newAssignment);
            _dbContext.SaveChanges();

            var newCategoryAssignment = new CategoryAssignment
            {
                AssignmentId = newAssignment.Id,
                CategoryId = categoryId
            };
            _dbContext.CategoryAssignments.Add(newCategoryAssignment);
            _dbContext.SaveChanges();

            var justAddedAssignmentDto = _mapper.Map<AssignmentDto>(newAssignment);
            return justAddedAssignmentDto;
        }

        public void UpdateAssignment(AssignmentDto updatedAssignmentDto)
        {
            _dbContext.Assignments
                .Where(a => a.Id == updatedAssignmentDto.Id)
                .ExecuteUpdate(p =>
                 p.SetProperty(a => a.Name, updatedAssignmentDto.Name)
                 .SetProperty(a => a.Deadline, updatedAssignmentDto.Deadline)
                 .SetProperty(a => a.IsChecked, updatedAssignmentDto.IsChecked)
                 .SetProperty(a => a.IsImportant, updatedAssignmentDto.IsImportant));
        }

        public void DeleteAssignment(int assignmentId)
        {
            _dbContext.Assignments
                .Where(a => a.Id == assignmentId)
                .ExecuteDelete();
        }

        public void ShareAssignment(int assignmentId)
        {
            bool isAlreadyShared = _dbContext.CategoryAssignments
                .Any(ca => ca.AssignmentId == assignmentId && ca.CategoryId == 1);

            if (!isAlreadyShared)
            {
                var newCategoryAssignment = new CategoryAssignment
                {
                    AssignmentId = assignmentId,
                    CategoryId = 1 //Wiemy, że "Zadania" mają Id = 1 - a raczej będą miały (możnaby też ręcznie znaleźć to Id).
                };
                _dbContext.CategoryAssignments.Add(newCategoryAssignment);

                _dbContext.SaveChanges();
            }
        }
    }
}
