using System;
namespace SharedPacketLib.DataPackets
{
    /// <summary>
    /// Specifies packet IDs that trigger an action in Brianwave
    /// </summary>
    public enum ServerOpCode
    {
        //Common block - 1 - 9
        Core_client_sends_version = 1,
        Ping_Response = 2,

    }
    /// <summary>
    /// Operation code keeper
    /// </summary>
    public static class ServerOpcodeCheck
    {
        /// <summary>
        /// Checks if an integer is an operation code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool isOpCode(int code)
        {
            return Enum.IsDefined(typeof(ServerOpCode), code);
        }
    }
}
