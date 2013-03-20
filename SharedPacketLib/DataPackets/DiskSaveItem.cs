using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SharedPacketLib.DataPackets
{
    public class DiskSaveItem : AbstractOutgoingPacket
    {
        private string fileName;
        private int type;
        public DiskSaveItem(string fileName, short type = 0) : base(type)
        {
            this.fileName = fileName;
            this.type = type;
        }

        /// <summary>
        /// indication if the operation was succesfull
        /// </summary>
        /// <returns></returns>
        public bool SaveToDisk(bool deleteExisting = true)
        {
            try
            {
                bool fileExists = File.Exists(this.fileName);
                if (fileExists && !deleteExisting)
                {
                    return false;
                }
                else if (fileExists)
                {
                    File.Delete(this.fileName);
                }
                File.WriteAllBytes(this.fileName, base.GetPacketData());
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
