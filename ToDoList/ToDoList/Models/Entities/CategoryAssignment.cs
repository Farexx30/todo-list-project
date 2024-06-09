using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Entities
{
    //Klasa spinajaca Category oraz Assignment.
    public class CategoryAssignment
    {
        public int CategoryId { get; set; } 
        public int AssignmentId { get; set; }
    }
}
