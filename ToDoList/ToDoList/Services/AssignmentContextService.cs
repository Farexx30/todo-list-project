using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Dtos;

namespace ToDoList.Services
{
    public interface IAssignmentContextService
    {
        AssignmentDto? CurrentAssignment { get; set; }
    }

    public class AssignmentContextService : IAssignmentContextService
    {
        public AssignmentDto? CurrentAssignment { get; set; }
    }
}
