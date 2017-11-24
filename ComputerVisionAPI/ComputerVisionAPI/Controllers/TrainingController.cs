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
    public class TrainingController : ApiController
    {

        // POST: api/Training
        public void Post([FromBody]string value)
        {
            try
            {   
                    TrainingApiCredentials trainingCredentials = new TrainingApiCredentials("ea94a8906af14ac7a34df51ea4ba6750");
                    TrainingApi trainingApi = new TrainingApi(trainingCredentials);

                    Guid projectid = new Guid("57471653-6e79-455f-b874-ee00d1014c37");
                    ProjectModel model = trainingApi.GetProject(projectid);
                
                    Console.WriteLine("\tTraining");
                    var iteration = trainingApi.TrainProject(projectid);

                    // The returned iteration will be in progress, and can be queried periodically to see when it has completed
                    while (iteration.Status == "Training")
                    {
                        Thread.Sleep(1000);

                        // Re-query the iteration to get it's updated status
                        iteration = trainingApi.GetIteration(projectid, iteration.Id);
                    }

                    // The iteration is now trained. Make it the default project endpoint
                    iteration.IsDefault = true;
                    trainingApi.UpdateIteration(projectid, iteration.Id, iteration);
                    Console.WriteLine("Done!\n");
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error when training model! Exception: " + e.Message);
            }
        }
    }
}
