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
            ComputerVisionConnection cvc = new ComputerVisionConnection("ChiaveCV", @"PathImmagineLocale");
            string[] resoult = cvc.GetTags();

            foreach (string tag in resoult)
            {
                Console.WriteLine(tag);
            }

            Console.ReadLine();
        }
    }
}
