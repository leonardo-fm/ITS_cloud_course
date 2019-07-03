using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConnectionComputerVision
{
    /*  
     *  Pacchetti da installare (Microsoft.Azure.CognitiveServices.Vision.ComputerVision)
     *  Passare nel costruttore CHIAVE DEL COMPUTER VISION  e URL dell'immagine
     *  Per ottenere i tag usare il metodo GetTags()
     *  I Tags vengono restotuiti tramite un array di stringhe, vengono presi solo quelli con una cofidenza superiore allo 0.5
     *  Per cambiare url usare SetRemoteImageUrl()
     */

    class ComputerVisionConnection
    {
        // subscriptionKey = "0123456789abcdef0123456789ABCDEF"
        private string subscriptionKey = "";

        // Url of the image = "https://upload.wikimedia.org/wikipedia/commons/3/3c/Shaki_waterfall.jpg"
        private string remoteImageUrl = "";

        // Specify the features to return
        private static readonly List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Description,
            VisualFeatureTypes.Tags
        };

        private static string tagsResoult = "";

        public ComputerVisionConnection(string subscriptionKey, string remoteImageUrl){
            
            this.subscriptionKey = subscriptionKey;
            this.remoteImageUrl = remoteImageUrl;
        }

        public string[] GetTags()
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            // Specify the Azure region
            computerVision.Endpoint = "https://northeurope.api.cognitive.microsoft.com";

            try
            {
                var t1 = AnalyzeRemoteAsync(computerVision, remoteImageUrl);
                Task.WhenAll(t1).Wait(5000);

                string[] res = tagsResoult.Split(';');

                return res;
            }
            catch (Exception)
            {
                throw new ArgumentException("Wrong API parameters");
            }
        }

        public void SetRemoteImageUrl(string remoteImageUrl)
        {
            this.remoteImageUrl = remoteImageUrl;
        }

        // Analyze a remote image
        private static async Task AnalyzeRemoteAsync(ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                throw new ArgumentException("\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
            }

            ImageAnalysis analysis = await computerVision.AnalyzeImageAsync(imageUrl, features);
            tagsResoult = DisplayResults(analysis);
        }

        // Display the most relevant caption for the image
        private static string DisplayResults(ImageAnalysis analysis)
        {
            string resoult = "";

            if (analysis.Description.Captions.Count != 0){

                int numberOfTags = analysis.Tags.Count;
                for (int i = 0; i < numberOfTags; i++)
                {
                    if(analysis.Tags[i].Confidence > 0.5f)
                        resoult += analysis.Tags[i].Name + ";";
                }
            }

            return resoult;
        }
    }
}    
