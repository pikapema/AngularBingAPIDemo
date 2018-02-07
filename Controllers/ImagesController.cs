using System.Threading.Tasks;
 using Microsoft.AspNetCore.Mvc;
 using Microsoft.WindowsAzure.Storage;
 using Microsoft.WindowsAzure.Storage.Blob;
 using System.Net;
 using WebApplicationBasic.Models;
 using System.Collections.Generic;
 using System.Linq;

 namespace WebApplicationBasic.Controllers
 {
     [Route("api/[controller]")]
     public class ImagesController : Controller
     {
         private CloudBlobContainer _container;
         private AzureToolkitContext _context;
        public ImagesController(AzureToolkitContext context)
         {
             _context = context;
             CloudStorageAccount storageAccount = new CloudStorageAccount(
                 new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                         "azuretkstoragekapeltol",
                         "q0G+LdWBn7JjlbQ7nQxluufU+3mkRQw6YtwrvW5H8547ZxmKNzMiZmQmWmDuOq3bZP7KEjno2NifKzUW8/z80g=="), true);
             // Create a blob client.
             CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
             _container = blobClient.GetContainerReference("savedimages");
         }

         [HttpPost]
        public async Task<IActionResult> PostImage([FromBody]ImagePostRequest request)
         {
             //Upload Image
             CloudBlockBlob blockBlob = _container.GetBlockBlobReference($"{request.Id}.{request.EncodingFormat}");

             HttpWebRequest aRequest = (HttpWebRequest)WebRequest.Create(request.URL);
             HttpWebResponse aResponse = (await aRequest.GetResponseAsync()) as HttpWebResponse;

             var stream = aResponse.GetResponseStream();
             await blockBlob.UploadFromStreamAsync(stream);
             stream.Dispose();

             //Save metadata
             var savedImage = new SavedImage();
             savedImage.UserId = request.UserId;
             savedImage.Description = request.Description;
             savedImage.StorageUrl = blockBlob.Uri.ToString();
             savedImage.Tags = new List<SavedImageTag>();        

             foreach(var tag in request.Tags)
             {
                 savedImage.Tags.Add(new SavedImageTag() {Tag = tag});
             }

             savedImage.NoOfPeople = request.NoOfPeople;
             savedImage.Age = new List<int>();
             foreach(var age in request.Age)
             {
                 savedImage.Age.Add(age);
             }

             savedImage.Gender = new List<string>();
             foreach(var gender in request.Gender)
             {
                 savedImage.Gender.Add(gender);
             }


             _context.Add(savedImage);
             _context.SaveChanges();

             return Ok();
         }
     }
     public class ImagePostRequest
     {
         public string UserId { get; set; }
         public string Description { get; set; }
         public int NoOfPeople { get; set; }
         public string[] Tags { get; set; }
         public string[] Gender { get; set; }
         public int[] Age { get; set; }
         public string URL { get; set; }
         public string Id { get; set; }
         public string EncodingFormat { get; set; }
     }
    
 }