using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Dtos;
using ToDoList.Models.Entities;

namespace ToDoList.Services
{
    public interface ICurrentDataService
    {
        CategoryDto? CurrentCategory { get; set; }
        AssignmentDto? CurrentAssignment { get; set; }
    }

    public class CurrentDataService : ICurrentDataService
    {
        public CategoryDto? CurrentCategory { get; set; }
        public AssignmentDto? CurrentAssignment { get; set; }
    }
}
