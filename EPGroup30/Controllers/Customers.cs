using EPGroup30.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using EPGroup30.Models;
using System.Threading.Tasks;
using EPGroup30.Areas.Identity.Data;
using EPGroup30.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace EPGroup30.Controllers
{
    public class Customers : Controller
    {
        private readonly EPGroup30Context context1;
        public Customers(EPGroup30Context context)
        {
            context1 = context;
        }
        private const string bucketname = "eventplanninggroup30";

        private List<string> getKeys()
        {
            //create any empty list to store back the key value from appsetting.json file
            List<string> keys = new List<string>();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfiguration configure = builder.Build(); //build the file

            //add the key to the list store
            keys.Add(configure["keys:key1"]);
            keys.Add(configure["keys:key2"]);
            keys.Add(configure["keys:key3"]);

            return keys;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Bookings()
        {
            return View();
        }

        public IActionResult Inquires()
        {
            return View();
        }

        //function 2: inserting data into the table

        [HttpPost]

        [AutoValidateAntiforgeryToken]// to avoid cross-side attack

        public async Task<IActionResult> Book(Booking books)

        {

            //check the form is valid or not

            if (ModelState.IsValid)

            {

                context1.BookingTable.Add(books);// add venue to the table

                await context1.SaveChangesAsync();// save the changes in the table

                return RedirectToAction("Index", "Home"); //return to venue list to see update list after insert

            }

            return View("Bookings", books); // if form has error it will return back to this view of the form

        }


        [HttpPost]

        [AutoValidateAntiforgeryToken]// to avoid cross-side attack

        public async Task<IActionResult> Enquiry(Inquiry inquiry)

        {

            //check the form is valid or not

            if (ModelState.IsValid)

            {

                context1.InquiryTable.Add(inquiry);// add venue to the table

                await context1.SaveChangesAsync();// save the changes in the table

                return RedirectToAction("Index", "Home"); //return to venue list to see update list after insert

            }


            return View("Inquires", inquiry);
            // if form has error it will return back to this view of the form


        }
    }
}

