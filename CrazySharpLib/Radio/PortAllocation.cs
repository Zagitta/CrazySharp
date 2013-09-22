namespace CrazySharpLib.Radio
{
    /// <summary>
    /// The port used to describe the target on the crazyflie
    /// </summary>
    public enum PortAllocation : byte
    {
        /// <summary>
        /// Read console text that is printed to the console on the Crazyflie using consoleprintf 
        /// </summary>
        Console = 0x0,
        /// <summary>
        /// Get/set parameters from the Crazyflie. Parameters are defined using a macro in the Crazyflie source-code
        /// </summary>
        Parameters = 0x2,
        /// <summary>
        /// Sending control set-points for the roll/pitch/yaw/thrust regulators
        /// </summary>
        Commander = 0x3,
        /// <summary>
        /// Set up log blocks with variables that will be sent back to the Crazyflie at a specified period. Log variables are defined using a macro in the Crazyflie source-code
        /// </summary>
        Log = 0x5,
        /// <summary>
        /// Used to control and query the communication link
        /// </summary>
        LinkLayer = 0x15
    }
}