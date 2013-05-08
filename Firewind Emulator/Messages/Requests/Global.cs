
using Firewind.Core;
using System;
namespace Firewind.Messages
{
    partial class GameClientMessageHandler
    {
        internal void Pong()
        {
            //Session.PongOK = true;
            Session.TimePingedReceived = DateTime.Now;
        }

        internal void EventLog()
        {
            string category = Request.ReadString();
            string type = Request.ReadString();
            string action = Request.ReadString();
            string extraString = Request.ReadString();
            int extraInt = Request.ReadInt32();

            //Logging.LogDebug(String.Format("Event log: Category {0}, type {1}, action {2}, extraString {3}, extraInt {4}", category, type, action, extraString, extraInt));
        }
        internal void PerformanceLog()
        {
            // int, string, string, string, string, int(bool), int, int, int, int, int
        }

        //internal void RegisterGlobal()
        //{
        //    RequestHandlers.Add(196, new RequestHandler(Pong));
        //}

        //internal void UnregisterGlobals()
        //{
        //    RequestHandlers.Remove(196);
        //}
    }
}
