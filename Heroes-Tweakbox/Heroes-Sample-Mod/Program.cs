using System;
using SonicHeroes.Functions;
using SonicHeroes.Controller;
using SonicHeroes.Memory;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Text;
using static SonicHeroes.Networking.Client_Functions;
using SharpDX.Direct2D1;
using System.Threading;
using System.Globalization;

namespace Heroes_Sample_Mod
{
    public class Program
    {
        /// Mod loader DLL Skeleton Code
        public const string Mod_Name = "Heroes Tweakbox"; // Set name of project.
        public static SonicHeroes.Networking.WebSocket_Client Sonic_Heroes_Networking_Client = new SonicHeroes.Networking.WebSocket_Client(); // Set up client for networking, this client communicates with the server to call methods under subscribe hook
        public static Process Sonic_Heroes_Process; // Get Sonic Heroes Process
        /// Mod loader DLL Skeleton Code

        /// Global Settings
        public static float Menu_Horizontal_Percentage = 50.0F;
        public static float Menu_Vertical_Percentage = 50.0F;

        /// <summary>
        /// Your program starts here!
        /// </summary>
        [DllExport]
        static void Main()
        {
            try
            {
                // Set Culture
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

                ////////////// MOD LOADER DLL SKELETON CODE ///////////////////////////////////////////////////////////////////////////////////////////////////
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(SonicHeroes.Misc.SonicHeroes_Miscallenous.CurrentDomain_SetAssemblyResolve);
                Sonic_Heroes_Networking_Client.SetupClient(IPAddress.Loopback); /// Set up networking with the Sonic Heroes Mod Loader.
                byte[] Response = Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " | Loading... OK!"), true); /// Say to the Mod Loader that we have loaded so the end user can know.
                Sonic_Heroes_Process = Process.GetCurrentProcess(); /// We will use this for reading and writing memory.
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Enable Aero Glass if Disabled
                // Check to see if composition is Enabled
                if (!Invoke_External_Class.DwmIsCompositionEnabled()) { Invoke_External_Class.DwmEnableComposition(true); }

                // Sets up DirectX & Direct2D
                Sonic_Heroes_Overlay = new SonicHeroes.Overlay.Overlay_External_Direct2D();
                Sonic_Heroes_Overlay.Initialize_DirectX();
                Sonic_Heroes_Overlay.direct2DRenderMethod = Draw_DirectX;
                Windows_Form_Thread = new Thread(() => { while (true) { Application.Run(Sonic_Heroes_Overlay.overlayWinForm); } });
                Windows_Form_Thread.Start();

                // Get Original OnFrame Controller Poll Instruction (Used for Controller Disabling)
                Original_Controller_Call_OnFrame = Sonic_Heroes_Process.ReadMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Master_Function_Control_OnFrame_Call, 5);

                // Initialize Controller
                Controller_Manager = new DirectInput_Joystick_Manager();

                // Initialize Thread to Update Controller Inputs.
                Heroes_Controller_Update_Thread = new Thread(() => {
                    while (true) { Controller_Poll_Method(); Thread.Sleep(16); }
                }
                );

                // Initialize Thread to Update GUI
                Heroes_GUI_Update_Thread = new Thread(() => {
                    while (true) { DirectX_Render_Method(); Thread.Sleep(16); }
                });

                // Run the Two Individual Threads
                Heroes_GUI_Update_Thread.Start();
                Heroes_Controller_Update_Thread.Start();

                // Initialize Features
                Feature_PAC_Clip_Player_X = new Feature_PAC_Utilities();
                Feature_Force_Load_Level_X = new Feature_Force_Load_Level();
                Feature_ADX_Player_X = new Feature_ADX_Utilities();
                Feature_AFS_Utilities_X = new Feature_AFS_Utilities();
                Feature_Toggle_Character_Chatter_X = new Feature_Toggle_Character_Chatter();
                Feature_Toggle_Music_OnPause_X = new Feature_Toggle_Music_OnPause();
                Feature_Physics_Swapper_X = new Feature_Physics_Swap();
                Feature_Magnetic_Barrier_X = new Feature_Magnetic_Barrier();
                Feature_Party_Mode_X = new Feature_Party_Mode();
                Feature_Toggle_Moveset_Restrictions_X = new Feature_Toggle_Moveset_Restrictions();
                Feature_Invisibility_Fixes_X = new Feature_Invisbility_Fixes();
                Feature_Cycle_RGB_Colours_X = new Feature_Cycle_RGB_Colours();
                Feature_Minimal_HUD_X = new Feature_Minimal_HUD();
                Feature_Trail_Editor_X = new Feature_Trail_Editor();
                Feature_Toggle_Super_Metal_Characters_X = new Feature_Toggle_Super_Metal_Characters();
                Feature_Position_XYZ_Window_X = new Feature_Position_XYZ_Window();
                Feature_Enhanced_Debug_Movement_Mode_X = new Feature_Enhanced_Debug_Movement_Mode();
                Feature_Set_Editor_X = new Feature_SET_Editor();
                Feature_Enhanced_FOV_Fix_X = new Feature_Enhanced_FOV_Fix();
                Feature_Enhanced_FOV_Fix_X.Button_Set_FOV_Aspect_Ratio_Fix();

                // Initialize Menus
                Menu_Main = new Menu_Main_Menu();
                Menu_Sound_Test = new Menu_Sound_Test();
                Menu_Graphics_Tweaks = new Menu_Graphics_Tweaks();
                Menu_Trail_Editor = new Menu_Trail_Editor();
                Menu_Miscallenous = new Menu_Misc_Menu();
                Menu_Moveset_Tweaks = new Menu_Moveset_Tweaks();
                Menu_Experiments = new Menu_Experiments();
                Menu_Gameplay_Items = new Menu_Gameplay_Items();
                Menu_Physics_Swapper = new Menu_Physics_Swapper();
                Menu_Debug_Stuff = new Menu_Debug_Stuff();
                Menu_HUD_Adjustments = new Menu_HUD_Adjustments();

                // Set the current startup menu.
                Current_Menu = Menu_Main;

                // Load Station Square
                Feature_Toggle_Character_Chatter_X.Toggle_CharacterChatter();
                Feature_Toggle_Character_Chatter_X.Toggle_CharacterCommentChatter();
            }
            catch (Exception Ex) { Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " Failed To Load: " + Ex.Message + " | " + Ex.StackTrace), false); }
        }

        // libSonicHeroes Modules
        static SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager;
        public static SonicHeroes.Overlay.Overlay_External_Direct2D Sonic_Heroes_Overlay;

        // Parallel Processing Threads
        static Thread Heroes_Controller_Update_Thread; // Thread which will process updates.
        static Thread Heroes_GUI_Update_Thread; // Thread which will process updates.
        static Thread Windows_Form_Thread; // Thread which will process updates.

        // Direct2D Current Menu Properties
        static Menu_Base Current_Menu; // Current Opened Menu
        static Menu_Base Last_Menu; // Current Opened Menu

        // Flags
        public static bool Draw_Current_Menu = true; // Shows current menu to the screen.

        // Screen Render Hook
        public static SonicHeroes.Hooking.Injection DirectX_Draw_Hook;
        public static SonicHeroes.Hooking.Injection DirectX_Draw_Hook_II;
        public static SonicHeroes.Hooking.Injection DirectX_Draw_Hook_III;

        // All Menus
        public static Menu_Main_Menu Menu_Main;
        public static Menu_Sound_Test Menu_Sound_Test;
        public static Menu_Graphics_Tweaks Menu_Graphics_Tweaks;
        public static Menu_Trail_Editor Menu_Trail_Editor;
        public static Menu_Misc_Menu Menu_Miscallenous;
        public static Menu_Moveset_Tweaks Menu_Moveset_Tweaks;
        public static Menu_Experiments Menu_Experiments;
        public static Menu_Gameplay_Items Menu_Gameplay_Items;
        public static Menu_Physics_Swapper Menu_Physics_Swapper;
        public static Menu_Debug_Stuff Menu_Debug_Stuff;
        public static Menu_HUD_Adjustments Menu_HUD_Adjustments;

        // All of the Individual Features.
        public static Feature_PAC_Utilities Feature_PAC_Clip_Player_X;
        public static Feature_ADX_Utilities Feature_ADX_Player_X;
        public static Feature_AFS_Utilities Feature_AFS_Utilities_X;
        public static Feature_Toggle_Character_Chatter Feature_Toggle_Character_Chatter_X;
        public static Feature_Toggle_Music_OnPause Feature_Toggle_Music_OnPause_X;
        public static Feature_Physics_Swap Feature_Physics_Swapper_X;
        public static Feature_Magnetic_Barrier Feature_Magnetic_Barrier_X;
        public static Feature_Party_Mode Feature_Party_Mode_X;
        public static Feature_Toggle_Moveset_Restrictions Feature_Toggle_Moveset_Restrictions_X;
        public static Feature_Invisbility_Fixes Feature_Invisibility_Fixes_X;
        public static Feature_Enhanced_FOV_Fix Feature_Enhanced_FOV_Fix_X;
        public static Feature_Cycle_RGB_Colours Feature_Cycle_RGB_Colours_X;
        public static Feature_Minimal_HUD Feature_Minimal_HUD_X;
        public static Feature_Trail_Editor Feature_Trail_Editor_X;
        public static Feature_Toggle_Super_Metal_Characters Feature_Toggle_Super_Metal_Characters_X;
        public static Feature_Position_XYZ_Window Feature_Position_XYZ_Window_X;
        public static Feature_Enhanced_Debug_Movement_Mode Feature_Enhanced_Debug_Movement_Mode_X;
        public static Feature_Force_Load_Level Feature_Force_Load_Level_X;
        public static Feature_SET_Editor Feature_Set_Editor_X;
        public static Feature_Station_Square Feature_Inject_Stage_I;
        public static Feature_Inject_Stage Feature_Inject_Stage_II;

        // Swaps Menus as Necessary
        public static void Set_New_Menu(Menu_Base New_Menu) { Last_Menu = Current_Menu; Current_Menu = New_Menu; Current_Menu.Pass_MessageBox_Message(); Current_Menu.Set_Toggle_State(); }
        public static void Set_New_SubMenu(Menu_Base New_Menu) { Current_Menu = New_Menu; Current_Menu.Pass_MessageBox_Message(); Current_Menu.Set_Toggle_State(); }
        public static void Return_Last_Menu() { Menu_Base Temp_Menu = Current_Menu; Current_Menu = Last_Menu; Last_Menu = Temp_Menu; Current_Menu.Pass_MessageBox_Message(); Current_Menu.Set_Toggle_State(); }

        // Delegates for additional controller checking.
        public delegate void Delegate_ControllerPoll(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager);
        public static Delegate_ControllerPoll Controller_Poll_Delegate;

        /// <summary>
        /// Original bytes used by game to call the check inputs function, accepts no inputs if the game is not focused.
        /// </summary>
        static byte[] Original_Controller_Call_OnFrame;

        /// <summary>
        /// Checks if the renderer is ready using a flag specified in Sonic_Heroes_Overlay, if it is, the renderer is called.
        /// </summary>
        static void DirectX_Render_Method() { Sonic_Heroes_Overlay.DirectX_Render(); }

        /// <summary>
        /// .The Main Drawing Thread for DirectX. Pretty much a delegate, passes rendering into the current form/menu.
        /// </summary>
        /// <param name="DirectX_Graphics_Window">Window that you are drawing upon.</param>
        static void Draw_DirectX(WindowRenderTarget DirectX_Graphics_Window) { if (Draw_Current_Menu && (Current_Menu != null)) { Current_Menu.Render_This_Menu(DirectX_Graphics_Window); } }

        /// <summary>
        /// Disables calls to controller polling method and resets all values. For when you want to enable the menu.
        /// </summary>
        static void Disable_Game_Controller() { Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Master_Function_Control_OnFrame_Call, new byte[5] { 0x90, 0x90, 0x90, 0x90, 0x90 }); }

        /// <summary>
        /// Enables calls to controller polling method and resets all values. For when you want to enable the menu.
        /// </summary>
        static void Enable_Game_Controller() { Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Master_Function_Control_OnFrame_Call, Original_Controller_Call_OnFrame); }

        /// <summary>
        /// Method to play a sound if the user holds arrow up on the controller.
        /// </summary>
        static void Controller_Poll_Method()
        {
            try
            {
                for (int x = 0; x < Controller_Manager.PlayerControllers.Count; x++)
                {
                    // Get Controller State!
                    Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller = Controller_Manager.PlayerControllers[x].Get_Whole_Controller_State();

                    // Set index
                    Menu_Base.controllerIndex = x;

                    // Toggle Menu Visibility with Guide Button
                    if (P1_Controller.ControllerButtons.Button_L3)
                    {
                        // Render next frame.
                        if (Draw_Current_Menu) { Draw_Current_Menu = false; Enable_Game_Controller(); Current_Menu.Set_Toggle_State(); }
                        else { Draw_Current_Menu = true; Disable_Game_Controller(); Current_Menu.Set_Toggle_State(); }

                        // Wait for L3 Release for toggling menu.
                        while (P1_Controller.ControllerButtons.Button_L3) { P1_Controller = Controller_Manager.PlayerControllers[Menu_Base.controllerIndex].Get_Whole_Controller_State(); Thread.Sleep(32); }
                    }

                    // Handle current menu screen.
                    if (Draw_Current_Menu && (Current_Menu != null)) { Current_Menu.Handle_Controller_Inputs(P1_Controller, Controller_Manager); }

                    // Try executing the controller handler.
                    try { Controller_Poll_Delegate?.Invoke(P1_Controller, Controller_Manager); } catch { }
                }
            }
            catch (Exception Ex) { Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " Failed To Load: " + Ex.Message + " | " + Ex.StackTrace), false); }
        }
    }
}