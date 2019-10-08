using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConnectionComputerVision
{
    /*  
     *  Pacchetti da installare (Microsoft.Azure.CognitiveServices.Vision.ComputerVision)
     *  Passare nel costruttore CHIAVE DEL COMPUTER VISION  e URL locale dell'immagine
     *  Per ottenere i tag usare il metodo GetTags()
     *  I Tags vengono restotuiti tramite il resoult del task tramite un array di stringhe, vengono presi solo quelli con una cofidenza superiore allo 0.5 
     */

    class ComputerVisionConnection
    {
        // subscriptionKey = "0123456789abcdef0123456789ABCDEF"
        public static string subscriptionKey { get; set; }

        // localImagePath = @"C:\Documents\LocalImage.jpg"
        public static string localImagePath { get; set; }

        private static float _recognizeLevel = 0.5f;
        // level of confidentiality of the AI on the image recognize
        public static float recognizeLevel
        {
            get { return _recognizeLevel; }
            set { if (value > 1f || value < 0f)
                    throw new Exception("Parameter must be between 0.0 and 1.0");
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

        public static async Task<string[]> GetTags()
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            // Specify the Azure region
            computerVision.Endpoint = "https://northeurope.api.cognitive.microsoft.com";

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
