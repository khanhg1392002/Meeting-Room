using MeetingRoomAPI.Data;
using MeetingRoomAPI.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MeetingRoomAPI.Repositories
{
    public class BookingRepository
    {
        private readonly DatabaseContext _context;

        public BookingRepository(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Booking> GetAllBookings()
        {
            var bookings = new List<Booking>();
            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                // Ép kiểu connection thành MySqlConnection
                var mySqlConnection = (MySqlConnection)connection;

                // Lấy thông tin Booking và Room, Branch
                var command = new MySqlCommand(
                    "SELECT b.*, r.RoomName, r.Capacity, r.Status AS RoomStatus, " +
                    "br.BranchID AS Branch_BranchID, br.BranchName, br.BranchAddress, br.Status AS BranchStatus " +
                    "FROM Booking b " +
                    "LEFT JOIN Rooms r ON b.RoomID = r.RoomID " +
                    "LEFT JOIN Branches br ON r.BranchID = br.BranchID " +
                    "WHERE b.Status IN ('Confirmed', 'Completed')",
                    mySqlConnection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var booking = new Booking
                        {
                            BookingID = reader.GetInt32("BookingID"),
                            RoomID = reader.GetInt32("RoomID"),
                            Organizer = reader.IsDBNull(reader.GetOrdinal("Organizer")) ? null : reader.GetString("Organizer"),
                            MeetingTitle = reader.IsDBNull(reader.GetOrdinal("MeetingTitle")) ? null : reader.GetString("MeetingTitle"),
                            StartTime = reader.GetDateTime("StartTime"),
                            EndTime = reader.GetDateTime("EndTime"),
                            Purpose = reader.IsDBNull(reader.GetOrdinal("Purpose")) ? null : reader.GetString("Purpose"),
                            IsConfidential = reader.GetBoolean("IsConfidential"),
                            UserIDs = reader.IsDBNull(reader.GetOrdinal("UserIDs")) ? null : reader.GetString("UserIDs"),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString("Status"),
                            Room = new Room
                            {
                                RoomID = reader.GetInt32("RoomID"),
                                RoomName = reader.IsDBNull(reader.GetOrdinal("RoomName")) ? null : reader.GetString("RoomName"),
                                Capacity = reader.GetInt32("Capacity"),
                                //Status = reader.GetBoolean("RoomStatus"),
                                Branch = new Branch
                                {
                                    //BranchID = reader.GetInt32("Branch_BranchID"),
                                    BranchName = reader.IsDBNull(reader.GetOrdinal("BranchName")) ? null : reader.GetString("BranchName"),
                                    BranchAddress = reader.IsDBNull(reader.GetOrdinal("BranchAddress")) ? null : reader.GetString("BranchAddress"),
                                    //Status = reader.GetBoolean("BranchStatus")
                                }
                            }
                        };
                        bookings.Add(booking);
                    }
                }

                // Lấy thông tin Users cho tất cả bookings
                foreach (var booking in bookings)
                {
                    if (!string.IsNullOrEmpty(booking.UserIDs))
                    {
                        // Phân tích chuỗi UserIDs (dạng "[1,2]")
                        var userIds = ParseUserIds(booking.UserIDs);
                        if (userIds.Any())
                        {
                            booking.Users = GetUsersByIds(userIds, mySqlConnection);
                        }
                    }
                }
            }
            return bookings;
        }

        public Booking? GetBookingById(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                // Ép kiểu connection thành MySqlConnection
                var mySqlConnection = (MySqlConnection)connection;

                // Lấy thông tin Booking và Room, Branch
                var command = new MySqlCommand(
                    "SELECT b.*, r.RoomName, r.Capacity, r.Equipment, r.Status AS RoomStatus, " +
                    "br.BranchID AS Branch_BranchID, br.BranchName, br.BranchAddress, br.Status AS BranchStatus " +
                    "FROM Booking b " +
                    "LEFT JOIN Rooms r ON b.RoomID = r.RoomID " +
                    "LEFT JOIN Branches br ON r.BranchID = br.BranchID " +
                    "WHERE b.BookingID = @BookingID",
                    mySqlConnection);
                command.Parameters.AddWithValue("@BookingID", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var booking = new Booking
                        {
                            BookingID = reader.GetInt32("BookingID"),
                            RoomID = reader.GetInt32("RoomID"),
                            Organizer = reader.IsDBNull(reader.GetOrdinal("Organizer")) ? null : reader.GetString("Organizer"),
                            MeetingTitle = reader.IsDBNull(reader.GetOrdinal("MeetingTitle")) ? null : reader.GetString("MeetingTitle"),
                            StartTime = reader.GetDateTime("StartTime"),
                            EndTime = reader.GetDateTime("EndTime"),
                            Purpose = reader.IsDBNull(reader.GetOrdinal("Purpose")) ? null : reader.GetString("Purpose"),
                            IsConfidential = reader.GetBoolean("IsConfidential"),
                            UserIDs = reader.IsDBNull(reader.GetOrdinal("UserIDs")) ? null : reader.GetString("UserIDs"),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString("Status"),
                            Room = new Room
                            {
                                RoomID = reader.GetInt32("RoomID"),
                                RoomName = reader.IsDBNull(reader.GetOrdinal("RoomName")) ? null : reader.GetString("RoomName"),
                                Capacity = reader.GetInt32("Capacity"),
                                Status = reader.GetBoolean("RoomStatus"),
                                Branch = new Branch
                                {
                                    //BranchID = reader.GetInt32("Branch_BranchID"),
                                    BranchName = reader.IsDBNull(reader.GetOrdinal("BranchName")) ? null : reader.GetString("BranchName"),
                                    BranchAddress = reader.IsDBNull(reader.GetOrdinal("BranchAddress")) ? null : reader.GetString("BranchAddress"),
                                    //Status = reader.GetBoolean("BranchStatus")
                                }
                            }
                        };

                        // Lấy thông tin Users
                        if (!string.IsNullOrEmpty(booking.UserIDs))
                        {
                            var userIds = ParseUserIds(booking.UserIDs);
                            if (userIds.Any())
                            {
                                booking.Users = GetUsersByIds(userIds, mySqlConnection);
                            }
                        }

                        return booking;
                    }
                }
            }
            return null;
        }

        public int AddBooking(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO Booking (RoomID, Organizer, MeetingTitle, StartTime, EndTime, Purpose, IsConfidential, UserIDs, Status) " +
                    "VALUES (@RoomID, @Organizer, @MeetingTitle, @StartTime, @EndTime, @Purpose, @IsConfidential, @UserIDs, @Status)",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@RoomID", booking.RoomID);
                command.Parameters.AddWithValue("@Organizer", (object)booking.Organizer ?? DBNull.Value);
                command.Parameters.AddWithValue("@MeetingTitle", (object)booking.MeetingTitle ?? DBNull.Value);
                command.Parameters.AddWithValue("@StartTime", booking.StartTime);
                command.Parameters.AddWithValue("@EndTime", booking.EndTime);
                command.Parameters.AddWithValue("@Purpose", (object)booking.Purpose ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsConfidential", booking.IsConfidential);
                command.Parameters.AddWithValue("@UserIDs", (object)booking.UserIDs ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object)booking.Status ?? "Confirmed");
                command.ExecuteNonQuery();
                return (int)command.LastInsertedId;
            }
        }

        public bool UpdateBooking(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "UPDATE Booking SET RoomID = @RoomID, Organizer = @Organizer, MeetingTitle = @MeetingTitle, " +
                    "StartTime = @StartTime, EndTime = @EndTime, Purpose = @Purpose, IsConfidential = @IsConfidential, " +
                    "UserIDs = @UserIDs, Status = @Status WHERE BookingID = @BookingID",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@BookingID", booking.BookingID);
                command.Parameters.AddWithValue("@RoomID", booking.RoomID);
                command.Parameters.AddWithValue("@Organizer", (object)booking.Organizer ?? DBNull.Value);
                command.Parameters.AddWithValue("@MeetingTitle", (object)booking.MeetingTitle ?? DBNull.Value);
                command.Parameters.AddWithValue("@StartTime", booking.StartTime);
                command.Parameters.AddWithValue("@EndTime", booking.EndTime);
                command.Parameters.AddWithValue("@Purpose", (object)booking.Purpose ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsConfidential", booking.IsConfidential);
                command.Parameters.AddWithValue("@UserIDs", (object)booking.UserIDs ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object)booking.Status ?? "Confirmed");
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool HasTimeConflict(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM Booking WHERE RoomID = @RoomID AND Status = 'Confirmed' AND " +
                            "((StartTime <= @StartTime AND EndTime > @StartTime) OR " +
                            "(StartTime < @EndTime AND EndTime >= @EndTime))";
                if (excludeBookingId.HasValue)
                {
                    query += " AND BookingID != @ExcludeBookingId";
                }
                var command = new MySqlCommand(query, connection as MySqlConnection);
                command.Parameters.AddWithValue("@RoomID", roomId);
                command.Parameters.AddWithValue("@StartTime", startTime);
                command.Parameters.AddWithValue("@EndTime", endTime);
                if (excludeBookingId.HasValue)
                {
                    command.Parameters.AddWithValue("@ExcludeBookingId", excludeBookingId.Value);
                }

                var count = Convert.ToInt32(command.ExecuteScalar());
                Console.WriteLine($"HasTimeConflict - RoomID: {roomId}, StartTime: {startTime}, EndTime: {endTime}, Count: {count}");
                return count > 0;
            }
        }

        // Hàm hỗ trợ để phân tích UserIDs
        private List<int> ParseUserIds(string? userIdsString)
        {
            var userIds = new List<int>();
            if (string.IsNullOrEmpty(userIdsString)) return userIds;

            try
            {
                // Chuỗi dạng "[1,2]" -> parse thành danh sách số
                var cleanedString = userIdsString.Trim('[', ']');
                if (!string.IsNullOrEmpty(cleanedString))
                {
                    userIds = cleanedString.Split(',')
                        .Select(id => int.Parse(id.Trim()))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing UserIDs: {userIdsString}, Error: {ex.Message}");
            }
            return userIds;
        }

        // Hàm hỗ trợ để lấy danh sách Users từ danh sách UserIDs
        private List<User> GetUsersByIds(List<int> userIds, MySqlConnection connection)
        {
            var users = new List<User>();
            if (!userIds.Any()) return users;

            var command = new MySqlCommand(
                "SELECT * FROM Users WHERE UserID IN (" + string.Join(",", userIds) + ") AND Status = TRUE",
                connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        //UserID = reader.GetInt32("UserID"),
                        //UserName = reader.GetString("UserName"),
                        FullName = reader.GetString("FullName"),
                        Email = reader.GetString("Email"),
                        //Team = reader.IsDBNull(reader.GetOrdinal("Team")) ? null : reader.GetString("Team"),
                        //Status = reader.GetBoolean("Status")
                    });
                }
            }
            return users;
        }
    }
}
