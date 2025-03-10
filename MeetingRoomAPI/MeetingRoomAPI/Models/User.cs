using System.ComponentModel.DataAnnotations;

namespace MeetingRoomAPI.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "UserName is required.")]
        [StringLength(100, ErrorMessage = "UserName cannot exceed 100 characters.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Team cannot exceed 100 characters.")]
        public string? Team { get; set; }

        public bool Status { get; set; } = true;
    }
}