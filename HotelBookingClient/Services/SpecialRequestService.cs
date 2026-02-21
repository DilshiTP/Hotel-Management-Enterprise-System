using HotelBookingDomain.Models;
using System.Net.Http.Json;

namespace HotelBookingClient.Services
{
    public class SpecialRequestService
    {
        private readonly HttpClient _httpClient;

        public SpecialRequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SpecialRequest>> GetRequestsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<SpecialRequest>>("https://specialrequestapi20250810155319-hvfnhzb6atgyagbe.centralus-01.azurewebsites.net/api/SpecialRequests")
                ?? new List<SpecialRequest>();
        }

        public async Task<SpecialRequest?> GetRequestsByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<SpecialRequest>($"https://specialrequestapi20250810155319-hvfnhzb6atgyagbe.centralus-01.azurewebsites.net/api/SpecialRequests/{id}");
        }

        public async Task CreateRequestsAsync(SpecialRequest request)
        {
            await _httpClient.PostAsJsonAsync("https://specialrequestapi20250810155319-hvfnhzb6atgyagbe.centralus-01.azurewebsites.net/api/SpecialRequests", request);
        }

        public async Task UpdateRequestsAsync(SpecialRequest request)
        {
            await _httpClient.PutAsJsonAsync($"https://specialrequestapi20250810155319-hvfnhzb6atgyagbe.centralus-01.azurewebsites.net/api/SpecialRequests/{request.Id}", request);
        }

        public async Task DeleteRequestsAsync(int id)
        {
            await _httpClient.DeleteAsync($"https://specialrequestapi20250810155319-hvfnhzb6atgyagbe.centralus-01.azurewebsites.net/api/SpecialRequests/{id}");
        }
    }
}
