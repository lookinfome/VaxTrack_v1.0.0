using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VaxTrack_v1.Models;

namespace VaxTrack_v1.Models
{
    public class AppDbContext : IdentityDbContext<AppUserModel>
    {
        public DbSet<UserDetailsModel> UserDetails { get; set; }
        public DbSet<UserVaccinationDetailsModel> UserVaccinationDetails {get; set;}
        public DbSet<BookingDetailsModel> BookingDetails {get; set;}
        public DbSet<HospitalDetailsModel> HospitalDetails {get; set;}
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }

    // public class AppDbContext : DbContext
    // {
    //     public DbSet<UserDetailsModel> UserDetails { get; set; }
    //     public DbSet<UserVaccinationDetailsModel> UserVaccinationDetails {get; set;}
    //     public DbSet<BookingDetailsModel> BookingDetails {get; set;}
    //     public DbSet<HospitalDetailsModel> HospitalDetails {get; set;}
    //     public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    // }
}