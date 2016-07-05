using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsat.SpecProc;
using System.IO;

namespace TestSpecProc
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = new FileStream(@"C:\Users\Victor\Documents\Visual Studio 2015\Projects\GroundAnalyser\WnData_20160201_163638_000_zyj2.dat", FileMode.Open);
            byte[] buf = new byte[36000000];
            fs.Read(buf, 0, 36000000);
            FileStream fs2 = new FileStream(@"C:\Users\Victor\Documents\Visual Studio 2015\Projects\GroundAnalyser\2.dat", FileMode.Create);
            fs2.Write(buf, 0, 36000000);
            fs2.Close();

        }
    }
}
