using System;
using System.Collections.Generic;
using SonicHeroes.Controller;

namespace Heroes_Sample_Mod
{
    public class Menu_Graphics_Tweaks : Menu_Base
    {
        // Menu Items Enum
        public Menu_Graphics_Tweaks() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Graphics Tweaks");

            // Insert all items!
            Set_Toggle_State();
        }

        // Items contained in this menu.
        public enum Menu_Items
        {
            Menu_Item_Field_Of_View_Scale = 0x00,
            Menu_Item_RGB_Colour_Cycle_Rate = 0x01,
            Menu_Item_Toggle_Aspect_Ratio_Fix = 0x02,
            Menu_Item_RGB_Formation_Barriers = 0x03,
            Menu_Item_RGB_Character_Balls = 0x04,
            Menu_Item_RGB_Character_Trails = 0x05,
            Menu_Item_RGB_Character_Tornadoes = 0x06,
            Menu_Item_RGB_Combined = 0x07,
        }

        // Controller Handler
        public override void Handle_Controller_Inputs(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        {
            // Execute Base Controller Code. UP/DOWN navigation + back button.
            base.Handle_Controller_Inputs(P1_Controller, Controller_Manager);

            // Check any known controller buttons.
            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT)
            {
                if (Menu_Index == (int)Menu_Items.Menu_Item_Field_Of_View_Scale) { Program.Feature_Enhanced_FOV_Fix_X.Decrement_Field_Of_View_Scale(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_RGB_Colour_Cycle_Rate) { Program.Feature_Cycle_RGB_Colours_X.Decrement_Colours_Per_Second(); }

                // Refresh Menu State
                Set_Toggle_State();

                // Sleep the thread for repeat inputs.
                DPAD_Longpress_Sleep();
            }
            else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT)
            {
                if (Menu_Index == (int)Menu_Items.Menu_Item_Field_Of_View_Scale) { Program.Feature_Enhanced_FOV_Fix_X.Increment_Field_Of_View_Scale(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_RGB_Colour_Cycle_Rate) { Program.Feature_Cycle_RGB_Colours_X.Increment_Colours_Per_Second(); }

                // Refresh Menu State
                Set_Toggle_State();

                // Sleep the thread for repeat inputs.
                DPAD_Longpress_Sleep();
            }
            
            // Check any known button presses.
            if (P1_Controller.ControllerButtons.Button_A)
            {
                // Check Indexes
                if (Menu_Index == (int)Menu_Items.Menu_Item_Toggle_Aspect_Ratio_Fix) { Program.Feature_Enhanced_FOV_Fix_X.Button_Set_FOV_Aspect_Ratio_Fix(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_RGB_Formation_Barriers) { Program.Feature_Cycle_RGB_Colours_X.Toggle_HUE_Cycle_Formation_Barriers(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_RGB_Character_Balls) { Program.Feature_Cycle_RGB_Colours_X.Toggle_HUE_Cycle_Character_Balls(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_RGB_Character_Trails) { Program.Feature_Cycle_RGB_Colours_X.Toggle_HUE_Cycle_Character_Trails(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_RGB_Character_Tornadoes) { Program.Feature_Cycle_RGB_Colours_X.Toggle_HUE_Cycle_Character_Tornadoes(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_RGB_Combined) { Program.Feature_Cycle_RGB_Colours_X.Toggle_HUE_Cycle_All_Combined(); }

                // Refresh Menu State
                Set_Toggle_State();

                // Wait for release.
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

            Mod_Menu_Page_Strings.Add("Field of View Scale: " + Program.Feature_Enhanced_FOV_Fix_X.Get_FOVScale().ToString("0.00") + "F");
            Mod_Menu_Page_Strings.Add(String.Format("Approximate Colours Per Second: {0} ({1})", Program.Feature_Cycle_RGB_Colours_X.Get_Colours_Per_Second().ToString("000"), Program.Feature_Cycle_RGB_Colours_X.Get_Sleep_Delay().ToString("000")));

            // Enhanced Aspect Ratio FOV Fix
            if (Program.Feature_Enhanced_FOV_Fix_X.Get_IsEnabled()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Enhanced Aspect Ratio/FOV Fix"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Enhanced Aspect Ratio/FOV Fix"); }

            // Cycling Formation Barrier Colours
            if (Program.Feature_Cycle_RGB_Colours_X.Get_CyclingBarrierHue()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " RGB Formation Barriers"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " RGB Formation Barriers"); }

            // Cycling Ball Colours
            if (Program.Feature_Cycle_RGB_Colours_X.Get_CyclingBallHue()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " RGB Character Balls"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " RGB Character Balls"); }

            // Cycling Trail Colours
            if (Program.Feature_Cycle_RGB_Colours_X.Get_CyclingTrailHue()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " RGB Character Trails"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " RGB Character Trails"); }

            // Cycling Tornado Colours
            if (Program.Feature_Cycle_RGB_Colours_X.Get_CyclingTornadoHue()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " RGB Character Tornadoes"); }
            else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " RGB Character Tornadoes"); }

            if (Program.Feature_Cycle_RGB_Colours_X.Get_CyclingAllHue())
            {
                Mod_Menu_Page_Strings.Add(Tick_Enabled + " Synchronous RGB Barriers, Tornadoes, Trails & Balls");
                Mod_Menu_Page_Strings[(int)Menu_Items.Menu_Item_RGB_Character_Tornadoes] = (Tick_Enabled + " RGB Character Tornadoes");
                Mod_Menu_Page_Strings[(int)Menu_Items.Menu_Item_RGB_Character_Trails] = Tick_Enabled + " RGB Character Trails";
                Mod_Menu_Page_Strings[(int)Menu_Items.Menu_Item_RGB_Character_Balls] = (Tick_Enabled + " RGB Character Balls");
                Mod_Menu_Page_Strings[(int)Menu_Items.Menu_Item_RGB_Formation_Barriers] = (Tick_Enabled + " RGB Formation Barriers");
            }
            else
            {
                Mod_Menu_Page_Strings.Add(Tick_Disabled + " Synchronous RGB Barriers, Tornadoes, Trails & Balls");
            }

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
                case (int)Menu_Items.Menu_Item_Field_Of_View_Scale:
                    Set_MessageBox_Message(new List<string>() { "Scales the currently shown horizontal and vertical aspect ratios, ", "resulting in an increased FOV. This setting allows you to play at wider", "viewing angles, permitting you to see more of the world, enhanced to deliver zero clipping." });
                    break;
                case (int)Menu_Items.Menu_Item_RGB_Colour_Cycle_Rate:
                    Set_MessageBox_Message(new List<string>() { "Sets the approximate number of colour switches per second for RGB Colour Cycling.", "The actual true value spent waiting in milliseconds for next colour is shown directly in brackets." });
                    break;
                case (int)Menu_Items.Menu_Item_Toggle_Aspect_Ratio_Fix:
                    Set_MessageBox_Message(new List<string>() { "Toggles the Enhanced Aspect Ratio fix of this mod :3", "In order for the changes to take effect, the main menu maestro needs to be reloaded.", "To force a reload, press ALT+F4 and hit Cancel (that generally does it), alternatively enter and exit a stage." });
                    break;
                case (int)Menu_Items.Menu_Item_RGB_Combined:
                    Set_MessageBox_Message(new List<string>() { "Fulfills the deepest wet dreams of (almost) every PC builder.", "Makes a few lights go pretty RGB, and a bit more, and a bit more...", "< Insert Corsair Joke Here >" });
                    break;
                case (int)Menu_Items.Menu_Item_RGB_Formation_Barriers:
                    Set_MessageBox_Message(new List<string>() { "Hue cycles the character wonderful, blisterous lights emitting from the formation gates!", "Soo... pretty...." });
                    break;
                case (int)Menu_Items.Menu_Item_RGB_Character_Balls:
                    Set_MessageBox_Message(new List<string>() { "Hue cycles the character Aura Balls seen onscreen when you, for example, jump.", "Pretty... colooooors......." });
                    break;
                case (int)Menu_Items.Menu_Item_RGB_Character_Trails:
                    Set_MessageBox_Message(new List<string>() { "Moooarrrrr..... Pretty... colooooors......." });
                    break;
                case (int)Menu_Items.Menu_Item_RGB_Character_Tornadoes:
                    Set_MessageBox_Message(new List<string>() { "It's not like I d-don't like colours...." });
                    break;
            }
        }
    }
}
