using MeetingRoomAPI.Data;
using MeetingRoomAPI.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MeetingRoomAPI.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Users WHERE Status = TRUE", connection as MySqlConnection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserID = reader.GetInt32("UserID"),
                            UserName = reader.GetString("UserName"),
                            FullName = reader.GetString("FullName"),
                            Email = reader.GetString("Email"),
                            Team = reader.IsDBNull(reader.GetOrdinal("Team")) ? null : reader.GetString("Team"),
                            Status = reader.GetBoolean("Status")
                        });
                    }
                }
            }
            return users;
        }

        public User? GetUserById(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Users WHERE UserID = @UserID", connection as MySqlConnection);
                command.Parameters.AddWithValue("@UserID", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserID = reader.GetInt32("UserID"),
                            UserName = reader.GetString("UserName"),
                            FullName = reader.GetString("FullName"),
                            Email = reader.GetString("Email"),
                            Team = reader.IsDBNull(reader.GetOrdinal("Team")) ? null : reader.GetString("Team"),
                            Status = reader.GetBoolean("Status")
                        };
                    }
                }
            }
            return null;
        }

        public int AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO Users (UserName, FullName, Email, Team, Status) VALUES (@UserName, @FullName, @Email, @Team, @Status)",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@FullName", user.FullName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Team", (object)user.Team ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", user.Status);
                command.ExecuteNonQuery();
                return (int)command.LastInsertedId;
            }
        }

        public bool UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "UPDATE Users SET UserName = @UserName, FullName = @FullName, Email = @Email, Team = @Team, Status = @Status WHERE UserID = @UserID",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@UserID", user.UserID);
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@FullName", user.FullName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Team", (object)user.Team ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", user.Status);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteUser(int id)
        {
            var user = GetUserById(id);
            if (user == null) return false;

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "UPDATE Users SET Status = FALSE WHERE UserID = @UserID",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@UserID", id);
                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}