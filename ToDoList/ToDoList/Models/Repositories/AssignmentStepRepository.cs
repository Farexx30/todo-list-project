﻿using AutoMapper;
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
        void SaveAssignmentStepsChanges();
    }

    public class AssignmentStepRepository(ToDoListDbContext dbContext, IMapper mapper) : IAssignmentStepRepository
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

        public AssignmentStepDto AddAssignmentStep(AssignmentStepDto newAssignmentStepDto, int assignmentId)
        {
            var newAssignmentStep = _mapper.Map<AssignmentStep>(newAssignmentStepDto);
            newAssignmentStep.AssignmentId = assignmentId;

            _dbContext.AssignmentSteps.Add(newAssignmentStep);
            _assignmentStepsCache.Add(newAssignmentStep);

            var justAddedAssignmentStep = _mapper.Map<AssignmentStepDto>(newAssignmentStep);
            return justAddedAssignmentStep;
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

        public void SaveAssignmentStepsChanges()
        {
            _dbContext.SaveChanges();
            _assignmentStepsCache.Clear();
        }
    }
}