using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Firewind.Messages.ClientMessages
{
    class ClientMessageFactory
    {
        private static Queue freeObjects;

        internal static void Init()
        {
            freeObjects = new Queue();
        }

        internal static ClientMessage GetClientMessage(int messageID, byte[] body)
        {
            ClientMessage message;

            lock (freeObjects.SyncRoot)
            {
                if (freeObjects.Count > 0)
                    message = (ClientMessage)freeObjects.Dequeue();
                else
                    return new ClientMessage(messageID, body);
            }
            if (message == null)
                return new ClientMessage(messageID, body);

            message.Init(messageID, body);
            return message;
        }

        internal static void ObjectCallback(ClientMessage message)
        {
            lock (freeObjects.SyncRoot)
            {
                freeObjects.Enqueue(message);
            }
        }
    }
}
