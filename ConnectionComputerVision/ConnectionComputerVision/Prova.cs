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
            ComputerVisionConnection cvc = new ComputerVisionConnection("cb1109b4c9f54f36a4d41c62b760e555", @"C:\Users\l.ferrero\Desktop\Cazzi miei\City.jpg");
            string[] resoult = cvc.GetTags().Result;

            foreach (string tag in resoult)
            {
                Console.WriteLine(tag);
            }

            Console.ReadLine();
        }
    }
}
