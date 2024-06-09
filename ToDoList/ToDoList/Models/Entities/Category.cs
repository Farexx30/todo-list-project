using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsBuiltIn { get; set; }

        //Relacje:

        //Relacja z Userem (relacja 1 do wielu):
        public Guid? UserId { get; set; } //Klucz obcy to tabeli User.
        public User? User { get; set; }

        //Relacja z Assignment (relacja wiele do wielu):
        public ICollection<Assignment> Assignments { get; set; } = [];

    }
}
