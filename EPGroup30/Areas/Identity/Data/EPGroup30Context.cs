using EPGroup30.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace EPGroup30.Data;

public class EPGroup30Context : IdentityDbContext<EPGroup30User>
{
    public EPGroup30Context(DbContextOptions<EPGroup30Context> options)
        : base(options)
    {
    }


    public DbSet<EPGroup30.Models.admintable>AdminTable { get; set; }
    public DbSet<EPGroup30.Models.Venues>VenueTable { get; set; }
    public DbSet<EPGroup30.Models.VenuesBirthday> VenueBirthdayTable { get; set; }
    public DbSet<EPGroup30.Models.VenuesCooperate> VenueCooperateTable { get; set; }
    public DbSet<EPGroup30.Models.VenuesGraduation> VenueGraduationTable { get; set; }
    public DbSet<EPGroup30.Models.Booking> BookingTable { get; set; }
    public DbSet<EPGroup30.Models.Inquiry> InquiryTable { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
