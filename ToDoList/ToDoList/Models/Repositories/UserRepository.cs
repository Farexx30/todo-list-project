using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToDoList.Models;
using ToDoList.Models.Dtos;
using ToDoList.Models.Entities;

namespace ToDoList.Models.Repositories
{
    public interface ILoginUserRepository
    {
        UserDto? LoginUser(RegisterOrLoginUserDto loginUserDto);
    }

    public interface IRegisterUserRepository
    {
        UserDto? RegisterUser(RegisterOrLoginUserDto newUserDto);
    }


    public class UserRepository(ToDoListDbContext dbContext, IMapper mapper, IPasswordHasher<User> passwordHasher) : ILoginUserRepository, IRegisterUserRepository
    {
        private readonly ToDoListDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        public UserDto? LoginUser(RegisterOrLoginUserDto loginUserDto)
        {
            var user = _dbContext.Users
                .FirstOrDefault(u => u.Name.ToLower() == loginUserDto.Name.ToLower());

            if (user == null)
            {
                return null;
            }

            var isPasswordCorrect = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);
            if (isPasswordCorrect == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public UserDto? RegisterUser(RegisterOrLoginUserDto newUserDto)
        {
            bool isUsernameInUse = _dbContext.Users
                .Any(u => u.Name == newUserDto.Name);

            if (isUsernameInUse)
            {
                return null;
            }

            var newUser = _mapper.Map<User>(newUserDto);
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, newUserDto.Password);

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            var userDto = _mapper.Map<UserDto>(newUser);
            return userDto;
        }
    }
}
