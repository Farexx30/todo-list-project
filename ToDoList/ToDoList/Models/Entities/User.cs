using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;

        //Relacje:

        //Relacja z Category (relacja 1 do wielu):
        public ICollection<Category> Categories { get; set; } = [];

        //Relacja z Assignment (relacja 1 do wielu):
        public ICollection<Assignment> Assignments { get; set; } = [];
    }
}
