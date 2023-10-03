using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EPGroup30.Models
{
    public class Inquiry
    {
        [Key]//primary key for below column
        public int ID { get; set; }
        [Required(ErrorMessage = "You must key in the Booking Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "You must key in the Booking Category")]
        [Display(Name = "Email")]

        public string Email { get; set; }
        [Required(ErrorMessage = "You must key in your inquiry")]
        [Display(Name = "Enquiry About")]
        public string Enquiry { get; set; }
        [Required(ErrorMessage = "You must key in the number of guests")]
        [Display(Name = "No of Guests")]
        public int Guests { get; set; }
        [Required(ErrorMessage = "You must key in your contact number")]
        [Display(Name = "Contact")]
        public String Contact { get; set; }
        [Required(ErrorMessage = "You must key in the Date")]
        [Display(Name = "Date :")]
        public DateTime Date { get; set; }
    }
}
