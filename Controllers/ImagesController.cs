using System.Threading.Tasks;
 using Microsoft.AspNetCore.Mvc;
 using Microsoft.WindowsAzure.Storage;
 using Microsoft.WindowsAzure.Storage.Blob;
 using System.Net;
 namespace WebApplicationBasic.Controllers
 {
     [Route("api/[controller]")]
     public class ImagesController : Controller
     {
         private CloudBlobContainer _container;
         public ImagesController()
         {
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
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference($"{request.Id}.{request.EncodingFormat}");
            HttpWebRequest aRequest = (HttpWebRequest)WebRequest.Create(request.URL);
            HttpWebResponse aResponse = (await aRequest.GetResponseAsync()) as HttpWebResponse;
            var stream = aResponse.GetResponseStream();
            await blockBlob.UploadFromStreamAsync(stream);
            stream.Dispose();
            return Ok();
        }
     }
     public class ImagePostRequest
    {
        public string URL { get; set; }
        public string Id { get; set; }
        public string EncodingFormat { get; set; }
    }
    
 }