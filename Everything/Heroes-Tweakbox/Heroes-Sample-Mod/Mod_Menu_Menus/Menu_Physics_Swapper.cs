using System.Collections.Generic;
using SonicHeroes.Controller;

namespace Heroes_Sample_Mod
{
    public class Menu_Physics_Swapper : Menu_Base
    {
        // Menu Items Enum
        public Menu_Physics_Swapper() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Physics Swapper");

            // Insert all items!
            Set_Toggle_State();
        }

        // Items contained in this menu.
        public enum Menu_Items
        {
            Menu_Item_Character_Assignment_Sonic = 0x00,
            Menu_Item_Character_Assignment_Knuckles = 0x01,
            Menu_Item_Character_Assignment_Tails = 0x02,
            Menu_Item_Character_Assignment_Shadow = 0x03,
            Menu_Item_Character_Assignment_Omega = 0x04,
            Menu_Item_Character_Assignment_Rouge = 0x05,
            Menu_Item_Character_Assignment_Amy = 0x06,
            Menu_Item_Character_Assignment_Big = 0x07,
            Menu_Item_Character_Assignment_Cream = 0x08,
            Menu_Item_Character_Assignment_Espio = 0x09,
            Menu_Item_Character_Assignment_Vector = 0x0A,
            Menu_Item_Character_Assignment_Charmy = 0x0B,
        }


        /// <summary>
        /// Main Menu Controller Handling Method
        /// </summary>
        public override void Handle_Controller_Inputs(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        {
            // Execute Base Controller Code. UP/DOWN navigation + back button.
            base.Handle_Controller_Inputs(P1_Controller, Controller_Manager);

            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT)
            {
                // Toggle X/Y/Z Variable
                if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Sonic) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Sonic); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Knuckles) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Knuckles); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Tails) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Tails); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Shadow) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Shadow); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Omega) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Omega); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Rouge) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Rouge); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Amy) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Amy); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Big) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Big); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Cream) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Cream); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Espio) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Espio); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Vector) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Vector); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Charmy) { Program.Feature_Physics_Swapper_X.Decrement_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Charmy); }

                // Refresh all Strings
                Set_Toggle_State();

                // Longpress DPAD
                DPAD_Longpress_Sleep();
            }
            else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT)
            {
                // Toggle X/Y/Z Variable
                if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Sonic) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Sonic); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Knuckles) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Knuckles); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Tails) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Tails); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Shadow) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Shadow); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Omega) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Omega); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Rouge) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Rouge); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Amy) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Amy); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Big) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Big); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Cream) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Cream); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Espio) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Espio); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Vector) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Vector); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Character_Assignment_Charmy) { Program.Feature_Physics_Swapper_X.Increment_Current_Physics_Character(Feature_Physics_Swap.Characters_Heroes.Charmy); }

                // Refresh all Strings
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
            Mod_Menu_Page_Strings.Add("Sonic's Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Sonic));
            Mod_Menu_Page_Strings.Add("Knuckles' Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Knuckles));
            Mod_Menu_Page_Strings.Add("Tails' Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Tails));
            Mod_Menu_Page_Strings.Add("Shadow's Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Shadow));
            Mod_Menu_Page_Strings.Add("Omega's Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Omega));
            Mod_Menu_Page_Strings.Add("Rouge's Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Rouge));
            Mod_Menu_Page_Strings.Add("Amy's Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Amy));
            Mod_Menu_Page_Strings.Add("Big's Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Big));
            Mod_Menu_Page_Strings.Add("Cream's Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Cream));
            Mod_Menu_Page_Strings.Add("Espio's Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Espio));
            Mod_Menu_Page_Strings.Add("Vector's Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Vector));
            Mod_Menu_Page_Strings.Add("Charmy's Physics Profile: " + Program.Feature_Physics_Swapper_X.Get_Current_Character_Assignment_Name(Feature_Physics_Swap.Characters_Heroes.Charmy));

            // In case menu width changes.
            if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }
        }

        /// <summary>
        /// Sends an appropriate message to the base form to be shown in the menu's messagebox... Message is decided by the current highlighted item.
        /// </summary>
        public override void Pass_MessageBox_Message()
        {
            Set_MessageBox_Message
            (new List<string>()
                {
                    "Swaps the physics of X character with the physics of Y character, live in real time.",
                    "In order to apply the changes, simply switch the leader character to another character than the",
                    "character you are modifying and switch the leader back to the character once you are done."
                }
            );
        }

    }
}
