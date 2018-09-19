using System.Collections.Generic;
using SonicHeroes.Controller;

namespace Heroes_Sample_Mod
{
    public class Menu_Experiments : Menu_Base
    {
        // Menu Items Enum
        public Menu_Experiments() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Experimental Stuff");

            Set_Toggle_State();
        }

        // Items contained in this menu.
        public enum Menu_Items
        {
            Menu_Item_Select_Level = 0x00,
            Menu_Item_Enter_Level = 0x01,
            Menu_Item_Easy_Menu_Test = 0x02,
            Menu_Item_SET_Editor = 0x03,
        }

        // Controller Handler
        public override void Handle_Controller_Inputs(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        {
            // Execute Base Controller Code. UP/DOWN navigation + back button.
            base.Handle_Controller_Inputs(P1_Controller, Controller_Manager);

            if (P1_Controller.ControllerButtons.Button_A)
            {
                // Toggle Super Sonic/Tails/Nipples Enchilada.
                if (Menu_Index == (int)Menu_Items.Menu_Item_Enter_Level) { Program.Feature_Force_Load_Level_X.Force_Load_Level(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Easy_Menu_Test) { Program.Feature_Force_Load_Level_X.Force_Load_Debug_Menu(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_SET_Editor) { Program.Feature_Set_Editor_X.SET_Editor_Toggle(); }

                // Set Menu Bounds
                Set_Toggle_State();
                if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }

                // Wait for button release.
                Wait_For_Controller_Release(Controller_Keys.Button_A, Controller_Manager);
            }

            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT)
            {
                if (Menu_Index == (int)Menu_Items.Menu_Item_Select_Level) { Program.Feature_Force_Load_Level_X.Decrement_StageID_Index(); }

                // Set Menu Bounds
                Set_Toggle_State();
                if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }

                // Sleep for Repeat Inputs.
                DPAD_Longpress_Sleep();
            }
            else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT)
            {
                if (Menu_Index == (int)Menu_Items.Menu_Item_Select_Level) { Program.Feature_Force_Load_Level_X.Increment_StageID_Index(); }

                // Set Menu Bounds
                Set_Toggle_State();
                if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }

                // Sleep for Repeat Inputs.
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

            // Append current menu item names
            Mod_Menu_Page_Strings.Add("Select Level: " + Program.Feature_Force_Load_Level_X.Stage_Entry_List[Program.Feature_Force_Load_Level_X.Stage_Enum_Index].Stage_Name);
            Mod_Menu_Page_Strings.Add("Load Selected Level");
            Mod_Menu_Page_Strings.Add("Load Easy Menu");
            Mod_Menu_Page_Strings.Add("Toggle Built-in SET Editor");
 
            // In case menu width changes, update the screen.
            if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }
        }

        /// <summary>
        /// Sends an appropriate message to the base form to be shown in the menu's messagebox... Message is decided by the current highlighted item.
        /// </summary>
        public override void Pass_MessageBox_Message()
        {
            switch (Menu_Index)
            {
                case (int)Menu_Items.Menu_Item_Select_Level:
                    Set_MessageBox_Message(new List<string>() { "Select the Level to Load." });
                    break;
                case (int)Menu_Items.Menu_Item_Enter_Level:
                    Set_MessageBox_Message(new List<string>() { "Loads any level (almost, *cough*, testlevel) at any time, on the fly." });
                    break;
                case (int)Menu_Items.Menu_Item_Easy_Menu_Test:
                    Set_MessageBox_Message(new List<string>() { "The Easy Menu!, Fully restored along with its functionality!", "WARNING: HITTING THIS IS A POINT OF NO RETURN.", "ENTERING SETS AN INVALID MENU STATE, ATTEMPTING TO RE-ENTER MAIN MENU WILL END IN DISASTER,", "SO WILL GENERALLY SWITCHING LEVELS MID LEVEL..." });
                    break;
                case (int)Menu_Items.Menu_Item_SET_Editor:
                    Set_MessageBox_Message(new List<string>() { "Enables Sonic Heroes' Built-in SET Editor for editing stage object layouts.", "Should you be in posession of 4 controllers, do let me know if the controls for this menu actually function.", "May crash in some areas for unknown reasons (Emerald Coast too :/)" });
                    break;
            }

        }

    }
}
