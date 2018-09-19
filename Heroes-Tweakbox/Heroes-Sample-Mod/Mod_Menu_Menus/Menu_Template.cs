using System.Collections.Generic;
using SonicHeroes.Controller;

namespace Heroes_Sample_Mod
{
    public class Menu_Template : Menu_Base
    {
        // Menu Items Enum
        public Menu_Template() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Menu Template");

            // Insert all items!
            Set_Toggle_State();
        }

        // Items contained in this menu.
        public enum Menu_Items
        {
            Menu_Item_Null = 0x00,
        }

        // List of Features/Variables

        // List of Menu Text Flags
        bool Null_Enabled = true;

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


                // Set New Menu Bounds
                if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }

                // Wait for button release.
                Wait_For_Controller_Release(Controller_Keys.Button_A, Controller_Manager);
            }

            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT)
            {
                // Toggle X/Y/Z Variable
                if (Menu_Index == (int)Menu_Items.Menu_Item_Null) { Set_Toggle_State(); }

                // Longpress DPAD
                DPAD_Longpress_Sleep();
            }
            else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT)
            {
                // Toggle X/Y/Z Variable
                if (Menu_Index == (int)Menu_Items.Menu_Item_Null) { Set_Toggle_State(); }

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
            Mod_Menu_Page_Strings.Add("Null_Value: " + "");

            if (Null_Enabled)
            {
                Mod_Menu_Page_Strings.Add(Tick_Enabled + " Null Button");
            }
            else
            {
                Mod_Menu_Page_Strings.Add(Tick_Disabled + " Null Button");
            }

            // In case menu width changes.
            if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }
        }

        /// <summary>
        /// Switches the boolean which controls whether an item renders itself as "Enabled" or "Disabled".
        /// </summary>
        /// <param name="Menu_Item"></param>
        public void Switch_UI_Enable_State(Menu_Items Menu_Item)
        {
            switch (Menu_Item)
            {
                case Menu_Items.Menu_Item_Null:
                    if (Null_Enabled) { Null_Enabled = false; } else { Null_Enabled = true; }
                    break;
            }
        }

        /// <summary>
        /// Sends an appropriate message to the base form to be shown in the menu's messagebox... Message is decided by the current highlighted item.
        /// </summary>
        public override void Pass_MessageBox_Message()
        {
            if (Menu_Index == (int)Menu_Items.Menu_Item_Null)
            {
                Set_MessageBox_Message
                (new List<string>()
                    {
                        "Null button description"
                    }
                );
            }
        }

    }
}
