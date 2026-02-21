// using Microsoft.AspNetCore.Mvc.RazorPages;
// using HotelBookingDomain.Models;
// using HotelBookingClient.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace HotelBookingClient.Pages.Reports
// {
//     public class WeeklyModel : PageModel
//     {
//         private readonly BookingService _bookingService;
//         private readonly SpecialRequestService _requestService;

//         public List<WeeklyReportItem> Report { get; set; } = new();
//         public Dictionary<string, int> BookingsByRoomType { get; set; } = new();

//         public WeeklyModel(BookingService bookingService, SpecialRequestService requestService)
//         {
//             _bookingService = bookingService;
//             _requestService = requestService;
//         }

//         //public async void OnGet(DateTime? weekStart)
//         public async Task OnGetAsync(DateTime? weekStart)
//         {
//             var startOfWeek = weekStart ?? DateTime.Today;
//             startOfWeek = startOfWeek.AddDays(-(int)startOfWeek.DayOfWeek); // Sunday
//             var endOfWeek = startOfWeek.AddDays(6);

//             Report.Clear();

//             for (int i = 0; i < 7; i++)
//             {
//                 var date = startOfWeek.AddDays(i);
//                 var allb = await _bookingService.GetBookingsAsync();
//                 var bookings = allb
//                     .Count(b => b.CheckIn.Date <= date && b.CheckOut.Date >= date);
//                 var allr = await _requestService.GetRequestsAsync();
//                 var requests = allr
//                     .Count(r => r.RequestedDate.Date == date);
                
//                 Report.Add(new WeeklyReportItem
//                 {
//                     Date = date,
//                     BookingCount = bookings,
//                     SpecialRequestCount = requests
//                 });
//             }

//             await GroupByRoomType(startOfWeek);
//         }

//         private async Task GroupByRoomType(DateTime startOfWeek)
//         {
//             var allb = await _bookingService.GetBookingsAsync();
//             var bookings = allb
//                 .Where(b => b.CheckIn <= startOfWeek.AddDays(6) && b.CheckOut >= startOfWeek);

//             BookingsByRoomType = bookings
//                 .Where(b => b.RoomType != null)
//                 .GroupBy(b => b.RoomType!)
//                 .ToDictionary(g => g.Key, g => g.Count());
//         }
//     }
// }

using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelBookingDomain.Models;
using HotelBookingClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingClient.Pages.Reports
{
    public class WeeklyModel : PageModel
    {
        private readonly BookingService _bookingService;
        private readonly SpecialRequestService _requestService;

        public List<WeeklyReportItem> Report { get; set; } = new();
        public Dictionary<string, int> BookingsByRoomType { get; set; } = new();

        public WeeklyModel(BookingService bookingService, SpecialRequestService requestService)
        {
            _bookingService = bookingService;
            _requestService = requestService;
        }

        public async Task OnGetAsync(DateTime? weekStart)
        {
            var startOfWeek = weekStart ?? DateTime.Today;
            startOfWeek = startOfWeek.AddDays(-(int)startOfWeek.DayOfWeek); 
            var endOfWeek = startOfWeek.AddDays(6);

            // Fetch all data from the APIs concurrently at the beginning of the method.
            // This is the key change for I/O performance.
            var allBookingsTask = _bookingService.GetBookingsAsync();
            var allRequestsTask = _requestService.GetRequestsAsync();

            await Task.WhenAll(allBookingsTask, allRequestsTask);

            var allBookings = await allBookingsTask;
            var allRequests = await allRequestsTask;
            
            Report.Clear();

            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                
                var bookingsCount = allBookings
                    .Count(b => b.CheckIn.Date <= date && b.CheckOut.Date >= date);
                
                var requestsCount = allRequests
                    .Count(r => r.RequestedDate.Date == date);
                
                Report.Add(new WeeklyReportItem
                {
                    Date = date,
                    BookingCount = bookingsCount,
                    SpecialRequestCount = requestsCount
                });
            }
            GroupByRoomType(startOfWeek, allBookings);
        }

        private void GroupByRoomType(DateTime startOfWeek, List<Booking> allBookings)
        {
            var bookings = allBookings
                .Where(b => b.CheckIn <= startOfWeek.AddDays(6) && b.CheckOut >= startOfWeek);

            BookingsByRoomType = bookings
                .Where(b => b.RoomType != null)
                .GroupBy(b => b.RoomType!)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
