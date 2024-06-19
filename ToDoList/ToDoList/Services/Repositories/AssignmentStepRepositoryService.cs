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
        List<AssignmentStepDto> GetAssignmentSteps(int assignmentId);
        void AddAssignmentStep(AssignmentStepDto newAssignmentStepDto);
        void UpdateAssignmentStep(AssignmentStepDto updatedAssignmentStepDto);
        void DeleteAssignmentStep(int assignmentStepId);
        void SaveAssignmentSteps();
    }

    public class AssignmentStepRepositoryService(ToDoListDbContext dbContext, IMapper mapper) : IAssignmentStepRepositoryService
    {
        private readonly ToDoListDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        private readonly List<AssignmentStep> _assignmentStepsCache = [];

        public List<AssignmentStepDto> GetAssignmentSteps(int assignmentId)
        {
            var assignmentSteps = _dbContext.AssignmentSteps
                .Where(aStep => aStep.AssignmentId == assignmentId)
                .ToList();

            _assignmentStepsCache.AddRange(assignmentSteps);

            var assignmentStepsDtos = _mapper.Map<List<AssignmentStepDto>>(assignmentSteps);
            return assignmentStepsDtos;
        }

        public void AddAssignmentStep(AssignmentStepDto newAssignmentStepDto)
        {
            var newAssignmentStep = _mapper.Map<AssignmentStep>(newAssignmentStepDto);

            _dbContext.AssignmentSteps.Attach(newAssignmentStep);
            _assignmentStepsCache.Add(newAssignmentStep);
        }

        public void UpdateAssignmentStep(AssignmentStepDto updatedAssignmentStepDto)
        {
            var assignmentStepToUpdate = _assignmentStepsCache
                .First(aStep => aStep.Id == updatedAssignmentStepDto.Id);

            assignmentStepToUpdate.Name = updatedAssignmentStepDto.Name;
        }

        public void DeleteAssignmentStep(int assignmentStepId)
        {
            var assignmentStepToDelete = _assignmentStepsCache
                .First(aStep => aStep.Id == assignmentStepId);

            _assignmentStepsCache.Remove(assignmentStepToDelete);
        }

        public void SaveAssignmentSteps() => _dbContext.SaveChanges();
    }
}
