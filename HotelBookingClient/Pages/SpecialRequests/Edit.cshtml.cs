using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelBookingDomain.Models;
using HotelBookingClient.Services;

namespace HotelBookingClient.Pages.SpecialRequests
{
    public class EditModel : PageModel
    {
        private readonly SpecialRequestService _service;

        [BindProperty]
        public SpecialRequest SRequest { get; set; } = new();

        public EditModel(SpecialRequestService service)
        {
            _service = service;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var req = await _service.GetRequestsByIdAsync(id);
            if (req == null) return NotFound();
            SRequest = req;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _service.UpdateRequestsAsync(SRequest);
            return RedirectToPage("Index");
        }
    }
}
