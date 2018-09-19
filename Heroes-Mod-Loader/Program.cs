using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SonicHeroes.Memory;
using SonicHeroes.Controller;
using System.Net;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using SonicHeroes.Networking;
using System.Linq;
using System.Windows.Forms;
using static SonicHeroes.Controller.Sonic_Heroes_Joystick;
using Binarysharp.Assemblers.Fasm;

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
        public static string Executable_Path; // The path of the game executable.
        public static string Root_Directory; // The directory of the game root.
        public static string Mod_Loader_Directory; // The directory of the mod loader.
        public static string Mod_Loader_Backup_Directory; // The directory of backup game files.
        public static List<string> SonicHeroes_Newly_Added_Files = new List<string>(); // New Files Added by the Mod Loader.
        public static string[] Library_Directories; // Stores an array of directories of currently loaded mods.
        public static FileUnblocker fileUnblocker; // Unlocks the files!

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
            ConsoleX_WriteLine_Center("β version\n");
            ConsoleX_WriteLine_Center("SHC2017 Edition");
            ConsoleX_WriteLine_Center("\n");

            ConsoleX_WriteLine_Center("Having Issues? Try one of the following...");
            ConsoleX_WriteLine_Center("Running the Mod Loader as Administrator");

            ConsoleX_WriteLine_Center("\n");
            ConsoleX_WriteLine_Center("Operation not Supported?");
            ConsoleX_WriteLine_Center("Check if DLLs are allowed to Run | Mod-Loader-Libraries/<DLL-File> => Right Click, Properties, Unblock");
            ConsoleX_WriteLine_Center("Consider any of Mod DLLs too | Mod-Loader-Mods/<Mod>/<DLL-File> => Right Click, Properties, Unblock");
            ConsoleX_WriteLine_Center("There isn't anything I can do about this Windows quirk at this moment in time :/");
            ConsoleX_WriteLine_Center("\n");
            ConsoleX_WriteLine_Center("Please contact me if you are stuck: Public Discord link in Configurator.");
            ConsoleX_WriteLine_Center("\n");

            ConsoleX_WriteLine_Center("Current Controller Order:");
            for (int x = 0; x < Joystick_Manager.PlayerControllers.Count; x++)
            {
                ConsoleX_WriteLine_Center("[" + x + "] "+ Joystick_Manager.PlayerControllers[x].Information.ProductName + "-" + Joystick_Manager.PlayerControllers[x].Information.ProductGuid);
            }
            ConsoleX_WriteLine_Center("\n");
            ConsoleX_WriteLine_Center("Unless overwritten on a per-mod basis your mods will see the order as this.");
            ConsoleX_WriteLine_Center("If this is undesiable, consider changing Controller_ID field for each");
            ConsoleX_WriteLine_Center("device in Mod-Loader-Config/");
            ConsoleX_WriteLine_Center("Lower number = Higher priority");
            ConsoleX_WriteLine_Center("\n");

            // Show Message
            Thread.Sleep(1000);

            Setup_Server(); // Starts up the Mod Loader Server.
            Setup_Directories(); // Sets up the directories to be used by the mod loader.
            Unblock_DLLs(); // Unblock all DLLs.
            Restore_Backup_Files(); // Restores any files if present in backup folder.

            // Test
            if (!File.Exists(Mod_Loader_Directory + "\\Mod-Loader-Config\\Setup_Complete"))
            {
                ConsoleX_WriteLine_Center("Please run the Mod Loader Configurator First.");
                ConsoleX_WriteLine_Center("Think that's wrong? Remove Mod-Loader-Config/Setup_Complete");
                ConsoleX_WriteLine_Center("Press Enter to Exit.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            // Retrieve the current enabled list of mods.
            Library_Directories = File.ReadAllLines(Mod_Loader_Directory + "\\Mod-Loader-Config\\Enabled_Mods.txt");
            Console.WriteLine(GetCurrentTime() + "Modifications Found: " + Library_Directories.Length);

            Console.WriteLine(GetCurrentTime() + "Committing File Replacements! Kapow! | " + Root_Directory);
            Commit_File_Replacements();
            Executable_Path = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Executable_Path.txt"); // Main directory for Sonic Heroes

            // Leave a text file with the path of the mod loader libraries with the executable.
            File.WriteAllText(Path.GetDirectoryName(Executable_Path) + "\\Mod_Loader_Libraries.txt", Path.GetDirectoryName(Mod_Loader_Directory));
            File.WriteAllText(Path.GetDirectoryName(Executable_Path) + "\\Mod_Loader_Config.txt", Path.GetDirectoryName(Mod_Loader_Directory));

            // Start the game.
            Console.WriteLine(GetCurrentTime() + "Target Locked! | " + Executable_Path);
            ProcessStartInfo Heroes_Start_Info = new ProcessStartInfo(); // Sonic Heroes will crash if not launched from executable directory. BAD PROGRAMMING SANIC TEEM, BAD PROGRAMMING.
            Heroes_Start_Info.WorkingDirectory = Path.GetDirectoryName(Executable_Path); // Set Working Directory for TSonic_Win.exe
            Heroes_Start_Info.FileName = Executable_Path;
            SonicHeroesEXE = Process.Start(Heroes_Start_Info);
            SonicHeroesEXE.EnableRaisingEvents = true; // Allow events to be raised.
            SonicHeroesEXE.Exited += Shutdown; // Shutdown with the game.

            // Time for DLL injection, let's do it one by one ;).
            Commit_DLL_Injections(); 

            Console.WriteLine(GetCurrentTime() + "All Systems Operational! | Loading Success!");
            Console.WriteLine(GetCurrentTime() + "It is not recommended that you close this window.");
            Console.WriteLine(GetCurrentTime() + "Should you encounter any issue it is recommended that you read the log above.");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Shutdown); // On process exit, run one last thing :)
            Console.CancelKeyPress += (sender, eArgs) =>  { Console_Quit_Event.Set();  eArgs.Cancel = true; };
            Console_Quit_Event.WaitOne(); // Wait until user presses quit keyboard combination.
            Console.ReadLine(); // Wait for user.
        }


        ///////////////////////
        /////////////////////// MOD LOADER CORE SECTION
        ///////////////////////

        static void Unblock_DLLs()
        {
            fileUnblocker = new FileUnblocker();
            string[] DLLFiles = Directory.GetFiles(Mod_Loader_Directory, "*.dll", SearchOption.AllDirectories);
            for (int x = 0; x < DLLFiles.Length; x++)
            {
                ConsoleX_WriteLine_Center("Auto Unlock DLL | " + DLLFiles[x].Substring(Mod_Loader_Directory.Length));
                fileUnblocker.Unblock(DLLFiles[x]);
            }
        }
        

        /// Handle the mod loader individual DLL injections.
        static void Commit_DLL_Injections()
        {
            // Load each mod one by one.
            for (int x = 0; x < Library_Directories.Length; x++)
            {
                if (File.Exists(Mod_Loader_Directory + "Mod-Loader-Mods\\" + Library_Directories[x] + @"\main.dll"))
                {
                    Console.WriteLine(GetCurrentTime() + "Loading Mod | DLL Injection: " + Library_Directories[x]);
                    string DLL_Path = Mod_Loader_Directory + "Mod-Loader-Mods\\" + Library_Directories[x] + @"\main.dll";

                    // In order to get function address we must load library in our process and extract it as needed, GetLibraryAddress handles that.
                    SonicHeroesEXE.LoadLibrary(DLL_Path); // Load the library into the process!
                    IntPtr LibraryAddress = GetLibraryAddress(DLL_Path); // Get the library function address.
                    SonicHeroesEXE.CallLibrary_Async(LibraryAddress, IntPtr.Zero); // Say hello to the library!

                    ModuleAddressesEXE.Add(LibraryAddress); // Add new module address to list of addresses (we can unload it when the game/loader quits).

                    Console.WriteLine(GetCurrentTime() + "Successfully Injected: " + Library_Directories[x] + " | Sleeping 100ms");
                    Thread.Sleep(100);
                }
            }
        }

        /// Handle the mod loader file replacements
        static void Commit_File_Replacements()
        {
            // Load each mod one by one.
            for (int x = 0; x < Library_Directories.Length; x++)
            {
                // Copy Data/Files over as necessary.
                if (Directory.Exists(Mod_Loader_Directory + "\\Mod-Loader-Mods\\" + Library_Directories[x] + @"\root"))
                {
                    Console.WriteLine(GetCurrentTime() + "Loading Mod | File Replacement: " + Library_Directories[x]);

                    // Set up Mod Directory Properties
                    string Mod_Directory = Mod_Loader_Directory + "Mod-Loader-Mods\\" + Library_Directories[x] + @"\root\";

                    // Get all Sonic Heroes Mod Files
                    DirectoryInfo Heroes_Mod_Directory_Info = new DirectoryInfo(Mod_Directory);
                    FileInfo[] Heroes_Mod_All_Files_List = Heroes_Mod_Directory_Info.EnumerateFiles("*.*", System.IO.SearchOption.AllDirectories).ToArray();

                    for (int z = 0; z < Heroes_Mod_All_Files_List.Length; z++)
                    {
                        // Relative File Location
                        string Relative_File_Location = Heroes_Mod_All_Files_List[z].FullName.Substring(Mod_Directory.Length);

                        // Generate File Backup
                        Directory.CreateDirectory(Mod_Loader_Backup_Directory + Path.GetDirectoryName(Relative_File_Location));
                        if (!File.Exists(Mod_Loader_Backup_Directory + Relative_File_Location) && File.Exists(Root_Directory + Relative_File_Location))
                        {
                            // If it is a file replacement, commit a backup of the file.
                            File.Copy(Root_Directory + Relative_File_Location, Mod_Loader_Backup_Directory + Relative_File_Location, false);
                        }
                        else
                        {
                            SonicHeroes_Newly_Added_Files.Add(Relative_File_Location); // Otherwise add the file to the overall list of added files (pending removal on next startup/game shutdown).
                        }

                        // Copy the file if it doesn't exist ^^

                        // Copy the new file from the mod to replace the original.
                        File.Copy(Mod_Directory + Relative_File_Location, Root_Directory + Relative_File_Location, true);
                    }
                }
            }

            // Dump all newly added files.
            File.WriteAllLines(Mod_Loader_Backup_Directory + "Session-Files-Added.txt", SonicHeroes_Newly_Added_Files);
        }

        /// Reads the directories to be used by the mod loader from the text files.
        static void Setup_Directories()
        {
            Root_Directory = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Root_Directory.txt") + "\\"; // Main directory for Sonic Heroes
            Mod_Loader_Directory = AppDomain.CurrentDomain.BaseDirectory; // The directory containing the mod loader itself.
            Mod_Loader_Backup_Directory = AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Backup\"; // Backup directory for Sonic Heroes
            Executable_Path = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Executable_Path.txt"); // Main directory for Sonic Heroes
            if (Executable_Path == "null") { MessageBox.Show("The directories containing the game and the game data are not set, please run the configuration tool."); Environment.Exit(0); }
            if (Root_Directory == "null") { MessageBox.Show("The directories containing the game and the game data are not set, please run the configuration tool."); Environment.Exit(0); }

            // Remove Handles
            string potentialPath = "Controller_Acquire.txt";
            string potentialPath2 = Path.GetDirectoryName(Executable_Path) + "\\Controller_Acquire.txt";
            if (File.Exists(potentialPath))
            {
                File.Delete("Controller_Acquire.txt");
            }
            if (File.Exists(potentialPath2))
            {
                File.Delete(potentialPath2);
            }
        }

        // Starts the local mod loader server.
        static void Setup_Server()
        {
            // Define a new host to which we will be sending information to.
            ConsoleX_WriteLine_Center("Server | Starting on Port 13370!");
            Mod_Loader_Server.SetupServer(IPAddress.Loopback);
            Mod_Loader_Server.ProcessBytesMethod += ProcessData;
            ConsoleX_WriteLine_Center("Server | Running!");
        }

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
            File.Delete(Path.GetDirectoryName(Executable_Path) + "\\Mod_Loader_Libraries.txt");
            File.Delete(Path.GetDirectoryName(Executable_Path) + "\\Mod_Loader_Config.txt");
            
            // Restore Old Files
            // Get all Sonic Heroes Mod Files
            DirectoryInfo Heroes_Backup_Directory_Info = new DirectoryInfo(Mod_Loader_Backup_Directory);
            FileInfo[] Heroes_Backup_All_Files_List = Heroes_Backup_Directory_Info.EnumerateFiles("*.*", System.IO.SearchOption.AllDirectories).ToArray();

            for (int z = 0; z < Heroes_Backup_All_Files_List.Length; z++)
            {
                // Relative File Location
                string Relative_File_Location = Heroes_Backup_All_Files_List[z].FullName.Substring(Mod_Loader_Backup_Directory.Length);

                // Restore File Backup
                Directory.CreateDirectory(Root_Directory + Path.GetDirectoryName(Relative_File_Location));
                if (File.Exists(Root_Directory + Relative_File_Location))
                {
                    File.Delete(Root_Directory + Relative_File_Location);
                    File.Move(Mod_Loader_Backup_Directory + Relative_File_Location, Root_Directory + Relative_File_Location);
                }
            }

            // Remove any newly added files to restore game to original state.
            try
            {
                string[] SonicHeroes_Newly_Added_Files_Temp = File.ReadAllLines(Mod_Loader_Backup_Directory + "Session-Files-Added.txt");
                foreach (string FilePath in SonicHeroes_Newly_Added_Files_Temp) { File.Delete(Root_Directory + FilePath); }
                File.Delete(Mod_Loader_Backup_Directory + "Session-Files-Added.txt");
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
                case Client_Functions.Message_Type.Client_Call_Assemble_x86_Mnemonics:
                    Compile_X86_Mnemonics(SonicHeroes.Misc.SonicHeroes_Miscallenous.Get_Byte_Range_From_Array(Data, Data.Length - 1, 1), SocketX);
                    break;
            }
        }

        /// <summary>
        /// Makes use of FASM.NET in order to compile the supplied mnemonics.
        /// </summary>
        public static void Compile_X86_Mnemonics(byte[] Mnemonics, Socket SocketX)
        {
            // Deserialize X86 Mnemonics
            string[] Mnemonics_X = Client_Functions.Deserialize_x86_ASM_Mnemonics(Mnemonics);
            try
            {
                SocketX.Send(FasmNet.Assemble(Mnemonics_X)); // Try sending assembled data.
            }
            catch
            {
                SocketX.Send(new byte[1] { (byte)0x90 }); // If assembly was unsuccessful, reply with NOP 
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
            if (Check_Only == false)
            {
                Hook_Handler_List.Add(Hook_Handler); Console.WriteLine(GetCurrentTime() + "Function Subscribed!");
            }

            if (Is_Already_Hooked)
            {
                SocketX.Send(new byte[1] { (byte)Client_Functions.Message_Type.Reply_Function_Already_Hooked });
            }
            else
            {
                SocketX.Send(new byte[1] { (byte)Client_Functions.Message_Type.Reply_Okay });
            }
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
            try
            {
                byte[] Controller_Inputs_Serialized_II = Controller_Inputs_Serialized[Controller_ID];
                SocketX.Send(Controller_Inputs_Serialized_II);
            }
            catch
            {
                SocketX.Send(new byte[] { 0x00 }); // Reply with nothing
            }
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
            // Get left side
            int consolePointer = (Console.WindowWidth - Message.Length) / 2;
            if (consolePointer < 0) { consolePointer = 0; }

            Console.SetCursorPosition(consolePointer, Console.CursorTop);
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
