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
        void UpdateAssignmentName(string newAssignmentName, int assignmentId);
        void UpdateAssignmentDeadline(DateTime? newAssignmentDeadline, int assignmentId);
        void UpdateAssignmentCheck(bool newAssignmentCheck, int assignmentId);
        void UpdateAssignmentImportance(bool newAssignmentImportance, int assignmentId);
        void UpdateAssignmentSharing(bool newAssignmentSharing, int assignmentId);
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
                .Where(a => a.UserId == userId 
                && a.Deadline != null
                && a.Deadline.Value >= DateTime.Today.AddDays(1))
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

        //Update:
        public void UpdateAssignmentName(string newAssignmentName, int assignmentId)
        {
            var assignmentToUpdate = GetAssignmentToUpdate(assignmentId);

            assignmentToUpdate.Name = newAssignmentName;
            _dbContext.SaveChanges();
        }

        public void UpdateAssignmentDeadline(DateTime? newAssignmentDeadline, int assignmentId)
        {
            var assignmentToUpdate = GetAssignmentToUpdate(assignmentId);

            assignmentToUpdate.Deadline = newAssignmentDeadline;
            _dbContext.SaveChanges();
        }

        public void UpdateAssignmentCheck(bool newAssignmentCheck, int assignmentId)
        {
            var assignmentToUpdate = GetAssignmentToUpdate(assignmentId);

            assignmentToUpdate.IsChecked = newAssignmentCheck;
            _dbContext.SaveChanges();
        }

        public void UpdateAssignmentImportance(bool newAssignmentImportance, int assignmentId)
        {
            var assignmentToUpdate = GetAssignmentToUpdate(assignmentId);

            assignmentToUpdate.IsImportant = newAssignmentImportance;
            _dbContext.SaveChanges();
        }

        public void UpdateAssignmentSharing(bool newAssignmentSharing, int assignmentId)
        {
            var assignmentToUpdate = GetAssignmentToUpdate(assignmentId);

            assignmentToUpdate.IsShared = newAssignmentSharing;
            if (newAssignmentSharing) 
            {
                ShareAssignment(assignmentId);
            }
            else
            {
                StopSharingAssignment(assignmentId);
            }
        }

        private Assignment GetAssignmentToUpdate(int assignmentId) 
            => _dbContext.Assignments
                .First(a => a.Id == assignmentId);

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

        //Delete:
        public void DeleteAssignment(int assignmentId)
        {
            var assignmentToDelete = _dbContext.Assignments
                .First(a => a.Id == assignmentId);

            _dbContext.Assignments.Remove(assignmentToDelete);
            _dbContext.SaveChanges();
        }
    }
}
