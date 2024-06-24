using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public interface IAssignmentRepository
    {
        List<AssignmentDto> GetAssignments(CategoryMode mode, Guid userId, int? categoryId = null);
        AssignmentDto AddAssignment(AssignmentDto newAssignmentDto, Guid userId, int categoryId);
        void UpdateAssignment(AssignmentDto updatedAssignmentDto);
        void DeleteAssignment(int assignmentId);
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

            _assignmentsMethods.Add(CategoryMode.Database, GetDbAssignments);
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
            var allUserAssingments = _dbContext.Assignments
                .Where(a => a.UserId == userId)
                .ToList();

            var todaysAssignments = allUserAssingments
                .Where(a => a.Deadline.HasValue && a.Deadline.Value == DateTime.Today)
                .ToList();

            return todaysAssignments;
        }

        private List<Assignment> GetPlannedAssignments(Guid userId, int? categoryId)
        {
            var assignments = _dbContext.Assignments
                .Where(a => a.UserId == userId && a.Deadline != null)
                .ToList();

            return assignments;
        }

        private List<Assignment> GetImportantAssignments(Guid userId, int? categoryId)
        {
            var assignments = _dbContext.Assignments
                .Where(a => a.UserId == userId && a.IsImportant == true)
                .ToList();

            return assignments;
        }

        public AssignmentDto AddAssignment(AssignmentDto newAssignmentDto, Guid userId, int categoryId)
        {
            var newAssignment = _mapper.Map<Assignment>(newAssignmentDto);
            newAssignment.UserId = userId;
            if (categoryId == 1) newAssignment.IsShared = true;
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
            var assignmentToUpdate = _dbContext.Assignments
                .First(a => a.Id == updatedAssignmentDto.Id);

            assignmentToUpdate.Name = updatedAssignmentDto.Name;
            assignmentToUpdate.Deadline = updatedAssignmentDto.Deadline;
            assignmentToUpdate.IsChecked = updatedAssignmentDto.IsChecked;
            assignmentToUpdate.IsImportant = updatedAssignmentDto.IsImportant;

            bool doesSharingChanged = false;
            if (assignmentToUpdate.IsShared != updatedAssignmentDto.IsShared)
            {
                assignmentToUpdate.IsShared = updatedAssignmentDto.IsShared;
                doesSharingChanged = true;
            }

            _dbContext.SaveChanges();

            if (assignmentToUpdate.IsShared && doesSharingChanged)
            {
                ShareAssignment(assignmentToUpdate.Id);
            }
            else if(doesSharingChanged)
            {
                StopSharingAssignment(assignmentToUpdate.Id);
            }            
        }

        public void DeleteAssignment(int assignmentId)
        {
            var assignmentToDelete = _dbContext.Assignments
                .First(a => a.Id == assignmentId);

            _dbContext.Assignments.Remove(assignmentToDelete);

            _dbContext.SaveChanges();
        }

        private void ShareAssignment(int assignmentId)
        {
            var newCategoryAssignment = new CategoryAssignment
            {
                AssignmentId = assignmentId,
                CategoryId = 1
            };
            _dbContext.CategoryAssignments.Add(newCategoryAssignment);

            _dbContext.SaveChanges();
        }

        private void StopSharingAssignment(int assignmentId)
        {
           var categoryAssignmentToDelete = _dbContext.CategoryAssignments
                .First(ca => ca.AssignmentId == assignmentId && ca.CategoryId == 1);

           _dbContext.CategoryAssignments.Remove(categoryAssignmentToDelete);
           _dbContext.SaveChanges();
        }
    }
}
