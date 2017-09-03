﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SonicHeroes.Memory;
using System.Net;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using SonicHeroes.Networking;
using System.Linq;

namespace HeroesInjectionTest
{
    class Program
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        /// 
        /// Critical Modules
        /// 
        public static Process SonicHeroesEXE; // This process will hold a handle to the Sonic Heroes Executable.        
        public static List<IntPtr> ModuleAddressesEXE = new List<IntPtr>(); // Addresses of all injected modules.
        public static string SonicHeroes_Directory; // The directory of the mod loader and game.
        public static string SonicHeroes_Backup_Directory; // The directory of backup game files.
        public static List<string> SonicHeroes_Newly_Added_Files = new List<string>(); // New Files Added by the Mod Loader.

        ///
        /// Additional Addon Modules
        /// 
        public static SonicHeroes.Networking.WebSocket_Host Mod_Loader_Server = new SonicHeroes.Networking.WebSocket_Host(); // Hosting server for the mod loader.
        public static SonicHeroes.Controller.DirectInput_Joystick_Manager Joystick_Manager = new SonicHeroes.Controller.DirectInput_Joystick_Manager(); // New controller manager.
        public static List<byte[]> Controller_Inputs_Serialized; // Serialized controller inputs are stored here to send to clients!
        public static Thread Controller_Poll_Thread; // This thread constantly polls for controller state changes.
        static ManualResetEvent Console_Quit_Event = new ManualResetEvent(false); // Event to be triggered on console quit by Ctrl+C, other shortcut or window.

        /// <summary>
        /// List of hooks to be ran at a specified address.
        /// </summary>
        public static List<SonicHeroes.Networking.Client_Functions.Multi_Hook_Handler> Hook_Handler_List = new List<SonicHeroes.Networking.Client_Functions.Multi_Hook_Handler>();

        static void Main(string[] args)
        {
            ThemeConsole(); // Get those themes out! Even though the Windows Console themes crappily.

            ConsoleX_WriteLine_Center("-------------------------");
            ConsoleX_WriteLine_Center("Heroes Nemesis Centauri");
            ConsoleX_WriteLine_Center("-------------------------");
            ConsoleX_WriteLine_Center("α version\n");

            // Define a new host to which we will be sending information to.
            Mod_Loader_Server.SetupServer(IPAddress.Loopback);
            Mod_Loader_Server.ProcessBytesMethod += ProcessData;

            ConsoleX_WriteLine_Center("Server | Running!");
            ConsoleX_WriteLine_Center("Waiting For Game!\n");

            SonicHeroes_Directory = AppDomain.CurrentDomain.BaseDirectory; // Main directory for Sonic Heroes
            SonicHeroes_Backup_Directory = AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Backup\"; // Backup directory for Sonic Heroes
            Restore_Backup_Files(); // Restores any files if present in backup folder.

            // Retrieve the current enabled list of mods.
            string[] Library_Directories = File.ReadAllLines(SonicHeroes_Directory + "\\Mod-Loader-Config\\EnabledMods.txt");

            Console.WriteLine(GetCurrentTime() + "Modifications Found: " + Library_Directories.Length);

            // Load each mod one by one.
            for (int x = 0; x < Library_Directories.Length; x++)
            {
                // Copy Data/Files over as necessary.
                if (Directory.Exists(SonicHeroes_Directory + "\\Mod-Loader-Mods\\" + Library_Directories[x] + @"\root"))
                {
                    Console.WriteLine(GetCurrentTime() + "Loading Mod | File Replacement: " + Library_Directories[x]);

                    // Set up Mod Directory Properties
                    string Mod_Directory = SonicHeroes_Directory + "Mod-Loader-Mods\\" + Library_Directories[x] + @"\root\";

                    // Get all Sonic Heroes Mod Files
                    DirectoryInfo Heroes_Mod_Directory_Info = new DirectoryInfo(Mod_Directory);
                    FileInfo[] Heroes_Mod_All_Files_List = Heroes_Mod_Directory_Info.EnumerateFiles("*.*", System.IO.SearchOption.AllDirectories).ToArray();

                    for (int z = 0; z < Heroes_Mod_All_Files_List.Length; z++)
                    {
                        // Relative File Location
                        string Relative_File_Location = Heroes_Mod_All_Files_List[z].FullName.Substring(Mod_Directory.Length);

                        // Generate File Backup
                        Directory.CreateDirectory(SonicHeroes_Backup_Directory + Path.GetDirectoryName(Relative_File_Location));
                        if (!File.Exists(SonicHeroes_Backup_Directory + Relative_File_Location) && File.Exists(SonicHeroes_Directory + Relative_File_Location))
                        {
                            File.Copy(SonicHeroes_Directory + Relative_File_Location, SonicHeroes_Backup_Directory + Relative_File_Location, false);
                        }
                        else
                        {
                            SonicHeroes_Newly_Added_Files.Add(Relative_File_Location);
                        }

                        // Copy the file if it doesn't exist ^^

                        // Move New File
                        File.Copy(Mod_Directory + Relative_File_Location, SonicHeroes_Directory + Relative_File_Location, true);
                    }
                }
            }

            // Dump all newly added files.
            File.WriteAllLines(SonicHeroes_Backup_Directory + "Session-Files-Added.txt", SonicHeroes_Newly_Added_Files);

            // Start the game.
            SonicHeroesEXE = Process.Start(File.ReadAllText("Mod-Loader-Config/ExecutableName.txt"));
            SonicHeroesEXE.SuspendProcess(); // Suspend the game.
            SonicHeroesEXE.EnableRaisingEvents = true; // Allow events to be raised.
            SonicHeroesEXE.Exited += Shutdown; // Shutdown with the game.

            // Load each mod one by one.
            for (int x = 0; x < Library_Directories.Length; x++)
            {
                if (File.Exists(SonicHeroes_Directory + "Mod-Loader-Mods\\" + Library_Directories[x] + @"\main.dll"))
                {
                    Console.WriteLine(GetCurrentTime() + "Loading Mod | DLL Injection: " + Library_Directories[x]);
                    string DLL_Path = SonicHeroes_Directory + "Mod-Loader-Mods\\" + Library_Directories[x] + @"\main.dll";

                    // In order to get function address we must load library in our process and extract it as needed, GetLibraryAddress handles that.
                    SonicHeroesEXE.LoadLibrary(DLL_Path); // Load the library into the process!
                    IntPtr LibraryAddress = GetLibraryAddress(DLL_Path); // Get the library function address.

                    SonicHeroesEXE.ResumeProcess();
                    SonicHeroesEXE.CallLibrary_Async(LibraryAddress, IntPtr.Zero); // Say hello to the library!
                    SonicHeroesEXE.SuspendProcess();
                    ModuleAddressesEXE.Add(LibraryAddress); // Add new module address to list of addresses (we can unload it when the game/loader quits).

                    Console.WriteLine(GetCurrentTime() + "Successfully Injected: " + Library_Directories[x]);
                }
            }

            SonicHeroesEXE.ResumeProcess(); // Resume the game.
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Shutdown); // On process exit, run one last thing :)
            Console.CancelKeyPress += (sender, eArgs) =>  { Console_Quit_Event.Set();  eArgs.Cancel = true; };
            Console_Quit_Event.WaitOne(); // Wait until user presses quit keyboard combination.
            Console.ReadLine(); // Wait for user.
        }

        /// 
        /// Methods!
        /// 

        ///// If the game exits, we might aswell.
        static void Shutdown(object sender, EventArgs e)
        {
            // Restore the original files.
            Restore_Backup_Files();

            // Free memory from old libraries.
            for (int x = 0; x < ModuleAddressesEXE.Count; x++) { SonicHeroesEXE.FreeMemory(ModuleAddressesEXE[x]); }
            Environment.Exit(0);
        }

        /// <summary>
        /// Restores the files in the backup folder of the mod loader.
        /// </summary>
        public static void Restore_Backup_Files()
        {
            // :)
            Console.WriteLine(GetCurrentTime() + "Mod Loader | Restoring Original Files");

            // Restore Old Files
            // Get all Sonic Heroes Mod Files
            DirectoryInfo Heroes_Backup_Directory_Info = new DirectoryInfo(SonicHeroes_Backup_Directory);
            FileInfo[] Heroes_Backup_All_Files_List = Heroes_Backup_Directory_Info.EnumerateFiles("*.*", System.IO.SearchOption.AllDirectories).ToArray();

            for (int z = 0; z < Heroes_Backup_All_Files_List.Length; z++)
            {
                // Relative File Location
                string Relative_File_Location = Heroes_Backup_All_Files_List[z].FullName.Substring(SonicHeroes_Backup_Directory.Length);

                // Restore File Backup
                Directory.CreateDirectory(SonicHeroes_Directory + Path.GetDirectoryName(Relative_File_Location));
                if (File.Exists(SonicHeroes_Directory + Relative_File_Location))
                {
                    File.Delete(SonicHeroes_Directory + Relative_File_Location);
                    File.Move(SonicHeroes_Backup_Directory + Relative_File_Location, SonicHeroes_Directory + Relative_File_Location);
                }
            }

            // Remove any newly added files to restore game to original state.
            try
            {
                string[] SonicHeroes_Newly_Added_Files_Temp = File.ReadAllLines(SonicHeroes_Backup_Directory + "Session-Files-Added.txt");
                foreach (string FilePath in SonicHeroes_Newly_Added_Files_Temp) { File.Delete(SonicHeroes_Directory + FilePath); }
                File.Delete(SonicHeroes_Backup_Directory + "Session-Files-Added.txt");
            }
            catch { }

        }

        ///////////////////////
        /////////////////////// MOD LOADER SERVER SECTION
        ///////////////////////

        /// <summary>
        /// The information sent to the server will be processed here!
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="SocketX"></param>
        public static void ProcessData(byte[] Data, Socket SocketX)
        {
            // Get the message type.
            SonicHeroes.Networking.Client_Functions.Message_Type Message_Type = (SonicHeroes.Networking.Client_Functions.Message_Type)Data[0];

            switch (Message_Type)
            {
                case Client_Functions.Message_Type.Client_Call_Start_Controller_Server: Start_Controller_Listener_Server(); break;

                case Client_Functions.Message_Type.Client_Call_Send_Message:
                    Console.WriteLine(GetCurrentTime() + Encoding.ASCII.GetString(SonicHeroes.Misc.SonicHeroes_Miscallenous.Get_Byte_Range_From_Array(Data, Data.Length - 1, 1)));
                    SocketX.Send(new byte[1] { (byte)SonicHeroes.Networking.Client_Functions.Message_Type.Reply_Okay });
                    break; // Send okay! break;

                case Client_Functions.Message_Type.Client_Call_Get_Controller:
                    int Controller_ID = BitConverter.ToInt32(SonicHeroes.Misc.SonicHeroes_Miscallenous.Get_Byte_Range_From_Array(Data, Data.Length - 1, 1), 0);
                    Return_Controller_Status(SocketX, Controller_ID);
                    break;

                case Client_Functions.Message_Type.Client_Call_Subscribe_DLL_Function:
                    Subscribe_Hook_Handler(SonicHeroes.Misc.SonicHeroes_Miscallenous.Get_Byte_Range_From_Array(Data, Data.Length - 1, 1), SocketX, false);
                    break;

                case Client_Functions.Message_Type.Client_Call_Call_Subscribed_Function:
                    Call_Hook_Handler(SocketX, BitConverter.ToInt32(SonicHeroes.Misc.SonicHeroes_Miscallenous.Get_Byte_Range_From_Array(Data, Data.Length - 1, 1),0));
                    break;

                case Client_Functions.Message_Type.Client_Call_Check_Address_Hook_State:
                    Subscribe_Hook_Handler(SonicHeroes.Misc.SonicHeroes_Miscallenous.Get_Byte_Range_From_Array(Data, Data.Length - 1, 1), SocketX, true);
                    break;
            }
        }

        /// <summary>
        /// Subscribes a selected method onto the hook handling mechanism for managing multiple hooks!
        /// </summary>
        public static void Subscribe_Hook_Handler(byte[] Hook_Handler_Struct, Socket SocketX, bool Check_Only) /// Check Only: Doesn't register hook, just confirms whether one for an address exists or not.
        {
            bool Is_Already_Hooked = false;
            SonicHeroes.Networking.Client_Functions.Multi_Hook_Handler Hook_Handler = SonicHeroes.Networking.Client_Functions.Deserialize_Subscribe_Hook_Handler(Hook_Handler_Struct);

            // For each subscribed hook.
            for (int x = 0; x < Hook_Handler_List.Count; x++)
            {
                // Length of absolute jump is 5 bytes! If any function is within 5 bytes, thus also assigned to the subscribe call, we must execute it.
                if ((Hook_Handler.Hook_Address >= Hook_Handler_List[x].Hook_Address - 5) && (Hook_Handler.Hook_Address <= Hook_Handler_List[x].Hook_Address + 5))
                {
                    // If there already is a hook within the range, tell that we are already hooked!
                    Is_Already_Hooked = true;
                    break;
                }
            }
            if (Check_Only == false) { Hook_Handler_List.Add(Hook_Handler); Console.WriteLine(GetCurrentTime() + "Function Subscribed!"); }
            if (Is_Already_Hooked) { SocketX.Send(new byte[1] { (byte)Client_Functions.Message_Type.Reply_Function_Already_Hooked }); }
            else { SocketX.Send(new byte[1] { (byte)Client_Functions.Message_Type.Reply_Okay }); }
        }

        /// <summary>
        /// From the received hook address, call all methods which hook at the specified address.
        /// </summary>
        public static void Call_Hook_Handler(Socket SocketX, int Hook_Address_Sent)
        {
            // For each subscribed hook.    
            for (int x = 0; x < Hook_Handler_List.Count; x++)
            {
                // Length of absolute jump is 5 bytes! If any function is within 5 bytes, thus also assigned to the subscribe call, we must execute it.
                if ( (Hook_Address_Sent >= Hook_Handler_List[x].Hook_Address - 5) && (Hook_Address_Sent <= Hook_Handler_List[x].Hook_Address + 5) )
                {
                    SonicHeroesEXE.CallLibrary_Async((IntPtr)Hook_Handler_List[x].Method_Address, IntPtr.Zero); // Say hello to the library!
                }
            }
            return;
        }

        /// <summary>
        /// Returns controller status to the requesting client.
        /// </summary>
        /// <param name="SocketX"></param>
        public static void Return_Controller_Status(Socket SocketX, int Controller_ID)
        {
            byte[] Controller_Inputs_Serialized_II = Controller_Inputs_Serialized[Controller_ID];
            SocketX.Send(Controller_Inputs_Serialized_II);
        }

        /// <summary>
        /// Starts polling controller inputs on a server. The server must have a variable containing the joystick state and a valid joystick manager.
        /// </summary>
        /// <param name="Joystick_Manager"></param>
        /// <param name="Controller_Inputs"></param>
        public static void Start_Controller_Listener_Server()
        {
            // Pre-allocate memory for each controller.
            Controller_Inputs_Serialized = new List<byte[]>(Joystick_Manager.PlayerControllers.Count);
            // Populate the list, hacky.
            for (int x = 0; x < Joystick_Manager.PlayerControllers.Count; x++) { Controller_Inputs_Serialized.Add(new byte[0]);}
            
            if (Controller_Poll_Thread == null)
            {
                Controller_Poll_Thread = new Thread
                (
                    () =>
                    {
                        while (true)
                        {
                            for (int x = 0; x < Joystick_Manager.PlayerControllers.Count; x++)
                            {
                                Controller_Inputs_Serialized[x] = SonicHeroes.Networking.Client_Functions.Serialize_Controller_Inputs_Manual(Joystick_Manager.PlayerControllers[x].Get_Whole_Controller_State());
                            }
                            Thread.Sleep(16);
                        }
                    }
                );
                Controller_Poll_Thread.Start();
                Console.WriteLine(GetCurrentTime() + "Controller Poll Thread Started (Mod Loader)");
            }
        }

        ///////////////////////
        /////////////////////// END MOD LOADER SERVER SECTION
        ///////////////////////

        /// <summary>
        /// Returns the address of the DLL Exported method in the library.
        /// </summary>
        /// <param name="LibraryPath"></param>
        /// <returns></returns>
        public static IntPtr GetLibraryAddress(string LibraryPath)
        {
            IntPtr LibraryCopy = LoadLibrary(LibraryPath);
            return GetProcAddress(LibraryCopy, "Main");
        }

        /// <summary>
        /// Resolves current time to be appended to a message.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentTime() { return "[" + DateTime.Now.ToString("hh:mm:ss") + "] "; }

        /// <summary>
        /// Writes a centered line to the console.
        /// </summary>
        /// <param name=""></param>
        /// <param name="Message"></param>
        public static void ConsoleX_WriteLine_Center(string Message)
        {
            Console.SetCursorPosition((Console.WindowWidth - Message.Length) / 2, Console.CursorTop);
            Console.WriteLine(Message);
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        /// <summary>
        /// This method is responsible for theming the console to the slate gray theme!
        /// </summary>
        public static void ThemeConsole()
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
