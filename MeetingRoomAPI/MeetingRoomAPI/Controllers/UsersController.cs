using MeetingRoomAPI.Models;
using MeetingRoomAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public ActionResult<User> Post([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = _userService.AddUser(user);
                user.UserID = userId;
                return CreatedAtAction(nameof(Get), new { id = userId }, user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the user.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.UserID = id;

            if (id != user.UserID) return BadRequest();
            var existingUser = _userService.GetUserById(id);
            if (existingUser == null) return NotFound();

            try
            {
                if (!_userService.UpdateUser(user))
                {
                    return Conflict(new { message = "A user with this username already exists." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_userService.GetUserById(id) == null) return NotFound();
            if (_userService.DeleteUser(id))
            {
                return NoContent();
            }
            return StatusCode(500, "Delete failed");
        }
    }
}