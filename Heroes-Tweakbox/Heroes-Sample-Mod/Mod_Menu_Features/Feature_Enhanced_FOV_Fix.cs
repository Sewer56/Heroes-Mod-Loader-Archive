using SonicHeroes.Memory;
using SonicHeroes.Overlay;
using SonicHeroes.Variables;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// My own implementation of the FOV Fix, superior to others as it offers no clipping on objects and also works on victory screens - at the expense of a slightly derped results screen.
    /// </summary>
    public class Feature_Enhanced_FOV_Fix
    {
        /////////////// ///////////// //
        /////////////// FOV FIX STUFF //
        /////////////// ///////////// //

        /// <summary>
        /// Assembly instruction which controls an unknown (unused?) HUD Vertical Position.
        /// </summary>
        const int UNKNOWN_VERTICAL_POSITION_ASM_INSTRUCTION = 0x6AA3D2;

        /// <summary>
        /// Assembly instruction which controls an unknown (unused?) HUD Horizontal Position.
        /// </summary>
        const int UNKNOWN_HORIZONTAL_POSITION_ASM_INSTRUCTION = 0x6AA3CC;

        /// <summary>
        /// Assembly instruction which controls the Horiztonal Shown Field of View.
        /// </summary>
        const int HORIZONTAL_FOV_ASM_INSTRUCTION = 0x64AC8B;

        /// <summary>
        /// Assembly instruction which controls the Vertical Shown Field of View.
        /// </summary>
        const int VERTICAL_FOV_ASM_INSTRUCTION = 0x64ACA5;

        /// <summary>
        /// Instruction which controls the Aspect Ratio for the Victory Screen, X.
        /// </summary>
        const int VICTORY_SCREEN_X_ASPECTRATIO_INSTRUCTION = 0x61E530;

        /// <summary>
        /// Instruction which controls the Aspect Ratio for the Victory Screen, Y.
        /// </summary>
        const int VICTORY_SCREEN_Y_ASPECTRATIO_INSTRUCTION = 0x61E544;

        /// <summary>
        /// Defines the new address (ASM Address OVerride) from where the game will read the "Magic" alignment value. This address is assigned when this menu is loaded.
        /// </summary>
        IntPtr Menu_Center_Value_Address;

        /// <summary>
        /// Defines the new address (ASM Address OVerride) from where the game will read the vertical menu centering value used for main menus. This address is assigned when this menu is loaded.
        /// </summary>
        IntPtr Menu_Vertical_Center_Value_Address; // This address is assigned when this menu is loaded.

        /// <summary>
        /// Defines the new memory address from where the game will read the Screen Aspect Length/Extension on the Horizontal Axis. 
        /// Declares the scale of the viewable area and extends the screen space. Think Dolphin Widescreen Hack.
        /// </summary>
        IntPtr FOV_ASM_Menu_Horizontal_Value_Address;

        /// <summary>
        /// Defines the new memory address from where the game will read the Screen Aspect Length/Extension on the Vertical Axis. 
        /// Declares the scale of the viewable area and extends the screen space. Think Dolphin Widescreen Hack.
        /// </summary>
        IntPtr FOV_ASM_Menu_Vertical_Value_Address; // This address is assigned when this menu is loaded.

        /// <summary>
        /// Defines the Horizontal Scale for the presented Field of View.
        /// </summary>
        float Menu_Horizonal_Field_Of_View_Scale = 1.0F;

        /// <summary>
        /// Defines the Vertical Scale for the presented Field of View.
        /// </summary>
        float Menu_Vertical_Field_Of_View_Scale = 1.0F;

        /// <summary>
        /// Defines the "magic" alignment value which moves the menu from the left side of the screen towards the center of the screen under our own FOV/Aspect hax.
        /// </summary>
        float Menu_Center_Value_FOV = 1.23F;

        /// <summary>
        /// This hook is executed when the player leaves from the state of "in-game" and enters the main menu. Switches the FOV fix from Game Mode to Menu Mode
        /// </summary>
        public static SonicHeroes.Hooking.Injection Menu_Enter_Hook;

        /// <summary>
        /// This hook is executed when the player enters the state of "in-game" from the main menu. Switches the FOV fix from Menu Mode to Game Mode
        /// </summary>
        public static SonicHeroes.Hooking.Injection Menu_Exit_Hook;

        /// <summary>
        /// Currently inactive, an attempt of fixing the results screen display.
        /// </summary>
        public static SonicHeroes.Hooking.Injection Results_Enter_Hook;

        /// ASM Instructions for the FOV/Screen Extension Handling of the Main Menu.
        byte[] ASM_Menu_FOV_Instruction_Vertical_Original;
        byte[] ASM_Menu_FOV_Instruction_Horizontal_Original;
        List<byte> ASM_Menu_FOV_Instruction_Vertical_Override = new List<byte>();
        List<byte> ASM_Menu_FOV_Instruction_Horizontal_Override = new List<byte>();

        /// ASM Instructions for the HUD Handling while InGame
        byte[] ASM_HUD_FOV_Horizontal_Instruction_Original; // Original bytes for positioning the HUD.
        byte[] ASM_HUD_FOV_Vertical_Instruction_Original; // Original bytes for positioning the HUD.
        List<byte> ASM_HUD_FOV_Horizontal_Instruction_Override = new List<byte>(); // New bytes for positioning the HUD.
        List<byte> ASM_HUD_FOV_Vertical_Instruction_Override = new List<byte>(); // New bytes for positioning the HUD.

        // ASM Instructions which handle the FOV X & Y scaling during the results screen sequence.
        List<byte> Victory_Screen_FOV_X_Instruction_Override = new List<byte>();
        List<byte> Victory_Screen_FOV_Y_Instruction_Override = new List<byte>();
        byte[] Victory_Screen_FOV_X_Instruction_Original;
        byte[] Victory_Screen_FOV_Y_Instruction_Original;

        // Listing of delegates used for casting individual hook methods to be executed.
        public delegate void Enable_Menu_FOV();
        public delegate void Disable_Menu_FOV();
        public delegate void Set_Results_Menu_Location_Live();

        // Flags
        bool Menu_Aspect_Ratio_Fix_Toggle_Enabled = false;
        bool First_Time_Setup = true;

        // Game's Original Variables
        float Menu_Horizontal_Aspect_Ratio;
        float Menu_Vertical_Aspect_Ratio;

        // Our own Current Variables
        float Menu_New_Horizontal_Aspect_Ratio;
        float Menu_New_Vertical_Aspect_Ratio;

        // Delegate for Window Adjustment
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        public WinEventDelegate Heroes_Window_Move_Hook_Delegate_Graphics_Menu;

        /// <summary>
        /// Constructor
        /// </summary>
        public Feature_Enhanced_FOV_Fix()
        {
            // Initializes the FOV and Aspect Ratio Fixes
            Initialize_FOV_Aspect_Ratio_Fix();
        }

        /// <summary>
        /// When pressing the button to set the aspect ratio fix.
        /// </summary>
        public void Button_Set_FOV_Aspect_Ratio_Fix()
        {
            // Calculate FOV to be set.
            Get_New_Field_Of_View(); 

            // Change Internal Toggle State.
            if (Menu_Aspect_Ratio_Fix_Toggle_Enabled) { Menu_Aspect_Ratio_Fix_Toggle_Enabled = false; }
            else { Menu_Aspect_Ratio_Fix_Toggle_Enabled = true; }

            // Sets calculated FOV Scale
            Set_Field_Of_View_Scale(); 
        }

        /// <summary>
        /// Returns whether the Enhanced FOV Fix is enabled.
        /// </summary>
        public bool Get_IsEnabled() { return Menu_Aspect_Ratio_Fix_Toggle_Enabled; }

        /// <summary>
        /// Return the active field of view scale.
        /// </summary>
        /// <returns></returns>
        public float Get_FOVScale() { return Menu_Horizonal_Field_Of_View_Scale; }

        // Initializes the FOV/Aspect Ratio Fix
        public void Initialize_FOV_Aspect_Ratio_Fix()
        {
            // Read Stock FoV
            Get_Stock_Field_Of_View(); // Calculate Stock Aspect Ratio

            // Add to the delegate in the transparent overlay Window to act on changes based on Window Size and Location.
            Heroes_Window_Move_Hook_Delegate_Graphics_Menu = new WinEventDelegate(WinEventProc);

            // Set Delegate
            SetWinEventHook(WINAPI_Components.EVENT_OBJECT_LOCATIONCHANGE, WINAPI_Components.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, Heroes_Window_Move_Hook_Delegate_Graphics_Menu, 0, 0, WINAPI_Components.WINEVENT_OUTOFCONTEXT);

            // Allocate Memory to Store HUD Centering Value
            Menu_Center_Value_Address = Program.Sonic_Heroes_Process.AllocateMemory(sizeof(float));
            Menu_Vertical_Center_Value_Address = Program.Sonic_Heroes_Process.AllocateMemory(sizeof(float));
            FOV_ASM_Menu_Horizontal_Value_Address = Program.Sonic_Heroes_Process.AllocateMemory(sizeof(float));
            FOV_ASM_Menu_Vertical_Value_Address = Program.Sonic_Heroes_Process.AllocateMemory(sizeof(float));

            // Write Default Values
            Program.Sonic_Heroes_Process.WriteMemory(Menu_Center_Value_Address, BitConverter.GetBytes(1.00F)); // Make sure to not offset menus, this value will be auto set when entering a stage.
            Program.Sonic_Heroes_Process.WriteMemory(Menu_Vertical_Center_Value_Address, BitConverter.GetBytes(1.00F)); // Make sure to not offset menus, this value will be auto set when entering a stage.
            Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Horizontal_Value_Address, BitConverter.GetBytes(Menu_Horizonal_Field_Of_View_Scale));
            Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Vertical_Value_Address, BitConverter.GetBytes(Menu_Vertical_Field_Of_View_Scale));

            // Backup the original instructions.
            ASM_HUD_FOV_Horizontal_Instruction_Original = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)UNKNOWN_HORIZONTAL_POSITION_ASM_INSTRUCTION, 6); // Not in Variables because real purpose is unknown. Shifts UI.
            ASM_HUD_FOV_Vertical_Instruction_Original = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)UNKNOWN_VERTICAL_POSITION_ASM_INSTRUCTION, 5); // Not in Variables because real purpose is unknown. Shifts UI.
            ASM_Menu_FOV_Instruction_Horizontal_Original = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)HORIZONTAL_FOV_ASM_INSTRUCTION, 6); // Original Instruction.
            ASM_Menu_FOV_Instruction_Vertical_Original = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)VERTICAL_FOV_ASM_INSTRUCTION, 6); // Original Instruction.
            Victory_Screen_FOV_X_Instruction_Original = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)VICTORY_SCREEN_X_ASPECTRATIO_INSTRUCTION, 6); // Original Instruction.
            Victory_Screen_FOV_Y_Instruction_Original = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)VICTORY_SCREEN_Y_ASPECTRATIO_INSTRUCTION, 6); // Original Instruction.

            // Write the new instructions.
            ASM_HUD_FOV_Horizontal_Instruction_Override.AddRange(new byte[] { 0x8B, 0x15 }); // mov edx
            ASM_HUD_FOV_Horizontal_Instruction_Override.AddRange(BitConverter.GetBytes((int)Menu_Center_Value_Address)); // Address for mov edx.

            // Write the new instructions.
            ASM_HUD_FOV_Vertical_Instruction_Override.AddRange(new byte[] { 0xA1 }); // mov eax
            ASM_HUD_FOV_Vertical_Instruction_Override.AddRange(BitConverter.GetBytes((int)Menu_Vertical_Center_Value_Address)); // Address for mov eax.

            ASM_Menu_FOV_Instruction_Horizontal_Override.AddRange(new byte[] { 0xD9, 0x05 }); // fld
            ASM_Menu_FOV_Instruction_Horizontal_Override.AddRange(BitConverter.GetBytes((int)FOV_ASM_Menu_Horizontal_Value_Address)); // Address for fld.

            ASM_Menu_FOV_Instruction_Vertical_Override.AddRange(new byte[] { 0xD9, 0x05 }); // fld
            ASM_Menu_FOV_Instruction_Vertical_Override.AddRange(BitConverter.GetBytes((int)FOV_ASM_Menu_Vertical_Value_Address)); // Address for fld.

            Victory_Screen_FOV_X_Instruction_Override.AddRange(new byte[] { 0xD8, 0x0D }); // FMUL
            Victory_Screen_FOV_Y_Instruction_Override.AddRange(new byte[] { 0xD8, 0x0D }); // FMUL
            Victory_Screen_FOV_X_Instruction_Override.AddRange(BitConverter.GetBytes((int)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Horizontal_Aspect_Ratio)); // Point address to X FOV
            Victory_Screen_FOV_Y_Instruction_Override.AddRange(BitConverter.GetBytes((int)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Vertical_Aspect_Ratio)); // Point address to Y FOV

            // Enable Menu FOV Fix on Game Launch
            Set_Menu_Scale();

            // Set Hooks
            Menu_Enter_Hook = new SonicHeroes.Hooking.Injection((IntPtr)0x6B842C, (Enable_Menu_FOV)Enable_Menu_Scaling, 6, Program.Sonic_Heroes_Networking_Client, false);
            Menu_Exit_Hook = new SonicHeroes.Hooking.Injection((IntPtr)0x6B84FC, (Enable_Menu_FOV)Disable_Menu_Scaling, 8, Program.Sonic_Heroes_Networking_Client, false);
            Results_Enter_Hook = new SonicHeroes.Hooking.Injection((IntPtr)0x6CECB0, (Set_Results_Menu_Location_Live)Set_Results_Menu_Location, 6, Program.Sonic_Heroes_Networking_Client, false);
            Menu_Enter_Hook.Activate();
            Menu_Exit_Hook.Activate();
            //Results_Enter_Hook.Activate();
        }


        // Set Main Menu Aspect Ratio
        public void Set_Main_Menu_Aspect_Ratio()
        {
            // On startup, can't use the resolution of the overlay window, use the executable defined resolution.
            if (First_Time_Setup) { Set_Menu_Aspect_FirstTime(); First_Time_Setup = false; }
            else if (Menu_Aspect_Ratio_Fix_Toggle_Enabled) { Set_Menu_Aspect(); }
            else 
            {
                /// If not enabled, do not use, reset the values.
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Horizontal_Value_Address, BitConverter.GetBytes(1.00F));
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Vertical_Value_Address, BitConverter.GetBytes(1.00F));
            }
        }

        /// <summary>
        /// Increments the field of view by 1.00F and sets it in game.
        /// </summary>
        public void Increment_Field_Of_View_Scale()
        {
            // Increment the FOV
            Menu_Horizonal_Field_Of_View_Scale = Menu_Horizonal_Field_Of_View_Scale += 0.01F;

            // Set the scale
            Set_Field_Of_View_Scale();
        }

        /// <summary>
        /// Decrements the field of view by 1.00F and sets it in game.
        /// </summary>
        public void Decrement_Field_Of_View_Scale()
        {
            // Increment the FOV
            Menu_Horizonal_Field_Of_View_Scale = Menu_Horizonal_Field_Of_View_Scale -= 0.01F;

            // Set the scale
            Set_Field_Of_View_Scale();
        }

        /// <summary>
        /// Disables the main menu scaling when loading a level.
        /// </summary>
        public void Disable_Menu_Scaling()
        {
            if (Menu_Aspect_Ratio_Fix_Toggle_Enabled)
            {
                // Write ASM Override
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)HORIZONTAL_FOV_ASM_INSTRUCTION, ASM_Menu_FOV_Instruction_Horizontal_Original); // Not in Variables because real purpose is unknown. Shifts UI.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)VERTICAL_FOV_ASM_INSTRUCTION, ASM_Menu_FOV_Instruction_Vertical_Original); // Not in Variables because real purpose is unknown. Shifts UI.
                Program.Sonic_Heroes_Process.WriteMemory(Menu_Center_Value_Address, BitConverter.GetBytes(Menu_Center_Value_FOV));
                Get_New_Field_Of_View();
                Set_Field_Of_View_Scale();
            }
        }

        /// <summary>
        /// Enables the main menu scaling when exiting a level.
        /// </summary>
        public void Enable_Menu_Scaling()
        {
            if (Menu_Aspect_Ratio_Fix_Toggle_Enabled)
            {
                Set_Menu_Scale();
            }
        }

        /// <summary>
        /// Enables the main menu scaling when exiting a level.
        /// </summary>
        private void Set_Menu_Scale()
        {
            // Write ASM Override
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)HORIZONTAL_FOV_ASM_INSTRUCTION, ASM_Menu_FOV_Instruction_Horizontal_Override.ToArray()); // Not in Variables because real purpose is unknown. Shifts UI.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)VERTICAL_FOV_ASM_INSTRUCTION, ASM_Menu_FOV_Instruction_Vertical_Override.ToArray()); // Not in Variables because real purpose is unknown. Shifts UI.
            Program.Sonic_Heroes_Process.WriteMemory(Menu_Center_Value_Address, BitConverter.GetBytes(1.00F));
            Set_Main_Menu_Aspect_Ratio();
        }

        /// <summary>
        /// Sets the current field of View Scale according to settings..
        /// </summary>
        public void Set_Field_Of_View_Scale()
        {
            // If Aspect Ratio Toggle Enabled
            if (Menu_Aspect_Ratio_Fix_Toggle_Enabled)
            {
                // Check if in level.
                byte In_Level = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);
                if (In_Level == 1)
                {
                    Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Horizontal_Value_Address, BitConverter.GetBytes(1.00F)); // Reset Visual Squishing
                    Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Vertical_Value_Address, BitConverter.GetBytes(1.00F)); // Reset Visual Squishing
                    Program.Sonic_Heroes_Process.WriteMemory(Menu_Center_Value_Address, BitConverter.GetBytes(Menu_Center_Value_FOV)); // Move HUD Position
                }

                // Write ASM Override
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)UNKNOWN_HORIZONTAL_POSITION_ASM_INSTRUCTION, ASM_HUD_FOV_Horizontal_Instruction_Override.ToArray()); // Not in Variables because real purpose is unknown. Shifts UI.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)UNKNOWN_VERTICAL_POSITION_ASM_INSTRUCTION, ASM_HUD_FOV_Vertical_Instruction_Override.ToArray()); // Not in Variables because real purpose is unknown. Shifts UI.

                // Write Victory Screen Override
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Victory_Screen_Aspect_Ratio_Horizontal_ASM_Instruction, Victory_Screen_FOV_X_Instruction_Override.ToArray());
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Victory_Screen_Aspect_Ratio_Vertical_ASM_Instruction, Victory_Screen_FOV_Y_Instruction_Override.ToArray());

                // Write new Memory Values.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.HUD_Aspect_Ratio_Horizontal, BitConverter.GetBytes(Menu_New_Horizontal_Aspect_Ratio));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.HUD_Aspect_Ratio_Vertical, BitConverter.GetBytes(Menu_New_Vertical_Aspect_Ratio));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Horizontal_Aspect_Ratio, BitConverter.GetBytes(Menu_New_Horizontal_Aspect_Ratio * Menu_Horizonal_Field_Of_View_Scale));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Vertical_Aspect_Ratio, BitConverter.GetBytes(Menu_New_Vertical_Aspect_Ratio * Menu_Horizonal_Field_Of_View_Scale));
            }
            else
            {
                // Center HUD
                Program.Sonic_Heroes_Process.WriteMemory(Menu_Center_Value_Address, BitConverter.GetBytes(1.00F));

                // Set the aspect ratios accordingly.
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Horizontal_Value_Address, BitConverter.GetBytes(Menu_Horizonal_Field_Of_View_Scale));
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Horizontal_Value_Address, BitConverter.GetBytes(Menu_Vertical_Field_Of_View_Scale));

                // Write ASM Override
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)UNKNOWN_HORIZONTAL_POSITION_ASM_INSTRUCTION, ASM_HUD_FOV_Horizontal_Instruction_Original); // Not in Variables because real purpose is unknown. Shifts UI.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)UNKNOWN_VERTICAL_POSITION_ASM_INSTRUCTION, ASM_HUD_FOV_Vertical_Instruction_Original); // Not in Variables because real purpose is unknown. Shifts UI.

                // Write Victory Screen Override
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Victory_Screen_Aspect_Ratio_Horizontal_ASM_Instruction, Victory_Screen_FOV_X_Instruction_Original);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Victory_Screen_Aspect_Ratio_Vertical_ASM_Instruction, Victory_Screen_FOV_Y_Instruction_Original);

                // Write new Memory Values.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.HUD_Aspect_Ratio_Horizontal, BitConverter.GetBytes(Menu_Horizontal_Aspect_Ratio));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.HUD_Aspect_Ratio_Vertical, BitConverter.GetBytes(Menu_Vertical_Aspect_Ratio));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Horizontal_Aspect_Ratio, BitConverter.GetBytes(Menu_Horizontal_Aspect_Ratio * Menu_Horizonal_Field_Of_View_Scale));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Vertical_Aspect_Ratio, BitConverter.GetBytes(Menu_Vertical_Aspect_Ratio * Menu_Horizonal_Field_Of_View_Scale));
            }
        }

        /// Process delegate to use with the event fired when window gets moved.
        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // Filter out non-HWND namechanges, e.g. items within a listbox.
            if (idObject != 0 || idChild != 0) { return; }

            // Set size and location
            if (Menu_Aspect_Ratio_Fix_Toggle_Enabled)
            {
                Get_New_Field_Of_View();
                Set_Field_Of_View_Scale();
            }

        }

        /// <summary>
        /// Sets the menu aspect ratio to be used for displaying the menu.
        /// </summary>
        private void Set_Menu_Aspect()
        {
            // Get all necessary resolutions.
            float NeW_Resolution_X = Program.Sonic_Heroes_Overlay.overlayWinForm.Width;
            float NeW_Resolution_Y = Program.Sonic_Heroes_Overlay.overlayWinForm.Height;
            float Resolution_640_480_Scale = 1.33333333333333333F;

            float Resolution_Menu_Scale; // Default Scale
            if (NeW_Resolution_Y > NeW_Resolution_X)
            {
                Resolution_Menu_Scale = (Resolution_640_480_Scale / NeW_Resolution_Y) * NeW_Resolution_X;
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Horizontal_Value_Address, BitConverter.GetBytes(1.00F));
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Vertical_Value_Address, BitConverter.GetBytes(Resolution_Menu_Scale));
            }
            else
            {
                Resolution_Menu_Scale = (Resolution_640_480_Scale / NeW_Resolution_X) * NeW_Resolution_Y;
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Horizontal_Value_Address, BitConverter.GetBytes(Resolution_Menu_Scale));
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Vertical_Value_Address, BitConverter.GetBytes(1.00F));
            }
        }

        /// <summary>
        /// Sets the menu aspect ratio for the first time, by reading resolution from game memory.
        /// </summary>
        private void Set_Menu_Aspect_FirstTime()
        {
            // Get all necessary resolutions.
            float NeW_Resolution_X = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)0x7C9290, 4);
            float NeW_Resolution_Y = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)0x7C9294, 4);
            float Resolution_640_480_Scale = 1.33333333333333333F;

            float Resolution_Menu_Scale; // Default Scale
            if (NeW_Resolution_Y > NeW_Resolution_X)
            {
                Resolution_Menu_Scale = (Resolution_640_480_Scale / NeW_Resolution_Y) * NeW_Resolution_X;
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Horizontal_Value_Address, BitConverter.GetBytes(1.00F));
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Vertical_Value_Address, BitConverter.GetBytes(Resolution_Menu_Scale));
            }
            else
            {
                Resolution_Menu_Scale = (Resolution_640_480_Scale / NeW_Resolution_X) * NeW_Resolution_Y;
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Horizontal_Value_Address, BitConverter.GetBytes(Resolution_Menu_Scale));
                Program.Sonic_Heroes_Process.WriteMemory(FOV_ASM_Menu_Vertical_Value_Address, BitConverter.GetBytes(1.00F));
            }
        }

        /// <summary>
        /// Fixes the results menu location, currently disabled.
        /// </summary>
        private void Set_Results_Menu_Location()
        {
            // Check and apply HUD move if in level.
            byte In_Level = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);
            if (In_Level == 1 && Menu_Aspect_Ratio_Fix_Toggle_Enabled == true)
            {
                Program.Sonic_Heroes_Process.WriteMemory(Menu_Center_Value_Address, BitConverter.GetBytes(1.77777F));
            }
        }

        /// <summary>
        /// Retrieves the stock field of View
        /// </summary>
        private void Get_Stock_Field_Of_View()
        {
            Menu_Horizontal_Aspect_Ratio = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Horizontal_Aspect_Ratio, 4);
            Menu_Vertical_Aspect_Ratio = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Vertical_Aspect_Ratio, 4);
            Menu_New_Vertical_Aspect_Ratio = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_Field_Of_View_and_Aspect_Ratio.Vertical_Aspect_Ratio, 4);
        }

        // Get Stock Field of View
        private void Get_New_Field_Of_View()
        {
            // Wait for Window to Complete Setup if Incomplete
            while (Program.Sonic_Heroes_Overlay.overlayWinForm.Window_Setup_Complete == false) { }

            // Get all necessary resolutions.
            float NeW_Resolution_X = Program.Sonic_Heroes_Overlay.overlayWinForm.Width;
            float NeW_Resolution_Y = Program.Sonic_Heroes_Overlay.overlayWinForm.Height;

            if (NeW_Resolution_Y > NeW_Resolution_X)
            {
                // Set the aspect ratios accordingly.
                Menu_New_Vertical_Aspect_Ratio = (NeW_Resolution_Y / NeW_Resolution_X) * Menu_Horizontal_Aspect_Ratio; // Vertical FoV Remains Unchanged
                Menu_New_Horizontal_Aspect_Ratio = Menu_Horizontal_Aspect_Ratio;
            }
            else
            {
                // Set the aspect ratios accordingly.
                Menu_New_Horizontal_Aspect_Ratio = (NeW_Resolution_X / NeW_Resolution_Y) * Menu_Vertical_Aspect_Ratio; // Vertical FoV Remains Unchanged
                Menu_New_Vertical_Aspect_Ratio = Menu_Vertical_Aspect_Ratio;
            }

        }

        /// <summary>
        /// P/Invoke. Set the Windows Window event hook.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
    }
}
