using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CrazySharpLib;

namespace CrazySharpSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var dongle = CrazyFlieDongle.GetDongles().FirstOrDefault();

            dongle.OpenDevice();

            
            while (true)
            {
                dongle.SendPacket(new CRTPCommand(0) {Thrust = ushort.MaxValue/2});
                Thread.Sleep(200);
            }

        }
    }
}
