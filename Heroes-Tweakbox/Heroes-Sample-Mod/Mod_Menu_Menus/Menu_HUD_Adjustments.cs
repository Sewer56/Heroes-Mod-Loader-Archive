using System.Collections.Generic;
using SonicHeroes.Controller;

namespace Heroes_Sample_Mod
{
    public class Menu_HUD_Adjustments : Menu_Base
    {
        // Menu Items Enum
        public Menu_HUD_Adjustments() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Minimal HUD");

            // Insert all items!
            Set_Toggle_State();
        }

        // Items contained in this menu.
        public enum Menu_Items
        {
            Menu_Item_Corner_X_Offset = 0x00,
            Menu_Item_Corner_Y_Offset = 0x01,
            Menu_Item_Corner_Item_Offset = 0x02,
            Menu_Item_Minimal_HUD_Toggle = 0x03,
            Menu_Item_Disable_HUD = 0x04,
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
                // Toggle Super Sonic/Tails/Nipples Enchilada.
                if (Menu_Index == (int)Menu_Items.Menu_Item_Minimal_HUD_Toggle) { Program.Feature_Minimal_HUD_X.Toggle_Minimal_HUD(); }
                if (Menu_Index == (int)Menu_Items.Menu_Item_Disable_HUD) { Program.Feature_Minimal_HUD_X.Toggle_HUD(); }

                // Refresh Menu State
                Set_Toggle_State();

                // Set New Menu Bounds
                if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }

                // Wait for button release.
                Wait_For_Controller_Release(Controller_Keys.Button_A, Controller_Manager);
            }

            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT)
            {
                // Toggle Super Sonic/Tails/Nipples Enchilada.
                if (Menu_Index == (int)Menu_Items.Menu_Item_Corner_X_Offset) { Program.Feature_Minimal_HUD_X.Change_X_Position(-0.01F); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Corner_Y_Offset) { Program.Feature_Minimal_HUD_X.Change_Y_Position(-0.01F); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Corner_Item_Offset) { Program.Feature_Minimal_HUD_X.Change_Vertical_Offset(-0.01F); }

                // Refresh Menu State
                Set_Toggle_State();

                // Longpress DPAD
                DPAD_Longpress_Sleep();
            }
            else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT)
            {
                // Toggle Super Sonic/Tails/Nipples Enchilada.
                if (Menu_Index == (int)Menu_Items.Menu_Item_Corner_X_Offset) { Program.Feature_Minimal_HUD_X.Change_X_Position(+0.01F); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Corner_Y_Offset) { Program.Feature_Minimal_HUD_X.Change_Y_Position(+0.01F); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Corner_Item_Offset) { Program.Feature_Minimal_HUD_X.Change_Vertical_Offset(+0.01F); }

                // Refresh Menu State
                Set_Toggle_State();

                // Longpress DPAD
                DPAD_Longpress_Sleep();
            }
        }

        /// <summary>
        /// Sets the visual string toggle state for each of the menu settings.
        /// </summary>
        public override void Set_Toggle_State()
        {
            // Remove all current items.
            Mod_Menu_Page_Strings.Clear();

            // Append current menu toggle states.
            Mod_Menu_Page_Strings.Add("Minimal HUD Heads Up Display X Offset: " + Program.Feature_Minimal_HUD_X.Positioning_X_Position.ToString("0.00F"));
            Mod_Menu_Page_Strings.Add("Minimal HUD Heads Up Display Y Offset: " + Program.Feature_Minimal_HUD_X.Positioning_Y_Position.ToString("0.00F"));
            Mod_Menu_Page_Strings.Add("Minimal HUD Heads Up Display Distance Between Items: " + Program.Feature_Minimal_HUD_X.Positioning_Vertical_Offset.ToString("0.00F"));

            if (Program.Feature_Minimal_HUD_X.Enabled) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Minimal HUD"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Minimal HUD"); }

            if (Program.Feature_Minimal_HUD_X.Enabled_HUD) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Disable HUD"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Disable HUD"); }

            // In case menu width changes.
            if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }
        }

        /// <summary>
        /// Sends an appropriate message to the base form to be shown in the menu's messagebox... Message is decided by the current highlighted item.
        /// </summary>
        public override void Pass_MessageBox_Message() { Set_MessageBox_Message(new List<string>() { "List of Settings for a new, built from the ground up, widescreen supporting Minimal HUD." }); }

    }
}
