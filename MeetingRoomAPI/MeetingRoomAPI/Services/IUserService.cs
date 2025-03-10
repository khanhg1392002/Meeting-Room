using MeetingRoomAPI.Models;
using System.Collections.Generic;

namespace MeetingRoomAPI.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User? GetUserById(int id);
        int AddUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(int id);
    }
}