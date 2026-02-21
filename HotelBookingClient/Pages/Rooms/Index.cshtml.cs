using HotelBookingDomain.Models;
using HotelBookingClient.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingClient.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly RoomService _roomService;

        public IndexModel(RoomService roomService)
        {
            _roomService = roomService;
        }

        public List<Room> Rooms { get; set; } = new();

        public async Task OnGetAsync()
        {
            Rooms = await _roomService.GetRoomsAsync();
        }
    }
}
