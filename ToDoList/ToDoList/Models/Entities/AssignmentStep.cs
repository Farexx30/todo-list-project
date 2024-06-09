using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Entities
{
    public class AssignmentStep
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        //Relacje:

        //Relacja z Assignment (relacja 1 do wielu):
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;
    }
}
