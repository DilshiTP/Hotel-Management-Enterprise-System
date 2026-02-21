using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelBookingDomain.Models;
using HotelBookingClient.Services;

namespace HotelBookingClient.Pages.SpecialRequests
{
    public class CreateModel : PageModel
    {
        private readonly SpecialRequestService _service;

        [BindProperty]
        public SpecialRequest SRequest { get; set; } = new();

        public CreateModel(SpecialRequestService service)
        {
            _service = service;
        }

        public void OnGet()
        {
            SRequest = new SpecialRequest
            {
                RequestedDate = DateTime.Today,
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _service.CreateRequestsAsync(SRequest);
            return RedirectToPage("Index");
        }
    }
}
