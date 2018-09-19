using SharpDX.Direct2D1;
using SonicHeroes.Controller;
using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Enables movement which resembles SADX' Debug Mode.
    /// </summary>
    public class Feature_Enhanced_Debug_Movement_Mode
    {
        // Constructor
        public Feature_Enhanced_Debug_Movement_Mode()
        {
            Free_Movement_Mode_Menu = new Menu_Free_Movement_Mode();
        }

        // Enabler/Disabler Flag
        public bool Free_Movement_Feature_Enabled;
        public bool Free_Movement_Enabled;

        // Max movement speed
        float Max_Movement_Speed = 5.0F;

        // Index
        private int Player_Position_Index = 0;

        // Free Movement Mode Menu
        private Menu_Free_Movement_Mode Free_Movement_Mode_Menu;

        // Array of player positions
        Player_Position_Entry[] Player_Positions = new Player_Position_Entry[10];

        // Array of player positions
        Player_Position_Entry Current_Player_Position = new Player_Position_Entry();

        /// <summary>
        /// Player Position Entry Struct, Defines a Player Position
        /// </summary>
        struct Player_Position_Entry
        {
            public bool Position_Set;
            public float Character_X;
            public float Character_Y;
            public float Character_Z;
        }

        // Enabler/Disabler of Free Movement Mode.
        public bool Get_Free_Movement_Enable_Status() { return Free_Movement_Feature_Enabled; }

        // Toggle Free Movement Mode
        public void Toggle_Free_Movement_Mode()
        {
            if (Free_Movement_Feature_Enabled)
            {
                Program.Sonic_Heroes_Overlay.direct2DRenderMethod -= Draw_Overlay;
                Program.Controller_Poll_Delegate -= Controller_Handler; // Add the controller handler (Drawing is handled in same method)
                Free_Movement_Feature_Enabled = false;
            }
            else
            {
                Program.Sonic_Heroes_Overlay.direct2DRenderMethod += Draw_Overlay;
                Program.Controller_Poll_Delegate += Controller_Handler; // Add the controller handler (Drawing is handled in same method)
                Free_Movement_Feature_Enabled = true;
            }
        }

        /// Disable Default InputHandler
        public void Controller_Handler(Sonic_Heroes_Joystick.Controller_Inputs_Generic P1_Controller, SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Manager)
        {
            // Get Information of whether the overlay should be shown and controls enabled.
            byte Is_Currently_In_Level = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);

            // If menu is not open
            if (! Program.Draw_Current_Menu && Is_Currently_In_Level == 1)
            {
                if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT)
                {
                    // Back_Up_Characters
                    Back_Up_Character();

                    // Set Menu Text
                    Free_Movement_Mode_Menu.Set_Menu_Text(Player_Position_Index, Player_Positions[Player_Position_Index].Character_X, Player_Positions[Player_Position_Index].Character_Y, Player_Positions[Player_Position_Index].Character_Z);

                    // Longpress DPAD
                    Free_Movement_Mode_Menu.Wait_For_Controller_Release(Menu_Base.Controller_Keys.DPAD_LEFT, Controller_Manager);
                }
                else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT)
                {
                    // Restore Characters
                    Restore_Character();

                    // Longpress DPAD
                    Free_Movement_Mode_Menu.Wait_For_Controller_Release(Menu_Base.Controller_Keys.DPAD_RIGHT, Controller_Manager);
                }
                else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.UP)
                {
                    // Toggle Free Movement Mode
                    if (Free_Movement_Enabled)
                    {
                        // Enable Camera
                        Program.Sonic_Heroes_Process.WriteMemory
                        (
                            (IntPtr)SonicHeroesVariables.Game_Camera_CurrentCamera.Camera_Enabled,
                            BitConverter.GetBytes((short)1)
                        );

                        // Enable Gravity/Natural Positioning
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x599376, new byte[] { 0xD9, 0x9D, 0x18, 0x01, 0x00, 0x00 });
                        //Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x599366, new byte[] { 0xD9, 0x19 });
                        //Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x59938A, new byte[] { 0xD9, 0x9D, 0x1C, 0x01, 0x00, 0x00 });

                        Free_Movement_Enabled = false;
                    }
                    else
                    {
                        // Disable Camera
                        Program.Sonic_Heroes_Process.WriteMemory
                        (
                            (IntPtr)SonicHeroesVariables.Game_Camera_CurrentCamera.Camera_Enabled,
                            BitConverter.GetBytes((short)0)
                        );

                        // Reset Camera Rotation
                        Program.Sonic_Heroes_Process.WriteMemory
                        (
                            (IntPtr)SonicHeroesVariables.Game_Camera_CurrentCamera.Camera_Angle_Horizontal_BAMS,
                            BitConverter.GetBytes((short)0)
                        );

                        // Disable Gravity/Natural Positioning
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x599376, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
                        //Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x599366, new byte[] { 0x90, 0x90 });
                        //Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x59938A, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });

                        // Get Initial Positions.
                        int Player_Pointer = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)SonicHeroesVariables.Characters_Addresses.CurrentPlayerControlledCharacter_Pointer, 4);
                        Current_Player_Position.Character_X = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.XPosition + Player_Pointer, 4);
                        Current_Player_Position.Character_Y = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.YPosition + Player_Pointer, 4);
                        Current_Player_Position.Character_Z = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.ZPosition + Player_Pointer, 4);

                        Free_Movement_Enabled = true;
                    }

                    // Longpress DPAD
                    Free_Movement_Mode_Menu.Wait_For_Controller_Release(Menu_Base.Controller_Keys.DPAD_UP, Controller_Manager);
                }
                else if (P1_Controller.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.DOWN)
                {
                    // Cycle Current Position
                    Increment_Current_Position();

                    // Longpress DPAD
                    Free_Movement_Mode_Menu.Wait_For_Controller_Release(Menu_Base.Controller_Keys.DPAD_DOWN, Controller_Manager);
                }

                // If free movement is enabled, handle free movement.
                if (Free_Movement_Enabled)
                {
                    // Get Controller Information
                    Sonic_Heroes_Joystick.Controller_Inputs_Generic Heroes_Controller_Button_Info = Controller_Manager.PlayerControllers[Menu_Base.controllerIndex].Get_Whole_Controller_State();

                    // Get Axis Values
                    float Stick_X = P1_Controller.LeftStick.X * 0.001F; // Normal Range = -1000 to 1000. Scale to -1 to 1
                    float Stick_Y = P1_Controller.LeftStick.Y * 0.001F; // Normal Range = -1000 to 1000. Scale to -1 to 1

                    float LT_Pressure = (P1_Controller.LeftTriggerPressure + 1000) * 0.0005F; // Normal Range = -1000 to 1000. Changed to 0 - 2000.
                    float RT_Pressure = (P1_Controller.RightTriggerPressure + 1000) * 0.0005F; // Normal Range = -1000 to 1000. Changed to 0 - 2000.

                    // 20% Deadzone
                    if (Stick_X < 0.20F && Stick_X > -0.20F) { Stick_X = 0.0F; }
                    if (Stick_Y < 0.20F && Stick_Y > -0.20F) { Stick_Y = 0.0F; }
                    if (LT_Pressure < 0.20F) { LT_Pressure = 0.0F; }
                    if (RT_Pressure < 0.20F) { RT_Pressure = 0.0F; }

                    // Get Current Player Location
                    int Player_Pointer = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)SonicHeroesVariables.Characters_Addresses.CurrentPlayerControlledCharacter_Pointer, 4);

                    // Set new positions
                    Current_Player_Position.Character_X += Stick_X * Max_Movement_Speed;
                    Current_Player_Position.Character_Y = Current_Player_Position.Character_Y - (LT_Pressure * Max_Movement_Speed) + (RT_Pressure * Max_Movement_Speed);
                    Current_Player_Position.Character_Z += Stick_Y * Max_Movement_Speed;

                    // Manipulate Positions
                    Program.Sonic_Heroes_Process.WriteMemory
                    (
                        (IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.XPosition + Player_Pointer,
                        BitConverter.GetBytes(Current_Player_Position.Character_X)
                    );

                    Program.Sonic_Heroes_Process.WriteMemory
                    (
                        (IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.YPosition + Player_Pointer, 
                        BitConverter.GetBytes(Current_Player_Position.Character_Y)
                    );

                    Program.Sonic_Heroes_Process.WriteMemory
                    (
                        (IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.ZPosition + Player_Pointer,
                        BitConverter.GetBytes(Current_Player_Position.Character_Z)
                    );

                    // Manipulate Camera Positions
                    Program.Sonic_Heroes_Process.WriteMemory
                    (
                        (IntPtr)SonicHeroesVariables.Game_Camera_CurrentCamera.Camera_X,
                        BitConverter.GetBytes(Current_Player_Position.Character_X)
                    );

                    Program.Sonic_Heroes_Process.WriteMemory
                    (
                        (IntPtr)SonicHeroesVariables.Game_Camera_CurrentCamera.Camera_Y,
                        BitConverter.GetBytes(Current_Player_Position.Character_Y + 20.0F)
                    );

                    Program.Sonic_Heroes_Process.WriteMemory
                    (
                        (IntPtr)SonicHeroesVariables.Game_Camera_CurrentCamera.Camera_Z,
                        BitConverter.GetBytes(Current_Player_Position.Character_Z + 70.0F)
                    );
                }
            }
        }

        /// <summary>
        /// Backs up the currently selected character.
        /// </summary>
        private void Back_Up_Character()
        {
            int Player_Pointer = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)SonicHeroesVariables.Characters_Addresses.CurrentPlayerControlledCharacter_Pointer, 4); 

            // Back up coordinates
            Player_Positions[Player_Position_Index].Character_X = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.XPosition + Player_Pointer, 4);
            Player_Positions[Player_Position_Index].Character_Y = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.YPosition + Player_Pointer, 4);
            Player_Positions[Player_Position_Index].Character_Z = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.ZPosition + Player_Pointer, 4);
            Player_Positions[Player_Position_Index].Position_Set = true;
        }

        /// <summary>
        /// Restores the coordinates of the currently selected character.
        /// </summary>
        private void Restore_Character()
        {
            if (Player_Positions[Player_Position_Index].Position_Set)
            {
                int Player_Pointer = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)SonicHeroesVariables.Characters_Addresses.CurrentPlayerControlledCharacter_Pointer, 4);

                // Restore coordinates
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.XPosition + Player_Pointer, BitConverter.GetBytes(Player_Positions[Player_Position_Index].Character_X));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.YPosition + Player_Pointer, BitConverter.GetBytes(Player_Positions[Player_Position_Index].Character_Y));
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Characters_Addresses_Offsets.ZPosition + Player_Pointer, BitConverter.GetBytes(Player_Positions[Player_Position_Index].Character_Z));
                Current_Player_Position.Character_X = Player_Positions[Player_Position_Index].Character_X;
                Current_Player_Position.Character_Y = Player_Positions[Player_Position_Index].Character_Y;
                Current_Player_Position.Character_Z = Player_Positions[Player_Position_Index].Character_Z;
            }
        }

        /// <summary>
        /// Increments the index of the current menu.
        /// </summary>
        public virtual void Increment_Current_Position()
        {
            if (Player_Position_Index == Player_Positions.Length - 1) { Player_Position_Index = 0; }
            else { Player_Position_Index = Player_Position_Index + 1; }
            Free_Movement_Mode_Menu.Set_Menu_Text(Player_Position_Index, Player_Positions[Player_Position_Index].Character_X, Player_Positions[Player_Position_Index].Character_Y, Player_Positions[Player_Position_Index].Character_Z);
        }

        /// <summary>
        /// Draws the overlay if necessary, i.e. if character is in a level.
        /// </summary>
        /// <param name="DirectX_Graphics_Window"></param>
        public void Draw_Overlay(WindowRenderTarget DirectX_Graphics_Window)
        {
            // Get Information of whether the overlay should be shown and controls enabled.
            byte Is_Currently_In_Level = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);

            // If the player is in a stage.
            if (Is_Currently_In_Level == 1)
            {
                // Render the Controls Menu
                Free_Movement_Mode_Menu.Render_This_Menu(DirectX_Graphics_Window);
            }
        }

    }
}
