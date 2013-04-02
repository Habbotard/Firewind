using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectionManager;
using Firewind;
using Firewind.Net;
using System.Collections;

namespace Firewind.Core
{
    public class ConnectionHandling
    {
        private SocketManager manager;
        private Hashtable liveConnections;

        public ConnectionHandling(int port, int maxConnections, int connectionsPerIP, bool enabeNagles)
        {
            liveConnections = new Hashtable();
            manager = new SocketManager();
            manager.init(port, maxConnections, connectionsPerIP, new InitialPacketParser(), !enabeNagles);
        }

        internal void init()
        {
            manager.connectionEvent += new SocketManager.ConnectionEvent(manager_connectionEvent);
        }

        private void manager_connectionEvent(ConnectionInformation connection)
        {
            liveConnections.Add(connection.getConnectionID(), connection);
            connection.connectionChanged += connectionChanged;
            FirewindEnvironment.GetGame().GetClientManager().CreateAndStartClient((uint)connection.getConnectionID(), connection);
            
        }

        private void connectionChanged(ConnectionInformation information, ConnectionState state)
        {
            if (state == ConnectionState.closed)
            {
                CloseConnection(information.getConnectionID());
                liveConnections.Remove(information.getConnectionID());
            }
        }

        internal void Start()
        {
            manager.initializeConnectionRequests();
        }

        internal void CloseConnection(int p)
        {
            try
            {
                Object info = liveConnections[p];
                if (info != null)
                {
                    ConnectionInformation con = (ConnectionInformation)info;
                    con.Dispose();
                    FirewindEnvironment.GetGame().GetClientManager().DisposeConnection((uint)p);
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
                //Logging.WriteLine(e.ToString());
            }
        }

        internal void Destroy()
        {
            manager.destroy();   
        }
    }
}
