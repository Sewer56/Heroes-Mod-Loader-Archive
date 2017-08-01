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
        const string Mod_Name = "Mod Loader Direct2D Drawing Sample"; // Set name of project.
        static SonicHeroes.Networking.WebSocket_Client Sonic_Heroes_Networking_Client = new SonicHeroes.Networking.WebSocket_Client(); // Set up client for networking, this client communicates with the server to call methods under subscribe hook
        static SonicHeroes.Networking.WebSocket_Client Sonic_Heroes_Networking_Client_II = new SonicHeroes.Networking.WebSocket_Client(); // Set up client for networking, this one is for non-subscribed calls.
        static Process Sonic_Heroes_Process; // Get Sonic Heroes Process
        /// Mod loader DLL Skeleton Code

        // Settings
        const float Overlay_Position_Percent_X = 100.0F;
        const float Overlay_Position_Percent_Y = 100.0F;

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

                // Time for some hooking, we are hooking with an "Injection Hook" onto the subroutine responsible for rendering a frame to the screen. i.e. This will be ran every frame.
                Direct2D_Drawing_Injection_Hook = new SonicHeroes.Hooking.Injection((IntPtr)SonicHeroes_Functions.Render_A_Frame.Render_Frame_Subroutine_Start, (Direct2D_Drawing_Injection_Delegate)Run_Subscribed_Method_Direct2D_Injection, 5, Sonic_Heroes_Networking_Client);
                Direct2D_Drawing_Injection_Hook.Subscribe(Sonic_Heroes_Networking_Client, (Injection_Hook_Sample_Delegate)Direct2D_Hook_Sample); // Subscribe to the methods to be ran on the hooked address.
                Direct2D_Drawing_Injection_Hook.Activate(); // Redirect the flow to our own game hook here.

                // Set up DirectX Rendering.
                Sonic_Heroes_Overlay.Initialize_DirectX(); // Sets up a rendering device for DirectX
                Sonic_Heroes_Overlay.Direct2D_Render_Method = Draw_Sonic_Statistics; // Set the delegate to the DirectX drawing call.

                // Set capacity of array to store our information to be rendered out to the screen.
                Drawing_Properties.Sonic_Statistics = new String[6];

                Application.Run(Sonic_Heroes_Overlay.OverlayForm); // Hold the overlay in a thread.
            }
            catch (Exception Ex) { Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " Failed To Load: " + Ex.Message), false); }
        }

        /// Own Variables
        static SonicHeroes.Hooking.Injection Direct2D_Drawing_Injection_Hook; // Hook Type: Injection.
        delegate void Injection_Hook_Sample_Delegate(); // Must create a delegate where program flow will be redirected to for each hook. Delegate return type and parameters must match method to be executed.
        delegate void Direct2D_Drawing_Injection_Delegate(); // Delegate to handle the subscribe function for the D2D hook.
        static SonicHeroes.Overlay.SonicHeroes_Overlay Sonic_Heroes_Overlay = new SonicHeroes.Overlay.SonicHeroes_Overlay(); // Declare an instance of the Sonic Heroes overlay.

        /// <summary>
        /// Runs the Subscribed Update Direct2D Overlay Method, this will also automatically run other methods subscribed to this address!
        /// </summary>
        static void Run_Subscribed_Method_Direct2D_Injection()
        {
            Sonic_Heroes_Networking_Client.SendData(Message_Type.Client_Call_Call_Subscribed_Function, BitConverter.GetBytes((int)Direct2D_Drawing_Injection_Hook.HookAddress), false); 
            return;
        }

        /// <summary>
        /// Sample D2D hook code which is injected and ran on each frame.
        /// </summary>
        static void Direct2D_Hook_Sample()
        {
            byte Is_Currently_In_Level = HeroesProcess.ReadMemory<byte>(Sonic_Heroes_Process, (IntPtr)SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);
            byte Is_Pause_Menu_Open = HeroesProcess.ReadMemory<byte>(Sonic_Heroes_Process, (IntPtr)SonicHeroesVariables.Game_CurrentState.IsIngamePauseMenuOpen, 1);

            // If the pause menu is not open and the player is in a stage.
            if ((Is_Currently_In_Level == 1) && (Is_Pause_Menu_Open == 0))  { Sonic_Heroes_Overlay.DirectX_Render(); } // Draw Sonic Statistics
            else { Sonic_Heroes_Overlay.DirectX_Clear_Screen(); } // Render Nothing
            return;

        }

        /// Defines the one time calculated rectangle draw properties
        static DirectX_2D_Overlay Drawing_Properties = new DirectX_2D_Overlay();
        static bool Drawing_Properties_Set = false;

        /// <summary>
        /// Defines the drawing properties of the menu rectangle.
        /// </summary>
        public struct DirectX_2D_Overlay
        {
            /// Generic
            public float Line_Spacing;
            public float Line_Height;

            /// DIRECTX
            public SharpDX.DirectWrite.TextFormat Text_Font_DirectX;
            public SharpDX.Direct2D1.SolidColorBrush Drawing_Brush_DirectX;
            public SharpDX.Direct2D1.SolidColorBrush Background_Brush_DirectX;

            /// public SharpDX.Mathematics.Interop.RawRectangleF Rectangle_Menu_DirectX;
            public Rectangle Rectangle_Menu_DirectX;

            /// Formatted strings representing locations will be held here.
            public string[] Sonic_Statistics;
        }

        /// <summary>
        /// For more convenient array access of Sonic_Statistics Above.
        /// </summary>
        enum Sonic_Statistics
        {
            X_Position = 0,
            Y_Position = 1,
            Z_Position = 2,
            X_Velocity = 3,
            Y_Velocity = 4,
            Z_Velocity = 5,
        }

        /// <summary>
        /// Rendering by DirectX is carried out here.
        /// </summary>
        static void Draw_Sonic_Statistics(WindowRenderTarget DirectX_Graphics_Device)
        {
            try
            {
                // Get the character's XYZ Position.
                Get_Sonic_Statistics();

                // Set overlay properties.
                // 100.0F will overflow on both edges but the position will self correct to corner of game screen.
                if (Drawing_Properties_Set == false) { Setup_Menu_Drawing_Properties(Drawing_Properties.Sonic_Statistics, Overlay_Position_Percent_X, Overlay_Position_Percent_Y, DirectX_Graphics_Device); }

                // Fill BG Rectangle
                DirectX_Graphics_Device.FillRectangle(new RawRectangleF(Drawing_Properties.Rectangle_Menu_DirectX.Left, Drawing_Properties.Rectangle_Menu_DirectX.Top, Drawing_Properties.Rectangle_Menu_DirectX.Right, Drawing_Properties.Rectangle_Menu_DirectX.Bottom), Drawing_Properties.Background_Brush_DirectX);

                // Draw all text
                for (int x = 0; x < Drawing_Properties.Sonic_Statistics.Length; x++)
                {
                    // Text to Draw
                    DirectX_Graphics_Device.DrawText(Drawing_Properties.Sonic_Statistics[x], Drawing_Properties.Text_Font_DirectX, 

                    // Rectangle for Text Wrapping
                    new SharpDX.Mathematics.Interop.RawRectangleF
                    (
                        (int)Drawing_Properties.Rectangle_Menu_DirectX.Left + (int)Drawing_Properties.Line_Spacing, // Left Edge | Make Space Equal to Line Spacing
                        (int)Drawing_Properties.Rectangle_Menu_DirectX.Top + ( (int)Drawing_Properties.Line_Spacing * x ) + ((int)Drawing_Properties.Line_Height * x), // Top Edge 
                        float.PositiveInfinity, // Right Edge | No Text Wrap
                        float.PositiveInfinity // Bottom Edge | No Text Wrap
                    ),
                    
                    // Brush to use
                    Drawing_Properties.Drawing_Brush_DirectX);
                }
            }
            catch (Exception Ex) { Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " Exception Encountered: " + Ex.Message), false); }
        }

        /// <summary>
        /// This struct sets the XYZ position of the character.
        /// </summary>
        static void Get_Sonic_Statistics()
        {
            // Get XYZ
            int Character_Pointer = HeroesProcess.ReadMemory<int>(Sonic_Heroes_Process, (IntPtr)SonicHeroesVariables.Characters_Addresses.CurrentPlayerControlledCharacter_Pointer, 4);
            int Character_Memory_Position_X = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.XPosition;
            int Character_Memory_Position_Y = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.YPosition;
            int Character_Memory_Position_Z = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.ZPosition;
            int Character_Memory_Velocity_X = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.XVelocity;
            int Character_Memory_Velocity_Y = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.YVelocity;
            int Character_Memory_Velocity_Z = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.ZVelocity;

            // Gets the character Y position as a byte[] array and converts the byte[] array to a float.
            float Character_X_Position = HeroesProcess.ReadMemory<float>(Sonic_Heroes_Process, (IntPtr)Character_Memory_Position_X, 4);
            float Character_Y_Position = HeroesProcess.ReadMemory<float>(Sonic_Heroes_Process, (IntPtr)Character_Memory_Position_Y, 4);
            float Character_Z_Position = HeroesProcess.ReadMemory<float>(Sonic_Heroes_Process, (IntPtr)Character_Memory_Position_Z, 4);
            float Character_X_Velocity = HeroesProcess.ReadMemory<float>(Sonic_Heroes_Process, (IntPtr)Character_Memory_Velocity_X, 4);
            float Character_Y_Velocity = HeroesProcess.ReadMemory<float>(Sonic_Heroes_Process, (IntPtr)Character_Memory_Velocity_Y, 4);
            float Character_Z_Velocity = HeroesProcess.ReadMemory<float>(Sonic_Heroes_Process, (IntPtr)Character_Memory_Velocity_Z, 4);

            // Set the XYZ Position
            Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.X_Position] = "X-Position: " + Character_X_Position.ToString("+00000.00000;-00000.00000;");
            Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.Y_Position] = "Y-Position: " + Character_Y_Position.ToString("+00000.00000;-00000.00000;");
            Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.Z_Position] = "Z-Position: " + Character_Z_Position.ToString("+00000.00000;-00000.00000;");
            Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.X_Velocity] = "X-Velocity: " + Character_X_Velocity.ToString("+00000.00000;-00000.00000;");
            Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.Y_Velocity] = "Y-Velocity: " + Character_Y_Velocity.ToString("+00000.00000;-00000.00000;");
            Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.Z_Velocity] = "Z-Velocity: " + Character_Z_Velocity.ToString("+00000.00000;-00000.00000;");
        }

        /// <summary>
        /// Sets up the drawing properties
        /// </summary>
        /// <param name="Menu_Text_Array"></param>
        /// <param name="Window_Position_X">Window position between 0 and 100 across the overlay</param>
        /// <param name="Window_Position_Y">Window position between 0 and 100 across the overlay</param>
        /// <param name="DirectX_Graphics_Device"></param>
        static void Setup_Menu_Drawing_Properties(string[] Menu_Text_Array, float Window_Position_X, float Window_Position_Y, WindowRenderTarget DirectX_Graphics_Device)
        {
            // Set-up UI Scaling such as that the interface scales alongside the resolution used.
            float Standard_Resolution_Height = 720; // I write and test my stuff on 1280 x 720.
            int Standard_Font_Size = 14; // Standard font size to be used for display.
            float Window_Scale_Overlay = (float)Sonic_Heroes_Overlay.OverlayForm.Height / (float)Standard_Resolution_Height; // Get scaling factor.
            Standard_Font_Size = (int)(Standard_Font_Size * Window_Scale_Overlay); // Adjust Font Size.
            Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " | UI Scaling Scale: " + Window_Scale_Overlay), false);

            // Set Brushes and Fonts.
            Drawing_Properties.Text_Font_DirectX = new SharpDX.DirectWrite.TextFormat(new SharpDX.DirectWrite.Factory(), "Consolas", Standard_Font_Size);
            Drawing_Properties.Drawing_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(255, 255, 255, 180));
            Drawing_Properties.Background_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 150));
            Drawing_Properties.Background_Brush_DirectX.Opacity = 0.7F;  // Alpha value is ignored! Use Opacity for Transparency!

            // Obtain TextLayout in order to obtain text width and height properties.
            SharpDX.DirectWrite.TextLayout[] All_Text_Layouts = new SharpDX.DirectWrite.TextLayout[Menu_Text_Array.Length]; // Get all text layout info
            for (int x = 0; x < Menu_Text_Array.Length; x++) {
                All_Text_Layouts[x] = new TextLayout(new SharpDX.DirectWrite.Factory(), Menu_Text_Array[x], Drawing_Properties.Text_Font_DirectX, float.PositiveInfinity, float.PositiveInfinity);
            }

            // Calculate Line Height & Spacing (20% of line height).
            Drawing_Properties.Line_Height = All_Text_Layouts[0].Metrics.Height;
            Drawing_Properties.Line_Spacing = (All_Text_Layouts[0].Metrics.Height / 100.0F) * 20.0F;

            // Obtain Largest Width and Height
            float Rectangle_Background_Width = All_Text_Layouts[0].Metrics.Width;
            for (int x = 1; x < All_Text_Layouts.Length; x++) {
                if (All_Text_Layouts[x].Metrics.Width > Rectangle_Background_Width) { Rectangle_Background_Width = All_Text_Layouts[x].Metrics.Width; }
            }

            // Get Height of Rectangle
            float Rectangle_Background_Height = All_Text_Layouts[0].Metrics.Height;
            for (int x = 1; x < All_Text_Layouts.Length; x++) { Rectangle_Background_Height += All_Text_Layouts[x].Metrics.Height; Rectangle_Background_Height += (Drawing_Properties.Line_Spacing); }

            // Make Rectangle Bigger (Styling)
            Rectangle_Background_Width += (Drawing_Properties.Line_Spacing * 2);

            // Obtain X and Y position across the form for the selected location of item.
            int Window_Position_X_Local = (int)( ((Sonic_Heroes_Overlay.OverlayForm.Width / 100.0F) * Window_Position_X));
            int Window_Position_Y_Local = (int)( ((Sonic_Heroes_Overlay.OverlayForm.Height / 100.0F) * Window_Position_Y));

            // Get center location of Rectangle & Size
            // X is left edge. 
            // Y is right edge.
            Point Rectangle_Location = new Point(Window_Position_X_Local - (int)(Rectangle_Background_Width / 2.0F), Window_Position_Y_Local - (int)(Rectangle_Background_Height / 2.0F) );
            Size Rectangle_Size = new Size((int)(Rectangle_Background_Width), (int)(Rectangle_Background_Height));

            // Ensure Rectangle doesn't escape screen space.

            // If the right edge will escape the screen, make sure it does not.
            if ( (Rectangle_Size.Width + Rectangle_Location.X) > Sonic_Heroes_Overlay.OverlayForm.Width)
            {
                int Menu_Overflow_Pixels = (Rectangle_Size.Width + Rectangle_Location.X) - Sonic_Heroes_Overlay.OverlayForm.Width; // Get the amount of pixels rectangle escapes of screen.
                Rectangle_Location.X = Rectangle_Location.X - Menu_Overflow_Pixels; // Move the rectangle by the overflow amount.
                Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " | [Debug] Corrected Right Edge Overflow X | " + Menu_Overflow_Pixels), true);
            }

            // If the bottom edge will escape the screen, make sure it does not.
            if ((Rectangle_Size.Height + Rectangle_Location.Y) > Sonic_Heroes_Overlay.OverlayForm.Height)
            {
                int Menu_Overflow_Pixels = (Rectangle_Size.Height + Rectangle_Location.Y) - Sonic_Heroes_Overlay.OverlayForm.Height; // Get the amount of pixels rectangle escapes of screen.
                Rectangle_Location.Y = Rectangle_Location.Y - Menu_Overflow_Pixels; // Move the rectangle by the overflow amount.
                Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " | [Debug] Corrected Bottom Edge Overflow Y | " + Menu_Overflow_Pixels), true);
            }

            // Check for top and left edges of overlay if they escape the view.
            if (Rectangle_Location.X < 0) { Rectangle_Location.X = 0; }
            if (Rectangle_Location.Y < 0) { Rectangle_Location.Y = 0; }

            // Define Background Rectangle
            Drawing_Properties.Rectangle_Menu_DirectX = new Rectangle(Rectangle_Location.X, Rectangle_Location.Y, Rectangle_Size.Width, Rectangle_Size.Height);

            Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " | Overlay HxW | " + Sonic_Heroes_Overlay.OverlayForm.Height + "x" + Sonic_Heroes_Overlay.OverlayForm.Width), true);

            Drawing_Properties_Set = true;
        }

    }
}
