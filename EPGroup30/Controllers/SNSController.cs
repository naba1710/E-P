using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Amazon.S3.Model;
using System.Security.Policy;

namespace EPGroup30.Controllers
{
    public class SNSController : Controller
    {
        private const string topicARN = "arn:aws:sns:us-east-1:156821520524:eventplanningSNSgroup30";





        //function 1: connect keys from appsettings.json file
        private List<string> getKeys()
        {
            //create and empty list to store back the key value from appsettings.json file
            List<string> keys = new List<string>();





            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfiguration configure = builder.Build(); //build the file





            //add the keys to the list store
            keys.Add(configure["keys:key1"]);
            keys.Add(configure["keys:key2"]);
            keys.Add(configure["keys:key3"]);





            return keys;
        }





        //function 2: how to the user can subscribe the nesletter for themselves
        //create a subscription page
        public IActionResult Index()
        {
            return View();
        }





        //function 3: create a function to submit the subscription request
        public async Task<IActionResult> newsletterSubscription(string email)
        {
            List<string> keys = getKeys(); //call the getKeys() method to get the key info from appsettigs.json
            AmazonSimpleNotificationServiceClient agent =
                new AmazonSimpleNotificationServiceClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);





            //submit the subscription request
            try
            {
                SubscribeRequest request = new SubscribeRequest
                {
                    TopicArn = topicARN,
                    Protocol = "Email",
                    Endpoint = email
                };
                SubscribeResponse response = await agent.SubscribeAsync(request);
                ViewBag.responseID = response.ResponseMetadata.RequestId; //to proof that the user already registered
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            return View()
;
        }





        //customized message
        public IActionResult customizedMessage()
        {
            return View();



        }



        //send the broadcast message to the users
        public async Task<IActionResult> broadcastMessage(string subjecttitle, string messageContent)
        {
            List<string> keys = getKeys(); //call the getKeys() method to get the key info from appsettigs.json
            AmazonSimpleNotificationServiceClient agent =
                new AmazonSimpleNotificationServiceClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);



            try
            {
                PublishRequest request = new PublishRequest
                {
                    TopicArn = topicARN,
                    Subject = subjecttitle,
                    Message = messageContent
                };
                await agent.PublishAsync(request);
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            return Content("Message sent to users");
        }
    }
}







