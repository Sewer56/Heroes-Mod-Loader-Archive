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
using System.Collections.Generic;

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
        const string MOD_NAME = "Mod Template";

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

                // Allocate Memory Space
                currentPlayerVelocity = gameProcess.AllocateMemory(4);

                // Setup the Boost Injection Process
                setupBoostInjection();

                // Remove cap to speed when boosting (also friction).
                gameProcess.WriteMemory((IntPtr)FRICTION_BOOST_CAP_ADDRESS, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
            }
            catch (Exception Ex) { Print("Failed To Load: " + Ex.Message + " | " + Ex.StackTrace); }
        }

        // Constants
        const int FRICTION_BOOST_CAP_ADDRESS = 0x4BAB7C;
        const int BOOSTPAD_BOOST_MEMORY_WRITE_ADDRESS = 0x4C7AA5;

        // Rest of the code.
        static List<Hook_Base> boostPlayerInjection = new List<Hook_Base>();

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
            int playerSpeedAddress = gameProcess.ReadMemory<int>(currentPlayerVelocity, 4);
            float playerSpeed = gameProcess.ReadMemory<float>((IntPtr)playerSpeedAddress, 4);

            // Multiply player speed.
            playerSpeed = playerSpeed * 1.1F;

            // Write back to memory.
            gameProcess.WriteMemory((IntPtr)playerSpeedAddress, BitConverter.GetBytes(playerSpeed));
        }

        /// <summary>
        /// Sets up the boost injection for overriding boost behaviour.
        /// </summary>
        public static void setupBoostInjection()
        {
            // Assembly Code to Write address of Player currently boosting to memory.
            string[] injectionString = new string[]
            {
                    "use32",
                    "add ebx,0x00000BE0",
                    "mov dword [0x" + currentPlayerVelocity.ToString("X") + "],ebx",
                    "add ebx,-0x00000BE0"
            };

            // Assemble ASM Code.
            byte[] playerPointerASM = serverClient.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(injectionString), true);

            // Create new Mixed Injection & Execute.
            Injection boostCSharpInjection = new Injection((IntPtr)BOOSTPAD_BOOST_MEMORY_WRITE_ADDRESS, (boostPlayerDelegate)boostPlayerAction, 6, serverClient, true);

            // Activate Injection
            boostCSharpInjection.Activate();
            
            // Declare ASM Injection
            ASM_Hook boostASMInjection = new ASM_Hook((IntPtr)BOOSTPAD_BOOST_MEMORY_WRITE_ADDRESS, playerPointerASM, 6, serverClient, true);

            // Activate Injection
            boostASMInjection.Activate();

            // Append Hooks onto List
            boostPlayerInjection.Add(boostCSharpInjection);
            boostPlayerInjection.Add(boostASMInjection);
        }
    }
}