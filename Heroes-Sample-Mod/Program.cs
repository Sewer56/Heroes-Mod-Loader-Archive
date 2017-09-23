using System;
using SonicHeroes.Functions;
using SonicHeroes.Controller;
using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Text;
using static SonicHeroes.Networking.Client_Functions;
using System.Drawing;
using SonicHeroes.Overlay;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.DirectWrite;
using Heroes_Sample_Mod;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace TestLibrary
{
    public class Program
    {
        /// Mod loader DLL Skeleton Code
        const string Mod_Name = "Mod Loader Controller Hook Sample"; // Set name of project.
        static SonicHeroes.Networking.WebSocket_Client Sonic_Heroes_Networking_Client = new SonicHeroes.Networking.WebSocket_Client(); // Set up client for networking, this client communicates with the server to call methods under subscribe hook
        static SonicHeroes.Networking.WebSocket_Client Sonic_Heroes_Networking_Client_II = new SonicHeroes.Networking.WebSocket_Client(); // Set up client for networking, this one is for non-subscribed calls.
        static Process Sonic_Heroes_Process; // Get Sonic Heroes Process
        /// Mod loader DLL Skeleton Code

        /// <summary>
        /// Your program starts here!
        /// </summary>
        [DllExport]
        static void Main()
        {
            try
            {
                ////////////// MOD LOADER DLL SKELETON CODE ///////////////////////////////////////////////////////////////////////////////////////////////////
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(SonicHeroes.Misc.SonicHeroes_Miscallenous.CurrentDomain_SetAssemblyResolve);
                Sonic_Heroes_Networking_Client.SetupClient(IPAddress.Loopback); /// Set up networking with the Sonic Heroes Mod Loader.
                Sonic_Heroes_Networking_Client_II.SetupClient(IPAddress.Loopback); /// Set up networking with the Sonic Heroes Mod Loader.
                byte[] Response = Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " | Loading... OK!"), true); /// Say to the Mod Loader that we have loaded so the end user can know.
                Sonic_Heroes_Process = Process.GetCurrentProcess(); /// We will use this for reading and writing memory.
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Load the Controller Deadzone Config
                Controller_Deadzones = new Controller_Config_Deadzones();
                Load_Controller_Deadzone_Configuration();

                // Define the controller manager.
                SonicHeroes_Joystick_Manager = new SonicHeroes.Controller.DirectInput_Joystick_Manager();


                // Declare new hook.
                Heroes_Controller_Function_Hook = new SonicHeroes.Hooking.Injection
                    (
                        (IntPtr)0x445B3A,
                        (Heroes_Controller_Function_Delegate)Heroes_Controller_Function_Override,
                        6
                    );

                // Activate our hook!
                Heroes_Controller_Function_Hook.Activate();
            }
            catch (Exception Ex) { Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " Failed To Load: " + Ex.Message), false); }
        }

        // Own Variables
        static SonicHeroes.Controller.DirectInput_Joystick_Manager SonicHeroes_Joystick_Manager;
        public delegate void Heroes_Controller_Function_Delegate(); // Define Delegate
        static SonicHeroes.Hooking.Injection Heroes_Controller_Function_Hook; // Hook for Controller Input Receiving
        static Controller_Config_Deadzones Controller_Deadzones; // Controller Deadzones

        // Own Structs/Classes
        public class Controller_Config_Deadzones
        {
            public float Deadzone_Left_Stick_X;
            public float Deadzone_Left_Stick_Y;
            public float Deadzone_Right_Stick_X;
            public float Deadzone_Right_Stick_Y;
            public float Deadzone_Left_Trigger;
            public float Deadzone_Right_Trigger;
        }


        /// <summary>
        /// Main Controller Check Status Loop
        /// </summary>
        static void Heroes_Controller_Function_Override()
        {
            try
            {
                // Get Controller Information
                Sonic_Heroes_Joystick.Controller_Inputs_Generic Heroes_Controller_Button_Info = SonicHeroes_Joystick_Manager.PlayerControllers[0].Get_Whole_Controller_State();

                // Write Empty Axis First
                Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Left_Trigger_Pressure, BitConverter.GetBytes(0x00));
                Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Left_Trigger_Pressure_II, BitConverter.GetBytes(0x00));
                Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Right_Trigger_Pressure, BitConverter.GetBytes(0x00));
                Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Right_Trigger_Pressure_II, BitConverter.GetBytes(0x00));

                // Get Axis Values
                float Stick_X = Heroes_Controller_Button_Info.LeftStick.X * 0.001F; // Normal Range = -1000 to 1000. Scale to -1 to 1
                float Stick_Y = Heroes_Controller_Button_Info.LeftStick.Y * 0.001F; // Normal Range = -1000 to 1000. Scale to -1 to 1
                float Right_Stick_X = Heroes_Controller_Button_Info.RightStick.X * 0.001F; // Normal Range = -1000 to 1000. Scale to -1 to 1
                float Right_Stick_Y = Heroes_Controller_Button_Info.RightStick.Y * 0.001F; // Normal Range = -1000 to 1000. Scale to -1 to 1

                float LT_Pressure = (Heroes_Controller_Button_Info.LeftTriggerPressure + 1000); // Normal Range = -1000 to 1000. Changed to 0 - 2000.
                float RT_Pressure = (Heroes_Controller_Button_Info.RightTriggerPressure + 1000); // Normal Range = -1000 to 1000. Changed to 0 - 2000.

                // Write all of the values to game memory.
                if (Stick_X > Controller_Deadzones.Deadzone_Left_Stick_X || Stick_X < (Controller_Deadzones.Deadzone_Left_Stick_X * -1.0F))
                { Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Left_Stick_X, BitConverter.GetBytes(Stick_X)); }
                else { Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Left_Stick_X, BitConverter.GetBytes(0.000F)); }

                if (Stick_Y > Controller_Deadzones.Deadzone_Left_Stick_Y || Stick_Y < -(Controller_Deadzones.Deadzone_Left_Stick_Y * -1.0F))
                { Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Left_Stick_Y, BitConverter.GetBytes(Stick_Y)); }
                else { Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Left_Stick_Y, BitConverter.GetBytes(0.000F)); }

                if (Right_Stick_X > Controller_Deadzones.Deadzone_Right_Stick_X || Stick_X < (Controller_Deadzones.Deadzone_Right_Stick_X * -1.0F))
                { Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Right_Stick_X, BitConverter.GetBytes(Right_Stick_X)); }
                else { Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Right_Stick_X, BitConverter.GetBytes(0.000F)); }

                if (Right_Stick_Y > Controller_Deadzones.Deadzone_Right_Stick_Y || Right_Stick_Y < -(Controller_Deadzones.Deadzone_Right_Stick_Y * -1.0F))
                { Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Right_Stick_Y, BitConverter.GetBytes(Right_Stick_Y)); }
                else { Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Right_Stick_Y, BitConverter.GetBytes(0.000F)); }

                SonicHeroes.Functions.SonicHeroes_Functions.Buttons_Flags_II Buttons_Flags_II = (SonicHeroes.Functions.SonicHeroes_Functions.Buttons_Flags_II)Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Buttons_Flags_II, 1);

                if (LT_Pressure > Controller_Deadzones.Deadzone_Left_Trigger)
                {
                    Buttons_Flags_II = Buttons_Flags_II | SonicHeroes_Functions.Buttons_Flags_II.Left_Trigger;
                }

                if (RT_Pressure > Controller_Deadzones.Deadzone_Right_Trigger)
                {
                    Buttons_Flags_II = Buttons_Flags_II | SonicHeroes_Functions.Buttons_Flags_II.Right_Trigger;
                }

                Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Buttons_Flags_II, new byte[1] { (byte)Buttons_Flags_II });
                Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Buttons_Flags_II + 0x04, new byte[1] { (byte)(0xFF - (byte)Buttons_Flags_II) });

            }
            catch
            {
                // Support Hotplugging Controller 1!
                byte[] Response = Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes(Mod_Name + " | Controller Disconnected, Polling for Controller!"), true);
                Thread Controller_Poll_Thread = new Thread
                (
                    () =>
                    {
                        do
                        {
                            SonicHeroes_Joystick_Manager = new SonicHeroes.Controller.DirectInput_Joystick_Manager();
                            Thread.Sleep(1500);
                        }
                        while (SonicHeroes_Joystick_Manager.PlayerControllers.Count < 1);
                    }
                );
                Controller_Poll_Thread.Start();
            }
            //Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Left_Trigger_Pressure, BitConverter.GetBytes(LT_Pressure_Byte));
            //Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Left_Trigger_Pressure_II, BitConverter.GetBytes(LT_Pressure_Byte));
            //Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Right_Trigger_Pressure, BitConverter.GetBytes(RT_Pressure_Byte));
            //Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Right_Trigger_Pressure_II, BitConverter.GetBytes(RT_Pressure_Byte));

            /*
            bool Button_Pressed = false;

            // Button Flags I
            SonicHeroes.Functions.SonicHeroes_Functions.Buttons_Flags_I Buttons_Flags_I = 0x00;

            // Obtain Button Flags I!
            if (Heroes_Controller_Button_Info.ControllerButtons.Button_A == true) { Buttons_Flags_I = Buttons_Flags_I | SonicHeroes_Functions.Buttons_Flags_I.Jump; Button_Pressed = true; }
            if (Heroes_Controller_Button_Info.ControllerButtons.Button_B == true) { Buttons_Flags_I = Buttons_Flags_I | SonicHeroes_Functions.Buttons_Flags_I.Action; Button_Pressed = true; }
            if (Heroes_Controller_Button_Info.ControllerButtons.Button_X == true) { Buttons_Flags_I = Buttons_Flags_I | SonicHeroes_Functions.Buttons_Flags_I.Formation_R; Button_Pressed = true; }
            if (Heroes_Controller_Button_Info.ControllerButtons.Button_Y == true) { Buttons_Flags_I = Buttons_Flags_I | SonicHeroes_Functions.Buttons_Flags_I.Formation_L; Button_Pressed = true; }
            if (Heroes_Controller_Button_Info.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.UP) { Buttons_Flags_I = Buttons_Flags_I | SonicHeroes_Functions.Buttons_Flags_I.DPAD_UP; Button_Pressed = true; }
            if (Heroes_Controller_Button_Info.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.DOWN) { Buttons_Flags_I = Buttons_Flags_I | SonicHeroes_Functions.Buttons_Flags_I.DPAD_DOWN; Button_Pressed = true; }
            if (Heroes_Controller_Button_Info.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.LEFT) { Buttons_Flags_I = Buttons_Flags_I | SonicHeroes_Functions.Buttons_Flags_I.DPAD_LEFT; Button_Pressed = true; }
            if (Heroes_Controller_Button_Info.ControllerDPad == (int)Sonic_Heroes_Joystick.DPAD_Direction.RIGHT) { Buttons_Flags_I = Buttons_Flags_I | SonicHeroes_Functions.Buttons_Flags_I.DPAD_RIGHT; Button_Pressed = true; }

            // Button Flags II
            SonicHeroes.Functions.SonicHeroes_Functions.Buttons_Flags_II Buttons_Flags_II = 0x00;

            // Obtain Button Flags II!
            if (Heroes_Controller_Button_Info.ControllerButtons.Button_L1 == true) { Buttons_Flags_II = Buttons_Flags_II | SonicHeroes_Functions.Buttons_Flags_II.Left_Trigger; Button_Pressed = true; }
            if (Heroes_Controller_Button_Info.ControllerButtons.Button_R1 == true) { Buttons_Flags_II = Buttons_Flags_II | SonicHeroes_Functions.Buttons_Flags_II.Right_Trigger; Button_Pressed = true; }
            if (Heroes_Controller_Button_Info.ControllerButtons.Button_Start == true) { Buttons_Flags_II = Buttons_Flags_II | SonicHeroes_Functions.Buttons_Flags_II.Start_Button; Button_Pressed = true; }
            if (LT_Pressure_Byte > 5)
            {
                Buttons_Flags_II = Buttons_Flags_II | SonicHeroes_Functions.Buttons_Flags_II.Left_Trigger;
                Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Left_Trigger_Pressure, BitConverter.GetBytes(LT_Pressure_Byte));
                Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Left_Trigger_Pressure_II, BitConverter.GetBytes(LT_Pressure_Byte));
                Button_Pressed = true;
            }

            if (RT_Pressure_Byte > 5)
            {
                Buttons_Flags_II = Buttons_Flags_II | SonicHeroes_Functions.Buttons_Flags_II.Right_Trigger;
                Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Right_Trigger_Pressure, BitConverter.GetBytes(RT_Pressure_Byte));
                Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Right_Trigger_Pressure_II, BitConverter.GetBytes(RT_Pressure_Byte));
                Button_Pressed = true;
            }

            // Button Flags III
            SonicHeroes.Functions.SonicHeroes_Functions.Buttons_Flags_III Buttons_Flags_III = 0x00;

            // Obtain Button Flags III!
            if (Heroes_Controller_Button_Info.ControllerButtons.Button_Back == true) { Buttons_Flags_III = Buttons_Flags_III | SonicHeroes_Functions.Buttons_Flags_III.Select; Button_Pressed = true; }

            Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Buttons_Flags_I, new byte[1] { (byte)Buttons_Flags_I });
            Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Buttons_Flags_II, new byte[1] { (byte)Buttons_Flags_II });
            Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Buttons_Flags_III, new byte[1] { (byte)Buttons_Flags_III });
            Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Buttons_Flags_I + 0x04, new byte[1] { (byte)(0xFF - (byte)Buttons_Flags_I) });
            Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Buttons_Flags_II + 0x04, new byte[1] { (byte)(0xFF - (byte)Buttons_Flags_II) });
            Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Buttons_Flags_III + 0x04, new byte[1] { (byte)(0xFF - (byte)Buttons_Flags_III) });

            */
            /*
            // Button Flags IV (Unused)
            SonicHeroes.Functions.SonicHeroes_Functions.Buttons_Flags_III Buttons_Flags_IV = 0x00;

            // Button Flags IV (Unused)            
            Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroes_Functions.Controller_Polling_Functions.Variable_Controller_Buttons_Flags_IV + 0x04, new byte[1] { (byte)(0xFF) });
            */
            return;
        }


        /// <summary>
        /// Loads the controller deadzone configuration.
        /// </summary>
        static void Load_Controller_Deadzone_Configuration()
        {
            try
            {
                string Local_Save_Path = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location) + "\\" + "Config.ini";
                string[] Save_File = File.ReadAllLines(Local_Save_Path);

                // I never use foreach but I'll give in once, it looks cleaner.
                foreach (string Save_File_String in Save_File)
                {
                    string Value = Save_File_String.Substring(Save_File_String.IndexOf("=") + 1);
                    // Ignore comments
                    if (Save_File_String.StartsWith("#")) { continue; }
                    else if (Save_File_String.StartsWith("Deadzone_Left_Stick_X=")) { Controller_Deadzones.Deadzone_Left_Stick_X = (1.000F / 100)*Byte.Parse(Value); }
                    else if (Save_File_String.StartsWith("Deadzone_Left_Stick_Y=")) { Controller_Deadzones.Deadzone_Left_Stick_Y= (1.000F / 100) * Byte.Parse(Value); }
                    else if (Save_File_String.StartsWith("Deadzone_Right_Stick_X=")) { Controller_Deadzones.Deadzone_Right_Stick_X = (1.000F / 100) * Byte.Parse(Value); }
                    else if (Save_File_String.StartsWith("Deadzone_Right_Stick_Y=")) { Controller_Deadzones.Deadzone_Right_Stick_Y = (1.000F / 100) * Byte.Parse(Value); }
                    else if (Save_File_String.StartsWith("Deadzone_Left_Trigger=")) { Controller_Deadzones.Deadzone_Left_Trigger = (2000F / 100) * Byte.Parse(Value); }
                    else if (Save_File_String.StartsWith("Deadzone_Right_Trigger=")) { Controller_Deadzones.Deadzone_Right_Trigger = (2000F / 100) * Byte.Parse(Value); }
                }
            }
            catch (Exception Ex) { MessageBox.Show(Ex.Message); }
        }
    }
}