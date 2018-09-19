using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SonicHeroes.Networking.Client_Functions;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Reimplementation of the Direct2D Overlay Sample as a module. No rewrite, so technically there is a little bit of code redundancy.
    /// </summary>
    public class Feature_Position_XYZ_Window
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Feature_Position_XYZ_Window() { Drawing_Properties.Sonic_Statistics = new String[6]; }

        // Flags
        bool Overlay_Enabled = false;

        // Settings
        const float Overlay_Position_Percent_X = 100.0F;
        const float Overlay_Position_Percent_Y = 100.0F;

        /// <summary>
        /// Toggles Overlay Visibility.
        /// </summary>
        public void Toggle_Overlay_Status()
        {
            if (Overlay_Enabled)
            {
                Program.Sonic_Heroes_Overlay.direct2DRenderMethod -= Draw_Overlay;
                Overlay_Enabled = false;
            }
            else
            {
                Program.Sonic_Heroes_Overlay.direct2DRenderMethod += Draw_Overlay;
                Overlay_Enabled = true;
            }
        }

        /// <summary>
        /// Returns on whether the overlay status is enabled or disabled.
        /// </summary>
        /// <returns></returns>
        public bool Get_Overlay_Status() { return Overlay_Enabled; }

        /// <summary>
        /// Draws the overlay if necessary, i.e. if character is in a level.
        /// </summary>
        /// <param name="DirectX_Graphics_Window"></param>
        public void Draw_Overlay(WindowRenderTarget DirectX_Graphics_Window)
        {
            // Get Information of whether the overlay should be shown.
            byte Is_Currently_In_Level = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);

            // If the player is in a stage.
            if (Is_Currently_In_Level == 1)
            {
                Draw_Window(DirectX_Graphics_Window);
            }
        }

        /// <summary>
        /// Draws the XYZ Overlay Window
        /// </summary>
        /// <param name="DirectX_Graphics_Window"></param>
        public void Draw_Window(WindowRenderTarget DirectX_Graphics_Window)
        {
            try
            {
                // Get the character's XYZ Position.
                Get_Sonic_Statistics();

                // Set overlay properties.
                // 100.0F will overflow on both edges but the position will self correct to corner of game screen.
                if (Drawing_Properties_Set == false)
                {
                    Setup_Menu_Drawing_Properties(Drawing_Properties.Sonic_Statistics, Overlay_Position_Percent_X, Overlay_Position_Percent_Y, DirectX_Graphics_Window);
                }


                // Fill BG Rectangle
                DirectX_Graphics_Window.FillRectangle(new RawRectangleF(Drawing_Properties.Rectangle_Menu_DirectX.Left, Drawing_Properties.Rectangle_Menu_DirectX.Top, Drawing_Properties.Rectangle_Menu_DirectX.Right, Drawing_Properties.Rectangle_Menu_DirectX.Bottom), Drawing_Properties.Background_Brush_DirectX);

                // Draw all text
                for (int x = 0; x < Drawing_Properties.Sonic_Statistics.Length; x++)
                {
                    // Text to Draw
                    DirectX_Graphics_Window.DrawText(Drawing_Properties.Sonic_Statistics[x], Drawing_Properties.Text_Font_DirectX,

                    // Rectangle for Text Wrapping
                    new SharpDX.Mathematics.Interop.RawRectangleF
                    (
                        (int)Drawing_Properties.Rectangle_Menu_DirectX.Left + (int)Drawing_Properties.Line_Spacing, // Left Edge | Make Space Equal to Line Spacing
                        (int)Drawing_Properties.Rectangle_Menu_DirectX.Top + ((int)Drawing_Properties.Line_Spacing) + ((int)Drawing_Properties.Line_Height * x), // Top Edge 
                        float.PositiveInfinity, // Right Edge | No Text Wrap
                        float.PositiveInfinity // Bottom Edge | No Text Wrap
                    ),

                    // Brush to use
                    Drawing_Properties.Drawing_Brush_DirectX);
                }
                
            }
            catch { }
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
        /// This struct sets the XYZ position of the character.
        /// </summary>
        static void Get_Sonic_Statistics()
        {
            try
            {
                // Get XYZ
                int Character_Pointer = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)SonicHeroesVariables.Characters_Addresses.CurrentPlayerControlledCharacter_Pointer, 4);

                int Character_Memory_Position_X = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.XPosition;
                int Character_Memory_Position_Y = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.YPosition;
                int Character_Memory_Position_Z = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.ZPosition;
                int Character_Memory_Velocity_X = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.XVelocity;
                int Character_Memory_Velocity_Y = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.YVelocity;
                int Character_Memory_Velocity_Z = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.ZVelocity;

                // Gets the character Y position as a byte[] array and converts the byte[] array to a float.
                float Character_X_Position = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)Character_Memory_Position_X, 4);
                float Character_Y_Position = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)Character_Memory_Position_Y, 4);
                float Character_Z_Position = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)Character_Memory_Position_Z, 4);
                float Character_X_Velocity = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)Character_Memory_Velocity_X, 4);
                float Character_Y_Velocity = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)Character_Memory_Velocity_Y, 4);
                float Character_Z_Velocity = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)Character_Memory_Velocity_Z, 4);

                // Set the XYZ Position
                Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.X_Position] = "X-Position: " + Character_X_Position.ToString("+00000.00000;-00000.00000;");
                Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.Y_Position] = "Y-Position: " + Character_Y_Position.ToString("+00000.00000;-00000.00000;");
                Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.Z_Position] = "Z-Position: " + Character_Z_Position.ToString("+00000.00000;-00000.00000;");
                Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.X_Velocity] = "X-Velocity: " + Character_X_Velocity.ToString("+00000.00000;-00000.00000;");
                Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.Y_Velocity] = "Y-Velocity: " + Character_Y_Velocity.ToString("+00000.00000;-00000.00000;");
                Drawing_Properties.Sonic_Statistics[(int)Sonic_Statistics.Z_Velocity] = "Z-Velocity: " + Character_Z_Velocity.ToString("+00000.00000;-00000.00000;");
            }
            catch (Exception Ex) { Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes("Properties GET Exception: " + Ex.Message), false); }
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
            float Window_Scale_Overlay = (float)Program.Sonic_Heroes_Overlay.overlayWinForm.Height / (float)Standard_Resolution_Height; // Get scaling factor.
            Standard_Font_Size = (int)(Standard_Font_Size * Window_Scale_Overlay); // Adjust Font Size.

            // Set Brushes and Fonts.
            Drawing_Properties.Text_Font_DirectX = new SharpDX.DirectWrite.TextFormat(new SharpDX.DirectWrite.Factory(), "Consolas", Standard_Font_Size);
            Drawing_Properties.Drawing_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(255, 255, 255, 180));
            Drawing_Properties.Background_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 150));
            Drawing_Properties.Background_Brush_DirectX.Opacity = 0.7F;  // Alpha value is ignored! Use Opacity for Transparency!

            // Obtain TextLayout in order to obtain text width and height properties.
            SharpDX.DirectWrite.TextLayout[] All_Text_Layouts = new SharpDX.DirectWrite.TextLayout[Menu_Text_Array.Length]; // Get all text layout info
            for (int x = 0; x < Menu_Text_Array.Length; x++)
            {
                All_Text_Layouts[x] = new TextLayout(new SharpDX.DirectWrite.Factory(), Menu_Text_Array[x], Drawing_Properties.Text_Font_DirectX, float.PositiveInfinity, float.PositiveInfinity);
            }

            // Calculate Line Height & Spacing (20% of line height).
            Drawing_Properties.Line_Height = All_Text_Layouts[0].Metrics.Height;
            Drawing_Properties.Line_Spacing = (All_Text_Layouts[0].Metrics.Height / 100.0F) * 20.0F;

            // Obtain Largest Width and Height
            float Rectangle_Background_Width = All_Text_Layouts[0].Metrics.Width;
            for (int x = 1; x < All_Text_Layouts.Length; x++)
            {
                if (All_Text_Layouts[x].Metrics.Width > Rectangle_Background_Width) { Rectangle_Background_Width = All_Text_Layouts[x].Metrics.Width; }
            }

            // Get Height of Rectangle
            float Rectangle_Background_Height = (Drawing_Properties.Line_Spacing * 2.0F);
            for (int x = 0; x < All_Text_Layouts.Length; x++) { Rectangle_Background_Height += All_Text_Layouts[x].Metrics.Height; }

            // Make Rectangle Bigger (Styling)
            Rectangle_Background_Width += (Drawing_Properties.Line_Spacing * 2);

            // Obtain X and Y position across the form for the selected location of item.
            int Window_Position_X_Local = (int)(((Program.Sonic_Heroes_Overlay.overlayWinForm.Width / 100.0F) * Window_Position_X));
            int Window_Position_Y_Local = (int)(((Program.Sonic_Heroes_Overlay.overlayWinForm.Height / 100.0F) * Window_Position_Y));

            // Get center location of Rectangle & Size
            // X is left edge. 
            // Y is right edge.
            Point Rectangle_Location = new Point(Window_Position_X_Local - (int)(Rectangle_Background_Width / 2.0F), Window_Position_Y_Local - (int)(Rectangle_Background_Height / 2.0F));
            Size Rectangle_Size = new Size((int)(Rectangle_Background_Width), (int)(Rectangle_Background_Height));

            // Ensure Rectangle doesn't escape screen space.

            // If the right edge will escape the screen, make sure it does not.
            if ((Rectangle_Size.Width + Rectangle_Location.X) > Program.Sonic_Heroes_Overlay.overlayWinForm.Width)
            {
                int Menu_Overflow_Pixels = (Rectangle_Size.Width + Rectangle_Location.X) - Program.Sonic_Heroes_Overlay.overlayWinForm.Width; // Get the amount of pixels rectangle escapes of screen.
                Rectangle_Location.X = Rectangle_Location.X - Menu_Overflow_Pixels; // Move the rectangle by the overflow amount.
            }

            // If the bottom edge will escape the screen, make sure it does not.
            if ((Rectangle_Size.Height + Rectangle_Location.Y) > Program.Sonic_Heroes_Overlay.overlayWinForm.Height)
            {
                int Menu_Overflow_Pixels = (Rectangle_Size.Height + Rectangle_Location.Y) - Program.Sonic_Heroes_Overlay.overlayWinForm.Height; // Get the amount of pixels rectangle escapes of screen.
                Rectangle_Location.Y = Rectangle_Location.Y - Menu_Overflow_Pixels; // Move the rectangle by the overflow amount.
            }

            // Check for top and left edges of overlay if they escape the view.
            if (Rectangle_Location.X < 0) { Rectangle_Location.X = 0; }
            if (Rectangle_Location.Y < 0) { Rectangle_Location.Y = 0; }

            // Define Background Rectangle
            Drawing_Properties.Rectangle_Menu_DirectX = new Rectangle(Rectangle_Location.X, Rectangle_Location.Y, Rectangle_Size.Width, Rectangle_Size.Height);

            Drawing_Properties_Set = true;
        }
    }
}
