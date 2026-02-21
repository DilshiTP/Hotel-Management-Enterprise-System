using HotelBookingDomain.Models;
using HotelBookingClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingClient.Pages.Rooms
{
    public class CreateModel : PageModel
    {
        private readonly RoomService _roomService;

        public CreateModel(RoomService roomService)
        {
            _roomService = roomService;
        }

        [BindProperty]
        public Room Room { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _roomService.CreateRoomAsync(Room);
            return RedirectToPage("Index");
        }
    }
}
