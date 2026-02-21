using HotelBookingDomain.Models;
using HotelBookingClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingClient.Pages.Bookings
{
    public class DeleteModel : PageModel
    {
        private readonly BookingService _bookingService;

        public DeleteModel(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [BindProperty]
        public Booking Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Booking = await _bookingService.GetBookingByIdAsync(id);
            if (Booking == null)
                return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _bookingService.DeleteBookingAsync(Booking.Id);
            return RedirectToPage("Index");
        }
    }
}
