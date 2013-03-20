using System;
using System.Collections.Generic;
using System.Text;
using Butterfly.Core;
using HabboEncryption;
using HabboEvents;
namespace Butterfly.Messages
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
            Response.AppendString(ButterflyEnvironment.publicToken);
            Response.AppendBoolean(false);
            SendResponse();
        }

        internal void InitSecretKey()
        {
            /**
            string CipherPublickey = Request.PopFixedString();

            if (!ButterflyEnvironment.globalCrypto.InitializeRC4ToSession(Session, CipherPublickey))
            {
                Logging.WriteLine("Error");
                return;
            }
            **/
            Session.TimePingedReceived = DateTime.Now;
            Response.Init(Outgoing.SecretKeyComposer);
            Response.AppendString(ButterflyEnvironment.globalCrypto.PublicKey.ToString());
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
