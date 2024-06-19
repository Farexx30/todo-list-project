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
    public interface IAssignmentRepositoryService
    {
        List<AssignmentDto> GetAssignments(Guid userId, int categoryId);
        AssignmentDto AddAssignment(AssignmentDto newAssignmentDto, int categoryId);
        void UpdateAssignment(Assignment updatedAssignmentDto);
        void DeleteAssignment(int assignmentId);
        void ShareAssignment(int assignmentId);
    }

    public class AssignmentRepositoryService(ToDoListDbContext dbContext, IMapper mapper) : IAssignmentRepositoryService
    {
        private readonly ToDoListDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public List<AssignmentDto> GetAssignments(Guid userId, int categoryId)
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

            var assignmentsDtos = _mapper.Map<List<AssignmentDto>>(assignments);
            return assignmentsDtos;
        }

        public AssignmentDto AddAssignment(AssignmentDto newAssignmentDto, int categoryId)
        {
            var newAssignment = _mapper.Map<Assignment>(newAssignmentDto);
            _dbContext.Assignments.Add(newAssignment);

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

        public void UpdateAssignment(Assignment updatedAssignment)
        {
            _dbContext.Assignments
                .Where(a => a.Id == updatedAssignment.Id)
                .ExecuteUpdate(p =>
                 p.SetProperty(a => a.Name, updatedAssignment.Name)
                 .SetProperty(a => a.Deadline, updatedAssignment.Deadline)
                 .SetProperty(a => a.IsChecked, updatedAssignment.IsChecked)
                 .SetProperty(a => a.IsImportant, updatedAssignment.IsImportant));
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
                    CategoryId = 1 //Wiemy, że "Zadania" mają Id = 1 (możnaby też ręcznie znaleźć to Id).
                };
                _dbContext.CategoryAssignments.Add(newCategoryAssignment);

                _dbContext.SaveChanges();
            }
        }
    }
}
