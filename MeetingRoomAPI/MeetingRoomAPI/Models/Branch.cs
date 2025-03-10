namespace MeetingRoomAPI.Models
{
    public class Branch
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}