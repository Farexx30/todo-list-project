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
    public interface IAssignmentStepRepository
    {
        List<AssignmentStepDto> GetAssignmentSteps(int assignmentId);
        AssignmentStepDto AddAssignmentStep(AssignmentStepDto newAssignmentStepDto, int assignmentId);
        void UpdateAssignmentStep(AssignmentStepDto updatedAssignmentStepDto);
        void DeleteAssignmentStep(int assignmentStepId);
    }

    public class AssignmentStepRepository(ToDoListDbContext dbContext, IMapper mapper) : IAssignmentStepRepository
    {
        private readonly ToDoListDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public List<AssignmentStepDto> GetAssignmentSteps(int assignmentId)
        {
            var assignmentSteps = _dbContext.AssignmentSteps
                .Where(aStep => aStep.AssignmentId == assignmentId)
                .ToList();

            var assignmentStepsDtos = _mapper.Map<List<AssignmentStepDto>>(assignmentSteps);
            return assignmentStepsDtos;
        }

        public AssignmentStepDto AddAssignmentStep(AssignmentStepDto newAssignmentStepDto, int assignmentId)
        {
            var newAssignmentStep = _mapper.Map<AssignmentStep>(newAssignmentStepDto);
            newAssignmentStep.AssignmentId = assignmentId;

            _dbContext.AssignmentSteps.Add(newAssignmentStep);
            _dbContext.SaveChanges();

            var justAddedAssignmentStep = _mapper.Map<AssignmentStepDto>(newAssignmentStep);
            return justAddedAssignmentStep;
        }

        public void UpdateAssignmentStep(AssignmentStepDto updatedAssignmentStepDto)
        {
            var assignmentStepToUpdate = _dbContext.AssignmentSteps
                .First(aStep => aStep.Id == updatedAssignmentStepDto.Id);

            assignmentStepToUpdate.IsChecked = updatedAssignmentStepDto.IsChecked;

            _dbContext.SaveChanges();
        }

        public void DeleteAssignmentStep(int assignmentStepId)
        {
            var assignmentStepToDelete = _dbContext.AssignmentSteps
                .First(aStep => aStep.Id == assignmentStepId);

            _dbContext.AssignmentSteps.Remove(assignmentStepToDelete);
            _dbContext.SaveChanges();
        }
    }
}
