using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CrazySharpLib;
using CrazySharpLib.Radio;

namespace CrazySharpSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var dongle = CrazyFlieDongle.GetDongles().FirstOrDefault();

            var radio = new CrazyRadio<DefaultDispatcher>(dongle, new DefaultDispatcher());

            radio.Dispatcher.ConsoleEvent += (sender, eventArgs) => Console.Write(eventArgs.Data);

            radio.Start();
            
            CrazyLog log = new CrazyLog(radio, radio.Dispatcher);

            var toc = log.GetTableOfContent();

            foreach (var item in toc)
            {
                Console.WriteLine(item);
            }


            CrazyCommander commander = new CrazyCommander(radio);

            while (true)
            {
                commander.SendValues(ushort.MaxValue / 2, 0f, 0f, 0f);
                Thread.Sleep(200);
            }

            

        }
    }
}
