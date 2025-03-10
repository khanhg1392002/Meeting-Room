using System.ComponentModel.DataAnnotations;

namespace MeetingRoomAPI.Models
{
    public class Room
    {
        public int RoomID { get; set; }

        [Required(ErrorMessage = "BranchID is required.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Room Name is required.")]
        [StringLength(100, ErrorMessage = "Room Name cannot exceed 100 characters.")]
        public string? RoomName { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public bool Status { get; set; } = true;

        // Thêm navigation property
        public Branch? Branch { get; set; }
    }
}