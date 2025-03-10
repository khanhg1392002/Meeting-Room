using MeetingRoomAPI.Models;
using MeetingRoomAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Room>> Get()
        {
            var rooms = _roomService.GetAllRooms();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public ActionResult<Room> Get(int id)
        {
            var room = _roomService.GetRoomById(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpPost]
        public ActionResult<Room> Post([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var roomId = _roomService.AddRoom(room);
                room.RoomID = roomId;
                return CreatedAtAction(nameof(Get), new { id = roomId }, room);
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
                return StatusCode(500, new { message = "An error occurred while adding the room.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            room.RoomID = id;

            if (id != room.RoomID) return BadRequest();
            var existingRoom = _roomService.GetRoomById(id);
            if (existingRoom == null) return NotFound();

            try
            {
                if (!_roomService.UpdateRoom(room))
                {
                    return Conflict(new { message = "A room with this name already exists." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the room.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var room = _roomService.GetRoomById(id);
            if (room == null) return NotFound();
            if (_roomService.DeleteRoom(id))
            {
                return NoContent();
            }
            return StatusCode(500, "Delete failed");
        }
    }
}