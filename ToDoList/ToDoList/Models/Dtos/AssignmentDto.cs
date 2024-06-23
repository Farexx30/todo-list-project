using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Entities;

namespace ToDoList.Models.Dtos
{
    public class AssignmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? Deadline { get; set; }
        public bool IsChecked { get; set; }
        public bool IsImportant { get; set; }
        public bool IsShared { get; set; }

        public List<CategoryDto> Categories { get; set; } = [];
        public List<AssignmentStepDto> AssignmentSteps { get; set; } = [];

        public override string ToString() => Name;
    }
}
