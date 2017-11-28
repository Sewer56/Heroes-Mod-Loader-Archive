using System;
using SonicHeroes.Functions;
using SonicHeroes.Controller;
using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Text;
using static SonicHeroes.Networking.Client_Functions;
using System.Drawing;
using SonicHeroes.Overlay;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.DirectWrite;
using Heroes_Sample_Mod;
using System.Threading;

namespace TestLibrary
{
    public class Program
    {
        /// Mod loader DLL Skeleton Code
        const string Mod_Name = "Mod Loader Magnetic Barrier Sample"; // Set name of project.
        static SonicHeroes.Networking.WebSocket_Client Sonic_Heroes_Networking_Client = new SonicHeroes.Networking.WebSocket_Client(); // Set up client for networking, this client communicates with the server to call methods under subscribe hook
        static SonicHeroes.Networking.WebSocket_Client Sonic_Heroes_Networking_Client_II = new SonicHeroes.Networking.WebSocket_Client(); // Set up client for networking, this one is for non-subscribed calls.
        static Process Sonic_Heroes_Process; // Get Sonic Heroes Process
        /// Mod loader DLL Skeleton Code

        // Settings
        const float Ring_Base_Speed = 2.0F; // Minimum speed at which the rings approach the player.
        const float Distance_Speed_Increase = 0.01F; // 1% - The rings will approach the player faster by 1% of the distance of the ring to the player.

        /// <summary>
        /// Your program starts here!
        /// </summary>
        [DllExport]
        static void Main()
        {
            try
            {
                ////////////// MOD LOADER DLL SKELETON CODE ///////////////////////////////////////////////////////////////////////////////////////////////////
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(SonicHeroes.Misc.SonicHeroes_Miscallenous.CurrentDomain_SetAssemblyResolve);
                Sonic_Heroes_Networking_Client.SetupClient(IPAddress.Loopback); /// Set up networking with the Sonic Heroes Mod Loader.
                Sonic_Heroes_Networking_Client_II.SetupClient(IPAddress.Loopback); /// Set up networking with the Sonic Heroes Mod Loader.
                byte[] Response = Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " | Loading... OK!"), true); /// Say to the Mod Loader that we have loaded so the end user can know.
                Sonic_Heroes_Process = Process.GetCurrentProcess(); /// We will use this for reading and writing memory.
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Thread used to update the overlay.
                while (true)
                {
                    Magnetic_Ring_Loop();
                    Thread.Sleep(16); // Roughly 60FPS
                }
            }
            catch (Exception Ex) { Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " Failed To Load: " + Ex.Message), false); }
        }

        /// Own Variables
        static int[] Active_Ring_Addresses = new int[64]; // Addresses which store the currently loaded rings on stage.
        static int Currently_Active_Rings = 0; // Number of last recognized rings.
        static Player_Coordinates Current_Leader_Coordinates; // Holds the coordinates of the current leader. 

        /// <summary>
        /// A struct containing and holding the current player coordinates.
        /// </summary>
        public struct Player_Coordinates
        {
            public float X_Position;
            public float Y_Position;
            public float Z_Position;
        }

        /// <summary>
        /// Sample D2D hook code which is injected and ran on each frame.
        /// </summary>
        static void Magnetic_Ring_Loop()
        {
            byte Is_Currently_In_Level = HeroesProcess.ReadMemory<byte>(Sonic_Heroes_Process, (IntPtr)SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);
            byte Is_Pause_Menu_Open = HeroesProcess.ReadMemory<byte>(Sonic_Heroes_Process, (IntPtr)SonicHeroesVariables.Game_CurrentState.IsIngamePauseMenuOpen, 1);

            // If the pause menu is not open and the player is in a stage.
            if ((Is_Currently_In_Level == 1) && (Is_Pause_Menu_Open == 0))
            {
                Identify_Ring_Addresses(); // Get all addresses of loaded rings.

                if (Currently_Active_Rings >= 0)
                {
                    Retrieve_Player_Coordinates(); // Retrieves the current player coordinates.
                    Move_All_Rings(); // Moves all currently loaded/found rings.
                }
            }
            return;
        }

        /// <summary>
        /// Moves all currently loaded/found rings towards the player.
        /// </summary>
        static void Move_All_Rings()
        {
            // For each known ring
            for (int x = 0; x < Currently_Active_Rings; x++)
            {
                float X_Position = Sonic_Heroes_Process.ReadMemory<float>((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.X_Position), 4);
                float Y_Position = Sonic_Heroes_Process.ReadMemory<float>((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.Y_Position), 4);
                float Z_Position = Sonic_Heroes_Process.ReadMemory<float>((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.Z_Position), 4);

                // Get the difference in current and new location in terms of XYZ coordinates.
                float X_Delta_Position = Current_Leader_Coordinates.X_Position - X_Position;
                float Y_Delta_Position = Current_Leader_Coordinates.Y_Position - Y_Position;
                float Z_Delta_Position = Current_Leader_Coordinates.Z_Position - Z_Position;

                // Get the difference in current and new location in terms of XYZ coordinates
                float X_Delta_Position_Distance_Scaled = (X_Delta_Position) * Distance_Speed_Increase;
                float Y_Delta_Position_Distance_Scaled = (Y_Delta_Position) * Distance_Speed_Increase;
                float Z_Delta_Position_Distance_Scaled = (Z_Delta_Position) * Distance_Speed_Increase;

                // Calculations necessary to obtain unit vector.
                float Length_Squared = (X_Delta_Position * X_Delta_Position) + (Y_Delta_Position * Y_Delta_Position) + (Z_Delta_Position * Z_Delta_Position);
                float Inverse_SquareRoot = Fast_InvSqrt(Length_Squared);

                // Scale to unit vector and scale again for speed of attraction.
                float X_Normal_Vector_Scaled = (X_Delta_Position * Inverse_SquareRoot) * Ring_Base_Speed; // Constant Rate/Speed towards player in direction
                float Y_Normal_Vector_Scaled = (Y_Delta_Position * Inverse_SquareRoot) * Ring_Base_Speed; // Constant Rate/Speed towards player in direction
                float Z_Normal_Vector_Scaled = (Z_Delta_Position * Inverse_SquareRoot) * Ring_Base_Speed; // Constant Rate/Speed towards player in direction

                // Write to memory.
                Sonic_Heroes_Process.WriteMemory((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.X_Position), BitConverter.GetBytes(X_Position + X_Normal_Vector_Scaled + X_Delta_Position_Distance_Scaled));
                Sonic_Heroes_Process.WriteMemory((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.Y_Position), BitConverter.GetBytes(Y_Position + Y_Normal_Vector_Scaled + Y_Delta_Position_Distance_Scaled));
                Sonic_Heroes_Process.WriteMemory((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.Z_Position), BitConverter.GetBytes(Z_Position + Z_Normal_Vector_Scaled + Z_Delta_Position_Distance_Scaled));
            }
        }

        /// <summary>
        /// God bless Quake III
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        unsafe static float Fast_InvSqrt(float x)
        {
            float xhalf = 0.5f * x;
            int i = *(int*)&x;              // get bits for floating value
            i = 0x5f375a86 - (i >> 1);      // gives initial guess y0
            x = *(float*)&i;                // convert bits back to float
            x = x * (1.5f - xhalf * x * x); // Newton step, repeating increases accuracy
            return x;
        }

        /// <summary>
        /// Identifies the current player coordinates and dumps them onto the struct instance.
        /// </summary>
        static void Retrieve_Player_Coordinates()
        {
            int CurrentPlayer_Address = Sonic_Heroes_Process.ReadMemory<int>((IntPtr)SonicHeroesVariables.Characters_Addresses.CurrentPlayerControlledCharacter_Pointer, 4);
            Current_Leader_Coordinates.X_Position = Sonic_Heroes_Process.ReadMemory<float>((IntPtr)((int)CurrentPlayer_Address + (int)SonicHeroesVariables.Characters_Addresses_Offsets.XPosition), 4);
            Current_Leader_Coordinates.Y_Position = Sonic_Heroes_Process.ReadMemory<float>((IntPtr)((int)CurrentPlayer_Address + (int)SonicHeroesVariables.Characters_Addresses_Offsets.YPosition), 4);
            Current_Leader_Coordinates.Z_Position = Sonic_Heroes_Process.ReadMemory<float>((IntPtr)((int)CurrentPlayer_Address + (int)SonicHeroesVariables.Characters_Addresses_Offsets.ZPosition), 4);
        }

        /// <summary>
        /// Obtains a listing of currently loaded in rings.
        /// </summary>
        static void Identify_Ring_Addresses()
        {
            // For every out of the 32 magnetic barrier's ring slots
            for (int x = 0; x < Active_Ring_Addresses.Length; x++)
            {
                // Retrieve the address where the ring is stored in memory and insert it into the array if it is not null.
                int Temp_Ring_Address = Sonic_Heroes_Process.ReadMemory<int>((IntPtr)(SonicHeroesVariables.Game_Objects_CurrentlyLoaded.Table_Rings + (x*4)), 4);

                if (Temp_Ring_Address != 0) {
                    Active_Ring_Addresses[x] = Temp_Ring_Address;
                }
                else
                {
                    Currently_Active_Rings = x - 1;
                    break;
                }
            }
        }
        
    }
}