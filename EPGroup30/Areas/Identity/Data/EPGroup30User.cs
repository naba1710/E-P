using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EPGroup30.Areas.Identity.Data;

// Add profile data for application users by adding properties to the EPGroup30User class
public class EPGroup30User : IdentityUser
{
    [PersonalData]
    public string CustomerName { get; set; }
    [PersonalData]

    public string PhoneNumber { get; set; }

}

