using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsBuiltIn { get; set; }
        public DtoState State { get; set; } = 0;

        public Guid? UserId { get; set; }
        public List<AssignmentDto> Assignments { get; set; } = [];
    }
}
