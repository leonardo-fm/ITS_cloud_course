using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnessioneBlobStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionBS cbs = new ConnectionBS();
            cbs.connectionToBlobStorage();
            Console.ReadKey();
        }
    }
}
