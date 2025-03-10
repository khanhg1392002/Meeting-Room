using MeetingRoomAPI.Models;
using MeetingRoomAPI.Repositories;
using System;
using System.Collections.Generic;

namespace MeetingRoomAPI.Services
{
    public class BranchService : IBranchService
    {
        private readonly BranchRepository _branchRepository;

        public BranchService(BranchRepository branchRepository)
        {
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
        }

        public List<Branch> GetAllBranches()
        {
            return _branchRepository.GetAllBranches();
        }

        public Branch? GetBranchById(int id)
        {
            return _branchRepository.GetBranchById(id);
        }

        public int AddBranch(Branch branch)
        {
            if (branch == null)
                throw new ArgumentNullException(nameof(branch));

            if (IsBranchNameExists(branch.BranchName))
                throw new InvalidOperationException("A branch with this name already exists.");

            return _branchRepository.AddBranch(branch);
        }

        public bool UpdateBranch(Branch branch)
        {
            if (branch == null)
                throw new ArgumentNullException(nameof(branch));

            var existingBranch = GetBranchById(branch.BranchID);
            if (existingBranch == null) return false;

            if (existingBranch.BranchName != branch.BranchName && IsBranchNameExists(branch.BranchName))
                return false;

            return _branchRepository.UpdateBranch(branch);
        }

        public bool DeleteBranch(int id)
        {
            return _branchRepository.DeleteBranch(id);
        }

        private bool IsBranchNameExists(string branchName)
        {
            var branches = _branchRepository.GetAllBranches();
            return branches.Any(b => b.BranchName == branchName && b.Status);
        }
    }
}