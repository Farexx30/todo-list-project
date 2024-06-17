using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Entities;

namespace ToDoList.Models.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public List<CategoryDto> Categories { get; set; } = [];
        public List<AssignmentDto> Assignments { get; set; } = [];
    }
}
