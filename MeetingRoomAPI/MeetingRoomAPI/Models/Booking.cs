using System.ComponentModel.DataAnnotations;

namespace MeetingRoomAPI.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        [Required(ErrorMessage = "RoomID is required.")]
        public int RoomID { get; set; }

        [Required(ErrorMessage = "Organizer is required.")]
        [StringLength(100, ErrorMessage = "Organizer cannot exceed 100 characters.")]
        public string? Organizer { get; set; }

        [Required(ErrorMessage = "Meeting Title is required.")]
        [StringLength(200, ErrorMessage = "Meeting Title cannot exceed 200 characters.")]
        public string? MeetingTitle { get; set; }

        [Required(ErrorMessage = "StartTime is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "EndTime is required.")]
        public DateTime EndTime { get; set; }

        public string? Purpose { get; set; }

        public bool IsConfidential { get; set; }

        [Required(ErrorMessage = "UserIDs is required.")]
        [StringLength(100, ErrorMessage = "UserIDs cannot exceed 100 characters.")]
        public string? UserIDs { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string? Status { get; set; }

        // Thêm navigation property
        public Room? Room { get; set; }
        public List<User>? Users { get; set; } // Thêm danh sách Users

    }
}