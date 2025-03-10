using MeetingRoomAPI.Models;
using MeetingRoomAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Booking>> Get()
        {
            var bookings = _bookingService.GetAllBookings();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public ActionResult<Booking> Get(int id)
        {
            var booking = _bookingService.GetBookingById(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpPost]
        public ActionResult<Booking> Post([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var bookingId = _bookingService.AddBooking(booking);
                booking.BookingID = bookingId;
                return CreatedAtAction(nameof(Get), new { id = bookingId }, booking);
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
                return StatusCode(500, new { message = "An error occurred while adding the booking.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            booking.BookingID = id;

            if (id != booking.BookingID) return BadRequest();
            var existingBooking = _bookingService.GetBookingById(id);
            if (existingBooking == null) return NotFound();

            try
            {
                if (_bookingService.UpdateBooking(booking))
                {
                    return NoContent();
                }
                return StatusCode(500, "Update failed"); // Trường hợp hiếm khi UpdateBooking trả về false
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the booking.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var booking = _bookingService.GetBookingById(id);
            if (booking == null) return NotFound();
            if (_bookingService.DeleteBooking(id))
            {
                return NoContent();
            }
            return StatusCode(500, "Delete failed");
        }
    }
}