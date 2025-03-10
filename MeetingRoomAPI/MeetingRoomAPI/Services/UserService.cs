using MeetingRoomAPI.Models;
using MeetingRoomAPI.Repositories;
using System;
using System.Collections.Generic;

namespace MeetingRoomAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public User? GetUserById(int id)
        {
            return _userRepository.GetUserById(id);
        }

        public int AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (IsUserNameExists(user.UserName))
                throw new InvalidOperationException("A user with this username already exists.");

            return _userRepository.AddUser(user);
        }

        public bool UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = GetUserById(user.UserID);
            if (existingUser == null) return false;

            if (existingUser.UserName != user.UserName && IsUserNameExists(user.UserName))
                return false;

            return _userRepository.UpdateUser(user);
        }

        public bool DeleteUser(int id)
        {
            return _userRepository.DeleteUser(id);
        }

        private bool IsUserNameExists(string? userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;
            var users = _userRepository.GetAllUsers();
            return users.Any(u => u.UserName == userName && u.Status);
        }
    }
}