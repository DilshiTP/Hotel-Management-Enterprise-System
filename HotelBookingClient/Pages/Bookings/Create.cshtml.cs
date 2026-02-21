using HotelBookingClient.Services;
using HotelBookingDomain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingClient.Pages.Bookings
{
    public class CreateModel : PageModel
    {
        private readonly BookingService _bookingService;

        public CreateModel(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [BindProperty]
        public Booking Booking { get; set; } = new();

        public void OnGet()
        {
            Booking = new Booking
            {
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(1)
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _bookingService.CreateBookingAsync(Booking);
            return RedirectToPage("Index");
        }
    }
}
