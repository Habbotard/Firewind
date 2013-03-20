using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anagenesis_Client.ConnectionClasses;
using ConnectionManager;
using SharedPacketLib.DataPackets;
using Helpers;

namespace SocketTest
{
    class Program
    {
        private static ClientConnectionManager ipcon;
        static void Main(string[] args)
        {
            Console.WindowHeight = Console.LargestWindowHeight - 20;
            Console.WindowWidth = Console.LargestWindowWidth - 20;
            Helpers.Out.startLogger(); //START LOGING!!
            ConnectionManager.SocketManager man = new ConnectionManager.SocketManager();
            man.init(9001, 1337, 10, new GamePacketParser(), true);
            man.connectionEvent += new ConnectionManager.SocketManager.ConnectionEvent(man_connectionEvent);
            man.initializeConnectionRequests();

            

            string s;
            ipcon = new ClientConnectionManager("127.0.0.1", 9001);
            ipcon.OnConnectionChange += new ClientConnectionManager.connectionChanged(ipcon_OnConnectionChange);
            
            while((s = Console.ReadLine()) != "exit")
            {
                switch (s)
                {
                    case "con":
                        {
                            ipcon.openConnection();
                            break;
                        }
                    case "p":
                        {
                            ipcon.processSyncedMessages();
                            break;
                        }
                }
            }
            ipcon.Dispose();
            man.destroy();
            Out.stopLogger();
        }

        static void ipcon_OnConnectionChange(ClientSide.ConnectionState state)
        {
            Console.WriteLine("New Client state: " + state);
            if (state == ClientSide.ConnectionState.open)
            {
                ipcon.registerPacket(ClientOpCode.Core_send_version, ClientReceivedVersion, true);
            }
        }

        static void ClientReceivedVersion(ClientIncomingPacket packet)
        {
            Console.WriteLine("Got version request packet from server!");
        }

        static void man_connectionEvent(ConnectionManager.ConnectionInformation connection)
        {
            //TODO something with your new connection
            
            new ConnectionHandeler(connection);
        }
    }
}
