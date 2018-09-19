using System.Collections.Generic;
using SonicHeroes.Controller;
using static SonicHeroes.Networking.Client_Functions;
using System.Text;

namespace Heroes_Sample_Mod
{
    public class Menu_Main_Menu : Menu_Base
    {
        public Menu_Main_Menu() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Main Menu");

            // Insert all menu strings!
            Mod_Menu_Page_Strings.Add("Sound Menu");
            Mod_Menu_Page_Strings.Add("Physics Swapper");
            Mod_Menu_Page_Strings.Add("Moveset Tweaks");
            Mod_Menu_Page_Strings.Add("Gameplay Tweaks");
            Mod_Menu_Page_Strings.Add("HUD Tweaks Menu");
            Mod_Menu_Page_Strings.Add("Graphics Tweaks");
            Mod_Menu_Page_Strings.Add("Trail Colour Editor");
            Mod_Menu_Page_Strings.Add("Miscellaneous Tweaks");
            Mod_Menu_Page_Strings.Add("Experimental Stuff");
            Mod_Menu_Page_Strings.Add("Debugging Assists");

            // Set universal menu description.
            Mod_Menu_MessageBox_Strings = new List<string> { "Use the DPAD to navigate the various menus and options.", "Pressing A will apply/select the currently highlighted item.", "Variables can be adjusted by using the DPAD L and DPAD R and menu toggled by pressing LS.", "The \"Sonic Adventure\" Hub World Demo can be toggled on/off within the \"Experiments\" Menu." };
        }

        // Items contained in this menu.
        public enum Menu_Items
        {
            Menu_Item_Sound_Test = 0x00,
            Menu_Item_Physics_Swap = 0x01,
            Menu_Item_Moveset_Tweaks = 0x02,
            Menu_Item_Gameplay_Items = 0x03,
            Menu_Item_HUD_Tweaks = 0x04,
            Menu_Item_Graphics_Tweaks = 0x05,
            Menu_Item_Trail_Editor = 0x06,
            Menu_Item_Misc = 0x07,
            Menu_Item_Experiments = 0x08,
            Menu_Item_Debug = 0x09,
        }

        /// <summary>
        /// Main Menu Controller Handling Method
        /// </summary>
        public override void Handle_Controller_Inputs(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        {
            // Up/Down Menu Movement with DPAD.
            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.DOWN) { Increment_Menu_Index(); Wait_For_Controller_Release(Controller_Keys.DPAD_DOWN, Controller_Manager); Pass_MessageBox_Message(); return; }
            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.UP) { Decrement_Menu_Index(); Wait_For_Controller_Release(Controller_Keys.DPAD_UP, Controller_Manager); Pass_MessageBox_Message(); return; }

            // Check any known indexes
            if (P1_Controller.ControllerButtons.Button_A)
            {
                if (Menu_Index == (int)Menu_Items.Menu_Item_Sound_Test) { Program.Set_New_Menu(Program.Menu_Sound_Test); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Physics_Swap) { Program.Set_New_Menu(Program.Menu_Physics_Swapper); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Gameplay_Items) { Program.Set_New_Menu(Program.Menu_Gameplay_Items); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Moveset_Tweaks) { Program.Set_New_Menu(Program.Menu_Moveset_Tweaks); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Graphics_Tweaks) { Program.Set_New_Menu(Program.Menu_Graphics_Tweaks); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_HUD_Tweaks) { Program.Set_New_Menu(Program.Menu_HUD_Adjustments); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Trail_Editor) { Program.Set_New_Menu(Program.Menu_Trail_Editor); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Misc) { Program.Set_New_Menu(Program.Menu_Miscallenous); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Experiments) { Program.Set_New_Menu(Program.Menu_Experiments); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Debug) { Program.Set_New_Menu(Program.Menu_Debug_Stuff); }

                // Wait for button release.
                Wait_For_Controller_Release(Controller_Keys.Button_A, Controller_Manager);
            }
        }
    }
}
