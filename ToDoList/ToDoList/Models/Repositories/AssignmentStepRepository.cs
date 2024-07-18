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
            try
            {
                var assignmentSteps = _dbContext.AssignmentSteps
                    .Where(aStep => aStep.AssignmentId == assignmentId)
                    .ToList();

                var assignmentStepsDtos = _mapper.Map<List<AssignmentStepDto>>(assignmentSteps);
                return assignmentStepsDtos;
            }            
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occured: {ex.Message}.\n\nThe application will shutdown.");
                Application.Current.Shutdown();
                return null!;
            }
        }

        public AssignmentStepDto AddAssignmentStep(AssignmentStepDto newAssignmentStepDto, int assignmentId)
        { 
            try
            {
                var newAssignmentStep = _mapper.Map<AssignmentStep>(newAssignmentStepDto);
                newAssignmentStep.AssignmentId = assignmentId;

                _dbContext.AssignmentSteps.Add(newAssignmentStep);
                _dbContext.SaveChanges();

                var justAddedAssignmentStep = _mapper.Map<AssignmentStepDto>(newAssignmentStep);
                return justAddedAssignmentStep;
            }           
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occured: {ex.Message}.\n\nThe application will shutdown.");
                Application.Current.Shutdown();
                return null!;
            }
        }

        public void UpdateAssignmentStep(AssignmentStepDto updatedAssignmentStepDto)
        {
            try
            {
                var assignmentStepToUpdate = _dbContext.AssignmentSteps
                    .First(aStep => aStep.Id == updatedAssignmentStepDto.Id);

                assignmentStepToUpdate.IsChecked = updatedAssignmentStepDto.IsChecked;
                _dbContext.SaveChanges();
            }           
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occured: {ex.Message}.\n\nThe application will shutdown.");
                Application.Current.Shutdown();
            }
        }

        public void DeleteAssignmentStep(int assignmentStepId)
        {
            try
            {
                var assignmentStepToDelete = _dbContext.AssignmentSteps
                    .First(aStep => aStep.Id == assignmentStepId);

                _dbContext.AssignmentSteps.Remove(assignmentStepToDelete);
                _dbContext.SaveChanges();
            }           
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occured: {ex.Message}.\n\nThe application will shutdown.");
                Application.Current.Shutdown();
            }
        }
    }
}
