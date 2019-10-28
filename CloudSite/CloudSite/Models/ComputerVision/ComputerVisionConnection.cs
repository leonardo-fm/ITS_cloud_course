using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CloudSite.Models;

namespace CloudSite.Models.ComputerVision
{

    class ComputerVisionConnection
    {
        private static float _recognizeLevel = 0.5f;
        public static float recognizeLevel
        {
            get { return _recognizeLevel; }
            set { if (value > 1f || value < 0f)
                    throw new ArgumentException("Parameter must be between 0.0 and 1.0", "wrongParameter");
                else
                    _recognizeLevel = value;
            }
        }

        // Specify the features to return
        private static readonly List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Description,
            VisualFeatureTypes.Tags
        };

        private static string tagsResoult = "";

        public static async Task<string[]> getTagsFromLocalImage(string localImagePath)
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(Variables.SUBSCRIPTION_KEY_FOR_AI_VISION),
                new System.Net.Http.DelegatingHandler[] { });

            // Specify the Azure region
            computerVision.Endpoint = Variables.ENDPOINT_FOR_AI_VISION;

            try
            {
                var t1 = AnalyzeLocalAsync(computerVision, localImagePath);
                Task.WhenAll(t1).Wait(5000);

                string[] res = tagsResoult.Split(';');

                return res;
            }
            catch (Exception)
            {
                throw new ArgumentException("Wrong API parameters");
            }
        }

        // Analyze a local image
        private static async Task AnalyzeLocalAsync(
            ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine(
                    "\nUnable to open or read localImagePath:\n{0} \n", imagePath);
                return;
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream, features);
                tagsResoult = DisplayResults(analysis);
            }
        }

        // Display the most relevant caption for the image
        private static string DisplayResults(ImageAnalysis analysis)
        {
            string resoult = "";

            if (analysis.Description.Captions.Count != 0){

                int numberOfTags = analysis.Tags.Count;
                for (int i = 0; i < numberOfTags; i++)
                {
                    if(analysis.Tags[i].Confidence > recognizeLevel)
                        resoult += analysis.Tags[i].Name + ";";
                }
            }

            return resoult;
        }
    }
}    
