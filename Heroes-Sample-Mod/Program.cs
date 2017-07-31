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
                Direct2D_Drawing_Injection_Hook = new SonicHeroes.Hooking.Injection((IntPtr)SonicHeroes_Functions.Render_A_Frame.Render_Frame_Subroutine_Start, (Direct2D_Drawing_Injection_Delegate)Run_Subscribed_Method_DPAD_Injection, 5, Sonic_Heroes_Networking_Client);
                Direct2D_Drawing_Injection_Hook.Subscribe(Sonic_Heroes_Networking_Client, (Injection_Hook_Sample_Delegate)DPAD_Hook_Sample); // Subscribe to the methods to be ran on the hooked address.
                Direct2D_Drawing_Injection_Hook.Activate(); // Redirect the flow to our own game hook here.

                // Set up DirectX Rendering
                Sonic_Heroes_Overlay.Initialize_DirectX(); // Sets up a rendering device for DirectX
                Sonic_Heroes_Overlay.Direct2D_Render_Method = DrawSonicPosition_DirectX_DXThread; // Set the delegate to the DirectX drawing call.

                Application.Run(Sonic_Heroes_Overlay.OverlayForm); // Hold the overlay in a thread.
            }
            catch (Exception Ex) { MessageBox.Show(Mod_Name + " Failed To Load: " + Ex.Message); }
        }

        /// Own Variables
        static SonicHeroes.Hooking.Injection Direct2D_Drawing_Injection_Hook; // Hook Type: Injection.
        delegate void Injection_Hook_Sample_Delegate(); // Must create a delegate where program flow will be redirected to for each hook. Delegate return type and parameters must match method to be executed.
        delegate void Direct2D_Drawing_Injection_Delegate(); // Delegate to handle the subscribe function for the DPAD hook.
        static SonicHeroes.Overlay.SonicHeroes_Overlay Sonic_Heroes_Overlay = new SonicHeroes.Overlay.SonicHeroes_Overlay(); // Declare an instance of the Sonic Heroes overlay.

        /// <summary>
        /// Runs the DPAD_Injection_Subscribed_Method, this will also automatically run other methods subscribed to this address!
        /// </summary>
        static void Run_Subscribed_Method_DPAD_Injection()
        {
            Sonic_Heroes_Networking_Client.SendData(Message_Type.Client_Call_Call_Subscribed_Function, BitConverter.GetBytes((int)Direct2D_Drawing_Injection_Hook.HookAddress), false); /// Response forces waiting for completion of execution.
            return;
        }

        /// <summary>
        /// Sample DPAD hook code which is injected and ran on each frame.
        /// </summary>
        static void DPAD_Hook_Sample()
        {
            byte Is_Currently_In_Level = HeroesProcess.ReadMemory<byte>(Sonic_Heroes_Process, (IntPtr)SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);
            byte Is_Pause_Menu_Open = HeroesProcess.ReadMemory<byte>(Sonic_Heroes_Process, (IntPtr)SonicHeroesVariables.Game_CurrentState.IsIngamePauseMenuOpen, 1);

            if ((Is_Currently_In_Level == 1) && (Is_Pause_Menu_Open == 0)) // If the pause menu is not open and the player is in a stage.
            {
                // Draw Sonic Coordinates
                DrawSonicPosition_DirectX();
            }
            return;

        }

        /// Defines the one time calculated rectangle draw properties
        static Rectangle_XYZ_Overlay DrawProps = new Rectangle_XYZ_Overlay();
        static bool DrawProps_Set = false;

        /// <summary>
        /// Defines the drawing properties of the menu rectangle.
        /// </summary>
        public struct Rectangle_XYZ_Overlay
        {
            /// Generic
            public float Line_Spacing;
            public float Line_Height;

            /// DIRECTX
            public SharpDX.DirectWrite.TextFormat Text_Font_DirectX;
            public SharpDX.Direct2D1.SolidColorBrush Drawing_Brush_DirectX;
            public SharpDX.Direct2D1.SolidColorBrush Background_Brush_DirectX;

            //public SharpDX.Mathematics.Interop.RawRectangleF Rectangle_Menu_DirectX;
            public Rectangle Rectangle_Menu_DirectX;

            /// XYZ Locations
            public string X_Position;
            public string Y_Position;
            public string Z_Position;
        }

        /// <summary>
        /// Rendering by DirectX is carried out here.
        /// </summary>

        static void DrawSonicPosition_DirectX()
        {
            Sonic_Heroes_Overlay.DirectX_Render();
        }
        static void DrawSonicPosition_DirectX_DXThread(WindowRenderTarget DirectX_Graphics_Device)
        {
            try
            {
                // Get the character's XYZ Position.
                Get_XYZ_Position();

                // Set overlay properties.
                if (DrawProps_Set == false) { Setup_Menu_Drawing_Properties(DrawProps.X_Position, DrawProps.Y_Position, DrawProps.Z_Position, DirectX_Graphics_Device); }

                // Fill rectangle
                DirectX_Graphics_Device.FillRectangle(new RawRectangleF(DrawProps.Rectangle_Menu_DirectX.Left, DrawProps.Rectangle_Menu_DirectX.Top, DrawProps.Rectangle_Menu_DirectX.Right, DrawProps.Rectangle_Menu_DirectX.Bottom), DrawProps.Background_Brush_DirectX);
                DirectX_Graphics_Device.DrawText(DrawProps.X_Position, DrawProps.Text_Font_DirectX, new SharpDX.Mathematics.Interop.RawRectangleF((int)DrawProps.Rectangle_Menu_DirectX.Left + (int)DrawProps.Line_Spacing, (int)DrawProps.Rectangle_Menu_DirectX.Top + (int)DrawProps.Line_Spacing, (int)DrawProps.Rectangle_Menu_DirectX.Left + (int)999, (int)DrawProps.Rectangle_Menu_DirectX.Top + (int)999), DrawProps.Drawing_Brush_DirectX);
                DirectX_Graphics_Device.DrawText(DrawProps.Y_Position, DrawProps.Text_Font_DirectX, new SharpDX.Mathematics.Interop.RawRectangleF((int)DrawProps.Rectangle_Menu_DirectX.Left + (int)DrawProps.Line_Spacing, (int)DrawProps.Rectangle_Menu_DirectX.Top + (int)DrawProps.Line_Spacing + (int)DrawProps.Line_Height, (int)DrawProps.Rectangle_Menu_DirectX.Left + (int)999, (int)DrawProps.Rectangle_Menu_DirectX.Top + (int)999 + (int)DrawProps.Line_Height), DrawProps.Drawing_Brush_DirectX);
                DirectX_Graphics_Device.DrawText(DrawProps.Z_Position, DrawProps.Text_Font_DirectX, new SharpDX.Mathematics.Interop.RawRectangleF((int)DrawProps.Rectangle_Menu_DirectX.Left + (int)DrawProps.Line_Spacing, (int)DrawProps.Rectangle_Menu_DirectX.Top + (int)DrawProps.Line_Spacing + (int)DrawProps.Line_Height + (int)DrawProps.Line_Height, (int)DrawProps.Rectangle_Menu_DirectX.Left + (int)999, (int)DrawProps.Rectangle_Menu_DirectX.Top + (int)999 + (int)DrawProps.Line_Height + (int)DrawProps.Line_Height), DrawProps.Drawing_Brush_DirectX);
            }
            catch (Exception Ex) { MessageBox.Show(Ex.Message); }
        }

        /// <summary>
        /// This struct sets the XYZ position of the character.
        /// </summary>
        static void Get_XYZ_Position()
        {
            // Get XYZ
            int Character_Pointer = HeroesProcess.ReadMemory<int>(Sonic_Heroes_Process, (IntPtr)SonicHeroesVariables.Characters_Addresses.CurrentPlayerControlledCharacter_Pointer, 4);
            int Character_Memory_Position_X = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.XPosition;
            int Character_Memory_Position_Y = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.YPosition;
            int Character_Memory_Position_Z = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.ZPosition;

            // Gets the character Y position as a byte[] array and converts the byte[] array to a float.
            float Character_X_Position = HeroesProcess.ReadMemory<float>(Sonic_Heroes_Process, (IntPtr)Character_Memory_Position_X, 4);
            float Character_Y_Position = HeroesProcess.ReadMemory<float>(Sonic_Heroes_Process, (IntPtr)Character_Memory_Position_Y, 4);
            float Character_Z_Position = HeroesProcess.ReadMemory<float>(Sonic_Heroes_Process, (IntPtr)Character_Memory_Position_Z, 4);

            // Set the XYZ Position
            DrawProps.Y_Position = "Y-Position: " + Character_Y_Position.ToString("+00000.00000;-00000.00000;");
            DrawProps.X_Position = "X-Position: " + Character_X_Position.ToString("+00000.00000;-00000.00000;");
            DrawProps.Z_Position = "Z-Position: " + Character_Z_Position.ToString("+00000.00000;-00000.00000;");
        }

        /// <summary>
        /// Sets up the drawing properties
        /// </summary>
        /// <param name="X_Position"></param>
        /// <param name="Y_Position"></param>
        /// <param name="Z_Position"></param>
        static void Setup_Menu_Drawing_Properties(string X_Position, string Y_Position, string Z_Position, WindowRenderTarget DirectX_Graphics_Device)
        {


            // Set Font Properties
            DrawProps.Text_Font_DirectX = new SharpDX.DirectWrite.TextFormat(new SharpDX.DirectWrite.Factory(), "Consolas", 18);
            DrawProps.Drawing_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(255, 255, 255, 180));
            DrawProps.Background_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 150));

            // Alpha value is ignored! Use Opacity for Transparency!
            DrawProps.Background_Brush_DirectX.Opacity = 0.7F;

            // Measure Text Length
            var TextLayout_PosX = new TextLayout(new SharpDX.DirectWrite.Factory(), X_Position, DrawProps.Text_Font_DirectX, float.PositiveInfinity, float.PositiveInfinity);
            var TextLayout_PosY = new TextLayout(new SharpDX.DirectWrite.Factory(), Y_Position, DrawProps.Text_Font_DirectX, float.PositiveInfinity, float.PositiveInfinity);
            var TextLayout_PosZ = new TextLayout(new SharpDX.DirectWrite.Factory(), Z_Position, DrawProps.Text_Font_DirectX, float.PositiveInfinity, float.PositiveInfinity);

            // Calculate Line Spacing
            DrawProps.Line_Spacing = (TextLayout_PosX.Metrics.Height / 100.0F) * 20.0F;

            // Get Largest Width
            float Rectangle_Background_Width = TextLayout_PosX.Metrics.Width;
            if (TextLayout_PosY.Metrics.Width > Rectangle_Background_Width) { Rectangle_Background_Width = TextLayout_PosY.Metrics.Width; }
            if (TextLayout_PosZ.Metrics.Width > Rectangle_Background_Width) { Rectangle_Background_Width = TextLayout_PosZ.Metrics.Width; }

            // Get Largest Height
            float Rectangle_Background_Height = TextLayout_PosX.Metrics.Height;
            Rectangle_Background_Height += TextLayout_PosY.Metrics.Height;
            Rectangle_Background_Height += TextLayout_PosZ.Metrics.Height;

            // Set line height
            DrawProps.Line_Height = TextLayout_PosX.Metrics.Height;

            // Make Rectangle Bigger to fit Text
            Rectangle_Background_Height += (DrawProps.Line_Spacing);
            Rectangle_Background_Width += (DrawProps.Line_Spacing);

            // Give space for text to be encapsulated.
            Rectangle_Background_Width = Rectangle_Background_Width + DrawProps.Line_Spacing;
            Rectangle_Background_Height = Rectangle_Background_Height + DrawProps.Line_Spacing;

            // Get position that would place the rectangle to center of screen.
            int Window_Position_X = Sonic_Heroes_Overlay.OverlayForm.Width / 2;
            int Window_Position_Y = (int)(Sonic_Heroes_Overlay.OverlayForm.Height * 0.8);
            Point Rectangle_Location_Centered = new Point(Window_Position_X - (int)(Rectangle_Background_Width / 2.0F), Window_Position_Y - (int)(Rectangle_Background_Height / 2.0F));
            Size Rectangle_Size = new Size((int)(Rectangle_Background_Width), (int)(Rectangle_Background_Height));

            // Define Background Rectangle
            DrawProps.Rectangle_Menu_DirectX = new Rectangle(Rectangle_Location_Centered.X, Rectangle_Location_Centered.Y, Rectangle_Size.Width, Rectangle_Size.Height);

            DrawProps_Set = true;
        }

    }
}
