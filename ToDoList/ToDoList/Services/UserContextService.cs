using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Dtos;

namespace ToDoList.Services
{
    public interface IUserContextService
    {
        UserDto? CurrentUserDto { get; set; }
    }

    public class UserContextService : IUserContextService
    {
        public UserDto? CurrentUserDto { get; set; }
    }
}
