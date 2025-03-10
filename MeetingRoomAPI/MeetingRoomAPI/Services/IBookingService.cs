using MeetingRoomAPI.Models;
using System;
using System.Collections.Generic;

namespace MeetingRoomAPI.Services
{
    public interface IBookingService
    {
        List<Booking> GetAllBookings();
        Booking? GetBookingById(int id);
        int AddBooking(Booking booking);
        bool UpdateBooking(Booking booking);
        bool DeleteBooking(int id);
        bool HasTimeConflict(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
    }
}