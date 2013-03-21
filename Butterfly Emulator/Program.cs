namespace Butterfly
{
    using Butterfly.Core;
    using ConsoleWriter;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using System.Threading;

    internal class Program
    {
        private static EventHandler _handler;
        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private delegate bool EventHandler(CtrlType sig);


        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        [STAThread]

        internal static void Main()
        {
            Writer.Init();

            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);

            InitEnvironment();

            while (true)
            {
                Console.CursorVisible = true;

                if (Logging.DisabledState)
                {
                    Console.Write("bfly> ");
                }

                ConsoleCommandHandling.InvokeCommand(Console.ReadLine());
            }
        }

        [MTAThread]
        private static void InitEnvironment()
        {
            if (!ButterflyEnvironment.isLive)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.CursorVisible = false;
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += MyHandler;

                ButterflyEnvironment.Initialize();
            }
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Logging.DisablePrimaryWriting(true);
            Exception e = (Exception)args.ExceptionObject;
            Logging.LogCriticalException("SYSTEM CRITICAL EXCEPTION: " + e);
            ButterflyEnvironment.SendMassMessage("A fatal error crashed the server, server shutting down.");
            ButterflyEnvironment.PreformShutDown();
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    {
                        ButterflyEnvironment.PreformShutDown();
                    }
                    return false;

                default:

                    return false;
            }
        }
    }
}