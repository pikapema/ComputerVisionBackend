using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Cognitive.CustomVision;
using Microsoft.Cognitive.CustomVision.Models;
using System.IO;
using System.Threading;

namespace ComputerVisionAPI.Controllers
{
    public class ImageController : ApiController
    {

        // POST: api/Image
        public async Task<HttpResponseMessage> PostFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                    // Create the Api, passing in a credentials object that contains the training key
                    Microsoft.Cognitive.CustomVision.
                    TrainingApiCredentials trainingCredentials = new TrainingApiCredentials("ea94a8906af14ac7a34df51ea4ba6750");
                    TrainingApi trainingApi = new TrainingApi(trainingCredentials);

                    Guid projectid = new Guid("57471653-6e79-455f-b874-ee00d1014c37");
                    ProjectModel model = trainingApi.GetProject(projectid);

                    MemoryStream memStream = new MemoryStream(File.ReadAllBytes(file.LocalFileName));

                    string tagname = provider.FormData.Get("tag");
                                        
                    var tag = trainingApi.GetTags(projectid).Tags.First(f => f.Name.Equals(tagname));
                    if( tag == null)
                    {
                        tag = trainingApi.CreateTag(projectid, tagname);
                    }                    
                    
                    trainingApi.CreateImagesFromData(projectid, memStream, new List<string>() { tag.Id.ToString() });

                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
