using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using static SonicHeroes.Networking.Client_Functions;

using SonicHeroes.ASMInjections;
using SonicHeroes.Controller;
using SonicHeroes.Functions;
using SonicHeroes.Hooking;
using SonicHeroes.Memory;
using SonicHeroes.Misc;
using SonicHeroes.Networking;
using SonicHeroes.Structs;

namespace Heroes_Sample_Mod
{
    public class Program
    {
        ////////////////////////////////
        /// Mod loader DLL Skeleton Code
        ////////////////////////////////

        /// <summary>
        /// Defines the name of the mod loader DLL Project.
        /// </summary>
        const string MOD_NAME = "Riders Slide Race";

        /// <summary>
        /// Stores the client of the Mod Loader Server. Can be used for communication with the external local server.
        /// </summary>
        public static WebSocket_Client serverClient = new WebSocket_Client();

        /// <summary>
        /// The process of the current running game which you are toying around with. Allows for manipulation in multiple ways, e.g. read/write memory.
        /// </summary>
        public static Process gameProcess;

        /// <summary>
        /// Initializes the Mod Loader DLL Mod.
        /// </summary>
        static void Initialize()
        {
            ////////////// MOD LOADER DLL SKELETON CODE ///////////////////////////////////////////////////////////////////////////////////////////////////
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(SonicHeroes.Misc.SonicHeroes_Miscallenous.CurrentDomain_SetAssemblyResolve);
            serverClient.SetupClient(IPAddress.Loopback); /// Set up networking with the Mod Loader.
            gameProcess = Process.GetCurrentProcess(); /// We will use this for reading and writing memory.
            Print("Setup... OK!"); /// Say to the Mod Loader that we have loaded so the end user can know.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        /// <summary>
        /// Prints a message to the mod loader server console
        /// </summary>
        /// <param name="message"></param>
        static void Print(string message)
        {
            // Send Message as ASCII to the loader server.
            serverClient.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(MOD_NAME + " | " + message), true); /// Say to the Mod Loader that we have loaded so the end user can know.
        }

        /// <summary>
        /// Main Entry Method for the Program.
        /// </summary>
        [DllExport]
        static void Main()
        {
            try
            {
                // Initialize the Loader.
                Initialize();

                // Change braking acceleration.
                gameProcess.WriteMemory((IntPtr)0x005C3104, BitConverter.GetBytes(0.001799999969F));
            }
            catch (Exception Ex) { Print("Failed To Load: " + Ex.Message + " | " + Ex.StackTrace); }
        }
    }
}