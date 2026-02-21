using HotelBookingDomain.Models;
using System.Net.Http.Json;

namespace HotelBookingClient.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;

        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Booking>> GetBookingsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>("https://hotelbookingapi20250810132147-eadectdnecc8ahhm.centralus-01.azurewebsites.net/api/Bookings") 
                ?? new List<Booking>();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Booking>($"https://hotelbookingapi20250810132147-eadectdnecc8ahhm.centralus-01.azurewebsites.net/api/Bookings/{id}");
        }

        public async Task CreateBookingAsync(Booking booking)
        {
            await _httpClient.PostAsJsonAsync("https://hotelbookingapi20250810132147-eadectdnecc8ahhm.centralus-01.azurewebsites.net/api/Bookings", booking);
        }

        // public async Task UpdateBookingAsync(int id, Booking booking)
        // {
        //     await _httpClient.PutAsJsonAsync($"https://hotelbookingapi20250810132147-eadectdnecc8ahhm.centralus-01.azurewebsites.net/api/Bookings/{id}", booking);
        // }
        public async Task UpdateBookingAsync(Booking booking)
        {
            await _httpClient.PutAsJsonAsync($"https://hotelbookingapi20250810132147-eadectdnecc8ahhm.centralus-01.azurewebsites.net/api/Bookings/{booking.Id}", booking);
        }

        public async Task DeleteBookingAsync(int id)
        {
            await _httpClient.DeleteAsync($"https://hotelbookingapi20250810132147-eadectdnecc8ahhm.centralus-01.azurewebsites.net/api/Bookings/{id}");
        }
    }
}
