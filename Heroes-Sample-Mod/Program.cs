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

                // Allocate Memory Space
                currentPlayerVelocity = Sonic_Heroes_Process.AllocateMemory(4);

                // Assembly Code to Write address of Player currently boosting to memory.
                string[] injectionString = new string[]
                {
                    "use32",
                    "add ebx,0x00000BE0",
                    "mov dword [0x" + currentPlayerVelocity.ToString("X") + "],ebx"
                };

                // Remove cap to speed when boosting (also friction).
                Sonic_Heroes_Process.WriteMemory((IntPtr)FRICTION_BOOST_CAP_ADDRESS, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });

                // Assemble ASM Code.
                byte[] playerPointerASM = Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(injectionString), true);

                // Create new Mixed Injection & Execute.
                boostPlayerInjection = new Injection_Mixed((IntPtr)BOOSTPAD_BOOST_MEMORY_WRITE_ADDRESS, playerPointerASM, (boostPlayerDelegate)boostPlayerAction, 6);
                boostPlayerInjection.Activate();
            }
            catch (Exception Ex) { Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " Failed To Load: " + Ex.Message + " | " + Ex.StackTrace), false); }
        }

        // Constants
        const int FRICTION_BOOST_CAP_ADDRESS = 0x4BAB7C;
        const int BOOSTPAD_BOOST_MEMORY_WRITE_ADDRESS = 0x4C7AA5;

        // Rest of the code.
        static Injection_Mixed boostPlayerInjection;

        // Delegate for when the player boosts.
        delegate void boostPlayerDelegate();

        /// <summary>
        /// Memory address for current player's velocity.
        /// </summary>
        static IntPtr currentPlayerVelocity;

        /// <summary>
        /// Executed when the player is boosted.
        /// </summary>
        public static void boostPlayerAction()
        {
            // Get and multiply player speed.
            int playerSpeedAddress = Sonic_Heroes_Process.ReadMemory<int>(currentPlayerVelocity, 4);
            float playerSpeed = Sonic_Heroes_Process.ReadMemory<float>((IntPtr)playerSpeedAddress, 4);

            // Multiply player speed.
            playerSpeed = playerSpeed * 1.1F;

            // Write back to memory.
            Sonic_Heroes_Process.WriteMemory((IntPtr)playerSpeedAddress, BitConverter.GetBytes(playerSpeed));
        }
    }
}