using SharpDX.Direct2D1;
using SonicHeroes.Controller;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Defines everything common about each Mod Menu Menu;
    /// </summary>
    public class Menu_Base
    {
        // Define Visual Style for Menu
        public DirectX_2D_Overlay_Properties Mod_Menu_Page_Visual_Style = new DirectX_2D_Overlay_Properties();
        public DirectX_2D_Message_Properties Mod_Menu_MessageBox_Properties = new DirectX_2D_Message_Properties();

        // List of Text to be Drawn in Menu
        public List<string> Mod_Menu_Page_Strings = new List<string>();
        public List<string> Mod_Menu_Title = new List<string>();
        public List<string> Mod_Menu_MessageBox_Strings = new List<string>();

        // Tick styles
        public const string Tick_Enabled = "[+]";
        public const string Tick_Disabled = "[-]";
        public const string Tick_NA = "[/]";
        public static int controllerIndex;

        // Current Index of The Menu
        public int Menu_Index = 0;

        // Have Rendering Properties Been Set?
        public bool Render_Properties_Set = false;

        // Longpress DPAD scroll sleep duration.
        public int Mod_Menu_DPAD_Sleep_Length = 64; 

        /// <summary>
        /// Main Controller Input Handler.
        /// </summary>
        /// <param name="P1_Controller"></param>
        /// <param name="Controller_Manager"></param>
        public virtual void Handle_Controller_Inputs(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        {
            // Returns to the previously set menu.
            if (P1_Controller.ControllerButtons.Button_B) { Program.Return_Last_Menu(); Wait_For_Controller_Release(Controller_Keys.Button_B, Controller_Manager); return; }

            // Up/Down Menu Movement with DPAD.
            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.DOWN) { Increment_Menu_Index(); Wait_For_Controller_Release(Controller_Keys.DPAD_DOWN, Controller_Manager); Pass_MessageBox_Message(); return; }
            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.UP) { Decrement_Menu_Index(); Wait_For_Controller_Release(Controller_Keys.DPAD_UP, Controller_Manager); Pass_MessageBox_Message(); return; }
        }

        /// <summary>
        /// Sleeps the current thread for simulated DPAD repeat inputs.
        /// </summary>
        public void DPAD_Longpress_Sleep()
        {
            Thread.Sleep(Mod_Menu_DPAD_Sleep_Length);
        }

        /// <summary>
        /// A listing of controller keys and DPAD directions.
        /// </summary>
        public enum Controller_Keys
        {
            Button_A = 0x1,
            Button_B = 0x2,
            Button_X = 0x3,
            Button_Y = 0x4,
            Button_L1 = 0x5,
            Button_R1 = 0x6,
            Button_Back = 0x7,
            Button_Start = 0x8,
            Button_L3 = 0x9,
            Button_R3 = 0xA,
            Optional_Button_Guide = 0xB,
            DPAD_LEFT = 0xC,
            DPAD_RIGHT = 0xD,
            DPAD_UP = 0xE,
            DPAD_DOWN = 0xF,
        }

        /// <summary>
        /// Placeholder to enable calling of derived classes.
        /// </summary>
        public virtual void Set_Toggle_State() { }

        /// <summary>
        /// Waits for the user to release a key which they may currently be holding.
        /// </summary>
        /// <param name="Controller_Key_Release"></param>
        /// <param name="Controller_Manager"></param>
        public virtual void Wait_For_Controller_Release(Controller_Keys Controller_Key_Release, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        {
            switch (Controller_Key_Release)
            {
                case Controller_Keys.Button_A:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Button_A) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.Button_B:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Button_B) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.Button_X:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Button_X) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.Button_Y:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Button_Y) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.Button_Back:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Button_Back) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.Button_Start:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Button_Start) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.Optional_Button_Guide:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Optional_Button_Guide) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.Button_L1:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Button_L1) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.Button_L3:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Button_L3) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.Button_R1:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Button_R1) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.Button_R3:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerButtons.Button_R3) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.DPAD_LEFT:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerDPad == (int)SonicHeroes.Controller.Sonic_Heroes_Joystick.DPAD_Direction.LEFT) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.DPAD_RIGHT:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerDPad == (int)SonicHeroes.Controller.Sonic_Heroes_Joystick.DPAD_Direction.RIGHT) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.DPAD_UP:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerDPad == (int)SonicHeroes.Controller.Sonic_Heroes_Joystick.DPAD_Direction.UP) { Thread.Sleep(16); }
                    break;
                case Controller_Keys.DPAD_DOWN:
                    while (Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State().ControllerDPad == (int)SonicHeroes.Controller.Sonic_Heroes_Joystick.DPAD_Direction.DOWN) { Thread.Sleep(16); }
                    break;
            }
        }

        /// <summary>
        /// Main Method Ran by the Rendering Thread. Renders the menu, messageboxes and any other items to screen.
        /// </summary>
        /// <param name="DirectX_Graphics_Window"></param>
        public virtual void Render_This_Menu(WindowRenderTarget DirectX_Graphics_Window)
        {
            try
            {
                // Sets render properties if not set.
                if (!Render_Properties_Set)
                {
                    if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }
                    Set_MessageBox_Drawing_Properties();
                    Render_Properties_Set = true;
                }

                // Draws the menu background rectangle.
                DirectX_Graphics_Window.FillRectangle(DirectX_Menu_Methods.Rectangle_To_RawRectangleF(Mod_Menu_Page_Visual_Style.Rectangle_Menu_DirectX), DirectX_2D_Overlay_Properties.Background_Brush_DirectX);

                // Draw Title Rectangle
                DirectX_Graphics_Window.FillRectangle(DirectX_Menu_Methods.Rectangle_To_RawRectangleF(Mod_Menu_Page_Visual_Style.Rectangle_Title_DirectX), DirectX_2D_Overlay_Properties.Title_Background_Brush_DirectX);

                // Render the title!
                for (int x = 0; x < Mod_Menu_Title.Count; x++)
                {
                    DirectX_Graphics_Window.DrawText
                    (
                        Mod_Menu_Title[x], // Current String to Draw
                        DirectX_2D_Overlay_Properties.Title_Font_DirectX, // Current Chosen Font
                        DirectX_Menu_Methods.RawRectangle_Get_Title_Location(Mod_Menu_Page_Visual_Style, Mod_Menu_Title[x]), // Current Text Rectangle Regions
                        DirectX_2D_Overlay_Properties.Title_Brush_DirectX
                    );
                }

                // Render all strings. Highlight current selection.
                for (int x = 0; x < Mod_Menu_Page_Strings.Count; x++)
                {
                    if (x != Menu_Index) // If it's not the current selection, render gray, else render the accent colour!
                    {
                        DirectX_Graphics_Window.DrawText
                        (
                            Mod_Menu_Page_Strings[x], // Current String to Draw
                            DirectX_2D_Overlay_Properties.Text_Font_DirectX, // Current Chosen Font
                            DirectX_Menu_Methods.RawRectangle_Get_Text_Location(Mod_Menu_Page_Visual_Style, x), // Current Text Rectangle Regions
                            DirectX_2D_Overlay_Properties.Drawing_Brush_DirectX
                        );
                    }
                    else
                    {
                        DirectX_Graphics_Window.DrawText
                        (
                            Mod_Menu_Page_Strings[x], // Current String to Draw
                            DirectX_2D_Overlay_Properties.Text_Font_DirectX, // Current Chosen Font
                            DirectX_Menu_Methods.RawRectangle_Get_Text_Location(Mod_Menu_Page_Visual_Style, x), // Current Text Rectangle Regions
                            DirectX_2D_Overlay_Properties.Highlight_Brush_DirectX
                        );
                    }
                }

                // Render the messagebox information
                DirectX_Graphics_Window.FillRectangle(DirectX_Menu_Methods.Rectangle_To_RawRectangleF(Mod_Menu_MessageBox_Properties.Rectangle_Menu_DirectX), DirectX_2D_Overlay_Properties.Background_Brush_DirectX);

                // Render all strings in messagebox.
                for (int x = 0; x < Mod_Menu_MessageBox_Strings.Count; x++)
                {
                    DirectX_Graphics_Window.DrawText
                    (
                        Mod_Menu_MessageBox_Strings[x], // Current String to Draw
                        DirectX_2D_Overlay_Properties.Text_Font_DirectX, // Current Chosen Font
                        DirectX_Menu_Methods.RawRectangle_Get_Text_Location_Message(Mod_Menu_MessageBox_Properties, x), // Current Text Rectangle Regions
                        DirectX_2D_Overlay_Properties.Drawing_Brush_DirectX
                    );
                }
            }
            catch { }
        }

        /// <summary>
        /// Defines the drawing properties of the menu rectangle.
        /// </summary>
        public struct DirectX_2D_Overlay_Properties
        {
            /// Generic
            public float Line_Spacing;
            public float Line_Height;
            public float Title_Spacing;
            public float Title_Height;
            public float Title_Offset;

            /// DIRECTX
            public static SharpDX.DirectWrite.TextFormat Text_Font_DirectX;
            public static SharpDX.DirectWrite.TextFormat Title_Font_DirectX;
            public static SharpDX.Direct2D1.SolidColorBrush Drawing_Brush_DirectX;
            public static SharpDX.Direct2D1.SolidColorBrush Highlight_Brush_DirectX;
            public static SharpDX.Direct2D1.SolidColorBrush Background_Brush_DirectX;
            public static SharpDX.Direct2D1.SolidColorBrush Title_Background_Brush_DirectX;
            public static SharpDX.Direct2D1.SolidColorBrush Title_Brush_DirectX;

            /// public SharpDX.Mathematics.Interop.RawRectangleF Rectangle_Menu_DirectX;
            public Rectangle Rectangle_Menu_DirectX;
            public Rectangle Rectangle_Title_DirectX;
        }

        /// <summary>
        /// Defines the drawing properties of the menu rectangle.
        /// </summary>
        public struct DirectX_2D_Message_Properties
        {
            /// Generic
            public float Line_Spacing;
            public float Line_Height;

            /// DIRECTX
            public static SharpDX.DirectWrite.TextFormat Text_Font_DirectX;
            public static SharpDX.Direct2D1.SolidColorBrush Drawing_Brush_DirectX;
            public static SharpDX.Direct2D1.SolidColorBrush Highlight_Brush_DirectX;
            public static SharpDX.Direct2D1.SolidColorBrush Background_Brush_DirectX;

            /// public SharpDX.Mathematics.Interop.RawRectangleF Rectangle_Menu_DirectX;
            public Rectangle Rectangle_Menu_DirectX;
        }

        /// <summary>
        /// Increments the index of the current menu.
        /// </summary>
        public virtual void Increment_Menu_Index()
        {
            if (Menu_Index == Mod_Menu_Page_Strings.Count - 1) { Menu_Index = 0; }
            else { Menu_Index = Menu_Index + 1; }
            Pass_MessageBox_Message();
        }

        /// <summary>
        /// Decrements the index of the current menu.
        /// </summary>
        public virtual void Decrement_Menu_Index()
        {
            if (Menu_Index == 0) { Menu_Index = Mod_Menu_Page_Strings.Count - 1; }
            else { Menu_Index = Menu_Index - 1; }
            Pass_MessageBox_Message();
        }

        /// <summary>
        /// Passes onward a message to be sent and displayed in the MessageBox. Spoiler: You're meant to override this method.
        /// </summary>
        public virtual void Pass_MessageBox_Message() { }
        
        /// <summary>
        /// Sets the current message to be displayed in the messagebox.
        /// </summary>
        /// <param name="Message"></param>
        public virtual void Set_MessageBox_Message(List<string> Message)
        {
            try
            {
                // Obtain current selection.
                Mod_Menu_MessageBox_Strings = Message;
                Mod_Menu_MessageBox_Properties = DirectX_Menu_Methods.Get_Menu_Size_Location_Message(Mod_Menu_MessageBox_Strings.ToArray(), 50.0F, 80.0F, Program.Sonic_Heroes_Overlay.direct2DWindowTarget, Program.Sonic_Heroes_Overlay.overlayWinForm);
            } catch { }
        }

        /// <summary>
        /// Sets the drawing properties, namely rectangle size and position for the current menu.
        /// </summary>
        public virtual void Set_Drawing_Properties()
        {
            try
            {
                Mod_Menu_Page_Visual_Style = DirectX_Menu_Methods.Get_Menu_Size_Location(this.Mod_Menu_Page_Strings.ToArray(), Program.Menu_Horizontal_Percentage, Program.Menu_Vertical_Percentage, Program.Sonic_Heroes_Overlay.direct2DWindowTarget, Program.Sonic_Heroes_Overlay.overlayWinForm);
            }
            catch { }
        }

        /// <summary>
        /// Sets the drawing properties, namely rectangle size and position for the current displayed messagebox at the bottom of the screen..
        /// </summary>
        public virtual void Set_MessageBox_Drawing_Properties()
        {
            try
            {
                Mod_Menu_MessageBox_Properties = DirectX_Menu_Methods.Get_Menu_Size_Location_Message(this.Mod_Menu_MessageBox_Strings.ToArray(), 50.0F, 80.0F, Program.Sonic_Heroes_Overlay.direct2DWindowTarget, Program.Sonic_Heroes_Overlay.overlayWinForm);
            }
            catch { }
        }
    }
}
