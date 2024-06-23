using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? Deadline { get; set; }
        public bool IsChecked { get; set; }
        public bool IsImportant { get; set; }
        public bool IsShared { get; set; }

        //Relacje:

        //Relacja z Userem (relacja 1 do wielu):
        public Guid UserId { get; set; } //Klucz obcy to tabeli User.
        public User User { get; set; } = null!;

        //Relacja z Category (relacja wiele do wielu):
        public ICollection<Category> Categories { get; set; } = [];

        //Relacja z AssignmentStep (relacja 1 do wielu):
        public ICollection<AssignmentStep> AssignmentSteps { get; set; } = [];
        public ICollection<CategoryAssignment> CategoryAssignments { get; set; } = [];
    }
}
