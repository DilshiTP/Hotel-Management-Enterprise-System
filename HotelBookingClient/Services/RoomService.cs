using HotelBookingDomain.Models;
using System.Net.Http.Json;

namespace HotelBookingClient.Services
{
    public class RoomService
    {
        private readonly HttpClient _httpClient;

        public RoomService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Room>> GetRoomsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Room>>("https://roomapi20250810151420-h8g0ejebfjdnhtap.centralus-01.azurewebsites.net/api/Rooms")
                ?? new List<Room>();
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Room>($"https://roomapi20250810151420-h8g0ejebfjdnhtap.centralus-01.azurewebsites.net/api/Rooms/{id}");
        }

        public async Task CreateRoomAsync(Room room)
        {
            await _httpClient.PostAsJsonAsync("https://roomapi20250810151420-h8g0ejebfjdnhtap.centralus-01.azurewebsites.net/api/Rooms", room);
        }

        public async Task UpdateRoomAsync(Room room)
        {
            await _httpClient.PutAsJsonAsync($"https://roomapi20250810151420-h8g0ejebfjdnhtap.centralus-01.azurewebsites.net/api/Rooms/{room.Id}", room);
        }

        public async Task DeleteRoomAsync(int id)
        {
            await _httpClient.DeleteAsync($"https://roomapi20250810151420-h8g0ejebfjdnhtap.centralus-01.azurewebsites.net/api/Rooms/{id}");
        }
    }
}
