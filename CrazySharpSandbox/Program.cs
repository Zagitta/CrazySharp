using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrazySharpLib;

namespace CrazySharpSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var dongle = CrazyFlieDongle.GetDongles().FirstOrDefault();

            dongle.OpenDevice();
            

            var scanChannels = dongle.ScanChannels();
        }
    }
}
