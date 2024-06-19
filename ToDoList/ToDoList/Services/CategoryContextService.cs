using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Dtos;
using ToDoList.Models.Entities;

namespace ToDoList.Services
{
    public interface ICategoryContextService
    {
        CategoryDto? CurrentCategory { get; set; }
    }

    public class CategoryContextService : ICategoryContextService
    {
        public CategoryDto? CurrentCategory { get; set; }
    }
}
