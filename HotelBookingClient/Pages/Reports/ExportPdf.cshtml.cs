using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelBookingClient.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HotelBookingDomain.Models;

namespace HotelBookingClient.Pages.Reports
{
    public class ExportPdfModel : PageModel
    {
        private readonly BookingService _bookingService;
        private readonly SpecialRequestService _requestService;

        public ExportPdfModel(BookingService bookingService, SpecialRequestService requestService)
        {
            _bookingService = bookingService;
            _requestService = requestService;
        }

        // public async Task<IActionResult> OnGet(DateTime? weekStart)
        // {
        //     QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        //     var startOfWeek = weekStart ?? DateTime.Today;
        //     startOfWeek = startOfWeek.AddDays(-(int)startOfWeek.DayOfWeek);

        //     var reportData = new List<(DateTime Date, int Bookings, int Requests, List<Booking> BookingsForDay, List<SpecialRequest> SpecialRForDay)>();

        //     for (int i = 0; i < 7; i++)
        //     {
        //         var date = startOfWeek.AddDays(i);
        //         var allb = await _bookingService.GetBookingsAsync();
        //         var bookings = allb
        //             .Count(b => b.CheckIn.Date <= date && b.CheckOut.Date >= date);
        //         var bookingsForDay = allb
        //             .Where(b => b.CheckIn.Date <= date && b.CheckOut.Date >= date)
        //             .ToList();
        //         var allr = await _requestService.GetRequestsAsync();
        //         var requestsForDay = allr
        //             .Where(r => r.RequestedDate.Date == date)
        //             .ToList();
        //         var requests = allr
        //             .Count(r => r.RequestedDate.Date == date);

        //         reportData.Add((date, bookings, requests, bookingsForDay, requestsForDay));
        //     }

        //     var pdfBytes = CreatePdf(reportData);

        //     return File(pdfBytes, "application/pdf", "WeeklyReport.pdf");
        // }

        public async Task<IActionResult> OnGet(DateTime? weekStart)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var startOfWeek = weekStart ?? DateTime.Today;
            startOfWeek = startOfWeek.AddDays(-(int)startOfWeek.DayOfWeek);

            // Fetch all bookings and all special requests concurrently.
            // This is the key change to improve performance.
            var allBookingsTask = _bookingService.GetBookingsAsync();
            var allRequestsTask = _requestService.GetRequestsAsync();

            await Task.WhenAll(allBookingsTask, allRequestsTask);

            var allBookings = await allBookingsTask;
            var allRequests = await allRequestsTask;

            var reportData = new List<(DateTime Date, int Bookings, int Requests, List<Booking> BookingsForDay, List<SpecialRequest> SpecialRForDay)>();

            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);

                // process the data from the in-memory lists,
                // instead of making new API calls in each iteration.
                var bookingsForDay = allBookings
                    .Where(b => b.CheckIn.Date <= date && b.CheckOut.Date >= date)
                    .ToList();
                
                var bookingsCount = bookingsForDay.Count();

                var requestsForDay = allRequests
                    .Where(r => r.RequestedDate.Date == date)
                    .ToList();
                
                var requestsCount = requestsForDay.Count();

                reportData.Add((date, bookingsCount, requestsCount, bookingsForDay, requestsForDay));
            }

            var pdfBytes = CreatePdf(reportData);

            return File(pdfBytes, "application/pdf", "WeeklyReport.pdf");
        }

        private byte[] CreatePdf(List<(DateTime Date, int Bookings, int Requests, List<Booking> BookingsForDay, List<SpecialRequest> SpecialRForDay)> data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Set up document-wide styles
                    var mainColor = Colors.Blue.Darken2;
                    var bookingsColor = Colors.Green.Darken2;
                    var requestsColor = Colors.Red.Darken2;
                    var headingFont = "Helvetica"; 

                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontFamily(headingFont).FontSize(10));

                    page.Header().Element(header =>
                    {
                        header.PaddingBottom(10).Text("Weekly Bookings & Special Requests Report")
                            .FontSize(20).Bold().FontColor(mainColor).AlignCenter();
                    });

                    page.Content().Column(column =>
                    {
                        // The existing summary table
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(160);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(mainColor).Padding(5).Text("Date").FontColor(Colors.White).Bold();
                                header.Cell().Background(mainColor).Padding(5).Text("Bookings").FontColor(Colors.White).Bold();
                                header.Cell().Background(mainColor).Padding(5).Text("Special Requests").FontColor(Colors.White).Bold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text(item.Date.ToString("dddd, MMM dd"));
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text(item.Bookings.ToString());
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text(item.Requests.ToString());
                            }
                        });
                        
                        column.Item().Padding(20);

                        foreach (var item in data)
                        {
                            if (item.BookingsForDay.Any())
                            {
                                column.Item()
                                    .PaddingVertical(10) // Use vertical padding for better spacing
                                    .Text($"Bookings for {item.Date.ToString("dddd, MMM dd")}")
                                    .FontSize(14).Bold().FontColor(bookingsColor); 

                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(3);
                                    });

                                    // Bookings Table Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Background(bookingsColor).Padding(5).Text("Guest Name").FontColor(Colors.White).Bold();
                                        header.Cell().Background(bookingsColor).Padding(5).Text("Check-In").FontColor(Colors.White).Bold();
                                        header.Cell().Background(bookingsColor).Padding(5).Text("Check-Out").FontColor(Colors.White).Bold();
                                        header.Cell().Background(bookingsColor).Padding(5).Text("Room Type").FontColor(Colors.White).Bold();
                                        header.Cell().Background(bookingsColor).Padding(5).Text("Special Request").FontColor(Colors.White).Bold();
                                    });

                                    foreach (var booking in item.BookingsForDay)
                                    {
                                        table.Cell().Padding(5).Text(booking.GuestName ?? "N/A");
                                        table.Cell().Padding(5).Text(booking.CheckIn.ToString("yyyy-MM-dd"));
                                        table.Cell().Padding(5).Text(booking.CheckOut.ToString("yyyy-MM-dd"));
                                        table.Cell().Padding(5).Text(booking.RoomType ?? "N/A");
                                        table.Cell().Padding(5).Text(booking.SpecialRequest ?? "None");
                                    }
                                });
                            }

                            // Detailed Special Requests Table
                            if (item.SpecialRForDay.Any())
                            {
                                column.Item()
                                    .PaddingVertical(10) // Use vertical padding for better spacing
                                    .Text($"Special Requests for {item.Date.ToString("dddd, MMM dd")}")
                                    .FontSize(14).Bold().FontColor(requestsColor); 

                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(3);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(requestsColor).Padding(5).Text("Guest Name").FontColor(Colors.White).Bold();
                                        header.Cell().Background(requestsColor).Padding(5).Text("Contact No").FontColor(Colors.White).Bold();
                                        header.Cell().Background(requestsColor).Padding(5).Text("Special Request").FontColor(Colors.White).Bold();
                                        header.Cell().Background(requestsColor).Padding(5).Text("Date").FontColor(Colors.White).Bold();
                                    });

                                    foreach (var request in item.SpecialRForDay)
                                    {
                                        table.Cell().Padding(5).Text(request.GuestName ?? "N/A");
                                        table.Cell().Padding(5).Text(request.GuestTel ?? "N/A");
                                        table.Cell().Padding(5).Text(request.RequestText ?? "None");
                                        table.Cell().Padding(5).Text(request.RequestedDate.ToString("yyyy-MM-dd"));
                                    }
                                });
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });

        return document.GeneratePdf();
        }
    }
}
