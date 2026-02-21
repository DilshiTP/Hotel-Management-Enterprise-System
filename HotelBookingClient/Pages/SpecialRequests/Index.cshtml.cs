using HotelBookingClient.Services;
using HotelBookingDomain.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingClient.Pages.SpecialRequests
{
    public class IndexModel : PageModel
    {
        private readonly SpecialRequestService _specialRequestService;

        public IndexModel(SpecialRequestService specialRequestService)
        {
            _specialRequestService = specialRequestService;
        }

        public List<SpecialRequest> SpecialRequests { get; set; } = new();

        public async Task OnGetAsync()
        {
            SpecialRequests = await _specialRequestService.GetRequestsAsync();
        }
    }
}
