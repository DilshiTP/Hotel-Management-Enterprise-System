using Microsoft.EntityFrameworkCore;
using HotelBookingDomain.Models;

namespace HotelBookingAPI.Data
{
    public class HotelBookingDbContext : DbContext
    {
        public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options) : base(options) { }

        public DbSet<Booking> Bookings { get; set; }

    }
}
