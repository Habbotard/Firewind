using System;
using System.Collections.Generic;
using System.Text;
using Firewind.Core;
using HabboEvents;
namespace Firewind.Messages
{
    partial class GameClientMessageHandler
    {
        internal void CheckRelease()
        {
            // Logging.WriteLine("Hallo, we're on " + Request.PopFixedString());
            Session.bannertimmer = DateTime.Now;
        }

        internal void InitCrypto()
        {
            Session.TimePingedReceived = DateTime.Now;
            Response.Init(Outgoing.SendBannerMessageComposer);
            Response.AppendString("nocryptonotoken");
            Response.AppendBoolean(false);
            SendResponse();
        }

        internal void InitSecretKey()
        {
            Session.TimePingedReceived = DateTime.Now;
            Response.Init(Outgoing.SecretKeyComposer);
            Response.AppendString("nocryptonokey");
            SendResponse();
        }

        internal void setClientVars()
        {
            string swfs = Request.PopFixedString();
            string vars = Request.PopFixedString();
        }

        internal void setUniqueIDToClient()
        {
            string Id = Request.PopFixedString();
            Session.MachineId = Id;
        }

        internal void SSOLogin()
        {
            if (Session.GetHabbo() == null)
            {
                TimeSpan result = DateTime.Now - Session.bannertimmer;
                //Logging.WriteLine("banner parsed on " + result.Milliseconds + "ms");
                Session.tryLogin(Request.PopFixedString());
                Session.TimePingedReceived = DateTime.Now;
                //if (Session.tryLogin(Request.PopFixedString()))
                //{
                //    //RegisterCatalog();
                //    //RegisterHelp();
                //    //RegisterNavigator();
                //    //RegisterMessenger();
                //    //RegisterUsers();
                //    //RegisterRooms();
                //    //RegisterGroups();
                //    //RegisterQuests();
                //}
            }
            else
                Session.SendNotif(LanguageLocale.GetValue("user.allreadylogedon"));
        }

        //internal void RegisterHandshake()
        //{
        //    RequestHandlers.Add(206, new RequestHandler(SendSessionParams));
        //    RequestHandlers.Add(415, new RequestHandler(SSOLogin));
        //}

        //internal void UnRegisterHandshake()
        //{
        //    RequestHandlers.Remove(206);
        //    RequestHandlers.Remove(415);
        //}
    }
}
