using MeetingRoomAPI.Models;
using MeetingRoomAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Branch>> Get()
        {
            var branches = _branchService.GetAllBranches();
            return Ok(branches);
        }

        [HttpGet("{id}")]
        public ActionResult<Branch> Get(int id)
        {
            var branch = _branchService.GetBranchById(id);
            if (branch == null) return NotFound();
            return Ok(branch);
        }

        [HttpPost]
        public ActionResult<Branch> Post([FromBody] Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var branchId = _branchService.AddBranch(branch);
                branch.BranchID = branchId;
                return CreatedAtAction(nameof(Get), new { id = branchId }, branch);
            }
            catch (InvalidOperationException ex)
            {
                // Trả về 409 Conflict với thông báo lỗi
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                // Xử lý trường hợp branch là null
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                return StatusCode(500, new { message = "An error occurred while adding the branch.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            branch.BranchID = id;

            if (id != branch.BranchID) return BadRequest();
            var existingBranch = _branchService.GetBranchById(id);
            if (existingBranch == null) return NotFound();

            try
            {
                if (!_branchService.UpdateBranch(branch))
                {
                    return Conflict(new { message = "A branch with this name already exists." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the branch.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_branchService.GetBranchById(id) == null) return NotFound();
            if (_branchService.DeleteBranch(id))
            {
                return NoContent();
            }
            return StatusCode(500, "Delete failed");
        }
    }
}