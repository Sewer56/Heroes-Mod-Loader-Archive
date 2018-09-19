using System;
using System.Collections.Generic;
using System.Threading;
using SonicHeroes.Controller;
using static Heroes_Sample_Mod.Feature_Trail_Editor;
using System.Text;
using static SonicHeroes.Networking.Client_Functions;
using System.Windows.Forms;

namespace Heroes_Sample_Mod
{
    public class Menu_Trail_Editor : Menu_Base
    {
        // Menu Items Enum
        public Menu_Trail_Editor() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Trail Colour Editor");

            // Set MessageBox Message
            Mod_Menu_MessageBox_Strings = new List<string> { "Note: The Alpha value for all tweakable colours available here are almost universally ignored by the game." };

            // Populate Menu Entries
            Trail_Editor_Populate_Current_Menu();
        }

        // Items contained in this menu.
        public enum Menu_Item_List
        {
            Menu_Item_Character = 0x00,
            Menu_Item_Jump_Ball_R = 0x01,
            Menu_Item_Jump_Ball_G = 0x02,
            Menu_Item_Jump_Ball_B = 0x03,
            Menu_Item_Jump_Ball_A = 0x04,
            Menu_Item_Trail_R = 0x05,
            Menu_Item_Trail_G = 0x06,
            Menu_Item_Trail_B = 0x07,
            Menu_Item_Trail_A = 0x08,
            Menu_Item_Tornado_R = 0x09,
            Menu_Item_Tornado_G = 0x0A,
            Menu_Item_Tornado_B = 0x0B,
            Menu_Item_Tornado_A = 0x0C,
            Menu_Item_Sonic_Overdrive_Ball_R = 0x0D,
            Menu_Item_Sonic_Overdrive_Ball_G = 0x0E,
            Menu_Item_Sonic_Overdrive_Ball_B = 0x0F,
            Menu_Item_Sonic_Overdrive_Ball_A = 0x10,
        }

        // String Names of Colour Items for Convenience.
        const string String_Jump_Ball_Colour = "Jump Ball Colour";
        const string String_Trail_Colour = "Trail Colour";
        const string String_Tornado_Colour = "Tornado Colour";
        const string String_Save_Character_Configuration = "Save Character Configuration";
        const string String_Load_Character_Configuration = "Load Character Configuration";
        const string String_Load_Default_Stock_Configuration = "Load Default/Stock Configuration";

        // Controller Handler
        public override void Handle_Controller_Inputs(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        {
            // Execute Base Controller Code. UP/DOWN navigation + back button.
            base.Handle_Controller_Inputs(P1_Controller, Controller_Manager);

            // Check face buttons.
            if (P1_Controller.ControllerButtons.Button_A)
            {
                // String based Menu Item Recognition.
                if (Mod_Menu_Page_Strings[Menu_Index].StartsWith(String_Save_Character_Configuration)) { Program.Feature_Trail_Editor_X.Save_All_Character_Settings(); }
                else if (Mod_Menu_Page_Strings[Menu_Index].StartsWith(String_Load_Character_Configuration)) { Program.Feature_Trail_Editor_X.Load_All_Character_Settings(false); Trail_Editor_Populate_Current_Menu(); }
                else if (Mod_Menu_Page_Strings[Menu_Index].StartsWith(String_Load_Default_Stock_Configuration)) { Program.Feature_Trail_Editor_X.Load_All_Character_Settings(true); Trail_Editor_Populate_Current_Menu(); }

                while (P1_Controller.ControllerButtons.Button_A) { P1_Controller = Controller_Manager.PlayerControllers[controllerIndex].Get_Whole_Controller_State(); Thread.Sleep(8); }
                return;
            }

            // Check DPAD.
            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT)
            {
                // Decrement Currently Selected Character (if on the correct field)
                Increment_Decrement_Current_Character(false);

                // Update Currently Shown Entries
                if (Menu_Index != (int)Menu_Item_List.Menu_Item_Character) { Update_Trail_Editor_Entries(false); }

                // Sleep the thread for repeated inputs.
                Wait_For_Controller_Release(Controller_Keys.DPAD_LEFT, Controller_Manager);
            }
            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT)
            {
                // Increment Currently Selected Character (if on the correct field)
                Increment_Decrement_Current_Character(true);

                // Update Currently Shown Entries
                if (Menu_Index != (int)Menu_Item_List.Menu_Item_Character) { Update_Trail_Editor_Entries(true); }

                // Sleep the thread for repeated inputs.
                Wait_For_Controller_Release(Controller_Keys.DPAD_RIGHT, Controller_Manager);
            }
        }

        /// <summary>
        /// Increments the currently shown menu character or decrements the currently shown menu character to show the last/next character.
        /// </summary>
        public void Increment_Decrement_Current_Character(bool True_if_Incrementing)
        {
            // Decrements and loads values for a character. 
            if (Menu_Index == (int)Menu_Item_List.Menu_Item_Character)
            {
                // Add or remove the current character index.
                if (True_if_Incrementing) { Program.Feature_Trail_Editor_X.Increment_Character_Index(); } else { Program.Feature_Trail_Editor_X.Decrement_Character_Index(); }
                
                // Populate the current menu items.
                Trail_Editor_Populate_Current_Menu();

                // Recalculate Menu Boundaries.
                if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }
            }
        }

        /// <summary>
        /// Writes the currently shown entries in the main menu to the Trail Editor class.
        /// </summary>
        public void Update_Trail_Editor_Entries(bool Set_True_If_Incrementing)
        {
            // Obtain Current Character Information.
            Characters Current_Character = Program.Feature_Trail_Editor_X.Sonic_Heroes_Characters[Program.Feature_Trail_Editor_X.Character_Index];

            // If the menu starts on any of the four possible colour categories.
            if (Mod_Menu_Page_Strings[Menu_Index].StartsWith(String_Jump_Ball_Colour))
            {
                if (Set_True_If_Incrementing)
                {
                    if (Mod_Menu_Page_Strings[Menu_Index].Contains("| R:")) { Current_Character.Character_Ball_Colours.Jump_Ball.R += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| G:")) { Current_Character.Character_Ball_Colours.Jump_Ball.G += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| B:")) { Current_Character.Character_Ball_Colours.Jump_Ball.B += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| A:")) { Current_Character.Character_Ball_Colours.Jump_Ball.A += 1; }
                }
                else
                {
                    if (Mod_Menu_Page_Strings[Menu_Index].Contains("| R:")) { Current_Character.Character_Ball_Colours.Jump_Ball.R -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| G:")) { Current_Character.Character_Ball_Colours.Jump_Ball.G -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| B:")) { Current_Character.Character_Ball_Colours.Jump_Ball.B -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| A:")) { Current_Character.Character_Ball_Colours.Jump_Ball.A -= 1; }
                }

            }
            else if (Mod_Menu_Page_Strings[Menu_Index].StartsWith(String_Tornado_Colour))
            {
                if (Set_True_If_Incrementing)
                {
                    if (Mod_Menu_Page_Strings[Menu_Index].Contains("| R:")) { Current_Character.Character_Ball_Colours.Tornado.R += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| G:")) { Current_Character.Character_Ball_Colours.Tornado.G += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| B:")) { Current_Character.Character_Ball_Colours.Tornado.B += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| A:")) { Current_Character.Character_Ball_Colours.Tornado.A += 1; }
                }
                else
                {
                    if (Mod_Menu_Page_Strings[Menu_Index].Contains("| R:")) { Current_Character.Character_Ball_Colours.Tornado.R -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| G:")) { Current_Character.Character_Ball_Colours.Tornado.G -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| B:")) { Current_Character.Character_Ball_Colours.Tornado.B -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| A:")) { Current_Character.Character_Ball_Colours.Tornado.A -= 1; }
                }
            }
            else if (Mod_Menu_Page_Strings[Menu_Index].StartsWith(String_Trail_Colour))
            {
                if (Set_True_If_Incrementing)
                {
                    if (Mod_Menu_Page_Strings[Menu_Index].Contains("| R:")) { Current_Character.Character_Ball_Colours.Trail.R += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| G:")) { Current_Character.Character_Ball_Colours.Trail.G += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| B:")) { Current_Character.Character_Ball_Colours.Trail.B += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| A:")) { Current_Character.Character_Ball_Colours.Trail.A += 1; }
                }
                else
                {
                    if (Mod_Menu_Page_Strings[Menu_Index].Contains("| R:")) { Current_Character.Character_Ball_Colours.Trail.R -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| G:")) { Current_Character.Character_Ball_Colours.Trail.G -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| B:")) { Current_Character.Character_Ball_Colours.Trail.B -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| A:")) { Current_Character.Character_Ball_Colours.Trail.A -= 1; }
                }
            }
            else if (Mod_Menu_Page_Strings[Menu_Index].StartsWith(Program.Feature_Trail_Editor_X.Sonic_Heroes_Characters[Program.Feature_Trail_Editor_X.Character_Index].Misc_Name))
            {
                if (Set_True_If_Incrementing)
                {
                    if (Mod_Menu_Page_Strings[Menu_Index].Contains("| R:")) { Current_Character.Character_Ball_Colours.Misc.R += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| G:")) { Current_Character.Character_Ball_Colours.Misc.G += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| B:")) { Current_Character.Character_Ball_Colours.Misc.B += 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| A:")) { Current_Character.Character_Ball_Colours.Misc.A += 1; }
                }
                else
                {
                    if (Mod_Menu_Page_Strings[Menu_Index].Contains("| R:")) { Current_Character.Character_Ball_Colours.Misc.R -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| G:")) { Current_Character.Character_Ball_Colours.Misc.G -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| B:")) { Current_Character.Character_Ball_Colours.Misc.B -= 1; }
                    else if (Mod_Menu_Page_Strings[Menu_Index].Contains("| A:")) { Current_Character.Character_Ball_Colours.Misc.A -= 1; }
                }
            }

            // Recalculate Menu Size
            if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }

            // Swap the Current Character.
            Program.Feature_Trail_Editor_X.Sonic_Heroes_Characters[Program.Feature_Trail_Editor_X.Character_Index] = Current_Character;

            // Update Current Character Colours
            Program.Feature_Trail_Editor_X.Write_Character_Colours(Current_Character);

            // Reload Current Character
            Trail_Editor_Populate_Current_Menu();
        }

        /// <summary>
        /// Populates the menu for the character for the currently selected Trail Editor index.
        /// </summary>
        public void Trail_Editor_Populate_Current_Menu()
        {
            // Wipe all current menu options.
            Mod_Menu_Page_Strings.Clear();

            // Get Current Character in a More Accessible Manner
            Characters Current_Character = Program.Feature_Trail_Editor_X.Sonic_Heroes_Characters[Program.Feature_Trail_Editor_X.Character_Index];

            // Write new Menu Option Title.
            Mod_Menu_Page_Strings.Add(String.Format("< Current Character: {0} >", Current_Character.Character_Name));

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Current_Character.Jump_Ball_Address != IntPtr.Zero)
            {
                // Write new Menu Option Title.
                Mod_Menu_Page_Strings.Add(String.Format("{1} | R: {0}", Current_Character.Character_Ball_Colours.Jump_Ball.R, String_Jump_Ball_Colour));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | G: {0}", Current_Character.Character_Ball_Colours.Jump_Ball.G, String_Jump_Ball_Colour));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | B: {0}", Current_Character.Character_Ball_Colours.Jump_Ball.B, String_Jump_Ball_Colour));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | A: {0}", Current_Character.Character_Ball_Colours.Jump_Ball.A, String_Jump_Ball_Colour));
            }

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Current_Character.Trail_Address != IntPtr.Zero)
            {
                // Write new Menu Option Title.
                Mod_Menu_Page_Strings.Add(String.Format("{1} | R: {0}", Current_Character.Character_Ball_Colours.Trail.R, String_Trail_Colour));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | G: {0}", Current_Character.Character_Ball_Colours.Trail.G, String_Trail_Colour));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | B: {0}", Current_Character.Character_Ball_Colours.Trail.B, String_Trail_Colour));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | A: {0}", Current_Character.Character_Ball_Colours.Trail.A, String_Trail_Colour));
            }

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Current_Character.Tornado_Address != IntPtr.Zero)
            {
                // Write new Menu Option Title.
                Mod_Menu_Page_Strings.Add(String.Format("{1} | R: {0}", Current_Character.Character_Ball_Colours.Tornado.R, String_Tornado_Colour));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | G: {0}", Current_Character.Character_Ball_Colours.Tornado.G, String_Tornado_Colour));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | B: {0}", Current_Character.Character_Ball_Colours.Tornado.B, String_Tornado_Colour));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | A: {0}", Current_Character.Character_Ball_Colours.Tornado.A, String_Tornado_Colour));
            }

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Current_Character.Misc_Address != IntPtr.Zero)
            {
                // Write new Menu Option Title.
                Mod_Menu_Page_Strings.Add(String.Format("{1} | R: {0}", Current_Character.Character_Ball_Colours.Misc.R, Current_Character.Misc_Name));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | G: {0}", Current_Character.Character_Ball_Colours.Misc.G, Current_Character.Misc_Name));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | B: {0}", Current_Character.Character_Ball_Colours.Misc.B, Current_Character.Misc_Name));
                Mod_Menu_Page_Strings.Add(String.Format("{1} | A: {0}", Current_Character.Character_Ball_Colours.Misc.A, Current_Character.Misc_Name));
            }

            // Save/Load
            Mod_Menu_Page_Strings.Add(String_Save_Character_Configuration);
            Mod_Menu_Page_Strings.Add(String_Load_Character_Configuration);
            Mod_Menu_Page_Strings.Add(String_Load_Default_Stock_Configuration);

            // Recalculate Menu Size
            if (Program.Sonic_Heroes_Overlay.Ready_To_Render) { Set_Drawing_Properties(); }
        }
    }
}
