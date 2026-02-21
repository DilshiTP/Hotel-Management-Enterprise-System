using HotelBookingDomain.Models;
using HotelBookingClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingClient.Pages.Rooms
{
    public class DeleteModel : PageModel
    {
        private readonly RoomService _roomService;

        public DeleteModel(RoomService roomService)
        {
            _roomService = roomService;
        }

        [BindProperty]
        public Room Room { get; set; }

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
            await _roomService.DeleteRoomAsync(Room.Id);
            return RedirectToPage("Index");
        }
    }
}
