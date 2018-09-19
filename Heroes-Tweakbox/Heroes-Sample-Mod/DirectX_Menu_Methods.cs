using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.DirectWrite;
using System;
using System.Drawing;
using System.Windows.Forms;
using static Heroes_Sample_Mod.Menu_Base;
using static SonicHeroes.Networking.Client_Functions;
using System.Text;

namespace Heroes_Sample_Mod
{
    public class DirectX_Menu_Methods
    {
        /// <summary>
        /// Accepts a list of strings/text to be drawn, a set of Window Positions, output device & overlay for test drawing and drawing properties.
        /// </summary>
        /// <param name="Menu_Text_Array"></param>
        /// <param name="Window_Position_X"></param>
        /// <param name="Window_Position_Y"></param>
        /// <param name="DirectX_Graphics_Device"></param>
        /// <param name="Drawing_Properties"></param>
        /// <param name="Sonic_Heroes_Overlay"></param>
        public static Menu_Base.DirectX_2D_Overlay_Properties Get_Menu_Size_Location(string[] Menu_Text_Array, float Window_Position_X, float Window_Position_Y, WindowRenderTarget DirectX_Graphics_Device, Form Windows_Forms_Fake_Transparent_Overlay)
        {
            try
            {
                // Drawing Properties
                Menu_Base.DirectX_2D_Overlay_Properties Drawing_Properties = new Menu_Base.DirectX_2D_Overlay_Properties();

                // Set-up UI Scaling such as that the interface scales alongside the resolution used.
                float Standard_Resolution_Height = 720; // I write and test my stuff on 1280 x 720.
                int Standard_Font_Size = 18; // Standard font size to be used for display.
                int Title_Font_Size = 24; // Standard font size to be used for display.
                float Window_Scale_Overlay = (float)Windows_Forms_Fake_Transparent_Overlay.Height / (float)Standard_Resolution_Height; // Get scaling factor.
                Standard_Font_Size = (int)(Standard_Font_Size * Window_Scale_Overlay); // Adjust Font Size.
                Title_Font_Size = (int)(Title_Font_Size * Window_Scale_Overlay); // Adjust Font Size.

                // Set Brushes and Fonts.
                DirectX_2D_Overlay_Properties.Text_Font_DirectX = new SharpDX.DirectWrite.TextFormat(new SharpDX.DirectWrite.Factory(), "Consolas", Standard_Font_Size);
                DirectX_2D_Overlay_Properties.Title_Font_DirectX = new SharpDX.DirectWrite.TextFormat(new SharpDX.DirectWrite.Factory(), "Consolas", Title_Font_Size);
                DirectX_2D_Overlay_Properties.Title_Font_DirectX.ParagraphAlignment = ParagraphAlignment.Center;
                DirectX_2D_Overlay_Properties.Title_Font_DirectX.TextAlignment = TextAlignment.Center;
                DirectX_2D_Overlay_Properties.Text_Font_DirectX.TextAlignment = TextAlignment.Center;

                DirectX_2D_Overlay_Properties.Drawing_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(255, 255, 255, 180));
                DirectX_2D_Overlay_Properties.Title_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(0.78F, 0.78F, 0.78F, 180));
                DirectX_2D_Overlay_Properties.Background_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(0.109F, 0.109F, 0.109F, 150));
                DirectX_2D_Overlay_Properties.Background_Brush_DirectX.Opacity = 0.8F;  // Alpha value is ignored! Use Opacity for Transparency!

                // Obtain TextLayout in order to obtain text width and height properties.
                SharpDX.DirectWrite.TextLayout[] All_Text_Layouts = new SharpDX.DirectWrite.TextLayout[Menu_Text_Array.Length]; // Get all text layout info
                for (int x = 0; x < Menu_Text_Array.Length; x++)
                {
                    All_Text_Layouts[x] = new TextLayout(new SharpDX.DirectWrite.Factory(), Menu_Text_Array[x], DirectX_2D_Overlay_Properties.Text_Font_DirectX, float.PositiveInfinity, float.PositiveInfinity);
                }

                // Calculate Line Height & Spacing (20% of line height).
                Drawing_Properties.Line_Height = All_Text_Layouts[0].Metrics.Height;
                Drawing_Properties.Line_Spacing = (All_Text_Layouts[0].Metrics.Height / 100.0F) * 20.0F;

                Drawing_Properties.Title_Height = All_Text_Layouts[0].Metrics.Height;
                Drawing_Properties.Title_Spacing = (All_Text_Layouts[0].Metrics.Height / 100.0F) * 20.0F;

                // Get Title Rectangle Properties
                float Title_Rectangle_Height = Drawing_Properties.Title_Height + Drawing_Properties.Title_Spacing * 2;

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
                int Window_Position_X_Local = (int)(((Windows_Forms_Fake_Transparent_Overlay.Width / 100.0F) * Window_Position_X));
                int Window_Position_Y_Local = (int)(((Windows_Forms_Fake_Transparent_Overlay.Height / 100.0F) * Window_Position_Y));

                // Get center location of Rectangle & Size
                // X is left edge. 
                // Y is right edge.
                Point Rectangle_Location = new Point(Window_Position_X_Local - (int)(Rectangle_Background_Width / 2.0F), Window_Position_Y_Local - (int)(Rectangle_Background_Height / 2.0F));
                Size Rectangle_Size = new Size((int)(Rectangle_Background_Width), (int)(Rectangle_Background_Height));

                // Ensure Rectangle doesn't escape screen space.

                // If the right edge will escape the screen, make sure it does not.
                if ((Rectangle_Size.Width + Rectangle_Location.X) > Windows_Forms_Fake_Transparent_Overlay.Width)
                {
                    int Menu_Overflow_Pixels = (Rectangle_Size.Width + Rectangle_Location.X) - Windows_Forms_Fake_Transparent_Overlay.Width; // Get the amount of pixels rectangle escapes of screen.
                    Rectangle_Location.X = Rectangle_Location.X - Menu_Overflow_Pixels; // Move the rectangle by the overflow amount.
                }

                // If the bottom edge will escape the screen, make sure it does not.
                if ((Rectangle_Size.Height + Rectangle_Location.Y) > Windows_Forms_Fake_Transparent_Overlay.Height)
                {
                    int Menu_Overflow_Pixels = (Rectangle_Size.Height + Rectangle_Location.Y) - Windows_Forms_Fake_Transparent_Overlay.Height; // Get the amount of pixels rectangle escapes of screen.
                    Rectangle_Location.Y = Rectangle_Location.Y - Menu_Overflow_Pixels; // Move the rectangle by the overflow amount.
                }

                // Check for top and left edges of overlay if they escape the view.
                if (Rectangle_Location.X < 0) { Rectangle_Location.X = 0; }
                if (Rectangle_Location.Y < 0) { Rectangle_Location.Y = 0; }

                // Define Background Rectangle
                Drawing_Properties.Rectangle_Menu_DirectX = new Rectangle(Rectangle_Location.X, Rectangle_Location.Y, Rectangle_Size.Width, Rectangle_Size.Height);

                // Define Title Rectangle
                Drawing_Properties.Rectangle_Title_DirectX = new Rectangle(Rectangle_Location.X, Rectangle_Location.Y - (int)(Title_Rectangle_Height), Rectangle_Size.Width, (int)(Title_Rectangle_Height));

                // Load the Current Mod Loader Configurator Theme.
                var Theme_Style_Tuple = SonicHeroes.Misc.SonicHeroes_Miscallenous.Load_Theme_Configurator();

                float Title_R = (Convert.ToSingle(Theme_Style_Tuple.Item2.R) / 255.0F);
                float Title_G = (Convert.ToSingle(Theme_Style_Tuple.Item2.G) / 255.0F);
                float Title_B = (Convert.ToSingle(Theme_Style_Tuple.Item2.B) / 255.0F);
                float Accent_R = (Convert.ToSingle(Theme_Style_Tuple.Item3.R) / 255.0F);
                float Accent_G = (Convert.ToSingle(Theme_Style_Tuple.Item3.G) / 255.0F);
                float Accent_B = (Convert.ToSingle(Theme_Style_Tuple.Item3.B) / 255.0F);

                DirectX_2D_Overlay_Properties.Highlight_Brush_DirectX = new SolidColorBrush(DirectX_Graphics_Device, new RawColor4(Accent_R, Accent_G, Accent_B, 255));
                DirectX_2D_Overlay_Properties.Title_Background_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(Title_R, Title_G, Title_B, 255));
                DirectX_2D_Overlay_Properties.Title_Background_Brush_DirectX.Opacity = 0.80F;

                return Drawing_Properties;
            }
            catch (Exception Ex)
            {
                Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes("[Debug] Tweakbox | " + Ex.Message + " | " + Ex.StackTrace + " | If you see this, you should report this back to me."), false);
                return new Menu_Base.DirectX_2D_Overlay_Properties();
            }
            
            // Return properties.
            return new Menu_Base.DirectX_2D_Overlay_Properties();
        }

        /// <summary>
        /// Accepts a list of strings/text to be drawn, a set of Window Positions, output device & overlay for test drawing and drawing properties.
        /// </summary>
        /// <param name="Menu_Text_Array"></param>
        /// <param name="Window_Position_X"></param>
        /// <param name="Window_Position_Y"></param>
        /// <param name="DirectX_Graphics_Device"></param>
        /// <param name="Drawing_Properties"></param>
        /// <param name="Sonic_Heroes_Overlay"></param>
        public static Menu_Base.DirectX_2D_Message_Properties Get_Menu_Size_Location_Message(string[] Menu_Text_Array, float Window_Position_X, float Window_Position_Y, WindowRenderTarget DirectX_Graphics_Device, Form Windows_Forms_Fake_Transparent_Overlay)
        {
            try
            {
                // Drawing Properties
                Menu_Base.DirectX_2D_Message_Properties Drawing_Properties = new Menu_Base.DirectX_2D_Message_Properties();

                // Set-up UI Scaling such as that the interface scales alongside the resolution used.
                float Standard_Resolution_Height = 720; // I write and test my stuff on 1280 x 720.
                int Standard_Font_Size = 18; // Standard font size to be used for display.
                float Window_Scale_Overlay = (float)Windows_Forms_Fake_Transparent_Overlay.Height / (float)Standard_Resolution_Height; // Get scaling factor.
                Standard_Font_Size = (int)(Standard_Font_Size * Window_Scale_Overlay); // Adjust Font Size.

                // Set Brushes and Fonts.
                DirectX_2D_Message_Properties.Text_Font_DirectX = new SharpDX.DirectWrite.TextFormat(new SharpDX.DirectWrite.Factory(), "Consolas", Standard_Font_Size);
                DirectX_2D_Message_Properties.Text_Font_DirectX.TextAlignment = TextAlignment.Center;
                DirectX_2D_Message_Properties.Drawing_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(255, 255, 255, 180));
                DirectX_2D_Message_Properties.Background_Brush_DirectX = new SharpDX.Direct2D1.SolidColorBrush(DirectX_Graphics_Device, new SharpDX.Mathematics.Interop.RawColor4(0.109F, 0.109F, 0.109F, 150));
                DirectX_2D_Message_Properties.Background_Brush_DirectX.Opacity = 0.7F;  // Alpha value is ignored! Use Opacity for Transparency!

                // Obtain TextLayout in order to obtain text width and height properties.
                SharpDX.DirectWrite.TextLayout[] All_Text_Layouts = new SharpDX.DirectWrite.TextLayout[Menu_Text_Array.Length]; // Get all text layout info
                for (int x = 0; x < Menu_Text_Array.Length; x++)
                {
                    All_Text_Layouts[x] = new TextLayout(new SharpDX.DirectWrite.Factory(), Menu_Text_Array[x], DirectX_2D_Overlay_Properties.Text_Font_DirectX, float.PositiveInfinity, float.PositiveInfinity);
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
                int Window_Position_X_Local = (int)(((Windows_Forms_Fake_Transparent_Overlay.Width / 100.0F) * Window_Position_X));
                int Window_Position_Y_Local = (int)(((Windows_Forms_Fake_Transparent_Overlay.Height / 100.0F) * Window_Position_Y));

                // Get center location of Rectangle & Size
                // X is left edge. 
                // Y is right edge.
                Point Rectangle_Location = new Point(Window_Position_X_Local - (int)(Rectangle_Background_Width / 2.0F), Window_Position_Y_Local - (int)(Rectangle_Background_Height / 2.0F));
                Size Rectangle_Size = new Size((int)(Rectangle_Background_Width), (int)(Rectangle_Background_Height));

                // Ensure Rectangle doesn't escape screen space.

                // If the right edge will escape the screen, make sure it does not.
                if ((Rectangle_Size.Width + Rectangle_Location.X) > Windows_Forms_Fake_Transparent_Overlay.Width)
                {
                    int Menu_Overflow_Pixels = (Rectangle_Size.Width + Rectangle_Location.X) - Windows_Forms_Fake_Transparent_Overlay.Width; // Get the amount of pixels rectangle escapes of screen.
                    Rectangle_Location.X = Rectangle_Location.X - Menu_Overflow_Pixels; // Move the rectangle by the overflow amount.
                }

                // If the bottom edge will escape the screen, make sure it does not.
                if ((Rectangle_Size.Height + Rectangle_Location.Y) > Windows_Forms_Fake_Transparent_Overlay.Height)
                {
                    int Menu_Overflow_Pixels = (Rectangle_Size.Height + Rectangle_Location.Y) - Windows_Forms_Fake_Transparent_Overlay.Height; // Get the amount of pixels rectangle escapes of screen.
                    Rectangle_Location.Y = Rectangle_Location.Y - Menu_Overflow_Pixels; // Move the rectangle by the overflow amount.
                }

                // Check for top and left edges of overlay if they escape the view.
                if (Rectangle_Location.X < 0) { Rectangle_Location.X = 0; }
                if (Rectangle_Location.Y < 0) { Rectangle_Location.Y = 0; }

                // Define Background Rectangle
                Drawing_Properties.Rectangle_Menu_DirectX = new Rectangle(Rectangle_Location.X, Rectangle_Location.Y, Rectangle_Size.Width, Rectangle_Size.Height);

                return Drawing_Properties;
            }
            catch (Exception Ex)
            {
                Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes("[Debug] Tweakbox | " + Ex.Message + " | " + Ex.StackTrace + " | If you see this, you should report this back to me."), false);
                return new Menu_Base.DirectX_2D_Message_Properties();
            }

            // Return properties.
            return new Menu_Base.DirectX_2D_Message_Properties();
        }

        /// <summary>
        /// If the properties have once been set, only change the important variables.
        /// </summary>
        /// <param name="Menu_Text_Array"></param>
        /// <param name="Window_Position_X"></param>
        /// <param name="Window_Position_Y"></param>
        /// <param name="DirectX_Graphics_Device"></param>
        /// <param name="Drawing_Properties"></param>
        /// <param name="Sonic_Heroes_Overlay"></param>
        public static Menu_Base.DirectX_2D_Overlay_Properties Adjust_Menu_Width(string[] Menu_Text_Array, float Window_Position_X, float Window_Position_Y, WindowRenderTarget DirectX_Graphics_Device, Form Windows_Forms_Fake_Transparent_Overlay, Menu_Base.DirectX_2D_Overlay_Properties Current_Drawing_Properties)
        {
            try
            {

                // Obtain TextLayout in order to obtain text width and height properties.
                SharpDX.DirectWrite.TextLayout[] All_Text_Layouts = new SharpDX.DirectWrite.TextLayout[Menu_Text_Array.Length]; // Get all text layout info
                for (int x = 0; x < Menu_Text_Array.Length; x++)
                {
                    All_Text_Layouts[x] = new TextLayout(new SharpDX.DirectWrite.Factory(), Menu_Text_Array[x], DirectX_2D_Overlay_Properties.Text_Font_DirectX, float.PositiveInfinity, float.PositiveInfinity);
                }

                // Obtain Largest Width and Height
                float Rectangle_Background_Width = All_Text_Layouts[0].Metrics.Width;
                for (int x = 1; x < All_Text_Layouts.Length; x++)
                {
                    if (All_Text_Layouts[x].Metrics.Width > Rectangle_Background_Width) { Rectangle_Background_Width = All_Text_Layouts[x].Metrics.Width; }
                }


                // Make Rectangle Bigger (Styling)
                Rectangle_Background_Width += (Current_Drawing_Properties.Line_Spacing * 2);

                // Obtain X position across the form for the selected location of item.
                int Window_Position_X_Local = (int)(((Windows_Forms_Fake_Transparent_Overlay.Width / 100.0F) * Window_Position_X));

                // Get horizontal location of rectangle
                int Rectangle_Location_X = Window_Position_X_Local - (int)(Rectangle_Background_Width / 2.0F);

                // If the right edge will escape the screen, make sure it does not.
                if ((Rectangle_Background_Width + Rectangle_Location_X) > Windows_Forms_Fake_Transparent_Overlay.Width)
                {
                    int Menu_Overflow_Pixels = ((int)Rectangle_Background_Width + Rectangle_Location_X) - Windows_Forms_Fake_Transparent_Overlay.Width; // Get the amount of pixels rectangle escapes of screen.
                    Rectangle_Location_X = Rectangle_Location_X - Menu_Overflow_Pixels; // Move the rectangle by the overflow amount.
                }

                // Check for top and left edges of overlay if they escape the view.
                if (Rectangle_Location_X < 0) { Rectangle_Location_X = 0; }

                // Define Background Rectangle
                Current_Drawing_Properties.Rectangle_Menu_DirectX.X = Rectangle_Location_X;
                Current_Drawing_Properties.Rectangle_Menu_DirectX.Width = (int)Rectangle_Background_Width;
                Current_Drawing_Properties.Rectangle_Title_DirectX.X = Rectangle_Location_X;
                Current_Drawing_Properties.Rectangle_Title_DirectX.Width = (int)Rectangle_Background_Width;

                return Current_Drawing_Properties;
            } catch (Exception Ex)
            {
                Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes("[Debug] Tweakbox | " + Ex.Message + " | " + Ex.StackTrace + " | If you see this, you should report this back to me."), false);
                return new Menu_Base.DirectX_2D_Overlay_Properties();
            }
        }


        /// <summary>
        /// If the properties have once been set, only change the important variables.
        /// </summary>
        /// <param name="Menu_Text_Array"></param>
        /// <param name="Window_Position_X"></param>
        /// <param name="Window_Position_Y"></param>
        /// <param name="DirectX_Graphics_Device"></param>
        /// <param name="Drawing_Properties"></param>
        /// <param name="Sonic_Heroes_Overlay"></param>
        public static Menu_Base.DirectX_2D_Message_Properties Adjust_Menu_Width_Message(string[] Menu_Text_Array, float Window_Position_X, float Window_Position_Y, WindowRenderTarget DirectX_Graphics_Device, Form Windows_Forms_Fake_Transparent_Overlay, Menu_Base.DirectX_2D_Message_Properties Current_Drawing_Properties)
        {
            try
            {
                // Obtain TextLayout in order to obtain text width and height properties.
                SharpDX.DirectWrite.TextLayout[] All_Text_Layouts = new SharpDX.DirectWrite.TextLayout[Menu_Text_Array.Length]; // Get all text layout info
                for (int x = 0; x < Menu_Text_Array.Length; x++)
                {
                    All_Text_Layouts[x] = new TextLayout(new SharpDX.DirectWrite.Factory(), Menu_Text_Array[x], DirectX_2D_Overlay_Properties.Text_Font_DirectX, float.PositiveInfinity, float.PositiveInfinity);
                }

                // Obtain Largest Width and Height
                float Rectangle_Background_Width = All_Text_Layouts[0].Metrics.Width;
                for (int x = 1; x < All_Text_Layouts.Length; x++)
                {
                    if (All_Text_Layouts[x].Metrics.Width > Rectangle_Background_Width) { Rectangle_Background_Width = All_Text_Layouts[x].Metrics.Width; }
                }


                // Make Rectangle Bigger (Styling)
                Rectangle_Background_Width += (Current_Drawing_Properties.Line_Spacing * 2);

                // Obtain X position across the form for the selected location of item.
                int Window_Position_X_Local = (int)(((Windows_Forms_Fake_Transparent_Overlay.Width / 100.0F) * Window_Position_X));

                // Get horizontal location of rectangle
                int Rectangle_Location_X = Window_Position_X_Local - (int)(Rectangle_Background_Width / 2.0F);

                // If the right edge will escape the screen, make sure it does not.
                if ((Rectangle_Background_Width + Rectangle_Location_X) > Windows_Forms_Fake_Transparent_Overlay.Width)
                {
                    int Menu_Overflow_Pixels = ((int)Rectangle_Background_Width + Rectangle_Location_X) - Windows_Forms_Fake_Transparent_Overlay.Width; // Get the amount of pixels rectangle escapes of screen.
                    Rectangle_Location_X = Rectangle_Location_X - Menu_Overflow_Pixels; // Move the rectangle by the overflow amount.
                }

                // Check for top and left edges of overlay if they escape the view.
                if (Rectangle_Location_X < 0) { Rectangle_Location_X = 0; }

                // Define Background Rectangle
                Current_Drawing_Properties.Rectangle_Menu_DirectX.X = Rectangle_Location_X;
                Current_Drawing_Properties.Rectangle_Menu_DirectX.Width = (int)Rectangle_Background_Width;
                return Current_Drawing_Properties;
            } catch (Exception Ex)
            {
                Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes("[Debug] Tweakbox | " + Ex.Message + " | " + Ex.StackTrace + " | If you see this, you should report this back to me."), false);
                return new Menu_Base.DirectX_2D_Message_Properties();
            }
        }

        /// <summary>
        /// Converts the coordinates of Rectangle and spits out RawRectangleF
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static RawRectangleF Rectangle_To_RawRectangleF(Rectangle RECT)
        {
            return new RawRectangleF(RECT.Left, RECT.Top, RECT.Right, RECT.Bottom);
        }

        /// <summary>
        /// Converts the coordinates of Rectangle and spits out RawRectangleF
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static RawRectangleF RawRectangle_Get_Text_Location(DirectX_2D_Overlay_Properties Mod_Menu_Page_Visual_Style, int Loop_Iteration)
        {
            try
            {
                return new
                SharpDX.Mathematics.Interop.RawRectangleF
                (
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Menu_DirectX.Left, // Left Edge | Make Space Equal to Line Spacing
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Menu_DirectX.Top + ((int)Mod_Menu_Page_Visual_Style.Line_Spacing) + ((int)Mod_Menu_Page_Visual_Style.Line_Height * Loop_Iteration), // Top Edge 
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Menu_DirectX.Right, // Right Edge | No Text Wrap
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Menu_DirectX.Top + ((int)Mod_Menu_Page_Visual_Style.Line_Spacing) + ((int)Mod_Menu_Page_Visual_Style.Line_Height * Loop_Iteration + 1) // Bottom Edge | No Text Wrap
                );
            } catch { return new SharpDX.Mathematics.Interop.RawRectangleF(); }
        }

        /// <summary>
        /// Converts the coordinates of Rectangle and spits out RawRectangleF
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static RawRectangleF RawRectangle_Get_Title_Location(DirectX_2D_Overlay_Properties Mod_Menu_Page_Visual_Style, string Text)
        {
            try
            {
                return new
                SharpDX.Mathematics.Interop.RawRectangleF
                (
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Title_DirectX.Left, // Left Edge | Make Space Equal to Line Spacing
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Title_DirectX.Top, // Top Edge 
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Title_DirectX.Right, // Right Edge | No Text Wrap
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Title_DirectX.Bottom // Bottom Edge | No Text Wrap
                );
            }
            catch { return new SharpDX.Mathematics.Interop.RawRectangleF(); }
        }

        /// <summary>
        /// Converts the coordinates of Rectangle and spits out RawRectangleF
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static RawRectangleF RawRectangle_Get_Text_Location_Message(DirectX_2D_Message_Properties Mod_Menu_Page_Visual_Style, int Loop_Iteration)
        {
            try
            {
                return new
                SharpDX.Mathematics.Interop.RawRectangleF
                (
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Menu_DirectX.Left, // Left Edge | Make Space Equal to Line Spacing
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Menu_DirectX.Top + ((int)Mod_Menu_Page_Visual_Style.Line_Spacing) + ((int)Mod_Menu_Page_Visual_Style.Line_Height * Loop_Iteration), // Top Edge 
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Menu_DirectX.Right, // Right Edge | No Text Wrap
                    (int)Mod_Menu_Page_Visual_Style.Rectangle_Menu_DirectX.Top + ((int)Mod_Menu_Page_Visual_Style.Line_Spacing) + ((int)Mod_Menu_Page_Visual_Style.Line_Height * Loop_Iteration + 1) // Bottom Edge | No Text Wrap
                );
            }
            catch { return new SharpDX.Mathematics.Interop.RawRectangleF(); }
        }
    }
}
