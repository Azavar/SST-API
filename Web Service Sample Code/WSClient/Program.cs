using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSClient.ServiceReference1;

namespace WSClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Run a test as a State
            State.Test();

            //Run a test Registration as a CSP
            //Registration.Test();
            

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

    }
}
