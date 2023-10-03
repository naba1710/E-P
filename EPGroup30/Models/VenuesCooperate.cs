using System.ComponentModel.DataAnnotations;

namespace EPGroup30.Models
{
    public class VenuesCooperate
    {
        [Key]//primary key for below column
        public int VenueID { get; set; }
        [Required(ErrorMessage = "You must key in the venue name")]
        [Display(Name = "Venue Name :")]
        public string VenueName { get; set; }
        [Required(ErrorMessage = "You must key in the Venue Category")]
        [Display(Name = "Venue Category :")]
        public string VenueCategory { get; set; }
        [Required(ErrorMessage = "You must key in the venue booked date")]
        [Display(Name = "Booked Date :")]
        public DateTime VenueBookedDate { get; set; }
        [Required(ErrorMessage = "You must key in the Venue Details")]
        [Display(Name = "Venue Details :")]
        public String VenueDetails { get; set; }
        [Required(ErrorMessage = "You must key in the Venue price")]
        [Display(Name = "Price :")]
        public decimal VenuePrice { get; set; }
        [Required(ErrorMessage = "You must key in Any price addition, if none please key in 0")]
        [Display(Name = "Additional costs :")]
        public decimal VenueAddPrice { get; set; }
        [Display(Name = "Venue Image :")]
        public String VenueURL { get; set; }
        public String VenueS3Key { get; set; }
    }
}
