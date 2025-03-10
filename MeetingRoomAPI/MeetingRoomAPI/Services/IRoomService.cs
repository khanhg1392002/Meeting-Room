using MeetingRoomAPI.Models;
using System.Collections.Generic;

namespace MeetingRoomAPI.Services
{
    public interface IRoomService
    {
        List<Room> GetAllRooms();
        Room? GetRoomById(int id);
        int AddRoom(Room room);
        bool UpdateRoom(Room room);
        bool DeleteRoom(int id);
    }
}