using System.Collections.Generic;
using SonicHeroes.Controller;

namespace Heroes_Sample_Mod
{
    public class Menu_Misc_Menu : Menu_Base
    {
        // Menu Items Enum
        public Menu_Misc_Menu() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Miscellaneous Options");

            // Insert all items!
            Set_Toggle_State();
        }

        // Items contained in this menu.
        public enum Misc_Menu_Items
        {
            Menu_Item_Super_Sonic_Flag = 0x00,
            Menu_Item_Super_Tails_Flag = 0x01,
            Menu_Item_Super_Knuckles_Flag = 0x02,
            Menu_Item_Metal_Characters_Flag = 0x03,
        }

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
                if (Menu_Index == (int)Misc_Menu_Items.Menu_Item_Super_Sonic_Flag) { Program.Feature_Toggle_Super_Metal_Characters_X.Toggle_Super_Sonic(); }
                else if (Menu_Index == (int)Misc_Menu_Items.Menu_Item_Super_Tails_Flag) { Program.Feature_Toggle_Super_Metal_Characters_X.Toggle_Super_Tails(); }
                else if (Menu_Index == (int)Misc_Menu_Items.Menu_Item_Super_Knuckles_Flag) { Program.Feature_Toggle_Super_Metal_Characters_X.Toggle_Super_Knuckles(); }
                else if (Menu_Index == (int)Misc_Menu_Items.Menu_Item_Metal_Characters_Flag) { Program.Feature_Toggle_Super_Metal_Characters_X.Toggle_Metal_Characters(); }

                // Redraw Menu
                Set_Toggle_State();

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

            // Append current menu toggle states.
            if (Program.Feature_Toggle_Super_Metal_Characters_X.Get_SuperSonicEnabled()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Always Super Sonic"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Always Super Sonic"); }

            if (Program.Feature_Toggle_Super_Metal_Characters_X.Get_SuperTailsEnabled()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Always Super Tails"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Always Super Tails"); }

            if (Program.Feature_Toggle_Super_Metal_Characters_X.Get_SuperKnucklesEnabled()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Always Super Knuckles"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Always Super Knuckles"); }

            if (Program.Feature_Toggle_Super_Metal_Characters_X.Get_MetalCharactersEnabled()) {  Mod_Menu_Page_Strings.Add(Tick_Enabled + " Metallic Characters Combination for 1P"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Metallic Characters Combination for 1P"); }

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
                case (int)Misc_Menu_Items.Menu_Item_Super_Sonic_Flag:
                    Set_MessageBox_Message(new List<string>() { "Enables real Super Sonic, Super Knuckles and/or real Super Tails when playing the game.", "Please do not use with Story Mode Cutscenes, I'll probably fix them when I'll stop being lazy - one day..." });
                    break;
                case (int)Misc_Menu_Items.Menu_Item_Super_Tails_Flag:
                    Set_MessageBox_Message(new List<string>() { "Enables real Super Sonic, Super Knuckles and/or real Super Tails when playing the game.", "Please do not use with Story Mode Cutscenes, I'll probably fix them when I'll stop being lazy - one day..." });
                    break;
                case (int)Misc_Menu_Items.Menu_Item_Super_Knuckles_Flag:
                    Set_MessageBox_Message(new List<string>() { "Enables real Super Sonic, Super Knuckles and/or real Super Tails when playing the game.", "Please do not use with Story Mode Cutscenes, I'll probably fix them when I'll stop being lazy - one day..." });
                    break;
                case (int)Misc_Menu_Items.Menu_Item_Metal_Characters_Flag:
                    Set_MessageBox_Message(new List<string>() { "Enables the hidden secret of Metal Characters when simultaneously holding A & Y in 1P Mode.", "It is recommended that you do not use this with Team Super." });
                    break;
            }
        }
    }
}
