using HotelBookingDomain.Models;
using HotelBookingClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingClient.Pages.Rooms
{
    public class EditModel : PageModel
    {
        private readonly RoomService _roomService;

        [BindProperty]
        public Room Room { get; set; }

        public EditModel(RoomService roomService)
        {
            _roomService = roomService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
                return NotFound();

            Room = room;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _roomService.UpdateRoomAsync(Room);
            return RedirectToPage("Index");
        }
    }
}
