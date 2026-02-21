using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelBookingClient.Services;
using HotelBookingClient.Extensions;
using System.Text.RegularExpressions;

namespace HotelBookingClient.Pages.ChatBot
{
    public class ChatMessage
    {
        public string Role { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class IndexModel : PageModel
    {
        private readonly BookingService _bookingService;

        private readonly RoomService _roomService;

        public IndexModel(BookingService bookingService, RoomService roomService)
        {
            _bookingService = bookingService;
            _roomService = roomService;
        }

        [BindProperty]
        public string UserMessage { get; set; } = string.Empty;

        public List<ChatMessage> ChatHistory { get; set; } = new();

        public void OnGet()
        {
            ChatHistory = HttpContext.Session.Get<List<ChatMessage>>("ChatHistory") ?? new List<ChatMessage>();
        }

        public async Task<IActionResult> OnPost()

        {
            ChatHistory = HttpContext.Session.Get<List<ChatMessage>>("ChatHistory") ?? new List<ChatMessage>();

            ChatHistory.Add(new ChatMessage { Role = "User", Message = UserMessage });

            var botResponse = await GenerateResponse(UserMessage);
            ChatHistory.Add(new ChatMessage { Role = "Bot", Message = botResponse });

            HttpContext.Session.Set("ChatHistory", ChatHistory);

            if (botResponse == "[REDIRECT_TO_BOOKING]")
            {
                return RedirectToPage("/Bookings/create");
            }

            return Page();
        }

        private async Task<string> GenerateResponse(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "Please ask a valid question.";

            message = message.ToLower();

            // 1. Booking intent detection
            if (Regex.IsMatch(message, @"\b(book|reserve|booking)\b"))
            {
                return "[REDIRECT_TO_BOOKING]";
            }
            // 2. Extract room type and dates
            var response = "";

            var roomTypeMatch = Regex.Match(message, @"\b(deluxe|standard|suite)\b");
            if (roomTypeMatch.Success)
            {
                response += $"You mentioned the room type: '{roomTypeMatch.Value}'.\n";
            }

            var dateMatch = Regex.Match(message, @"\b(\d{4}-\d{2}-\d{2})\b");
            if (dateMatch.Success && DateTime.TryParse(dateMatch.Value, out DateTime parsedDate))
            {
                response += $"You mentioned the date: {parsedDate:dddd, dd MMM yyyy}.\n";
            }

            if (roomTypeMatch.Success)
            {
                var roomType = roomTypeMatch.Value;
                response += PredictAvailabilityForRoomType(roomType);
                return response;
            }

            if (message.Contains("available") || message.Contains("vacant"))
                return await PredictAvailability();

            if (message.Contains("price") || message.Contains("pricing"))
                return await PredictPriceTrend();

            if (!string.IsNullOrEmpty(response))
                return response;

            return "I can help you with room availability and pricing trends. Try asking: 'Will rooms be available next week?' or 'What will be the price for single rooms next month?'";
        }

        private async Task<string> PredictAvailability()
        {
            var futureDate = DateTime.Today.AddDays(7);
            var totalRooms = 10;
            var allBooking = await _bookingService.GetBookingsAsync();
            var bookings = allBooking
                .Count(b => b.CheckIn <= futureDate && b.CheckOut >= futureDate);

            var available = totalRooms - bookings;
            return $"Based on current bookings, about {available} room(s) should be available on {futureDate:dddd, MMM dd}.";
        }

        private async Task<string> PredictPriceTrend()
        {
            var allBookings = await _bookingService.GetBookingsAsync();
            var last7Days = allBookings.Count(b => b.CheckIn >= DateTime.Today.AddDays(-7));
            var prev7Days = allBookings.Count(b => b.CheckIn < DateTime.Today.AddDays(-7) && b.CheckIn >= DateTime.Today.AddDays(-14));

            if (last7Days > prev7Days)
                return "The trend shows increasing bookings, so prices are likely to go up next week.";
            else if (last7Days < prev7Days)
                return "Bookings are slowing down — prices may stay stable or decrease.";
            else
                return "The booking trend is stable — prices are expected to remain the same.";
        }

        // private async Task<string> PredictAvailabilityForRoomType(string roomType)
        // {
        //     var futureDate = DateTime.Today.AddDays(7);
        //     var totalRooms = 10;

        //     try
        //     {
        //         var tr = await _roomService.GetRoomsAsync();
        //         totalRooms = tr
        //             .Count(r => 
        //             r.RoomType != null &&
        //             r.RoomType.Equals(roomType, StringComparison.OrdinalIgnoreCase));
        //     }
        //     catch
        //     {
        //         // If RoomService not injected or fails, fallback
        //     }
        //     var allBookings = await _bookingService.GetBookingsAsync();

        //     var bookings = allBookings.Count(b =>
        //         b.RoomType != null &&
        //         b.RoomType.Equals(roomType, StringComparison.OrdinalIgnoreCase) &&
        //         b.CheckIn <= futureDate &&
        //         b.CheckOut >= futureDate);

        //     var available = totalRooms - bookings;

        //     return $"Based on current bookings, about {available} {roomType} room(s) should be available on {futureDate:dddd, MMM dd}.";
        // }
        private async Task<string> PredictAvailabilityForRoomType(string roomType)
        {
            var futureDate = DateTime.Today.AddDays(7);
            var totalRooms = 10;

            // Start both asynchronous tasks concurrently
            var roomServiceTask = _roomService.GetRoomsAsync();
            var bookingServiceTask = _bookingService.GetBookingsAsync();

            try
            {
                // Wait for both tasks to complete at the same time
                await Task.WhenAll(roomServiceTask, bookingServiceTask);

                var tr = await roomServiceTask;
                totalRooms = tr
                    .Count(r =>
                    r.RoomType != null &&
                    r.RoomType.Equals(roomType, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                // If RoomService fails, we will proceed with the default totalRooms value.
                // The bookingServiceTask will still have a chance to complete.
            }

            var allBookings = await bookingServiceTask;

            var bookings = allBookings.Count(b =>
                b.RoomType != null &&
                b.RoomType.Equals(roomType, StringComparison.OrdinalIgnoreCase) &&
                b.CheckIn <= futureDate &&
                b.CheckOut >= futureDate);

            var available = totalRooms - bookings;

            return $"Based on current bookings, about {available} {roomType} room(s) should be available on {futureDate:dddd, MMM dd}.";
        }

        public IActionResult OnPostExportChat()
        {
            var chatHistory = HttpContext.Session.Get<List<ChatMessage>>("ChatHistory") ?? new List<ChatMessage>();

            var lines = chatHistory.Select(m => $"{m.Role}: {m.Message}");
            var text = string.Join(Environment.NewLine, lines);

            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return File(bytes, "text/plain", $"ChatHistory_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        }

    }
}
