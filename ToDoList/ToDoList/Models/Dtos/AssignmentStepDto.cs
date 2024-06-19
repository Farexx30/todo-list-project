using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Dtos
{
    public class AssignmentStepDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int AssignmentId { get; set; }
    }
}
