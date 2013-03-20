using System.Collections.Generic;
using System;

namespace Butterfly_Watchdog.ServerManager
{
    static class SessionManagement
    {
        private static List<Session> mSessions;

        internal static void Init()
        {
            mSessions = new List<Session>();
        }

        internal static void RegisterSession(Session pSession)
        {
            if (!mSessions.Contains(pSession))
                mSessions.Add(pSession);
        }

        internal static void RemoveSession(Session pSession)
        {
            mSessions.Remove(pSession);
        }

        internal static void IncreaseError()
        {
            foreach (Session Session in mSessions)
                Session.DisconnectionError++;
        }

        internal static void IncreaseDisconnection()
        {
            foreach (Session Session in mSessions)
                Session.Disconnection++;
        }

        internal static void BroadcastExceptionNotification(ExceptionType type, int token)
        {
            switch (type)
            {
                case ExceptionType.StandardException:
                    {
                        BroadcastMessage("exception:id=1," + token);
                        break;
                    }

                case ExceptionType.FatalException:
                    {
                        BroadcastMessage("exception:id=2," + token);
                        break;
                    }

                case ExceptionType.SQLException:
                    {
                        BroadcastMessage("exception:id=3," + token);
                        break;
                    }

                case ExceptionType.ThreadedException:
                    {
                        BroadcastMessage("exception:id=4," + token);
                        break;
                    }

                case ExceptionType.UserException:
                    {
                        BroadcastMessage("exception:id=5," + token);
                        break;
                    }

                case ExceptionType.DDOSException:
                    {
                        BroadcastMessage("exception:id=6," + token);
                        break;
                    }
            }
        }

        private static void BroadcastMessage(string message)
        {
            foreach (Session session in mSessions)
            {
                session.SendMessage(message);
            }
        }
    }
}
