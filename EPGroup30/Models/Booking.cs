using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EPGroup30.Models
{
    public class Booking
    {
        [Key]//primary key for below column
        public int BookingID { get; set; }
        [Required(ErrorMessage = "You must key in the Booking Name")]
        [Display(Name = "Booking Name:")]
        public string? BookingName { get; set; }
        [Required(ErrorMessage = "Please key in your email")]
        [Display(Name = "Email:")]
        public String? Email { get; set; }
        [Required(ErrorMessage = "You must key in the Booking Category")]
        [Display(Name = "Booked Category:")]

        public string? BookingCategory { get; set; }
        [Required(ErrorMessage = "You must key in the Booking Date")]
        [Display(Name = "Booked Date:")]
        public DateTime BookedDate { get; set; }
        [Required(ErrorMessage = "You must key in the number of guests")]
        [Display(Name = "No of Guests:")]
        public int Guests { get; set; }
        [Required(ErrorMessage = "You must key in the booking details")]
        [Display(Name = "Booking Details:")]
        public String? BookingDetails { get; set; }
        [Required(ErrorMessage = "Please type none if you don't have any extra requests")]
        [Display(Name = "Extra Request:")]
        public String? ExtraRequest { get; set; }
        



    }
}
