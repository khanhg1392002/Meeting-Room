using MeetingRoomAPI.Data;
using MeetingRoomAPI.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MeetingRoomAPI.Repositories
{
    public class BranchRepository
    {
        private readonly DatabaseContext _context;

        public BranchRepository(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Branch> GetAllBranches()
        {
            var branches = new List<Branch>();
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Branches WHERE Status = TRUE", connection as MySqlConnection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        branches.Add(new Branch
                        {
                            BranchID = reader.GetInt32("BranchID"),
                            BranchName = reader.GetString("BranchName"),
                            BranchAddress = reader.GetString("BranchAddress"),
                            Status = reader.GetBoolean("Status")
                        });
                    }
                }
            }
            return branches;
        }

        public Branch? GetBranchById(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Branches WHERE BranchID = @BranchID", connection as MySqlConnection);
                command.Parameters.AddWithValue("@BranchID", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Branch
                        {
                            BranchID = reader.GetInt32("BranchID"),
                            BranchName = reader.GetString("BranchName"),
                            BranchAddress = reader.GetString("BranchAddress"),
                            Status = reader.GetBoolean("Status")
                        };
                    }
                }
            }
            return null;
        }

        public int AddBranch(Branch branch)
        {
            if (branch == null)
                throw new ArgumentNullException(nameof(branch));

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO Branches (BranchName, BranchAddress, Status) VALUES (@BranchName, @BranchAddress, @Status)",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@BranchName", branch.BranchName);
                command.Parameters.AddWithValue("@BranchAddress", branch.BranchAddress);
                command.Parameters.AddWithValue("@Status", branch.Status);
                command.ExecuteNonQuery();
                return (int)command.LastInsertedId;
            }
        }

        public bool UpdateBranch(Branch branch)
        {
            if (branch == null)
                throw new ArgumentNullException(nameof(branch));

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "UPDATE Branches SET BranchName = @BranchName, BranchAddress = @BranchAddress, Status = @Status WHERE BranchID = @BranchID",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@BranchID", branch.BranchID);
                command.Parameters.AddWithValue("@BranchName", branch.BranchName);
                command.Parameters.AddWithValue("@BranchAddress", branch.BranchAddress);
                command.Parameters.AddWithValue("@Status", branch.Status);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteBranch(int id)
        {
            var branch = GetBranchById(id);
            if (branch == null) return false;

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "UPDATE Branches SET Status = FALSE WHERE BranchID = @BranchID",
                    connection as MySqlConnection);
                command.Parameters.AddWithValue("@BranchID", id);
                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}