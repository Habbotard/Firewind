using System;
namespace SharedPacketLib.DataPackets
{
    /// <summary>
    /// Specifies packet IDs that trigger an action in the Brainwave Online game client.
    /// </summary>
    public enum ClientOpCode : int
    {
        // Common (block 1-9)
        Core_send_version = 1,
        Core_receive_version_ok = 2,
        Core_receive_version_failed = 3,
        GenericError = 4,
    }
    /// <summary>
    /// Operation code keeper
    /// </summary>
    public static class ClientOpcodeCheck
    {
        /// <summary>
        /// Checks if an integer is an operation code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool isOpCode(int code)
        {
            return Enum.IsDefined(typeof(ClientOpCode), code);
        }
    }
}