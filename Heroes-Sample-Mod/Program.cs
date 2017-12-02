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
        const string Mod_Name = "Riders Test Mod"; // Set name of project.
        public static SonicHeroes.Networking.WebSocket_Client Sonic_Heroes_Networking_Client = new SonicHeroes.Networking.WebSocket_Client(); // Set up client for networking, this client communicates with the server to call methods under subscribe hook
        public static Process Sonic_Heroes_Process; // Get Sonic Heroes Process

        /// <summary>
        /// Main Entry Method
        /// </summary>
        [DllExport]
        static void Main()
        {
            try
            {
                ////////////// MOD LOADER DLL SKELETON CODE ///////////////////////////////////////////////////////////////////////////////////////////////////
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(SonicHeroes.Misc.SonicHeroes_Miscallenous.CurrentDomain_SetAssemblyResolve);
                Sonic_Heroes_Networking_Client.SetupClient(IPAddress.Loopback); /// Set up networking with the Sonic Heroes Mod Loader.
                byte[] Response = Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " | Loading... OK!"), true); /// Say to the Mod Loader that we have loaded so the end user can know.
                Sonic_Heroes_Process = Process.GetCurrentProcess(); /// We will use this for reading and writing memory.
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Remove cap to speed when boosting.
                Sonic_Heroes_Process.WriteMemory((IntPtr)0x4BD1E1, new byte[] { 0x90, 0x90 });
            }
            catch (Exception Ex) { Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " Failed To Load: " + Ex.Message + " | " + Ex.StackTrace), false); }
        }
    }
}