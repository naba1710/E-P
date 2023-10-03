using Microsoft.AspNetCore.Mvc;
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
    public class AdminController : Controller
    {
        private readonly EPGroup30Context context1;

        public AdminController(EPGroup30Context context)
        {
            context1 = context;
        }
        private const string bucketname = "eventplanninggroup30";

        //function extra: connection string to the AWS Account
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
        [Route("Admin/AdminLogin")]
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(admintable model)
        {
            if (ModelState.IsValid)
            {
                // Save the admin registration details in the AdminTable
                context1.AdminTable.Add(model);
                context1.SaveChanges();

                // Redirect to the login page or any other page as per your requirement
                return RedirectToAction("AdminLogin");
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(admintable model)
        {
            if (ModelState.IsValid)
            {
                // Check if the admin's credentials exist in the database
                var admin = context1.AdminTable.FirstOrDefault(a => a.AdminEmail == model.AdminEmail && a.AdminPassword == model.AdminPassword);

                if (admin != null)
                {
                    // If the admin exists, redirect to AdminHome page
                    return RedirectToAction("AdminHome");
                }
                else
                {
                    // If the admin does not exist, show an error message (optional)
                    ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
                }
            }

            return View();
        }

        public IActionResult AdminHome()
        {
            return View();
        }

        public async Task<IActionResult> AdminViewCustomers()
        {
            // Retrieve the customers' data from the database
            var customers = await context1.Users.ToListAsync();

            // Pass the list of customers to the view
            return View(customers);
        }

        public async Task<IActionResult> AdminViewBookings()
        {
            // Retrieve the customers' data from the database
            var bookings = await context1.BookingTable.ToListAsync();

            // Pass the list of customers to the view
            return View(bookings);
        }

        public async Task<IActionResult> AdminViewEnquiry()
        {
            // Retrieve the customers' data from the database
            var enquiry = await context1.InquiryTable.ToListAsync();

            // Pass the list of customers to the view
            return View(enquiry);
        }



        [HttpPost]
        [ValidateAntiForgeryToken] //avoids cross site attack

        public async Task<IActionResult> processInsert(Venues venues, List<IFormFile> imagefiles)
        {
            foreach (var image in imagefiles)
            {
                if (image.Length <= 0)
                {
                    return BadRequest("Error! File of " + image.FileName + " is an empty image!");
                }
                else if (image.Length > 1048576) // More than 1 MB
                {
                    return BadRequest("Error! File of " + image.FileName + " is > 1MB!");
                }
                else if (image.ContentType.ToLower() != "image/png" && image.ContentType.ToLower() != "image/jpeg")
                {
                    return BadRequest("Error! File of " + image.FileName + " is not a valid image format.");
                }
                else
                {
                    try
                    {
                        List<string> getkeys = getKeys();
                        var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                        using (var memoryStream = new MemoryStream())
                        {
                            image.CopyTo(memoryStream);

                            string s3Key = "weddingvenueimages/" + image.FileName;
                            PutObjectRequest uploadRequest = new PutObjectRequest
                            {
                                InputStream = memoryStream,
                                BucketName = bucketname,
                                Key = s3Key,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            await awsS3client.PutObjectAsync(uploadRequest);

                            // Assign the S3 URL and key to the venues object
                            venues.VenueURL = "https://" + bucketname + ".s3.amazonaws.com/" + s3Key;
                            venues.VenueS3Key = s3Key;

                            // Save the venue details to the database
                            context1.VenueTable.Add(venues);
                            await context1.SaveChangesAsync();
                        }
                    }
                    catch (AmazonS3Exception ex)
                    {
                        return BadRequest("Error: " + ex.Message);
                    }
                }
            }

            return RedirectToAction("ManageVenues", "Admin");
        }


        [HttpPost]
        [ValidateAntiForgeryToken] //avoids cross site attack

        public async Task<IActionResult> processInsertBirthday(VenuesBirthday venues, List<IFormFile> imagefiles)
        {
            foreach (var image in imagefiles)
            {
                if (image.Length <= 0)
                {
                    return BadRequest("Error! File of " + image.FileName + " is an empty image!");
                }
                else if (image.Length > 1048576) // More than 1 MB
                {
                    return BadRequest("Error! File of " + image.FileName + " is > 1MB!");
                }
                else if (image.ContentType.ToLower() != "image/png" && image.ContentType.ToLower() != "image/jpeg")
                {
                    return BadRequest("Error! File of " + image.FileName + " is not a valid image format.");
                }
                else
                {
                    try
                    {
                        List<string> getkeys = getKeys();
                        var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                        using (var memoryStream = new MemoryStream())
                        {
                            image.CopyTo(memoryStream);

                            string s3Key = "birthdayvenueimages/" + image.FileName;
                            PutObjectRequest uploadRequest = new PutObjectRequest
                            {
                                InputStream = memoryStream,
                                BucketName = bucketname,
                                Key = s3Key,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            await awsS3client.PutObjectAsync(uploadRequest);

                            // Assign the S3 URL and key to the venues object
                            venues.VenueURL = "https://" + bucketname + ".s3.amazonaws.com/" + s3Key;
                            venues.VenueS3Key = s3Key;

                            // Save the venue details to the database
                            context1.VenueBirthdayTable.Add(venues);
                            await context1.SaveChangesAsync();
                        }
                    }
                    catch (AmazonS3Exception ex)
                    {
                        return BadRequest("Error: " + ex.Message);
                    }
                }
            }

            return RedirectToAction("ManageBirthdayVenues", "Admin");
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //avoids cross site attack

        public async Task<IActionResult> processInsertGraduation(VenuesGraduation venues, List<IFormFile> imagefiles)
        {
            foreach (var image in imagefiles)
            {
                if (image.Length <= 0)
                {
                    return BadRequest("Error! File of " + image.FileName + " is an empty image!");
                }
                else if (image.Length > 1048576) // More than 1 MB
                {
                    return BadRequest("Error! File of " + image.FileName + " is > 1MB!");
                }
                else if (image.ContentType.ToLower() != "image/png" && image.ContentType.ToLower() != "image/jpeg")
                {
                    return BadRequest("Error! File of " + image.FileName + " is not a valid image format.");
                }
                else
                {
                    try
                    {
                        List<string> getkeys = getKeys();
                        var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                        using (var memoryStream = new MemoryStream())
                        {
                            image.CopyTo(memoryStream);

                            string s3Key = "graduationvenueimages/" + image.FileName;
                            PutObjectRequest uploadRequest = new PutObjectRequest
                            {
                                InputStream = memoryStream,
                                BucketName = bucketname,
                                Key = s3Key,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            await awsS3client.PutObjectAsync(uploadRequest);

                            // Assign the S3 URL and key to the venues object
                            venues.VenueURL = "https://" + bucketname + ".s3.amazonaws.com/" + s3Key;
                            venues.VenueS3Key = s3Key;

                            // Save the venue details to the database
                            context1.VenueGraduationTable.Add(venues);
                            await context1.SaveChangesAsync();
                        }
                    }
                    catch (AmazonS3Exception ex)
                    {
                        return BadRequest("Error: " + ex.Message);
                    }
                }
            }

            return RedirectToAction("ManageGraduationVenues", "Admin");
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //avoids cross site attack

        public async Task<IActionResult> processInsertCooperate(VenuesCooperate venues, List<IFormFile> imagefiles)
        {
            foreach (var image in imagefiles)
            {
                if (image.Length <= 0)
                {
                    return BadRequest("Error! File of " + image.FileName + " is an empty image!");
                }
                else if (image.Length > 1048576) // More than 1 MB
                {
                    return BadRequest("Error! File of " + image.FileName + " is > 1MB!");
                }
                else if (image.ContentType.ToLower() != "image/png" && image.ContentType.ToLower() != "image/jpeg")
                {
                    return BadRequest("Error! File of " + image.FileName + " is not a valid image format.");
                }
                else
                {
                    try
                    {
                        List<string> getkeys = getKeys();
                        var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                        using (var memoryStream = new MemoryStream())
                        {
                            image.CopyTo(memoryStream);

                            string s3Key = "cooperatevenueimages/" + image.FileName;
                            PutObjectRequest uploadRequest = new PutObjectRequest
                            {
                                InputStream = memoryStream,
                                BucketName = bucketname,
                                Key = s3Key,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            await awsS3client.PutObjectAsync(uploadRequest);

                            // Assign the S3 URL and key to the venues object
                            venues.VenueURL = "https://" + bucketname + ".s3.amazonaws.com/" + s3Key;
                            venues.VenueS3Key = s3Key;

                            // Save the venue details to the database
                            context1.VenueCooperateTable.Add(venues);
                            await context1.SaveChangesAsync();
                        }
                    }
                    catch (AmazonS3Exception ex)
                    {
                        return BadRequest("Error: " + ex.Message);
                    }
                }
            }

            return RedirectToAction("ManageCooperateVenues", "Admin");
        }
        public async Task<IActionResult> ManageVenues(string searchString)
        {
            List<Venues> managevenues = await context1.VenueTable.ToListAsync();

            //add on: if we receive any search word keyword filter the whole list first before send to frontend
            if (!string.IsNullOrEmpty(searchString))
            {
                managevenues = managevenues.Where(s => s.VenueName.Contains(searchString)).ToList();
            }
            return View(managevenues);//want to bring it ti tge front end to display
        }

        public async Task<IActionResult> ManageBirthdayVenues(string searchString)
        {
            List<VenuesBirthday> managevenues = await context1.VenueBirthdayTable.ToListAsync();

            //add on: if we receive any search word keyword filter the whole list first before send to frontend
            if (!string.IsNullOrEmpty(searchString))
            {
                managevenues = managevenues.Where(s => s.VenueName.Contains(searchString)).ToList();
            }
            return View(managevenues);//want to bring it ti tge front end to display
        }
        public async Task<IActionResult> ManageGraduationVenues(string searchString)
        {
            List<VenuesGraduation> managevenues = await context1.VenueGraduationTable.ToListAsync();

            //add on: if we receive any search word keyword filter the whole list first before send to frontend
            if (!string.IsNullOrEmpty(searchString))
            {
                managevenues = managevenues.Where(s => s.VenueName.Contains(searchString)).ToList();
            }
            return View(managevenues);//want to bring it ti tge front end to display
        }
        public async Task<IActionResult> ManageCooperateVenues(string searchString)
        {
            List<VenuesCooperate> managevenues = await context1.VenueCooperateTable.ToListAsync();

            //add on: if we receive any search word keyword filter the whole list first before send to frontend
            if (!string.IsNullOrEmpty(searchString))
            {
                managevenues = managevenues.Where(s => s.VenueName.Contains(searchString)).ToList();
            }
            return View(managevenues);//want to bring it ti tge front end to display
        }


        //function 1: How to load inset page

        public IActionResult AddNewVenue()
        {
            return View();
        }
        public IActionResult AddNewBirthdayVenue()
        {
            return View();
        }
        public IActionResult AddNewGraduationVenue()
        {
            return View();
        }
        public IActionResult AddNewCooperateVenue()
        {
            return View();
        }
        //function 2: inserting data into the table

        [HttpPost]

        [AutoValidateAntiforgeryToken]// to avoid cross-side attack

        public async Task<IActionResult> AddVenue(Venues venues)

        {

            //check the form is valid or not

            if (ModelState.IsValid)

            {

                context1.VenueTable.Add(venues);// add venue to the table

                await context1.SaveChangesAsync();// save the changes in the table

                return RedirectToAction("ManageVenues"); //return to venue list to see update list after insert

            }

            return View("AddNewVenue", venues); // if form has error it will return back to this view of the form

        }
        public async Task<IActionResult> AddBirthdayVenue(VenuesBirthday venues)

        {

            //check the form is valid or not

            if (ModelState.IsValid)

            {

                context1.VenueBirthdayTable.Add(venues);// add venue to the table

                await context1.SaveChangesAsync();// save the changes in the table

                return RedirectToAction("ManageBirthdayVenues"); //return to venue list to see update list after insert

            }

            return View("AddNewBirthdayVenue", venues); // if form has error it will return back to this view of the form

        }
        public async Task<IActionResult> AddGraduationVenue(VenuesGraduation venues)

        {

            //check the form is valid or not

            if (ModelState.IsValid)

            {

                context1.VenueGraduationTable.Add(venues);// add venue to the table

                await context1.SaveChangesAsync();// save the changes in the table

                return RedirectToAction("ManageGraduationVenues"); //return to venue list to see update list after insert

            }

            return View("AddNewGraduationVenue", venues); // if form has error it will return back to this view of the form

        }
        public async Task<IActionResult> AddCooperateVenue(VenuesCooperate venues)

        {

            //check the form is valid or not

            if (ModelState.IsValid)

            {

                context1.VenueCooperateTable.Add(venues);// add venue to the table

                await context1.SaveChangesAsync();// save the changes in the table

                return RedirectToAction("ManageGCooperateVenues"); //return to venue list to see update list after insert

            }

            return View("AddNewCooperateVenue", venues); // if form has error it will return back to this view of the form

        }


        //function 3: delete data from table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> deletepage(int? fid)
        {
            if (fid == null)
            {
                return NotFound();
            }
            try
            {
                var venue = await context1.VenueTable.FindAsync(fid);

                if (venue == null)
                {
                    return NotFound();
                }

                List<string> getkeys = getKeys();
                var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                DeleteObjectRequest deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketname, // Use the predefined bucket name
                    Key = venue.VenueS3Key
                };
                await awsS3client.DeleteObjectAsync(deleteRequest);

                context1.VenueTable.Remove(venue);
                await context1.SaveChangesAsync();

                return RedirectToAction("ManageVenues", "Admin");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> deletepagebirthday(int? fid)
        {
            if (fid == null)
            {
                return NotFound();
            }
            try
            {
                var venue = await context1.VenueBirthdayTable.FindAsync(fid);

                if (venue == null)
                {
                    return NotFound();
                }

                List<string> getkeys = getKeys();
                var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                DeleteObjectRequest deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketname, // Use the predefined bucket name
                    Key = venue.VenueS3Key
                };
                await awsS3client.DeleteObjectAsync(deleteRequest);

                context1.VenueBirthdayTable.Remove(venue);
                await context1.SaveChangesAsync();

                return RedirectToAction("ManageBirthdayVenues", "Admin");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> deletepagegraduation(int? fid)
        {
            if (fid == null)
            {
                return NotFound();
            }
            try
            {
                var venue = await context1.VenueGraduationTable.FindAsync(fid);

                if (venue == null)
                {
                    return NotFound();
                }

                List<string> getkeys = getKeys();
                var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                DeleteObjectRequest deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketname, // Use the predefined bucket name
                    Key = venue.VenueS3Key
                };
                await awsS3client.DeleteObjectAsync(deleteRequest);

                context1.VenueGraduationTable.Remove(venue);
                await context1.SaveChangesAsync();

                return RedirectToAction("ManageGraduationVenues", "Admin");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> deletepagecooperate(int? fid)
        {
            if (fid == null)
            {
                return NotFound();
            }
            try
            {
                var venue = await context1.VenueCooperateTable.FindAsync(fid);

                if (venue == null)
                {
                    return NotFound();
                }

                List<string> getkeys = getKeys();
                var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                DeleteObjectRequest deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketname, // Use the predefined bucket name
                    Key = venue.VenueS3Key
                };
                await awsS3client.DeleteObjectAsync(deleteRequest);

                context1.VenueCooperateTable.Remove(venue);
                await context1.SaveChangesAsync();

                return RedirectToAction("ManageCooperateVenues", "Admin");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> processEdit(Venues venues, IFormFile imagefile)
        {
            try
            {
                List<string> getkeys = getKeys();
                var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                using (var memoryStream = new MemoryStream())
                {
                    imagefile.CopyTo(memoryStream);

                    PutObjectRequest uploadRequest = new PutObjectRequest
                    {
                        InputStream = memoryStream,
                        BucketName = bucketname, // Only the bucket name, no need for the "weddingvenueimages/" part
                        Key = "weddingvenueimages/" + imagefile.FileName,
                        CannedACL = S3CannedACL.PublicRead // Use the correct ACL enum value
                    };

                    await awsS3client.PutObjectAsync(uploadRequest);
                }
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }

            venues.VenueURL = "https://" + bucketname + ".s3.amazonaws.com/weddingvenueimages/" + imagefile.FileName;
            venues.VenueS3Key = "weddingvenueimages/" + imagefile.FileName;

            if (ModelState.IsValid)
            {
                context1.VenueTable.Update(venues);
                await context1.SaveChangesAsync();
                return RedirectToAction("ManageVenues", "Admin");
            }

            return View("EditPage", venues);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> processEditBirthday(VenuesBirthday venues, IFormFile imagefile)
        {
            try
            {
                List<string> getkeys = getKeys();
                var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                using (var memoryStream = new MemoryStream())
                {
                    imagefile.CopyTo(memoryStream);

                    PutObjectRequest uploadRequest = new PutObjectRequest
                    {
                        InputStream = memoryStream,
                        BucketName = bucketname, 
                        Key = "birthdayvenueimages/" + imagefile.FileName,
                        CannedACL = S3CannedACL.PublicRead // Use the correct ACL enum value
                    };

                    await awsS3client.PutObjectAsync(uploadRequest);
                }
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }

            venues.VenueURL = "https://" + bucketname + ".s3.amazonaws.com/birthdayvenueimages/" + imagefile.FileName;
            venues.VenueS3Key = "birthdayvenueimages/" + imagefile.FileName;

            if (ModelState.IsValid)
            {
                context1.VenueBirthdayTable.Update(venues);
                await context1.SaveChangesAsync();
                return RedirectToAction("ManageBirthdayVenues", "Admin");
            }

            return View("EditPageBirthday", venues);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> processEditGraduation(VenuesGraduation venues, IFormFile imagefile)
        {
            try
            {
                List<string> getkeys = getKeys();
                var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                using (var memoryStream = new MemoryStream())
                {
                    imagefile.CopyTo(memoryStream);

                    PutObjectRequest uploadRequest = new PutObjectRequest
                    {
                        InputStream = memoryStream,
                        BucketName = bucketname,
                        Key = "graduationvenueimages/" + imagefile.FileName,
                        CannedACL = S3CannedACL.PublicRead // Use the correct ACL enum value
                    };

                    await awsS3client.PutObjectAsync(uploadRequest);
                }
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }

            venues.VenueURL = "https://" + bucketname + ".s3.amazonaws.com/graduationvenueimages/" + imagefile.FileName;
            venues.VenueS3Key = "graduationvenueimages/" + imagefile.FileName;

            if (ModelState.IsValid)
            {
                context1.VenueGraduationTable.Update(venues);
                await context1.SaveChangesAsync();
                return RedirectToAction("ManageGraduationVenues", "Admin");
            }

            return View("EditPageGraduation", venues);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> processEditCooperate(VenuesCooperate venues, IFormFile imagefile)
        {
            try
            {
                List<string> getkeys = getKeys();
                var awsS3client = new AmazonS3Client(getkeys[0], getkeys[1], getkeys[2], RegionEndpoint.USEast1);

                using (var memoryStream = new MemoryStream())
                {
                    imagefile.CopyTo(memoryStream);

                    PutObjectRequest uploadRequest = new PutObjectRequest
                    {
                        InputStream = memoryStream,
                        BucketName = bucketname,
                        Key = "cooperatevenueimages/" + imagefile.FileName,
                        CannedACL = S3CannedACL.PublicRead // Use the correct ACL enum value
                    };

                    await awsS3client.PutObjectAsync(uploadRequest);
                }
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }

            venues.VenueURL = "https://" + bucketname + ".s3.amazonaws.com/cooperatevenueimages/" + imagefile.FileName;
            venues.VenueS3Key = "cooperatevenueimages/" + imagefile.FileName;

            if (ModelState.IsValid)
            {
                context1.VenueCooperateTable.Update(venues);
                await context1.SaveChangesAsync();
                return RedirectToAction("ManageCooperateVenues", "Admin");
            }

            return View("EditPageCooperate", venues);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editpage(int? fid)
        {
            if (fid == null)
                return NotFound();

            Venues venues = await context1.VenueTable.FindAsync(fid);
            if (venues == null)
                return NotFound();

            return View(venues);//variable
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editpagebirthday(int? fid)
        {
            if (fid == null)
                return NotFound();

            VenuesBirthday venues = await context1.VenueBirthdayTable.FindAsync(fid);
            if (venues == null)
                return NotFound();

            return View(venues);//variable
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editpagegraduation(int? fid)
        {
            if (fid == null)
                return NotFound();

            VenuesGraduation venues = await context1.VenueGraduationTable.FindAsync(fid);
            if (venues == null)
                return NotFound();

            return View(venues);//variable
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editpagecooperate(int? fid)
        {
            if (fid == null)
                return NotFound();

            VenuesCooperate venues = await context1.VenueCooperateTable.FindAsync(fid);
            if (venues == null)
                return NotFound();

            return View(venues);//variable
        }

        //function 5: how to update data in table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updatepage(Venues venues)
        {
            //check whether the form is valid or not
            //if valid
            if (ModelState.IsValid)
            {
                context1.VenueTable.Update(venues);
                await context1.SaveChangesAsync();
                return RedirectToAction("ManageVenues", "Admin");

            }
            //if not valid
            return View("editpage", venues);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updatepagebirthday(VenuesBirthday venues)
        {
            //check whether the form is valid or not
            //if valid
            if (ModelState.IsValid)
            {
                context1.VenueBirthdayTable.Update(venues);
                await context1.SaveChangesAsync();
                return RedirectToAction("ManageGraduationVenues", "Admin");

            }
            //if not valid
            return View("editpagebirthday", venues);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updatepagegraduation(VenuesGraduation venues)
        {
            //check whether the form is valid or not
            //if valid
            if (ModelState.IsValid)
            {
                context1.VenueGraduationTable.Update(venues);
                await context1.SaveChangesAsync();
                return RedirectToAction("ManageGraduationVenues", "Admin");

            }
            //if not valid
            return View("editpagegraduation", venues);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updatepagecooperate(VenuesCooperate venues)
        {
            //check whether the form is valid or not
            //if valid
            if (ModelState.IsValid)
            {
                context1.VenueCooperateTable.Update(venues);
                await context1.SaveChangesAsync();
                return RedirectToAction("ManageCooperateVenues", "Admin");

            }
            //if not valid
            return View("editpagecooperate", venues);
        }

        public async Task<IActionResult> DisplayVenue()
        {
            try
            {
                List<Venues> venueList = await context1.VenueTable.ToListAsync();

                foreach (var venue in venueList)
                {
                    venue.VenueURL = $"https://{bucketname}.s3.amazonaws.com/{venue.VenueS3Key}";
                }

                return View(venueList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> DisplayVenueBirthday()
        {
            try
            {
                List<VenuesBirthday> venueList = await context1.VenueBirthdayTable.ToListAsync();

                foreach (var venue in venueList)
                {
                    venue.VenueURL = $"https://{bucketname}.s3.amazonaws.com/{venue.VenueS3Key}";
                }

                return View(venueList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> DisplayVenueGraduation()
        {
            try
            {
                List<VenuesGraduation> venueList = await context1.VenueGraduationTable.ToListAsync();

                foreach (var venue in venueList)
                {
                    venue.VenueURL = $"https://{bucketname}.s3.amazonaws.com/{venue.VenueS3Key}";
                }

                return View(venueList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> DisplayVenueCooperate()
        {
            try
            {
                List<VenuesCooperate> venueList = await context1.VenueCooperateTable.ToListAsync();

                foreach (var venue in venueList)
                {
                    venue.VenueURL = $"https://{bucketname}.s3.amazonaws.com/{venue.VenueS3Key}";
                }

                return View(venueList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}

