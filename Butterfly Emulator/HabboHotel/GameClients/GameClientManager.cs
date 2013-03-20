﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Butterfly.Core;
using Butterfly.HabboHotel.Misc;
using Butterfly.Messages;
using ConnectionManager;
using Database_Manager.Database.Session_Details.Interfaces;
using System.Threading;
using Butterfly.HabboHotel.Users.Messenger;
using System.Threading.Tasks;
using HabboEncryption;
using HabboEvents;
using Butterfly.Messages.ClientMessages;
namespace Butterfly.HabboHotel.GameClients
{
    class GameClientManager
    {
        #region Fields
        private Dictionary<uint, GameClient> clients;
        
        private Queue clientsAddQueue;
        private Queue clientsToRemove;
        private Queue creditQueuee;
        private Queue pixelQueuee;
        private Queue badgeQueue;
        private Queue authorizedPacketSending;
        private Queue broadcastQueue;

        private Hashtable usernameRegister;
        private Hashtable userIDRegister;

        private Hashtable usernameIdRegister;
        private Hashtable idUsernameRegister;

        private int pingInterval;

        private bool cycleCreditsEnabled;
        private int cycleCreditsAmount;
        private int cycleCreditsTime;
        private DateTime cycleCreditsLastUpdate;

        private bool cyclePixelsEnabled;
        private int cyclePixelsAmount;
        private int cyclePixelsTime;
        private DateTime cyclePixelsLastUpdate;

        internal int creditsOnLogin;
        internal int pixelsOnLogin;
        internal bool FastRun = false;

        private Task FirstThread_Task;
        private Task SecondThread_Task;
        private Task ThirdThread_Task;



        // MultiTasking RC4
        internal static Queue<PacketQueue.paqueteEnCola> RC4ToDecrypt_1 = new Queue<PacketQueue.paqueteEnCola>();
        internal static Queue<PacketQueue.paqueteEnCola> RC4ToDecrypt_2 = new Queue<PacketQueue.paqueteEnCola>();
        internal static Queue<PacketQueue.paqueteEnCola> RC4ToDecrypt_3 = new Queue<PacketQueue.paqueteEnCola>();
        internal static Queue<PacketQueue.paqueteEnCola> RC4ToDecrypt_4 = new Queue<PacketQueue.paqueteEnCola>();
        internal static Queue<PacketQueue.paqueteEnCola> RC4ToDecrypt_5 = new Queue<PacketQueue.paqueteEnCola>();
        internal static Queue<PacketQueue.paqueteEnCola> DataToProcess_1 = new Queue<PacketQueue.paqueteEnCola>();
        #endregion      

        #region Return values
        internal Queue<PacketQueue.paqueteEnCola> RC4Queue1
        {
            get
            {
                return RC4ToDecrypt_1;
            }
        }

        internal Queue<PacketQueue.paqueteEnCola> RC4Queue2
        {
            get
            {
                return RC4ToDecrypt_2;
            }
        }

        internal Queue<PacketQueue.paqueteEnCola> RC4Queue3
        {
            get
            {
                return RC4ToDecrypt_3;
            }
        }

        internal Queue<PacketQueue.paqueteEnCola> RC4Queue4
        {
            get
            {
                return RC4ToDecrypt_4;
            }
        }

        internal Queue<PacketQueue.paqueteEnCola> RC4Queue5
        {
            get
            {
                return RC4ToDecrypt_5;
            }
        }

        internal Queue<PacketQueue.paqueteEnCola> DataQueue1
        {
            get
            {
                return DataToProcess_1;
            }
        }
        internal int connectionCount
        {
            get
            {
                return clients.Count;
            }
        }

        internal GameClient GetClientByUserID(uint userID)
        {
            if (userIDRegister.ContainsKey(userID))
                return (GameClient)userIDRegister[userID];
            return null;
        }

        internal GameClient GetClientByUsername(string username)
        {
            if (usernameRegister.ContainsKey(username.ToLower()))
                return (GameClient)usernameRegister[username.ToLower()];
            return null;
        }

        internal GameClient GetClient(uint clientID)
        {
            if (clients.ContainsKey(clientID))
                return (GameClient)clients[clientID];
            return null;
        }

        internal int ClientCount
        {
            get
            {
                return clients.Count;
            }
        }

        internal string GetNameById(uint Id)
        {
            GameClient client = GetClientByUserID(Id);

            if (client != null)
                return client.GetHabbo().Username;

            string username;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT username FROM users WHERE id = " + Id);
                username = dbClient.getString();
            }

            return username;
        }

        internal IEnumerable<GameClient> GetClientsById(Dictionary<uint, MessengerBuddy>.KeyCollection users)
        {
            foreach (uint id in users)
            {
                GameClient client = GetClientByUserID(id);
                if (client != null)
                    yield return client;
            }
        }

        #endregion

        #region Constructor
        internal GameClientManager()
        {
            clients = new Dictionary<uint, GameClient>();
            clientsAddQueue = new Queue();
            clientsToRemove = new Queue();
            creditQueuee = new Queue();
            pixelQueuee = new Queue();
            badgeQueue = new Queue();
            broadcastQueue = new Queue();
            authorizedPacketSending = new Queue();
            timedOutConnections = new Queue();
            usernameRegister = new Hashtable();
            userIDRegister = new Hashtable();

            usernameIdRegister = new Hashtable();
            idUsernameRegister = new Hashtable();

            Thread timeOutThread = new Thread(new ThreadStart(HandleTimeouts));
            timeOutThread.Start();

            pingInterval = int.Parse(ButterflyEnvironment.GetConfig().data["client.ping.interval"]);

            if (ButterflyEnvironment.GetConfig().data.ContainsKey("game.pixel.enabled"))
            {
                cyclePixelsEnabled = (ButterflyEnvironment.GetConfig().data["game.pixel.enabled"] == "true");
                cyclePixelsAmount = int.Parse(ButterflyEnvironment.GetConfig().data["game.pixel.amount"]);
                cyclePixelsTime = int.Parse(ButterflyEnvironment.GetConfig().data["game.pixel.time"]) * 1000;
                pixelsOnLogin = int.Parse(ButterflyEnvironment.GetConfig().data["game.login.pixel.receiveamount"]);
            }
            else
            {
                cyclePixelsEnabled = false;
                cyclePixelsAmount = 0;
                cyclePixelsTime = 0;
                pixelsOnLogin = 0;
            }
            
            if (ButterflyEnvironment.GetConfig().data.ContainsKey("game.credits.enabled"))
            {
                cycleCreditsEnabled = (ButterflyEnvironment.GetConfig().data["game.credits.enabled"] == "true");
                cycleCreditsAmount = int.Parse(ButterflyEnvironment.GetConfig().data["game.credits.amount"]);
                cycleCreditsTime = int.Parse(ButterflyEnvironment.GetConfig().data["game.credits.time"]) * 1000;
                creditsOnLogin = int.Parse(ButterflyEnvironment.GetConfig().data["game.login.credits.receiveamount"]);
            }
            else
            {
                cycleCreditsEnabled = false;
                cycleCreditsAmount = 0;
                cycleCreditsTime = 0;
                creditsOnLogin = 0;
            }
        }
        #endregion

        #region Threads procesado de paquetes
        internal static void WorkerRC4ToDecrypt_1()
        {
            do
            {
                try
                {
                    if (RC4ToDecrypt_1.Count > 0)
                    {
                        Queue<PacketQueue.paqueteEnCola> RC4ToDecrypt_1Current = new Queue<PacketQueue.paqueteEnCola>(RC4ToDecrypt_1.ToArray());
                        //lock (RC4ToDecrypt_1)
                            RC4ToDecrypt_1.Clear();

                        PacketQueue.paqueteEnCola currentRC4 = new PacketQueue.paqueteEnCola();
                        do
                        {
                            try
                            {
                                currentRC4 = RC4ToDecrypt_1Current.Dequeue();
                                GameClient cliente = currentRC4.GameClient;
                                if (cliente.GetConnection() == null)
                                    continue;
                                currentRC4.datos = RC4.Decipher(ref cliente.table, ref cliente.i, ref cliente.j, currentRC4.datos);
                                cliente.listaTareas.StartNew(
                                    () => cliente.packetParser.handlePacketDataDecrypted(currentRC4.datos)
                                    );
                            }
                            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecryptPacket Exception"); }
                        } while (RC4ToDecrypt_1Current.Count > 0);

                    }
                } catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecrypt_1 Exception"); }
                Thread.Sleep(50);
            } while (true);
        }


        internal static void WorkerRC4ToDecrypt_2()
        {
            do
            {
                try
                {
                    if (RC4ToDecrypt_2.Count > 0)
                    {
                        Queue<PacketQueue.paqueteEnCola> RC4ToDecrypt_2Current = new Queue<PacketQueue.paqueteEnCola>(RC4ToDecrypt_2.ToArray());
                        //lock (RC4ToDecrypt_2)
                            RC4ToDecrypt_2.Clear();

                        PacketQueue.paqueteEnCola currentRC4 = new PacketQueue.paqueteEnCola();
                        do
                        {
                            try
                            {
                                currentRC4 = RC4ToDecrypt_2Current.Dequeue();
                                GameClient cliente = currentRC4.GameClient;
                                if (cliente.GetConnection() == null)
                                    continue;
                                currentRC4.datos = RC4.Decipher(ref cliente.table, ref cliente.i, ref cliente.j, currentRC4.datos);
                                cliente.listaTareas.StartNew(
                                    () => cliente.packetParser.handlePacketDataDecrypted(currentRC4.datos)
                                    );
                            }
                            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecryptPacket Exception"); }
                        } while (RC4ToDecrypt_2Current.Count > 0);
                    }
                }
                catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecrypt_2 Exception"); }
                Thread.Sleep(20);
            } while (true);
        }


        internal static void WorkerRC4ToDecrypt_3()
        {
            do
            {
                try
                {
                    if (RC4ToDecrypt_3.Count > 0)
                    {
                        Queue<PacketQueue.paqueteEnCola> RC4ToDecrypt_3Current = new Queue<PacketQueue.paqueteEnCola>(RC4ToDecrypt_3.ToArray());
                        //lock (RC4ToDecrypt_3)
                            RC4ToDecrypt_3.Clear();

                        PacketQueue.paqueteEnCola currentRC4 = new PacketQueue.paqueteEnCola();
                        do
                        {
                            try
                            {
                                currentRC4 = RC4ToDecrypt_3Current.Dequeue();
                                GameClient cliente = currentRC4.GameClient;
                                if (cliente.GetConnection() == null)
                                    continue;
                                currentRC4.datos = RC4.Decipher(ref cliente.table, ref cliente.i, ref cliente.j, currentRC4.datos);
                                cliente.listaTareas.StartNew(
                                    () => cliente.packetParser.handlePacketDataDecrypted(currentRC4.datos)
                                    );
                            }
                            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecryptPacket Exception"); }
                        } while (RC4ToDecrypt_3Current.Count > 0);
                    }
                }
                catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecrypt_3 Exception"); }
                Thread.Sleep(20);
            } while (true);
        }

        internal static void WorkerRC4ToDecrypt_4()
        {
            do
            {
                try
                {
                    if (RC4ToDecrypt_4.Count > 0)
                    {
                        Queue<PacketQueue.paqueteEnCola> RC4ToDecrypt_4Current = new Queue<PacketQueue.paqueteEnCola>(RC4ToDecrypt_4.ToArray());
                        //lock (RC4ToDecrypt_4)
                        RC4ToDecrypt_4.Clear();

                        PacketQueue.paqueteEnCola currentRC4 = new PacketQueue.paqueteEnCola();
                        do
                        {
                            try
                            {
                                currentRC4 = RC4ToDecrypt_4Current.Dequeue();
                                GameClient cliente = currentRC4.GameClient;
                                if (cliente.GetConnection() == null)
                                    continue;
                                currentRC4.datos = RC4.Decipher(ref cliente.table, ref cliente.i, ref cliente.j, currentRC4.datos);
                                cliente.listaTareas.StartNew(
                                    () => cliente.packetParser.handlePacketDataDecrypted(currentRC4.datos)
                                    );
                            }
                            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecryptPacket Exception"); }
                        } while (RC4ToDecrypt_4Current.Count > 0);
                    }
                }
                catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecrypt_4 Exception"); }
                Thread.Sleep(20);
            } while (true);
        }

        internal static void WorkerRC4ToDecrypt_5()
        {
            do
            {
                try
                {
                    if (RC4ToDecrypt_5.Count > 0)
                    {
                        Queue<PacketQueue.paqueteEnCola> RC4ToDecrypt_5Current = new Queue<PacketQueue.paqueteEnCola>(RC4ToDecrypt_5.ToArray());
                        //lock (RC4ToDecrypt_5)
                        RC4ToDecrypt_5.Clear();

                        PacketQueue.paqueteEnCola currentRC4 = new PacketQueue.paqueteEnCola();
                        do
                        {
                            try
                            {
                                currentRC4 = RC4ToDecrypt_5Current.Dequeue();
                                GameClient cliente = currentRC4.GameClient;
                                if (cliente.GetConnection() == null)
                                    continue;
                                currentRC4.datos = RC4.Decipher(ref cliente.table, ref cliente.i, ref cliente.j, currentRC4.datos);
                                cliente.packetParser.handlePacketDataDecrypted(currentRC4.datos);
                            }
                            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecryptPacket Exception"); }
                        } while (RC4ToDecrypt_5Current.Count > 0);
                    }
                }
                catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecrypt_5 Exception"); }
                Thread.Sleep(20);
            } while (true);
        }
        /***

        internal static void WorkerDataToProcess_1()
        {
            do
            {
                try
                {
                    if (DataToProcess_1.Count > 0)
                    {
                        Queue<PacketQueue.paqueteEnCola> DataToProcess_1Current = new Queue<PacketQueue.paqueteEnCola>(DataToProcess_1.ToArray());
                        lock (DataToProcess_1)
                            DataToProcess_1.Clear();

                        PacketQueue.paqueteEnCola currentDATA = new PacketQueue.paqueteEnCola();
                        do
                        {
                            try
                            {
                                currentDATA = DataToProcess_1Current.Dequeue();
                                var task = Task.Factory.StartNew(
                                    () => currentDATA.cliente.packetParser.handlePacketDataDecrypted(currentDATA.datos));
                            }
                            catch (Exception e)
                            {

                            }
                        } while (DataToProcess_1Current.Count > 0);

                    }
                }
                catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.WorkerRC4ToDecrypt_1 Exception"); }
                Thread.Sleep(20);
            } while (true);
        }
        ***/

        #endregion
        #region Threads procesado principal
        private void FirstThread_Execution()
        {
            try
            {
                GiveCredits(); //Give credit
                GivePixels();
                GiveBadges(); //Give badge
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.FirstThread_Execution Exception"); }
        }
        private void SecondThread_Execution()
        {
            try
            {
                CheckCycleUpdates(); // Pixel lolz
                TestClientConnections();
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.SecondThread_Execution Exception"); }
        }

        private void ThirdThread_Execution()
        {
            try
            {
                BroadcastPacketsWithRankRequirement(); //On disconnect 
                BroadcastPackets(); //On disconnect
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.ThirdThread_Execution Exception"); }
        }

        internal void OnCycle()
        {
            try
            {
                DateTime startTaskTime;
                TimeSpan spentTime;
                startTaskTime = DateTime.Now;

                if (ButterflyEnvironment.SeparatedTasksInGameClientManager != true)
                {
                    CheckCycleUpdates(); // Pixel lolz
                    TestClientConnections(); // User DC <== Causes lagg
                    RemoveClients();
                    AddClients();
                    GiveCredits(); //Give credit
                    GivePixels(); //Give credit
                    GiveBadges(); //Give badge
                    BroadcastPacketsWithRankRequirement(); //On disconnect 
                    BroadcastPackets(); //On disconnect
                    ButterflyEnvironment.GetGame().ClientManagerCycle_ended = true;
                }
                else
                {
                    if (FirstThread_Task == null || FirstThread_Task.IsCompleted == true)
                    {
                        FirstThread_Task = new Task(FirstThread_Execution);
                        FirstThread_Task.Start();
                    }

                    if (SecondThread_Task == null || SecondThread_Task.IsCompleted == true)
                    {
                        SecondThread_Task = new Task(SecondThread_Execution);
                        SecondThread_Task.Start();
                    }

                    if (ThirdThread_Task == null || ThirdThread_Task.IsCompleted == true)
                    {
                        ThirdThread_Task = new Task(ThirdThread_Execution);
                        ThirdThread_Task.Start();
                    }

                    do
                    {
                    } while (FirstThread_Task.IsCompleted == false || SecondThread_Task.IsCompleted == false || ThirdThread_Task.IsCompleted == false);

                    RemoveClients();
                    AddClients();
                    ButterflyEnvironment.GetGame().ClientManagerCycle_ended = true;
                }

                spentTime = DateTime.Now - startTaskTime;
                if (spentTime.TotalSeconds > 3)
                {
                    Logging.WriteLine("GameClientManager.OnCycle spent: " + spentTime.TotalSeconds + " seconds in working.");
                }
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.OnCycle Exception --> Not inclusive"); }
        }

        private static DateTime pixelLastExecution;
        private void CheckCycleUpdates()
        {
            try
            {
                DateTime startTaskTime;
                TimeSpan spentTime;
                startTaskTime = DateTime.Now;
                if (cyclePixelsEnabled)
                {
                    TimeSpan sinceLastTime = DateTime.Now - cyclePixelsLastUpdate;

                    if (sinceLastTime.TotalMilliseconds >= cyclePixelsTime)
                    {
                        cyclePixelsLastUpdate = DateTime.Now;
                        try
                        {
                            foreach (GameClient client in clients.Values)
                            {
                                if (client.GetHabbo() == null)
                                    continue;

                                PixelManager.GivePixels(client, cyclePixelsAmount);
                            }
                        }

                        catch (Exception e)
                        {
                            Logging.LogThreadException(e.ToString(), "GCMExt.cyclePixelsEnabled task");
                        }
                    }
                }

                if (cycleCreditsEnabled)
                {
                    TimeSpan sinceLastTime = DateTime.Now - cycleCreditsLastUpdate;

                    if (sinceLastTime.TotalMilliseconds >= cycleCreditsTime)
                    {
                        cycleCreditsLastUpdate = DateTime.Now;
                        try
                        {
                            foreach (GameClient client in clients.Values)
                            {
                                if (client.GetHabbo() == null)
                                    continue;

                                client.GetHabbo().Credits += cycleCreditsAmount;
                                client.GetHabbo().UpdateCreditsBalance();
                            }
                        }

                        catch (Exception e)
                        {
                            Logging.LogThreadException(e.ToString(), "GCMExt.cycleCreditsEnabled task");
                        }
                    }
                }

                CheckEffects();
                spentTime = DateTime.Now - startTaskTime;

                if (spentTime.TotalSeconds > 3)
                {
                    Logging.WriteLine("GameClientManager.CheckCycleUpdates spent: " + spentTime.TotalSeconds + " seconds in working.");
                }
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.CheckCycleUpdates Exception --> Not inclusive"); }
        }

        private Queue timedOutConnections;

        private static DateTime pingLastExecution;




        private void TestClientConnections()
        {
            try
            {
                DateTime startTaskTime;
                TimeSpan spentTime;
                startTaskTime = DateTime.Now;
                TimeSpan sinceLastTime = DateTime.Now - pingLastExecution;

                if (sinceLastTime.TotalMilliseconds >= pingInterval)
                {
                    try
                    {
                        ServerMessage PingMessage = new ServerMessage(Outgoing.Ping);

                        List<GameClient> ToPing = new List<GameClient>();
                        //List<GameClient> ToDisconnect = new List<GameClient>();


                        TimeSpan noise;
                        TimeSpan sinceLastPing;

                        foreach (GameClient client in clients.Values)
                        {
                            noise = DateTime.Now - pingLastExecution.AddMilliseconds(pingInterval); //For finding out if there is any lagg
                            sinceLastPing = DateTime.Now - client.TimePingedReceived;

                            if ((sinceLastPing.TotalMilliseconds - noise.TotalMilliseconds < pingInterval + 10000))
                            {
                                ToPing.Add(client);
                            }
                            else
                            {
                                    lock (timedOutConnections.SyncRoot)
                                    {
                                        timedOutConnections.Enqueue(client);
                                    }
                                //ToDisconnect.Add(client);
                                // pa ver los timeouts por si sale alguno mas por algun motivo
                                Logging.WriteLine(client.ConnectionID + " => Connection timed out");
                            }
                        }
                        DateTime start = DateTime.Now;

                        
                        byte[] PingMessageBytes = PingMessage.GetBytes();
                        foreach (GameClient Client in ToPing)
                        {
                            try
                            {
                                Client.GetConnection().SendUnsafeData(PingMessageBytes);
                            }
                            catch
                            {
                                //ToDisconnect.Add(Client);
                                lock (timedOutConnections.SyncRoot)
                                {
                                    timedOutConnections.Enqueue(Client);
                                }
                            }
                        }
                        

                        TimeSpan spent = DateTime.Now - start;
                        if (spent.TotalSeconds > 3)
                        {
                            Logging.WriteLine("Spent seconds on testing: " + (int)spent.TotalSeconds);
                        }


                        //start = DateTime.Now;
                        //foreach (GameClient client in ToDisconnect)
                        //{
                        //    try
                        //    {
                        //        client.Disconnect();
                        //    }
                        //    catch { }
                        //}
                        //spent = DateTime.Now - start;
                        if (spent.TotalSeconds > 3)
                        {
                            Logging.WriteLine("Spent seconds on disconnecting: " + (int)spent.TotalSeconds);
                        }

                        //ToDisconnect.Clear();
                        //ToDisconnect = null;
                        //ToPing.Clear();
                        //ToPing = null;


                    }
                    catch (Exception e) { Logging.LogThreadException(e.ToString(), "Connection checker task"); }
                    pingLastExecution = DateTime.Now;
                }
                spentTime = DateTime.Now - startTaskTime;

                if (spentTime.TotalSeconds > 3)
                {
                    Logging.WriteLine("GameClientManager.TestClientConnections spent: " + spentTime.TotalSeconds + " seconds in working.");
                }
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.TestClientConnections Exception --> Not inclusive"); }
        }

        private void HandleTimeouts()
        {
            while (true)
            {
                try
                {
                    while (timedOutConnections.Count > 0)
                    {
                        GameClient client = null;
                        lock (timedOutConnections.SyncRoot)
                        {
                            if (timedOutConnections.Count > 0)
                                client = (GameClient)timedOutConnections.Dequeue();
                        }

                        if (client != null)
                        {
                            client.Disconnect();
                        }
                    }
                }
                catch (Exception e)
                {
                    Logging.LogThreadException(e.ToString(), "HandleTimeouts");
                }

                Thread.Sleep(5000);
            }
        }        
        #endregion

        #region Collection modyfying
        private void AddClients()
        {
            DateTime startTaskTime;
            TimeSpan spentTime;
            startTaskTime = DateTime.Now;

            if (clientsAddQueue.Count > 0)
            {
                lock (clientsAddQueue.SyncRoot)
                {
                    while (clientsAddQueue.Count > 0)
                    {
                        GameClient client = (GameClient)clientsAddQueue.Dequeue();
                        clients.Add(client.ConnectionID, client);
                        client.StartConnection();
                    }
                }
            }
            spentTime = DateTime.Now - startTaskTime;
            if (spentTime.TotalSeconds > 3)
            {
                Logging.WriteLine("GameClientManager.AddClients spent: " + spentTime.TotalSeconds + " seconds in working.");
            }
        }

        private void RemoveClients()
        {
            try
            {
                DateTime startTaskTime;
                TimeSpan spentTime;
                startTaskTime = DateTime.Now;
                if (clientsToRemove.Count > 0)
                {
                    lock (clientsToRemove.SyncRoot)
                    {
                        while (clientsToRemove.Count > 0)
                        {
                            uint clientID = (uint)clientsToRemove.Dequeue();
                            clients.Remove(clientID);
                        }
                    }
                }

                spentTime = DateTime.Now - startTaskTime;

                if (spentTime.TotalSeconds > 3)
                {
                    Logging.WriteLine("GameClientManager.RemoveClients spent: " + spentTime.TotalSeconds + " seconds in working.");
                }
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.RemoveClients Exception --> Not inclusive"); }
        }

        private void GiveCredits()
        {
            try
            {
                DateTime startTaskTime;
                TimeSpan spentTime;
                startTaskTime = DateTime.Now;

                if (creditQueuee.Count > 0)
                {
                    lock (creditQueuee.SyncRoot)
                    {
                        while (creditQueuee.Count > 0)
                        {
                            int amount = (int)creditQueuee.Dequeue();
                            foreach (GameClient client in clients.Values)
                            {
                                if (client.GetHabbo() == null)
                                    continue;
                                try
                                {
                                    client.GetHabbo().Credits += amount;
                                    client.GetHabbo().UpdateCreditsBalance();
                                    client.SendNotif(LanguageLocale.GetValue("user.creditsreceived") + ": " + amount);
                                }
                                catch { }
                            }
                        }
                    }
                }
                spentTime = DateTime.Now - startTaskTime;

                if (spentTime.TotalSeconds > 3)
                {
                    Logging.WriteLine("GameClientManager.GiveCredits spent: " + spentTime.TotalSeconds + " seconds in working.");
                }
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.GiveCredits Exception --> Not inclusive"); }
        }

        private void GivePixels()
        {
            try
            {
                DateTime startTaskTime;
                TimeSpan spentTime;
                startTaskTime = DateTime.Now;

                if (pixelQueuee.Count > 0)
                {
                    lock (pixelQueuee.SyncRoot)
                    {
                        while (pixelQueuee.Count > 0)
                        {
                            int amount = (int)pixelQueuee.Dequeue();
                            foreach (GameClient client in clients.Values)
                            {
                                if (client.GetHabbo() == null)
                                    continue;
                                try
                                {
                                    client.GetHabbo().ActivityPoints += amount;
                                    client.GetHabbo().UpdateActivityPointsBalance(true);
                                    client.SendNotif(LanguageLocale.GetValue("user.pixelsreceived") + ": " + amount);
                                }
                                catch { }
                            }
                        }
                    }
                }
                spentTime = DateTime.Now - startTaskTime;

                if (spentTime.TotalSeconds > 3)
                {
                    Logging.WriteLine("GameClientManager.GivePixels spent: " + spentTime.TotalSeconds + " seconds in working.");
                }
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.GiveCredits Exception --> Not inclusive"); }
        }

        private void GiveBadges()
        {
            try
            {
                DateTime startTaskTime;
                TimeSpan spentTime;
                startTaskTime = DateTime.Now;
                if (badgeQueue.Count > 0)
                {
                    lock (badgeQueue.SyncRoot)
                    {
                        while (badgeQueue.Count > 0)
                        {
                            string badgeID = (string)badgeQueue.Dequeue();
                            foreach (GameClient client in clients.Values)
                            {
                                if (client.GetHabbo() == null)
                                    continue;
                                try
                                {
                                    client.GetHabbo().GetBadgeComponent().GiveBadge(badgeID, true);
                                    client.SendNotif(LanguageLocale.GetValue("user.badgereceived"));
                                }
                                catch { }
                            }
                        }
                    }
                }
                spentTime = DateTime.Now - startTaskTime;

                if (spentTime.TotalSeconds > 3)
                {
                    Logging.WriteLine("GameClientManager.GiveBadges spent: " + spentTime.TotalSeconds + " seconds in working.");
                }
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.GiveBadges Exception --> Not inclusive"); }
        }

        private void BroadcastPacketsWithRankRequirement()
        {
            try
            {
                DateTime startTaskTime;
                TimeSpan spentTime;
                startTaskTime = DateTime.Now;
                if (authorizedPacketSending.Count > 0)
                {
                    lock (authorizedPacketSending.SyncRoot)
                    {
                        while (authorizedPacketSending.Count > 0)
                        {
                            FusedPacket packet = (FusedPacket)authorizedPacketSending.Dequeue();
                            foreach (GameClient client in clients.Values)
                            {
                                try
                                {
                                    if (client.GetHabbo() != null && client.GetHabbo().HasFuse(packet.requirements))
                                        client.SendMessage(packet.content);
                                }
                                catch { }
                            }
                        }
                    }
                }
                spentTime = DateTime.Now - startTaskTime;

                if (spentTime.TotalSeconds > 3)
                {
                    Logging.WriteLine("GameClientManager.BroadcastPacketsWithRankRequirement spent: " + spentTime.TotalSeconds + " seconds in working.");
                }
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.BroadcastPacketsWithRankRequirement Exception --> Not inclusive"); }
        }

        private void BroadcastPackets()
        {
            try
            {
                DateTime startTaskTime;
                TimeSpan spentTime;
                startTaskTime = DateTime.Now;
                if (broadcastQueue.Count > 0)
                {
                    lock (broadcastQueue.SyncRoot)
                    {
                        while (broadcastQueue.Count > 0)
                        {
                            ServerMessage message = (ServerMessage)broadcastQueue.Dequeue();
                            byte[] bytes = message.GetBytes();

                            foreach (GameClient client in clients.Values)
                            {
                                try
                                {
                                    client.GetConnection().SendData(bytes);
                                }
                                catch
                                { }
                            }
                        }
                    }
                }
                spentTime = DateTime.Now - startTaskTime;

                if (spentTime.TotalSeconds > 3)
                {
                    Logging.WriteLine("GameClientManager.BroadcastPackets spent: " + spentTime.TotalSeconds + " seconds in working.");
                }
            }
            catch (Exception e) { Logging.LogThreadException(e.ToString(), "GameClientManager.BroadcastPackets Exception --> Not inclusive"); }
        }

        #endregion

        #region Methods
        internal void CreateAndStartClient(uint clientID, ConnectionInformation connection)
        {
            GameClient client = new GameClient(clientID, connection);
            if (clients.ContainsKey(clientID))
                clients.Remove(clientID);

            lock (clientsAddQueue.SyncRoot)
            {
                clientsAddQueue.Enqueue(client);
            }
        }

        internal void DisposeConnection(uint clientID)
        {
            GameClient Client = GetClient(clientID);
            if (Client != null)
            {
                Client.Stop();
            }

            lock (clientsToRemove.SyncRoot)
            {
                clientsToRemove.Enqueue(clientID);
            }
        }

        internal void QueueBroadcaseMessage(ServerMessage message)
        {
            lock (broadcastQueue.SyncRoot)
            {
                broadcastQueue.Enqueue(message);
            }
        }

        internal void QueueBroadcaseMessage(ServerMessage message, string requirements)
        {
            FusedPacket packet = new FusedPacket(message, requirements);
            lock (authorizedPacketSending.SyncRoot)
            {
                authorizedPacketSending.Enqueue(packet);
            }
        }

        private void BroadcastMessage(ServerMessage message)
        {
            lock (broadcastQueue.SyncRoot)
            {
                broadcastQueue.Enqueue(message);
            }
        }

        internal void QueueCreditsUpdate(int amount)
        {
            lock (creditQueuee.SyncRoot)
            {
                creditQueuee.Enqueue(amount);
            }
        }
        internal void QueuePixelsUpdate(int amount)
        {
            lock (pixelQueuee.SyncRoot)
            {
                pixelQueuee.Enqueue(amount);
            }
        }

        internal void QueueBadgeUpdate(string badge)
        {
            lock (badgeQueue.SyncRoot)
            {
                badgeQueue.Enqueue(badge);
            }
        }

        internal static void UnregisterConnection(uint connectionID)
        {
        }

        private void CheckEffects()
        {
            foreach (GameClient client in clients.Values)
            {
                if (client.GetHabbo() == null || client.GetHabbo().GetAvatarEffectsInventoryComponent() == null)
                    continue;

                client.GetHabbo().GetAvatarEffectsInventoryComponent().CheckExpired();
            }
        }

        internal void LogClonesOut(uint UserID)
        {
            GameClient client = GetClientByUserID(UserID);
            if (client != null)
                client.Disconnect();
        }

        internal void RegisterClient(GameClient client, uint userID, string username)
        {
            if (usernameRegister.ContainsKey(username.ToLower()))
                usernameRegister[username.ToLower()] = client;
            else
                usernameRegister.Add(username.ToLower(), client);

            if (userIDRegister.ContainsKey(userID))
                userIDRegister[userID] = client;
            else
                userIDRegister.Add(userID, client);

            if (!usernameIdRegister.ContainsKey(username))
                usernameIdRegister.Add(username, userID);
            if (!idUsernameRegister.ContainsKey(userID))
                idUsernameRegister.Add(userID, username);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                if (dbClient.dbType == Database_Manager.Database.DatabaseType.MySQL)
                    dbClient.setQuery("REPLACE INTO user_online VALUES (" + userID + ")");
                else
                    dbClient.setQuery("IF NOT EXISTS (SELECT userid FROM user_online WHERE userid = " + userID + ") " +
                                      "   INSERT INTO user_online VALUES (" + userID + ")");
            }
        }

        internal void UnregisterClient(uint userid, string username)
        {
            userIDRegister.Remove(userid);
            usernameRegister.Remove(username.ToLower());

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("DELETE FROM user_online WHERE id = " + userid);
            }
        }

        internal void CloseAll()
        {
            StringBuilder QueryBuilder = new StringBuilder();
            bool RunUpdate = false;
            int Count = 0;

            foreach (GameClient client in clients.Values)
            {
                if (client.GetHabbo() != null)
                    Count++;
            }
            if (Count < 1)
                Count = 1;

            int current = 0;
            int ClientLength = clients.Count;
            foreach (GameClient client in clients.Values)
            {
                current++;
                if (client.GetHabbo() != null)
                {
                    try
                    {
                        client.GetHabbo().GetInventoryComponent().RunDBUpdate();
                        QueryBuilder.Append(client.GetHabbo().GetQueryString);
                        RunUpdate = true;

                        Console.Clear();
                        Logging.WriteLine("<<- SERVER SHUTDOWN ->> IVNENTORY SAVE: " + String.Format("{0:0.##}", ((double)current / Count) * 100) + "%");
                    }
                    catch { }
                }
            }

            if (RunUpdate)
            {
                try
                {
                    if (QueryBuilder.Length > 0)
                    {
                        using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
                        {
                            dbClient.runFastQuery(QueryBuilder.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Logging.HandleException(e, "GameClientManager.CloseAll()");
                }
            }
            

            Logging.WriteLine("Done saving users inventory!");
            Logging.WriteLine("Closing server connections...");
            try
            {
                int i = 0;
                foreach (GameClient client in clients.Values)
                {
                    i++;
                    if (client.GetConnection() != null)
                    {
                        try
                        {
                            client.GetConnection().Dispose();
                        }
                        catch { }

                        Console.Clear();
                        Logging.WriteLine("<<- SERVER SHUTDOWN ->> CONNECTION CLOSE: " + String.Format("{0:0.##}", ((double)i / ClientLength) * 100) + "%");
                    }
                }
            }
            catch (Exception e)
            {
                Logging.LogCriticalException(e.ToString());
            }
            clients.Clear();
            Logging.WriteLine("Connections closed!");
        }
        #endregion
    }
}
