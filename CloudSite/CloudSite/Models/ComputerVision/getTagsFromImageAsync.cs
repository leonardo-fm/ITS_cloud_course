using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;

//https://docs.microsoft.com/it-it/azure/cognitive-services/computer-vision/quickstarts/csharp-analyze

namespace CloudSite.Models.ComputerVision
{

    class ComputerVisionConnection
    {
        private static float _recognizeLevel;
        private static float recognizeLevelPublic
        {
            get { return _recognizeLevel; }
            set { if (value > 1f || value < 0f)
                    throw new ArgumentException("Parameter must be between 0.0 and 1.0", "wrongParameter");
                else
                    _recognizeLevel = value;
            }
        }

        // Specify the features to return  
        private static readonly List<VisualFeatureTypes> features =
            new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Description,
            VisualFeatureTypes.Tags
        };

        public static string[] uploadImageAndHandleTagsResoult(Stream photo, float recognizeLevel = 0.5f)
        {
            recognizeLevelPublic = recognizeLevel;

            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(Variables.SUBSCRIPTION_KEY_FOR_AI_VISION),
                new System.Net.Http.DelegatingHandler[] { });

            computerVision.Endpoint = Variables.ENDPOINT_FOR_AI_VISION;

            Task<string[]> t1 = AnalyzeLocalAsync(computerVision, photo);
            t1.Wait();

            return t1.Result;
        }

        // Analyze a local image  
        private static async Task<string[]> AnalyzeLocalAsync(ComputerVisionClient computerVision, Stream photo)
        {
            ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(photo, features);
            return DisplayResults(analysis);
        }

        // Display the most relevant caption for the image  
        private static string[] DisplayResults(ImageAnalysis analysis)
        {
            List<string> resoult = new List<string>(); ;

            if (analysis.Description.Captions.Count != 0)
            {

                int numberOfTags = analysis.Tags.Count;
                for (int i = 0; i < numberOfTags; i++)
                {
                    if (analysis.Tags[i].Confidence > _recognizeLevel)
                        resoult.Add(analysis.Tags[i].Name);
                }
            }

            return resoult.ToArray();
        }
    }
}    
