using System.Collections.Generic;
using SonicHeroes.Controller;

namespace Heroes_Sample_Mod
{
    public class Menu_Debug_Stuff : Menu_Base
    {
        // Menu Items Enum
        public Menu_Debug_Stuff() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Debuggging Assists");

            // Insert all items!
            Set_Toggle_State();
        }

        // Items contained in this menu.
        public enum Menu_Items
        {
            Menu_Item_Position_XYZ = 0x00,
            Menu_Item_Enhanced_Debug_Movement_Mode = 0x01,
        }

        /// <summary>
        /// Main Menu Controller Handling Method
        /// </summary>
        public override void Handle_Controller_Inputs(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        {
            // Execute Base Controller Code. UP/DOWN navigation + back button.
            base.Handle_Controller_Inputs(P1_Controller, Controller_Manager);

            // Check menu to load if A_Button is pressed.
            if (P1_Controller.ControllerButtons.Button_A)
            {
                // Toggle X/Y/Z Button
                if (Menu_Index == (int)Menu_Items.Menu_Item_Position_XYZ) { Program.Feature_Position_XYZ_Window_X.Toggle_Overlay_Status(); Set_Toggle_State(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Enhanced_Debug_Movement_Mode) { Program.Feature_Enhanced_Debug_Movement_Mode_X.Toggle_Free_Movement_Mode(); Set_Toggle_State(); }

                // Set New Menu Bounds
                if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }

                // Wait for button release.
                Wait_For_Controller_Release(Controller_Keys.Button_A, Controller_Manager);
            }
        }

        /// <summary>
        /// Sets the visual string toggle state for each of the menu settings.
        /// </summary>
        public override void Set_Toggle_State()
        {
            // Remove all current items.
            Mod_Menu_Page_Strings.Clear();

            // Set Menu Status
            if (Program.Feature_Position_XYZ_Window_X.Get_Overlay_Status()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " XYZ Position/Velocity Information Window"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " XYZ Position/Velocity Information Window"); }

            // Set Menu Status
            if (Program.Feature_Enhanced_Debug_Movement_Mode_X.Get_Free_Movement_Enable_Status()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Enhanced Debug Movement Mode"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Enhanced Debug Movement Mode"); }

            // In case menu width changes.
            if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }
        }

    }
}
