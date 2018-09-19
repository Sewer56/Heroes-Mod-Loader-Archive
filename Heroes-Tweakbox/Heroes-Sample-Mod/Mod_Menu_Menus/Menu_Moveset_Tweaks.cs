using System;
using System.Collections.Generic;
using SonicHeroes.Controller;

namespace Heroes_Sample_Mod
{
    public class Menu_Moveset_Tweaks : Menu_Base
    {
        // Menu Items Enum
        public Menu_Moveset_Tweaks() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Moveset Tweaks");

            // Insert all items!
            Set_Toggle_State();
        }

        // Items contained in this menu.
        public enum Menu_Items
        {
            Menu_Item_Tornado_Leaf_User = 0x00,
            Menu_Item_Tornado_Hammer_User = 0x01,
            Menu_Item_Unlimited_Triangle_Jump_User = 0x02,
            Menu_Item_Disabled_Triangle_Jump_User = 0x03,
            Menu_Item_Light_Speed_Dash = 0x04,
            Menu_Item_Light_Speed_Attack = 0x05,
            Menu_Item_Invisibility_Fixes = 0x06,
            Menu_Item_Tornado_Hammer_Invisibility = 0x07,
            Menu_Item_Tornado_Spin_Invisibility = 0x08,
            Menu_Item_Player_Starts_Invisible = 0x09,
        }

        // List of Features/Variables

        Feature_Invisibility_Tweaks Invisibility_Tweaks_Feature = new Feature_Invisibility_Tweaks();

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
                if (Menu_Index == (int)Menu_Items.Menu_Item_Light_Speed_Attack) { Program.Feature_Toggle_Moveset_Restrictions_X.Toggle_Light_Speed_Attack();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Light_Speed_Dash) { Program.Feature_Toggle_Moveset_Restrictions_X.Toggle_Light_Speed_Dash();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Invisibility_Fixes) { Program.Feature_Invisibility_Fixes_X.Toggle_Invisibility_Fixes();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Tornado_Hammer_Invisibility) { Invisibility_Tweaks_Feature.Toggle_Hammer_Enables_Invisiblity();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Tornado_Spin_Invisibility) { Invisibility_Tweaks_Feature.Toggle_Spin_Enables_Invisiblity();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Player_Starts_Invisible) { Invisibility_Tweaks_Feature.Toggle_Player_Starts_Invisible();  }

                // Refresh Menu State
                Set_Toggle_State();

                // Set New Menu Bounds
                if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }

                // Wait for button release.
                Wait_For_Controller_Release(Controller_Keys.Button_A, Controller_Manager);
            }

            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT)
            {
                // Toggle X/Y/Z Variable
                if (Menu_Index == (int)Menu_Items.Menu_Item_Tornado_Hammer_User) { Program.Feature_Toggle_Moveset_Restrictions_X.Decrement_Tornado_Hammer_User();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Tornado_Leaf_User) { Program.Feature_Toggle_Moveset_Restrictions_X.Decrement_Tornado_Leaf_Swirl_User();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Unlimited_Triangle_Jump_User) { Program.Feature_Toggle_Moveset_Restrictions_X.Decrement_Unlimited_Triangle_Jump_User();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Disabled_Triangle_Jump_User) { Program.Feature_Toggle_Moveset_Restrictions_X.Decrement_No_Triangle_Jump_User();  }

                // Refresh Menu State
                Set_Toggle_State();

                // Longpress DPAD
                DPAD_Longpress_Sleep();
            }
            else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT)
            {
                // Toggle X/Y/Z Variable
                if (Menu_Index == (int)Menu_Items.Menu_Item_Tornado_Hammer_User) { Program.Feature_Toggle_Moveset_Restrictions_X.Increment_Tornado_Hammer_User();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Tornado_Leaf_User) { Program.Feature_Toggle_Moveset_Restrictions_X.Increment_Tornado_Leaf_Swirl_User();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Unlimited_Triangle_Jump_User) { Program.Feature_Toggle_Moveset_Restrictions_X.Increment_Unlimited_Triangle_Jump_User();  }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Disabled_Triangle_Jump_User) { Program.Feature_Toggle_Moveset_Restrictions_X.Increment_No_Triangle_Jump_User();  }

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

            // Get users of leaf swirl and tornado hammer.
            Feature_Toggle_Moveset_Restrictions.Speed_Character_Moveset_Details Leaf_Swirl_User = Program.Feature_Toggle_Moveset_Restrictions_X.Get_Current_Tornado_Leaf_Swirl_User();
            Feature_Toggle_Moveset_Restrictions.Speed_Character_Moveset_Details Tornado_Hammer_User = Program.Feature_Toggle_Moveset_Restrictions_X.Get_Current_Tornado_Hammer_User();
            Feature_Toggle_Moveset_Restrictions.Speed_Character_Moveset_Details Unlimited_Jump_User = Program.Feature_Toggle_Moveset_Restrictions_X.Get_Current_Unlimited_Triangle_Jump_User();
            Feature_Toggle_Moveset_Restrictions.Speed_Character_Moveset_Details No_Jump_User = Program.Feature_Toggle_Moveset_Restrictions_X.Get_Current_No_Triangle_Jump_User();

            // Append current menu toggle states.
            Mod_Menu_Page_Strings.Add(String.Format("Leaf Swirl User: {0} ({1})", Leaf_Swirl_User.Character_Name, Leaf_Swirl_User.Character_ID));
            Mod_Menu_Page_Strings.Add(String.Format("Tornado Hammer User: {0} ({1})", Tornado_Hammer_User.Character_Name, Tornado_Hammer_User.Character_ID));
            Mod_Menu_Page_Strings.Add(String.Format("Unlimited Triangle Jump Wall Cling User: {0} ({1})", Unlimited_Jump_User.Character_Name, Unlimited_Jump_User.Character_ID));
            Mod_Menu_Page_Strings.Add(String.Format("Disabled Triangle Jump User: {0} ({1})", No_Jump_User.Character_Name, No_Jump_User.Character_ID));

            // Light Speed Dash Enabled?
            if (Program.Feature_Toggle_Moveset_Restrictions_X.Get_Light_Speed_Dash_State()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Light Speed Dash"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Light Speed Dash"); }

            // Light Speed Attack Enabled?
            if (Program.Feature_Toggle_Moveset_Restrictions_X.Get_Light_Speed_Attack_State()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Light Speed Attack"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Light Speed Attack"); }

            // Invisibility Fixes
            if (Program.Feature_Invisibility_Fixes_X.Get_Invisibility_Fixes_Status()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Invisibility Fixes"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Invisibility Fixes"); }

            // Tornado Hammer Enabled Invisibility
            if (Invisibility_Tweaks_Feature.Get_Hammer_Enables_Invisiblity()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Tornado Hammer Enables Invisibility"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Tornado Hammer Enables Invisibility"); }

            // Tornado Spin Enabled Invisibility
            if (Invisibility_Tweaks_Feature.Get_Spin_Enables_Invisiblity()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Tornado Spin Enables Invisibility"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Tornado Spin Enables Invisibility"); }

            // Player Respawns Invisible
            if (Invisibility_Tweaks_Feature.Get_Player_Starts_Invisible()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Player Respawns Invisible"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Player Respawns Invisible"); }

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
                case (int)Menu_Items.Menu_Item_Tornado_Hammer_User:
                    Set_MessageBox_Message(new List<string>() { "Sets the speed type user of the tornado hammer technique (Amy's Tornado Swing).", "Removing this default from Amy will allow her to use the regular normally unused tornado technique." });
                    break;
                case (int)Menu_Items.Menu_Item_Tornado_Leaf_User:
                    Set_MessageBox_Message(new List<string>() { "Sets the speed type user of the tornado leaf technique (Espio's Invisibility).", "Removing this default from Espio will allow him to use the regular normally unused tornado technique." });
                    break;
                case (int)Menu_Items.Menu_Item_Unlimited_Triangle_Jump_User:
                    Set_MessageBox_Message(new List<string>() { "This character can cling onto walls for an unlimited amount of time during triangle jump.", "This is normally a property of Espio." });
                    break;
                case (int)Menu_Items.Menu_Item_Disabled_Triangle_Jump_User:
                    Set_MessageBox_Message(new List<string>() { "This character cannot cling onto walls, i.e. cannot perform triangle jump.", "This is normally a property of Amy." });
                    break;
                case (int)Menu_Items.Menu_Item_Light_Speed_Dash:
                    Set_MessageBox_Message(new List<string>() { "Enables Light Speed Dash for all characters.", "This is removed from Amy and Espio by default." });
                    break;
                case (int)Menu_Items.Menu_Item_Light_Speed_Attack:
                    Set_MessageBox_Message(new List<string>() { "Enables Light Speed Attack (Sonic Overdrive) for all characters." });
                    break;
                case (int)Menu_Items.Menu_Item_Invisibility_Fixes:
                    Set_MessageBox_Message(new List<string>() { "Toggles fixes for when other characters than Espio, such as Sonic", "Example: Enemies cannot see you anymore.", "Use in conjunction with turning invisible via the use of moveset tweaks." });
                    break;
                case (int)Menu_Items.Menu_Item_Tornado_Hammer_Invisibility:
                    Set_MessageBox_Message(new List<string>() { "Using Amy's tornado attack (Tornado Hammer) turns her invisible." });
                    break;
                case (int)Menu_Items.Menu_Item_Tornado_Spin_Invisibility:
                    Set_MessageBox_Message(new List<string>() { "Using the tornado attack of Sonic/Shadow (Tornado Spin) turns one invisible." });
                    break;
                case (int)Menu_Items.Menu_Item_Player_Starts_Invisible:
                    Set_MessageBox_Message(new List<string>() { "When the player enters a stage or respawns after death, they will turn into a", "state of invisibility (think Espio)." });
                    break;
            }
        }
    }
}
