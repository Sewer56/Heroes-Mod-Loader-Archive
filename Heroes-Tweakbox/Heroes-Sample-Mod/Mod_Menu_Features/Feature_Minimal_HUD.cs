using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SonicHeroes.Networking.Client_Functions;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Allows for the replacement of the normal game's HUD with the 
    /// </summary>
    public class Feature_Minimal_HUD
    {
        /// ///////////
        /// Subfeatures
        /// ///////////

        Feature_Minimal_HUD_BobsledToggler BobsledToggler = new Feature_Minimal_HUD_BobsledToggler();
        Feature_Minimal_HUD_TimeFormatToggler TimeFormatToggler = new Feature_Minimal_HUD_TimeFormatToggler();
        Feature_Minimal_HUD_MaestroToggler MaestroToggler = new Feature_Minimal_HUD_MaestroToggler();

        /// /////////
        /// Addresses
        /// /////////

        // X & Y Position Addresses - All need to be offset correctly in X Value
        const int ADDRESS_RINGCOUNT_XPOSITION = 0x7BB2C8;
        const int ADDRESS_RINGCOUNT_YPOSITION = 0x7BB2CC;
        const int ADDRESS_CURRENTTIME_XPOSITION = 0x7BB2E8;
        const int ADDRESS_CURRENTTIME_YPOSITION = 0x7BB2EC;
        const int ADDRESS_CURRENTSCORE_XPOSITION = 0x7BB2F0;
        const int ADDRESS_CURRENTSCORE_YPOSITION = 0x7BB2F4;
        int ADDRESS_TRICKSCORE_HUD_XPOSITION;
        int ADDRESS_TRICKSCORE_HUD_YPOSITION;

        /// ///////
        /// ALL HUD
        /// ///////
        const int ADDRESS_DRAW_HUD = 0x004037D1;
        byte[] ADDRESS_DRAW_HUD_BYTES = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)ADDRESS_DRAW_HUD, 5);

        /// ///////////////
        /// Positions - Old
        /// ///////////////

        // Original X & Y Positions
        float CURRENTSCORE_ORIGINAL_XPOSITION;
        float CURRENTSCORE_ORIGINAL_YPOSITION;
        float CURRENTTIME_ORIGINAL_XPOSITION;
        float CURRENTTIME_ORIGINAL_YPOSITION;
        float RINGCOUNT_ORIGINAL_XPOSITION;
        float RINGCOUNT_ORIGINAL_YPOSITION;
        float TRICKSCORE_ORIGINAL_XPOSITION;
        float TRICKSCORE_ORIGINAL_YPOSITION;

        /// ///////////////
        /// Positions - New
        /// ///////////////

        struct MinimalHUD_ObjectPosition
        {
            public int Address_X;
            public int Address_Y;

            public float Position_X;
            public float Position_Y;

            // Constructor
            public MinimalHUD_ObjectPosition(int AddressX, int AddressY) { Address_X = AddressX; Address_Y = AddressY; Position_X = 0; Position_Y = 0; }
        }

        enum MinimalHUD_ObjectPosition_Index
        {
            CURRENTSCORE = 0,
            CURRENTTIME = 1,
            CURRENTRINGS = 2,
            TRICKSCORE = 3
        }

        // Struct of object Positions
        MinimalHUD_ObjectPosition[] Positions_HudItems; 

        /// /////////
        /// Alignment
        /// /////////

        // Top Left Corner Coordinates
        float Alignment_TopLeftCorner_X;
        float Alignment_TopLeftCorner_Y = 1F; // Text is 0.038 large and coordinate is bottom corner.
        const float Alignment_Text_Height = 0.038F;

        /// ///////////
        /// Positioning
        /// ///////////
        
        public float Positioning_Vertical_Offset = 0.01F; // Offset between each subsequent item (starts from bottom of last text).
        public float Positioning_X_Position = 0.1F; // Offset from top left corner.
        public float Positioning_Y_Position = 0.1F; // Offset from top left corner.

        /// /////
        /// Other
        /// /////

        // Flag
        public bool Enabled; // Is Minimalistic UI Enabled?
        public bool Enabled_HUD; // Is Minimalistic UI Enabled?

        /// <summary>
        /// Constructor 
        /// </summary>
        public Feature_Minimal_HUD()
        {
            Setup_TrickScore_XYPosition();
            Backup_HUD_Positions();
            Calculate_TopLeftCorner_X();

            // Setup HUD Items
            Positions_HudItems = new MinimalHUD_ObjectPosition[]
            {
                new MinimalHUD_ObjectPosition(ADDRESS_CURRENTSCORE_XPOSITION, ADDRESS_CURRENTSCORE_YPOSITION),
                new MinimalHUD_ObjectPosition(ADDRESS_CURRENTTIME_XPOSITION, ADDRESS_CURRENTTIME_YPOSITION),
                new MinimalHUD_ObjectPosition(ADDRESS_RINGCOUNT_XPOSITION, ADDRESS_RINGCOUNT_YPOSITION),
                new MinimalHUD_ObjectPosition(ADDRESS_TRICKSCORE_HUD_XPOSITION, ADDRESS_TRICKSCORE_HUD_YPOSITION),
            };
        }

        /// <summary>
        /// Changes X Position Offset
        /// </summary>
        /// <param name="Amount"></param>
        public void Change_X_Position(float Amount)
        {
            Positioning_X_Position += Amount;
            Calculate_Minimal_HUD_Values();
            Write_Minimal_HUD_Values();
        }

        /// <summary>
        /// Changes Y Position Offset
        /// </summary>
        /// <param name="Amount"></param>
        public void Change_Y_Position(float Amount)
        {
            Positioning_Y_Position += Amount;
            Calculate_Minimal_HUD_Values();
            Write_Minimal_HUD_Values();
        }

        /// <summary>
        /// Changes Y Position Offset
        /// </summary>
        /// <param name="Amount"></param>
        public void Change_Vertical_Offset(float Amount)
        {
            Positioning_Vertical_Offset += Amount;
            Calculate_Minimal_HUD_Values();
            Write_Minimal_HUD_Values();
        }

        /// <summary>
        /// Toggles the Minimal HUD on and off.
        /// </summary>
        public void Toggle_Minimal_HUD()
        {
            if (Enabled) { Disable_Minimal_HUD(); Enabled = false; }
            else { Enable_Minimal_HUD(); Enabled = true; }
        }

        /// <summary>
        /// Toggles the Minimal HUD on and off.
        /// </summary>
        public void Toggle_HUD()
        {
            if (Enabled_HUD) { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_DRAW_HUD, ADDRESS_DRAW_HUD_BYTES); Enabled_HUD = false; }
            else { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_DRAW_HUD, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); Enabled_HUD = true; }
        }

        /// <summary>
        /// Enables the Minimal HUD
        /// </summary>
        private void Enable_Minimal_HUD()
        {
            // Calculate & Write Minimal HUD
            Calculate_Minimal_HUD_Values();
            Write_Minimal_HUD_Values();

            // Disable UI Elements
            BobsledToggler.Disable_Bobsled();
            MaestroToggler.Disable_Maestro();
            TimeFormatToggler.Disable_TimeFormat();
        }

        /// <summary>
        /// Disables the Minimal HUD
        /// </summary>
        private void Disable_Minimal_HUD()
        {
            // Restores HUD Positions.
            Restore_HUD_Positions();

            // Enable UI Elements
            BobsledToggler.Enable_Bobsled();
            MaestroToggler.Enable_Maestro();
            TimeFormatToggler.Enable_TimeFormat();
        }

        /// <summary>
        /// Calculates all Minimal HUD Values
        /// </summary>
        private void Calculate_Minimal_HUD_Values()
        {
            for (int x = 0; x < Positions_HudItems.Length; x++)
            {
                Positions_HudItems[x].Position_X = Alignment_TopLeftCorner_X + Positioning_X_Position;

                if (x == 0)
                {
                    Positions_HudItems[x].Position_Y = Alignment_TopLeftCorner_Y - (Alignment_Text_Height * x) - Positioning_Y_Position;
                }
                else
                {
                    Positions_HudItems[x].Position_Y = Alignment_TopLeftCorner_Y - (Alignment_Text_Height * x) - (Positioning_Vertical_Offset*x) - Positioning_Y_Position;
                }
                
            }
        }

        /// <summary>
        /// Writes all Minimal HUD Values to Memory
        /// </summary>
        private void Write_Minimal_HUD_Values()
        {
            for (int x = 0; x < Positions_HudItems.Length; x++)
            {
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)Positions_HudItems[x].Address_X, BitConverter.GetBytes(Positions_HudItems[x].Position_X));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)Positions_HudItems[x].Address_Y, BitConverter.GetBytes(Positions_HudItems[x].Position_Y));
            }
        }

        /// <summary>
        /// Sets up TrickScore X & Y Positions to become available configurable as Memory addresses rather than hardcoded.
        /// </summary>
        private void Setup_TrickScore_XYPosition()
        {
            // Get bytes for new location of X & Y Trick Score Positions
            ADDRESS_TRICKSCORE_HUD_XPOSITION = (int)Program.Sonic_Heroes_Process.AllocateMemory(4);
            ADDRESS_TRICKSCORE_HUD_YPOSITION = (int)Program.Sonic_Heroes_Process.AllocateMemory(4);

            // Write Default Values to Addresses
            float DefaultX = 0.06562499702F;
            float DefaultY = 0.7333300114F;

            // Write Defaults to memory.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_TRICKSCORE_HUD_XPOSITION, BitConverter.GetBytes(DefaultX));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_TRICKSCORE_HUD_YPOSITION, BitConverter.GetBytes(DefaultY));

            // Assemble code to Move X & Y Trick Source Location from Memory rather than hardcoding..
            string[] Get_From_New_XLocation = new string[]
            {
                "use32",
                "push edi",
                "push esi",
                "mov esi, " + "[" + ADDRESS_TRICKSCORE_HUD_XPOSITION + "]",
                "mov edi, " + "[" + ADDRESS_TRICKSCORE_HUD_YPOSITION + "]",
                "mov dword [esp+0x20], esi",
                "mov dword [esp+0x24], edi",
                "pop esi",
                "pop edi",
            };

            // Get Assembly Code for the Mnemonics
            byte[] Get_From_NewX = Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(Get_From_New_XLocation), true);

            // Overwrite old instructions
            scoreInjection = new SonicHeroes.Hooking.ASM_Hook((IntPtr)0x5B499C, Get_From_NewX, 20, Program.Sonic_Heroes_Networking_Client, true);
            scoreInjection.Activate();
        }

        // ASM Clean Injection
        private SonicHeroes.Hooking.ASM_Hook scoreInjection;

        /// <summary>
        /// Backs up all of the HUD Positions to the backup variables.
        /// </summary>
        private void Backup_HUD_Positions()
        {
            CURRENTSCORE_ORIGINAL_XPOSITION = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)ADDRESS_CURRENTSCORE_XPOSITION, 4);
            CURRENTSCORE_ORIGINAL_YPOSITION = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)ADDRESS_CURRENTSCORE_YPOSITION, 4);
            CURRENTTIME_ORIGINAL_XPOSITION = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)ADDRESS_CURRENTTIME_XPOSITION, 4);
            CURRENTTIME_ORIGINAL_YPOSITION = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)ADDRESS_CURRENTTIME_YPOSITION, 4);
            RINGCOUNT_ORIGINAL_XPOSITION = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)ADDRESS_RINGCOUNT_XPOSITION, 4);
            RINGCOUNT_ORIGINAL_YPOSITION = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)ADDRESS_RINGCOUNT_YPOSITION, 4);
            TRICKSCORE_ORIGINAL_XPOSITION = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)ADDRESS_TRICKSCORE_HUD_XPOSITION, 4);
            TRICKSCORE_ORIGINAL_YPOSITION = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)ADDRESS_TRICKSCORE_HUD_YPOSITION, 4);
        }

        /// <summary>
        /// Restores all of the HUD Positions from backup variables.
        /// </summary>
        private void Restore_HUD_Positions()
        {
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_CURRENTSCORE_XPOSITION, BitConverter.GetBytes(CURRENTSCORE_ORIGINAL_XPOSITION));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_CURRENTSCORE_YPOSITION, BitConverter.GetBytes(CURRENTSCORE_ORIGINAL_YPOSITION));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_CURRENTTIME_XPOSITION, BitConverter.GetBytes(CURRENTTIME_ORIGINAL_XPOSITION));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_CURRENTTIME_YPOSITION, BitConverter.GetBytes(CURRENTTIME_ORIGINAL_YPOSITION));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RINGCOUNT_XPOSITION, BitConverter.GetBytes(RINGCOUNT_ORIGINAL_XPOSITION));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RINGCOUNT_YPOSITION, BitConverter.GetBytes(RINGCOUNT_ORIGINAL_YPOSITION));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_TRICKSCORE_HUD_XPOSITION, BitConverter.GetBytes(TRICKSCORE_ORIGINAL_XPOSITION));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_TRICKSCORE_HUD_YPOSITION, BitConverter.GetBytes(TRICKSCORE_ORIGINAL_YPOSITION));
        }

        /// <summary>
        /// Calculates the value of X for alignment which would match the top left corner of the screen.
        /// </summary>
        private void Calculate_TopLeftCorner_X()
        {
            // Width for the same height specified in 4:3
            float FourByThree_Width = (float)Program.Sonic_Heroes_Overlay.overlayWinForm.Width / 4.0F * 3.0F;
            float WidthDifference = Program.Sonic_Heroes_Overlay.overlayWinForm.Width - FourByThree_Width;

            // Set Horizontal Offset
            Alignment_TopLeftCorner_X = -(WidthDifference / (float)Program.Sonic_Heroes_Overlay.overlayWinForm.Width);
        }
    }
}
