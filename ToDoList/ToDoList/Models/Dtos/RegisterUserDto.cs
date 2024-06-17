using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Dtos
{
    public class RegisterOrLoginUserDto
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
