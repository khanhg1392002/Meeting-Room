using MeetingRoomAPI.Models;
using MeetingRoomAPI.Repositories;
using System;
using System.Collections.Generic;

namespace MeetingRoomAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingRepository _bookingRepository;

        public BookingService(BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        }

        public List<Booking> GetAllBookings()
        {
            return _bookingRepository.GetAllBookings();
        }

        public Booking? GetBookingById(int id)
        {
            return _bookingRepository.GetBookingById(id);
        }

        public int AddBooking(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            // Kiểm tra thời gian quá khứ
            if (booking.StartTime <= DateTime.Now || booking.EndTime <= DateTime.Now)
                throw new InvalidOperationException("Booking time cannot be in the past.");

            // Kiểm tra khung giờ hợp lệ
            if (!IsValidBookingTime(booking.StartTime, booking.EndTime))
                throw new InvalidOperationException("Booking time must be between 08:00 and 17:00, excluding 12:00-13:00.");

            // Kiểm tra xung đột thời gian
            if (_bookingRepository.HasTimeConflict(booking.RoomID, booking.StartTime, booking.EndTime))
                throw new InvalidOperationException("The room is already booked for the selected time slot.");

            // Đặt trạng thái mặc định là "Confirmed"
            booking.Status = "Confirmed";
            return _bookingRepository.AddBooking(booking);
        }

        public bool UpdateBooking(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            var existingBooking = GetBookingById(booking.BookingID);
            if (existingBooking == null) return false;

            // Kiểm tra thời gian quá khứ
            if (booking.StartTime <= DateTime.Now || booking.EndTime <= DateTime.Now)
                throw new InvalidOperationException("Booking time cannot be in the past.");

            // Kiểm tra khung giờ hợp lệ
            if (!IsValidBookingTime(booking.StartTime, booking.EndTime))
                throw new InvalidOperationException("Booking time must be between 08:00 and 17:00, excluding 12:00-13:00.");

            // Kiểm tra xung đột thời gian (loại trừ booking hiện tại)
            if (_bookingRepository.HasTimeConflict(booking.RoomID, booking.StartTime, booking.EndTime, booking.BookingID))
                throw new InvalidOperationException("The room is already booked for the selected time slot.");

            // Cập nhật trạng thái nếu cần
            UpdateBookingStatus(booking);
            return _bookingRepository.UpdateBooking(booking);
        }

        public bool DeleteBooking(int id)
        {
            var booking = GetBookingById(id);
            if (booking == null) return false;

            // Đặt trạng thái thành "Cancelled" thay vì xóa
            booking.Status = "Cancelled";
            return _bookingRepository.UpdateBooking(booking);
        }

        public bool HasTimeConflict(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
        {
            return _bookingRepository.HasTimeConflict(roomId, startTime, endTime, excludeBookingId);
        }

        private bool IsValidBookingTime(DateTime startTime, DateTime endTime)
        {
            // Lấy thời gian trong ngày (bỏ qua ngày để chỉ kiểm tra giờ)
            var startHour = startTime.TimeOfDay;
            var endHour = endTime.TimeOfDay;

            // Kiểm tra khoảng thời gian hợp lệ: 08:00 - 17:00
            var startLimit = new TimeSpan(8, 0, 0); // 08:00
            var endLimit = new TimeSpan(17, 0, 0);  // 17:00
            var lunchStart = new TimeSpan(12, 0, 0); // 12:00
            var lunchEnd = new TimeSpan(13, 0, 0);   // 13:00

            if (startHour < startLimit || endHour > endLimit || startHour >= lunchStart && startHour < lunchEnd)
            {
                return false;
            }

            // Kiểm tra endTime không nằm trong giờ nghỉ trưa
            if (endHour > lunchStart && endHour <= lunchEnd)
            {
                return false;
            }

            // Kiểm tra startTime trước endTime
            if (startTime >= endTime)
            {
                return false;
            }

            return true;
        }

        private void UpdateBookingStatus(Booking booking)
        {
            if (booking.Status == "Confirmed" && DateTime.Now > booking.EndTime)
            {
                booking.Status = "Completed";
            }
            // Nếu trạng thái đã là "Cancelled" hoặc "Completed", giữ nguyên
        }
    }
}