using MeetingRoomAPI.Data;
using MeetingRoomAPI.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MeetingRoomAPI.Repositories
{
    public class RoomRepository
    {
        private readonly DatabaseContext _context;

        public RoomRepository(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Room> GetAllRooms()
        {
            var rooms = new List<Room>();
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "SELECT r.*, b.BranchName, b.BranchAddress, b.Status AS BranchStatus " +
                    "FROM Rooms r " +
                    "LEFT JOIN Branches b ON r.BranchID = b.BranchID " +
                    "WHERE r.Status = TRUE",
                    connection as MySqlConnection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            RoomID = reader.GetInt32("RoomID"),
                            BranchID = reader.GetInt32("BranchID"),
                            RoomName = reader.IsDBNull(reader.GetOrdinal("RoomName")) ? null : reader.GetString("RoomName"),
                            Capacity = reader.GetInt32("Capacity"),
                            Status = reader.GetBoolean("Status"),
                            Branch = new Branch
                            {
                                //BranchID = reader.GetInt32("BranchID"),
                                BranchName = reader.GetString("BranchName"),
                                BranchAddress = reader.GetString("BranchAddress"),
                                //Status = reader.GetBoolean("BranchStatus")
                            }
                        });
                    }
                }
            }
            return rooms;
        }

        public Room? GetRoomById(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "SELECT r.*, b.BranchName, b.BranchAddress, b.Status AS BranchStatus " +
                    "FROM Rooms r " +
                    "LEFT JOIN Branches b ON r.BranchID = b.BranchID " +
                    "WHERE r.RoomID = @RoomID AND r.Status = TRUE",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@RoomID", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Room
                        {
                            RoomID = reader.GetInt32("RoomID"),
                            BranchID = reader.GetInt32("BranchID"),
                            RoomName = reader.IsDBNull(reader.GetOrdinal("RoomName")) ? null : reader.GetString("RoomName"),
                            Capacity = reader.GetInt32("Capacity"),
                            Status = reader.GetBoolean("Status"),
                            Branch = new Branch
                            {
                                //BranchID = reader.GetInt32("BranchID"),
                                BranchName = reader.GetString("BranchName"),
                                BranchAddress = reader.GetString("BranchAddress"),
                                //Status = reader.GetBoolean("BranchStatus")
                            }
                        };
                    }
                }
            }
            return null;
        }

        public int AddRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            if (IsRoomNameExists(room.RoomName))
                throw new InvalidOperationException("A room with this name already exists.");

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO Rooms (BranchID, RoomName, Capacity,  Status) VALUES (@BranchID, @RoomName, @Capacity, @Status)",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@BranchID", room.BranchID);
                command.Parameters.AddWithValue("@RoomName", room.RoomName);
                command.Parameters.AddWithValue("@Capacity", room.Capacity);
                command.Parameters.AddWithValue("@Status", room.Status);
                command.ExecuteNonQuery();
                return (int)command.LastInsertedId;
            }
        }

        public bool UpdateRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            var existingRoom = GetRoomById(room.RoomID);
            if (existingRoom == null) return false;

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "UPDATE Rooms SET BranchID = @BranchID, RoomName = @RoomName, Capacity = @Capacity,  Status = @Status WHERE RoomID = @RoomID",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@RoomID", room.RoomID);
                command.Parameters.AddWithValue("@BranchID", room.BranchID);
                command.Parameters.AddWithValue("@RoomName", room.RoomName);
                command.Parameters.AddWithValue("@Capacity", room.Capacity);
                command.Parameters.AddWithValue("@Status", room.Status);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteRoom(int id)
        {
            var room = GetRoomById(id);
            if (room == null) return false;

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "UPDATE Rooms SET Status = FALSE WHERE RoomID = @RoomID",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@RoomID", id);
                return command.ExecuteNonQuery() > 0;
            }
        }

        private bool IsRoomNameExists(string? roomName)
        {
            if (string.IsNullOrEmpty(roomName)) return false;

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "SELECT COUNT(*) FROM Rooms WHERE RoomName = @RoomName AND Status = TRUE",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@RoomName", roomName);
                var count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
    }
}