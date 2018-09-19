using System.Collections.Generic;
using SonicHeroes.Controller;

namespace Heroes_Sample_Mod
{
    public class Menu_Sound_Test : Menu_Base
    {
        // Menu Items Enum
        public Menu_Sound_Test() : base()
        {
            // Set capacity of Menu
            Mod_Menu_Title.Add("Sound Menu");

            // Insert all items!
            Set_Toggle_State();
        }

        // Items contained in this menu.
        public enum Menu_Items
        {
            Menu_Item_Current_Bank = 0x00,
            Menu_Item_Current_Sound = 0x01,
            Menu_Item_Current_Music_Track = 0x02,
            Menu_Item_Current_AFS_Archive = 0x03,
            Menu_Item_Current_Bank3_Bank_Archive = 0x04,
            Menu_Item_Current_Disable_Character_Action_Sounds = 0x05,
            Menu_Item_Current_Disable_Character_Comment_Sounds = 0x06,
            Menu_Item_Toggle_Music_OnPause = 0x07,
        }

        // Controller Handler
        public override void Handle_Controller_Inputs(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        {
            // Execute Base Controller Code. UP/DOWN navigation + back button.
            base.Handle_Controller_Inputs(P1_Controller, Controller_Manager);

            // Check for directional button presses
            if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT)
            {
                // Check any known menu items
                if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Bank) { Program.Feature_PAC_Clip_Player_X.Decrement_Current_Bank(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Sound) { Program.Feature_PAC_Clip_Player_X.Decrement_Current_Bank_Sound(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Music_Track) { Program.Feature_ADX_Player_X.Decrement_Current_ADX_Track();}
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_AFS_Archive) { Program.Feature_AFS_Utilities_X.Decrement_Current_AFS_Archive(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Bank3_Bank_Archive) { Program.Feature_PAC_Clip_Player_X.Decrement_Current_Bank3_Bank(); }

                // Refresh Menu State
                Set_Toggle_State();

                // Sleep the thread for repeat inputs.
                DPAD_Longpress_Sleep();
            }
            else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT)
            {
                // Check any known menu items
                if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Bank) { Program.Feature_PAC_Clip_Player_X.Increment_Current_Bank(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Sound) { Program.Feature_PAC_Clip_Player_X.Increment_Current_Bank_Sound(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Music_Track) { Program.Feature_ADX_Player_X.Increment_Current_ADX_Track(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_AFS_Archive) { Program.Feature_AFS_Utilities_X.Increment_Current_AFS_Archive(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Bank3_Bank_Archive) { Program.Feature_PAC_Clip_Player_X.Increment_Current_Bank3_Bank(); }

                // Refresh Menu State
                Set_Toggle_State();

                // Sleep the thread for repeat inputs.
                DPAD_Longpress_Sleep();
            }

            // Toggle any items if necessary.
            if (P1_Controller.ControllerButtons.Button_A)
            {
                // Check any known menu items
                if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Sound) { Program.Feature_PAC_Clip_Player_X.Play_Sound_Bank(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Music_Track) { Program.Feature_ADX_Player_X.Play_ADX_Track(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_AFS_Archive) { Program.Feature_AFS_Utilities_X.Switch_AFS_File(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Bank3_Bank_Archive) { Program.Feature_PAC_Clip_Player_X.Reload_Bank3(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Disable_Character_Action_Sounds) { Program.Feature_Toggle_Character_Chatter_X.Toggle_CharacterChatter(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Current_Disable_Character_Comment_Sounds) { Program.Feature_Toggle_Character_Chatter_X.Toggle_CharacterCommentChatter(); }
                else if (Menu_Index == (int)Menu_Items.Menu_Item_Toggle_Music_OnPause) { Program.Feature_Toggle_Music_OnPause_X.Toggle_Music_OnPause(); }

                // Refresh Menu State
                Set_Toggle_State();

                // Wait for user to release button.
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

            // Append current menu item names
            Mod_Menu_Page_Strings.Add("Current Sound Bank (" + Program.Feature_PAC_Clip_Player_X.Get_Current_PAC_Name() + ") : " + Program.Feature_PAC_Clip_Player_X.Get_Current_PAC_ID());
            Mod_Menu_Page_Strings.Add("Current Sound In Bank : " + Program.Feature_PAC_Clip_Player_X.Get_Current_PAC_Sound_ID());
            Mod_Menu_Page_Strings.Add("Switch Music Track : " + Program.Feature_ADX_Player_X.Get_Current_ADX_Track_Name());
            Mod_Menu_Page_Strings.Add("Switch AFS Sound-Set! : " + Program.Feature_AFS_Utilities_X.Get_Current_AFS_Name());
            Mod_Menu_Page_Strings.Add("Switch Character Speech Sound Bank : " + Program.Feature_PAC_Clip_Player_X.Get_Current_Bank3_PAC_Name());

            // Is XYZ Enabled? Print Y/N
            if (Program.Feature_Toggle_Character_Chatter_X.Get_CharacterChatter()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Character Action Sounds"); } else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Character Action Sounds"); }
            if (Program.Feature_Toggle_Character_Chatter_X.Get_VoiceComments()) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Character Stage Chatter"); } else{ Mod_Menu_Page_Strings.Add(Tick_Disabled + " Character Stage Chatter"); }
            if (Program.Feature_Toggle_Music_OnPause_X.Enabled) { Mod_Menu_Page_Strings.Add(Tick_Enabled + " Do not Disable Music when Game Paused"); } else { Mod_Menu_Page_Strings.Add(Tick_Disabled + " Do not Disable Music when Game Paused"); }

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
                case (int)Menu_Items.Menu_Item_Current_Bank:
                case (int)Menu_Items.Menu_Item_Current_Sound:
                    Set_MessageBox_Message (new List<string>() { "Please note that not all Sound Banks are always loaded.", "To play a sound clip, hover over \"Current Sound In Bank\" and press A/Cross." } );
                    break;
                case (int)Menu_Items.Menu_Item_Current_Music_Track:
                    Set_MessageBox_Message(new List<string>() { "This feature allows you to play any music track on the fly.", "Feel free to try this with your own music tracks." });
                    break;
                case (int)Menu_Items.Menu_Item_Current_AFS_Archive:
                    Set_MessageBox_Message (new List<string>() { "Just Japanese voices wouldn't do *wink*.", "Feel free to switch the voice bank to play any file, from any game.", "\"Damn, I wanna go fishing!\" - Sonic" } );
                    break;
                case (int)Menu_Items.Menu_Item_Current_Bank3_Bank_Archive:
                    Set_MessageBox_Message(new List<string>() { "Changes character action sound effect archive in real time.", "Set this to bank3j if you demand to use Japanese character voices, else have a bit of fun." });
                    break;
                case (int)Menu_Items.Menu_Item_Current_Disable_Character_Action_Sounds:
                    Set_MessageBox_Message(new List<string>() { "Enables/Disables character action voices in real time.", "With this option you can disable or enable character action sounds." });
                    break;
                case (int)Menu_Items.Menu_Item_Current_Disable_Character_Comment_Sounds:
                    Set_MessageBox_Message(new List<string>() { "Disables/Enables character stage-triggered comments.", "\"Whoa! My head's no longer spinning!\"" });
                    break;
                case (int)Menu_Items.Menu_Item_Toggle_Music_OnPause:
                    Set_MessageBox_Message(new List<string>() { "Toggles whether the game pauses the music playback when entering the pause menu." });
                    break;
            }
        }
    }
}
