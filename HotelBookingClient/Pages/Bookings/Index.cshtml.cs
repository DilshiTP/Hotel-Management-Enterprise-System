using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelBookingDomain.Models;
using HotelBookingClient.Services;

namespace HotelBookingClient.Pages.Bookings
{
    public class IndexModel : PageModel
    {
        private readonly BookingService _bookingService;

        public IndexModel(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public List<Booking> Bookings { get; set; } = new();

        public async Task OnGetAsync()
        {
            var bookings = await _bookingService.GetBookingsAsync();
            Bookings = bookings;
        }
    }
}
