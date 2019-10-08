using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionComputerVision
{
    class Prova
    {
        static public void Main(String[] args)
        {
            ComputerVisionConnection.subscriptionKey = "subscriptionKey";
            ComputerVisionConnection.localImagePath = @"localImagePath";
            ComputerVisionConnection.recognizeLevel = 0.5f;

            string[] resoult = ComputerVisionConnection.GetTags().Result;

            foreach (string tag in resoult)
            {
                Console.WriteLine(tag);
            }

            Console.ReadLine();
        }
    }
}
