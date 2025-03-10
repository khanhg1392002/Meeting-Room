using MeetingRoomAPI.Models;
using MeetingRoomAPI.Repositories;
using System;
using System.Collections.Generic;

namespace MeetingRoomAPI.Services
{
    public class RoomService : IRoomService
    {
        private readonly RoomRepository _roomRepository;

        public RoomService(RoomRepository roomRepository)
        {
            _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        }

        public List<Room> GetAllRooms()
        {
            return _roomRepository.GetAllRooms();
        }

        public Room? GetRoomById(int id)
        {
            return _roomRepository.GetRoomById(id);
        }

        public int AddRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            if (IsRoomNameExists(room.RoomName))
                throw new InvalidOperationException("A room with this name already exists.");

            return _roomRepository.AddRoom(room);
        }

        public bool UpdateRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            var existingRoom = GetRoomById(room.RoomID);
            if (existingRoom == null) return false;

            if (existingRoom.RoomName != room.RoomName && IsRoomNameExists(room.RoomName))
                return false;

            return _roomRepository.UpdateRoom(room);
        }

        public bool DeleteRoom(int id)
        {
            return _roomRepository.DeleteRoom(id);
        }

        private bool IsRoomNameExists(string? roomName)
        {
            if (string.IsNullOrEmpty(roomName)) return false;
            var rooms = _roomRepository.GetAllRooms();
            return rooms.Any(r => r.RoomName == roomName && r.Status);
        }
    }
}