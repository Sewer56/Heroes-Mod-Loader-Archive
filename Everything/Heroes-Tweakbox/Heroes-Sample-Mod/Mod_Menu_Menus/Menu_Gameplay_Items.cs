using System.Collections.Generic;
using SonicHeroes.Controller;

namespace Heroes_Sample_Mod
{
    public class Menu_Gameplay_Items : Menu_Base
    {
        // Menu Items Enum
        public Menu_Gameplay_Items() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Gameplay Tweaks");

            // Insert all items!
            Set_Toggle_State();
        }

        // Items contained in this menu.
        public enum Menu_Items
        {
            Menu_Item_Magnetic_Barrier_Base_Speed = 0x00,
            Menu_Item_Magnetic_Barrier_Distance_Speed = 0x01,
            Menu_Item_Magnetic_Barrier = 0x02,
            Menu_Item_Party_Mode = 0x03,
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
                if (Menu_Index == (int)Menu_Items.Menu_Item_Magnetic_Barrier) { Program.Feature_Magnetic_Barrier_X.Toggle_Magnetic_Barrier();}
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Party_Mode) { Program.Feature_Party_Mode_X.Toggle_Party_Mode(); }

                // Refresh Menu State
                Set_Toggle_State();

                // Set New Menu Bounds
                if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }

                // Wait for button release.
                Wait_For_Controller_Release(Controller_Keys.Button_A, Controller_Manager);
            }
            else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT)
            {
                // Toggle Super Sonic/Tails/Nipples Enchilada.
                if (Menu_Index == (int)Menu_Items.Menu_Item_Magnetic_Barrier_Base_Speed) { Program.Feature_Magnetic_Barrier_X.Decrement_Base_Speed(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Magnetic_Barrier_Distance_Speed) { Program.Feature_Magnetic_Barrier_X.Decrement_Distance_Speed_Scale();  }

                // Refresh Menu State
                Set_Toggle_State();

                // Longpress DPAD
                DPAD_Longpress_Sleep();
            }
            else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT)
            {
                // Toggle Super Sonic/Tails/Nipples Enchilada.
                if (Menu_Index == (int)Menu_Items.Menu_Item_Magnetic_Barrier_Base_Speed) { Program.Feature_Magnetic_Barrier_X.Increment_Base_Speed(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Magnetic_Barrier_Distance_Speed) { Program.Feature_Magnetic_Barrier_X.Increment_Distance_Speed_Scale(); }

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
            Mod_Menu_Page_Strings.Add("Magnetic Barrier Base Suction Speed: " + Program.Feature_Magnetic_Barrier_X.Get_Base_Speed().ToString("0.00F"));
            Mod_Menu_Page_Strings.Add("Magnetic Barrier Distance Dropoff Speed: " + Program.Feature_Magnetic_Barrier_X.Get_Distance_Speed().ToString("0.000F"));

            if (Program.Feature_Magnetic_Barrier_X.Get_IsEnabled()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Magnetic Barrier"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Magnetic Barrier"); }

            if (Program.Feature_Party_Mode_X.Get_ToggleState()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Party Mode"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Party Mode"); }


            // In case menu width changes.
            if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }
        }

        /// <summary>
        /// Sends an appropriate message to the base form to be shown in the menu's messagebox... Message is decided by the current highlighted item.
        /// </summary>
        public override void Pass_MessageBox_Message()
        {
            switch (Menu_Index)
            {
                case (int)Menu_Items.Menu_Item_Magnetic_Barrier:
                    Set_MessageBox_Message(new List<string>() { "Enables an invisible permanent magnetic barrier.", "Nearby rings will feel a rush of immense lust, experiencing a physical force of attraction to you." });
                    break;
                case (int)Menu_Items.Menu_Item_Magnetic_Barrier_Base_Speed:
                    Set_MessageBox_Message(new List<string>() { "The constant speed at which rings will approach the player.", "Higher value means faster approaching rings." });
                    break;
                case (int)Menu_Items.Menu_Item_Magnetic_Barrier_Distance_Speed:
                    Set_MessageBox_Message(new List<string>() { "Distance based multiplier used for varying a constant velocity rate for rings far away.", "Higher values reduce suction on far away rings." });
                    break;
                case (int)Menu_Items.Menu_Item_Party_Mode:
                    Set_MessageBox_Message(new List<string>() { "Woohoo!!" });
                    break;
            }
        }

    }
}
