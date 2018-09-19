using System.Collections.Generic;
using SonicHeroes.Controller;
using System;

namespace Heroes_Sample_Mod
{
    public class Menu_Free_Movement_Mode : Menu_Base
    {
        // Menu Items Enum
        public Menu_Free_Movement_Mode() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Free Movement Controls");

            // Insert all items!
            Set_Menu_Text(0, 0,0,0);

            // Set index to not highlight any items.
            Menu_Index = -1;
        }

        /// Disable Default InputHandler
        public override void Handle_Controller_Inputs(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        { }

        /// <summary>
        /// Sets the drawing properties, namely rectangle size and position for the current menu.
        /// </summary>
        public override void Set_Drawing_Properties()
        {
            Mod_Menu_Page_Visual_Style = DirectX_Menu_Methods.Get_Menu_Size_Location(this.Mod_Menu_Page_Strings.ToArray(), 0.00F, 50.00F, Program.Sonic_Heroes_Overlay.direct2DWindowTarget, Program.Sonic_Heroes_Overlay.overlayWinForm);
        }

        /// <summary>
        /// Sets the menu text to be displayed.
        /// </summary>
        public void Set_Menu_Text(int Menu_Index_X, float Position_X, float Position_Y, float Position_Z)
        {
            Mod_Menu_Page_Strings.Clear();
            Mod_Menu_Page_Strings.Add(String.Format("Current Teleport: {0}/9", Menu_Index_X));
            Mod_Menu_Page_Strings.Add(String.Format("Location: {0},{1},{2}", Position_X.ToString("+00000;-00000;"), Position_Y.ToString("+00000;-00000;"), Position_Z.ToString("+00000;-00000;")));
            Mod_Menu_Page_Strings.Add("Toggle Free Movement Mode: DPAD UP");
            Mod_Menu_Page_Strings.Add("Free Movement Mode Raise/Lower: RT/LT");
            Mod_Menu_Page_Strings.Add("Free Movement Move Character: Left Stick");
            Mod_Menu_Page_Strings.Add("Save Current Position: DPAD LEFT");
            Mod_Menu_Page_Strings.Add("Load Position: DPAD RIGHT");
            Mod_Menu_Page_Strings.Add("Cycle Current Saved Location: DPAD DOWN");
        }
    }
}
