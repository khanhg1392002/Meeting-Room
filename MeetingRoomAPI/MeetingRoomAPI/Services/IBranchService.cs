using MeetingRoomAPI.Models;
using System.Collections.Generic;

namespace MeetingRoomAPI.Services
{
    public interface IBranchService
    {
        List<Branch> GetAllBranches();
        Branch? GetBranchById(int id);
        int AddBranch(Branch branch);
        bool UpdateBranch(Branch branch);
        bool DeleteBranch(int id);
    }
}