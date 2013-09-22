using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CrazySharpLib.Radio;
using CrazySharpLib.Radio.Packets;

namespace CrazySharpLib
{
    public class CrazyCommander
    {
        private readonly ICrazyRadio _radio;
        private readonly CRTPCommand _cmd; 


        public CrazyCommander(ICrazyRadio radio)
        {
            _radio = radio;
            _cmd = new CRTPCommand();
        }


        public void SendValues(UInt16 thrust, float roll, float pitch, float yaw)
        {
            _cmd.Thrust = thrust;
            _cmd.Roll = roll;
            _cmd.Pitch = pitch;
            _cmd.Yaw = yaw;

            _radio.EnqueueSend(_cmd);
        }

    }
}
