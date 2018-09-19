using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System;
using System.Threading;

namespace Heroes_Sample_Mod
{
    public class Feature_Magnetic_Barrier
    {
        /// <summary>
        /// Defines the base speed at which all of the rings continously approach the player.
        /// </summary>
        private float Ring_Base_Speed = 2.0F;

        /// <summary>
        /// Defines the distance based characteristic. The rings will approach the player faster by set percent distance of the ring to the player.
        /// </summary>
        private float Distance_Speed_Increase = 0.002F;

        /// <summary>
        /// An array that stores the currently active loaded in memory Addresses pointing to ring objects..
        /// </summary>
        private int[] Active_Ring_Addresses = new int[64];

        /// <summary>
        /// Number of rings which were recognized the last time a memory sweep for active rings was performed, used for optimization.
        /// </summary>
        private int Currently_Active_Rings = 0;

        /// <summary>
        /// Stores the current coordinates of the current leader in the formation.
        /// </summary>
        private Player_Coordinates Current_Leader_Coordinates; 
        
        /// <summary>
        /// Defines whether the magnetic barrier is enabled or not.
        /// </summary>
        private bool Magnetic_Barrier_Enabled = false;

        /// <summary>
        /// Toggles the active state of the magnetic barrier.
        /// </summary>
        public void Toggle_Magnetic_Barrier()
        {
            if (Magnetic_Barrier_Enabled) { Magnetic_Barrier_Enabled = false; }
            else { Enable_Magnetic_Barrier(); Magnetic_Barrier_Enabled = true; }
        }

        /// <summary>
        /// Increments the main speed by which the rings accelerate towards the player.
        /// </summary>
        public void Increment_Base_Speed() { Ring_Base_Speed += 0.025F; }

        /// <summary>
        /// Decrements the main speed by which the rings accelerate towards the player.
        /// </summary>
        public void Decrement_Base_Speed() { Ring_Base_Speed -= 0.025F; }

        /// <summary>
        /// Increments the distance based speed by which the rings accelerate towards the player.
        /// </summary>
        public void Increment_Distance_Speed_Scale() { Distance_Speed_Increase += 0.001F; }

        /// <summary>
        /// Dncrements the distance based speed by which the rings accelerate towards the player.
        /// </summary>
        public void Decrement_Distance_Speed_Scale() { Distance_Speed_Increase -= 0.001F; }

        /// <summary>
        /// Returns true if the magnetic barrier is enabled, else false.
        /// </summary>
        /// <returns></returns>
        public bool Get_IsEnabled() { return Magnetic_Barrier_Enabled; }

        /// <summary>
        /// Returns the base speed at which all of the rings continously approach the player.
        /// </summary>
        public float Get_Base_Speed() { return Ring_Base_Speed; }

        /// <summary>
        /// Returns the distance-based speed modifier which modifies how the rings continously approach the player.
        /// </summary>
        public float Get_Distance_Speed() { return Distance_Speed_Increase; }

        /// <summary>
        /// Enables the Magnetic Barrier functionality.
        /// </summary>
        private void Enable_Magnetic_Barrier()
        {
            // Initialize and start a thread to act as the magnetic barrier.
            Thread Magnetic_Barrier_Thread = new Thread(() =>
            {
                // Infinite loop until disabled.
                while (Magnetic_Barrier_Enabled)
                {
                    Magnetic_Ring_Loop();
                    Thread.Sleep(16); // Roughly 60FPS
                }
            });
            Magnetic_Barrier_Thread.Start();
        }

        /// <summary>
        /// A struct containing and holding the current player coordinates.
        /// </summary>
        private struct Player_Coordinates
        {
            public float X_Position;
            public float Y_Position;
            public float Z_Position;
        }

        /// <summary>
        /// The main loop of the magnetic barrier code, repeated without end until disabled.
        /// </summary>
        private void Magnetic_Ring_Loop()
        {
            // Confirm whether player is currently within level and is not paused.
            byte Is_Currently_In_Level = HeroesProcess.ReadMemory<byte>(Program.Sonic_Heroes_Process, (IntPtr)SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);
            byte Is_Pause_Menu_Open = HeroesProcess.ReadMemory<byte>(Program.Sonic_Heroes_Process, (IntPtr)SonicHeroesVariables.Game_CurrentState.IsIngamePauseMenuOpen, 1);

            // If the pause menu is not open and the player is in a stage.
            if ((Is_Currently_In_Level == 1) && (Is_Pause_Menu_Open == 0))
            {
                // Get all addresses of loaded rings.
                Identify_Ring_Addresses(); 

                // If there are rings in range.
                if (Currently_Active_Rings >= 0)
                {
                    // Retrieves the current player coordinates.
                    Retrieve_Player_Coordinates();

                    // Moves all currently loaded/found rings.
                    Move_All_Rings(); 
                }
            }
            return;
        }

        /// <summary>
        /// Moves all currently loaded/found rings towards the player.
        /// </summary>
        private void Move_All_Rings()
        {
            // For each known ring
            for (int x = 0; x < Currently_Active_Rings; x++)
            {
                // Calculate their XYZ Positions.
                float X_Position = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.X_Position), 4);
                float Y_Position = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.Y_Position), 4);
                float Z_Position = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.Z_Position), 4);

                // Get the difference in current and new location in terms of XYZ coordinates.
                float X_Delta_Position = Current_Leader_Coordinates.X_Position - X_Position;
                float Y_Delta_Position = Current_Leader_Coordinates.Y_Position - Y_Position;
                float Z_Delta_Position = Current_Leader_Coordinates.Z_Position - Z_Position;

                // Get the difference in current and new location in terms of XYZ coordinates
                float X_Delta_Position_Distance_Scaled = (X_Delta_Position) * Distance_Speed_Increase;
                float Y_Delta_Position_Distance_Scaled = (Y_Delta_Position) * Distance_Speed_Increase;
                float Z_Delta_Position_Distance_Scaled = (Z_Delta_Position) * Distance_Speed_Increase;

                // Vector Mathematics: Get the length squared.
                float Length_Squared = (X_Delta_Position * X_Delta_Position) + (Y_Delta_Position * Y_Delta_Position) + (Z_Delta_Position * Z_Delta_Position);

                // Quick compute the inverse square root to get a value which by which multiplied would give an approximate for the unit vector.
                float Unit_vector_Multiplier = Fast_InvSqrt(Length_Squared);

                // Calculate path ring must follow to go towards player at constant set rate.
                float X_Normal_Vector_Scaled = (X_Delta_Position * Unit_vector_Multiplier) * Ring_Base_Speed; // Constant Rate/Speed towards player in direction
                float Y_Normal_Vector_Scaled = (Y_Delta_Position * Unit_vector_Multiplier) * Ring_Base_Speed; // Constant Rate/Speed towards player in direction
                float Z_Normal_Vector_Scaled = (Z_Delta_Position * Unit_vector_Multiplier) * Ring_Base_Speed; // Constant Rate/Speed towards player in direction

                // Write the ring properties to memory.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.X_Position), BitConverter.GetBytes(X_Position + X_Normal_Vector_Scaled - X_Delta_Position_Distance_Scaled));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.Y_Position), BitConverter.GetBytes(Y_Position + Y_Normal_Vector_Scaled - Y_Delta_Position_Distance_Scaled));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)(Active_Ring_Addresses[x] + (int)SonicHeroesVariables.Game_Objects_CurrentlyLoaded_Table_Rings_Offsets.Z_Position), BitConverter.GetBytes(Z_Position + Z_Normal_Vector_Scaled - Z_Delta_Position_Distance_Scaled));
            }
        }

        /// <summary>
        /// The fast inverse square root function from Quake III.
        /// </summary>
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
        private void Retrieve_Player_Coordinates()
        {
            // Retrieves the basE address of the current player from the pointer.
            int CurrentPlayer_Address = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)SonicHeroesVariables.Characters_Addresses.CurrentPlayerControlledCharacter_Pointer, 4);

            // Retrieves the XYZ positions.
            Current_Leader_Coordinates.X_Position = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)((int)CurrentPlayer_Address + (int)SonicHeroesVariables.Characters_Addresses_Offsets.XPosition), 4);
            Current_Leader_Coordinates.Y_Position = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)((int)CurrentPlayer_Address + (int)SonicHeroesVariables.Characters_Addresses_Offsets.YPosition), 4);
            Current_Leader_Coordinates.Z_Position = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)((int)CurrentPlayer_Address + (int)SonicHeroesVariables.Characters_Addresses_Offsets.ZPosition), 4);
        }

        /// <summary>
        /// Obtains a listing of currently loaded in rings.
        /// </summary>
        private void Identify_Ring_Addresses()
        {
            // For every out of the 32 magnetic barrier's ring slots
            for (int x = 0; x < Active_Ring_Addresses.Length; x++)
            {
                // Retrieve the address where the ring is stored in memory and insert it into the array if it is not null.
                int Temp_Ring_Address = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)(SonicHeroesVariables.Game_Objects_CurrentlyLoaded.Table_Rings + (x * 4)), 4);

                if (Temp_Ring_Address != 0)
                {
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
