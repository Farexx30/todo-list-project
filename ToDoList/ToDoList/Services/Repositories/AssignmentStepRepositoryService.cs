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
    public interface IAssignmentStepRepositoryService
    {
        List<AssignmentStep> AssignmentStepsCache { get; set; }
        List<AssignmentStepDto> GetAssignmentSteps(int assignmentId);
        void AddAssignmentStep(AssignmentStepDto newAssignmentStepDto);
        void UpdateAssignmentStep(AssignmentStepDto updatedAssignmentStepDto);
        void DeleteAssignmentStep(int assignmentStepId);
        void SaveAssignmentStepsChanges();
    }

    public class AssignmentStepRepositoryService(ToDoListDbContext dbContext, IMapper mapper) : IAssignmentStepRepositoryService
    {
        private readonly ToDoListDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public List<AssignmentStep> AssignmentStepsCache { get; set; } = [];

        public List<AssignmentStepDto> GetAssignmentSteps(int assignmentId)
        {
            var assignmentSteps = _dbContext.AssignmentSteps
                .Where(aStep => aStep.AssignmentId == assignmentId)
                .ToList();

            AssignmentStepsCache.AddRange(assignmentSteps);

            var assignmentStepsDtos = _mapper.Map<List<AssignmentStepDto>>(assignmentSteps);
            return assignmentStepsDtos;
        }

        public void AddAssignmentStep(AssignmentStepDto newAssignmentStepDto)
        {
            var newAssignmentStep = _mapper.Map<AssignmentStep>(newAssignmentStepDto);

            _dbContext.AssignmentSteps.Add(newAssignmentStep);
            _dbContext.SaveChanges();

            AssignmentStepsCache.Add(newAssignmentStep);
        }

        public void UpdateAssignmentStep(AssignmentStepDto updatedAssignmentStepDto)
        {
            var assignmentStepToUpdate = AssignmentStepsCache
                .First(aStep => aStep.Id == updatedAssignmentStepDto.Id);

            assignmentStepToUpdate.Name = updatedAssignmentStepDto.Name;
        }

        public void DeleteAssignmentStep(int assignmentStepId)
        {
            var assignmentStepToDelete = AssignmentStepsCache
                .First(aStep => aStep.Id == assignmentStepId);

            AssignmentStepsCache.Remove(assignmentStepToDelete);
        }

        public void SaveAssignmentStepsChanges() => _dbContext.SaveChanges();
    }
}
