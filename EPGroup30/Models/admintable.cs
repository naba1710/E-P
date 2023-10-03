using System.ComponentModel.DataAnnotations;
namespace EPGroup30.Models
{
    public class admintable
    {
        [Key]//primary key for below column
        public int AdminID { get; set; }
        [Required(ErrorMessage = "You must key in your email")]
        [Display(Name = "Admin Email :")]
        public string AdminEmail { get; set; }
        [Required(ErrorMessage = "You must key in the Password")]
        [Display(Name = "Password :")]
        public string AdminPassword { get; set; }
    }
}
