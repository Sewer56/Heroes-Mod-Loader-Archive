using SonicHeroes.Memory;
using SonicHeroes.Misc;
using SonicHeroes.Variables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static SonicHeroes.Networking.Client_Functions;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Plays sound samples from Sonic Heroes' .PAC files.
    /// </summary>
    public class Feature_PAC_Utilities
    {
        /// <summary>
        /// Current PAC File Index.
        /// </summary>
        private byte PAC_File_Index = 1;

        /// <summary>
        /// Represents the selected clip within the current PAC file.
        /// </summary>
        private int PAC_File_Sound_Index = 0;

        /// <summary>
        /// Current index of the PAC file which will be used for the playback of Character Sounds.
        /// </summary>
        private int Bank3_PAC_Index = 1;

        /// <summary>
        /// Stores a list of all of the individual PAC files present in the /sound directory.
        /// </summary>
        List<List_Of_PAC_Files> PAC_List = new List<List_Of_PAC_Files>();

        /// <summary>
        /// Memory location at which an ASM branch/conditional jump is stored, normally preventing bank3 or any other bank from reloading once it is initially loaded. We remove this temporarily.
        /// </summary>
        const int BRANCH_CHECK_DO_NOT_RELOAD_FILES = 0x0062ADD6;

        /// <summary>
        /// Memory address at which the Bank3 file name is stored at in memory, the function will reload Bank3 with the file name set here.
        /// </summary>
        const int FILE_BANK3_LOCATION_MEMORY = 0x007873BC;

        /// <summary>
        /// The memory location whereby the individual game state is stored within.
        /// </summary>
        const int GAME_STATE_LOCATION_MEMORY = 0x8D66F0;

        /// <summary>
        /// The ID of the game state which currently defines that the characters are within a level/stage.
        /// </summary>
        const int GAME_STATE_CURRENTLY_WITHIN_LEVEL = 0x05;

        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Struct that defines a list of PAC files' sound effects to be played.
        /// </summary>
        struct List_Of_PAC_Files
        {
            /// <summary>
            /// The internal ID of the PAC file to be played.
            /// </summary>
            public byte BANK_ID;

            /// <summary>
            /// The file name of the PAC file we are playing sounds from.
            /// </summary>
            public string BANK_NAME;

            /// <summary>
            /// A list of bytes representing all of the sounds available for playback.
            /// </summary>
            public List<byte> SOUND_NUMBER;
        }

        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes this class for usage. Retrieves all PAC files for use within class.
        /// </summary>
        public Feature_PAC_Utilities() { Get_All_PAC(); }

        /// <summary>
        /// Sets a trigger to safely reload Bank3 during the occurrence of the next rendered frame.
        /// </summary>
        public void Reload_Bank3() { Program.Sonic_Heroes_Overlay.direct2DOnframeDelegate += Reload_Bank3_II; }

        /// <summary>
        /// Retrieves the ID of the currently selected PAC file, specified in hexadecimal notation.
        /// </summary>
        public string Get_Current_PAC_ID() { return String.Format("0x{0:X2}", PAC_List[PAC_File_Index].BANK_ID); }

        /// <summary>
        /// Retrieves the name of the currently selected PAC file.
        /// </summary>
        public string Get_Current_PAC_Name() { return PAC_List[PAC_File_Index].BANK_NAME; }

        /// <summary>
        /// Retrieves the internal ID of the sound inside the currently selected PAC file.
        /// </summary>
        public string Get_Current_PAC_Sound_ID() { return String.Format("0x{0:X2}", PAC_List[PAC_File_Index].SOUND_NUMBER[PAC_File_Sound_Index]); }

        /// <summary>
        /// Retrieves the name of the Bank3 file currently selected to be set for character voices.
        /// </summary>
        public string Get_Current_Bank3_PAC_Name() { return PAC_List[Bank3_PAC_Index].BANK_NAME; }

        /// <summary>
        /// Increments the currently selected BANK file which is to be played.
        /// </summary>
        public void Increment_Current_Bank()
        {
            if  (PAC_File_Index < PAC_List.Count - 1)
                 PAC_File_Index = Convert.ToByte(PAC_File_Index + 1);
            else PAC_File_Index = 0;

            PAC_File_Sound_Index = 0;
        }

        /// <summary>
        /// Decrements the currently selected BANK file/ID to play.
        /// </summary>
        public void Decrement_Current_Bank()
        {
            if (PAC_File_Index > 0)
                PAC_File_Index = Convert.ToByte(PAC_File_Index - 1);
            else PAC_File_Index = (byte)(PAC_List.Count - 1);

            PAC_File_Sound_Index = 0;
        }

        /// <summary>
        /// Increments the currently selected Bank3 to be played.
        /// </summary>
        public void Increment_Current_Bank3_Bank()
        {
            if  (Bank3_PAC_Index < PAC_List.Count - 1)
                 Bank3_PAC_Index = Convert.ToByte(Bank3_PAC_Index + 1);
            else Bank3_PAC_Index = 0;
        }

        /// <summary>
        /// Decrements the currently selected Bank3 to be played.
        /// </summary>
        public void Decrement_Current_Bank3_Bank()
        {
            if  (Bank3_PAC_Index > 0)
                 Bank3_PAC_Index = Convert.ToByte(Bank3_PAC_Index - 1);
            else Bank3_PAC_Index = (byte)(PAC_List.Count - 1);
        }

        /// <summary>
        /// Increments the currently selected BANK file/ID to play.
        /// </summary>
        public void Increment_Current_Bank_Sound()
        {
            if  (PAC_File_Sound_Index < PAC_List[PAC_File_Index].SOUND_NUMBER.Count - 1)
                 PAC_File_Sound_Index = PAC_File_Sound_Index + 1;
            else PAC_File_Sound_Index = 0;
        }

        /// <summary>
        /// Decrements the currently selected BANK file/ID to play.
        /// </summary>
        public void Decrement_Current_Bank_Sound()
        {
            if  (PAC_File_Sound_Index > 0)
                 PAC_File_Sound_Index = PAC_File_Sound_Index - 1;
            else PAC_File_Sound_Index = PAC_List[PAC_File_Index].SOUND_NUMBER.Count - 1;
        }

        /// <summary>
        /// Allows you to play any requested sound bank.
        /// </summary>
        public void Play_Sound_Bank(int BankID, int BankNumber) { Play_Sound_Bank_Internal(BankID, BankNumber); }

        /// <summary>
        /// Plays the currently selected in-menu selected sound bank.
        /// </summary>
        public void Play_Sound_Bank() { Play_Sound_Bank_Internal(PAC_List[PAC_File_Index].BANK_ID, PAC_List[PAC_File_Index].SOUND_NUMBER[PAC_File_Sound_Index]);  }


        /// <summary>
        /// Plays the passed in set combination of "Sound Bank" and "Sound ID"
        /// </summary>
        private void Play_Sound_Bank_Internal(int BankID, int BankNumber)
        {
            // Standard protection from any game function calling shenanigans.
            // The way the game reads sound effects from the passed in register is... weird to say the least.
            try
            {
                // Convert the Sound Bank ID into Hexadecimal.
                string Sound_Bank_Hex = String.Format("{0:X2}", BankID);

                // Convert the Sound ID in the bank to Hexadecimal.
                string Sound_Bank_ID_Hex = String.Format("{0:X2}", BankNumber);

                // Bytes to be pushed onto the register as a method operand.
                string Bytes_To_Push;

                // If we are playing from Bank0, set the Bank ID to E0 (This is weird, lol), else reverse sound bank ID.
                if (Sound_Bank_Hex == "00") { Bytes_To_Push = "E0" + Sound_Bank_ID_Hex; }
                else { Bytes_To_Push = Sound_Bank_Hex.Reverse_String() + Sound_Bank_ID_Hex; }

                // If we are not in a level and want to use Bank5, do nothing, it is not loaded.
                if (Bytes_To_Push.StartsWith("50") && Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1) == 0) { }
                else
                {
                    // Convert Hex String to Raw Hex.
                    int Sound_Parameter = Int32.Parse(Bytes_To_Push, System.Globalization.NumberStyles.HexNumber);

                    // Read unknown pointer.
                    int Sound_Pointer = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)0x00A2F8B0, 4);

                    // Play the Sound!
                    Invoke_External_Class.Play_Sound(Sound_Parameter, Sound_Pointer, 1, 1);
                }
            }
            catch { }
        }

        /// <summary>
        /// Loads the currently set bank file at Tsonic_win.EXE+3873BC to act as character voices.
        /// </summary>
        private void Reload_Bank3_II()
        {
            // In the case the game function returns an unexpected value (it shouldn't). 
            try
            {
                // Remove the onframe safety toggle.
                Program.Sonic_Heroes_Overlay.direct2DOnframeDelegate -= Reload_Bank3_II;

                // Convert desired track name to list of bytes.
                List<byte> TrackNameASCII = Encoding.ASCII.GetBytes("sound\\" + Get_Current_Bank3_PAC_Name()).ToList();

                // Add null terminator
                TrackNameASCII.Add(0x00); 

                // Write bank name to memory.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)FILE_BANK3_LOCATION_MEMORY, TrackNameASCII.ToArray());

                // Backup and remove check preventing a file to be loaded again.
                byte[] Backup_Array = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)BRANCH_CHECK_DO_NOT_RELOAD_FILES, 6);

                // Remove the ASM check which prevents the game from reloading the Bank3 file.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)BRANCH_CHECK_DO_NOT_RELOAD_FILES, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }); 

                // Retrieve the this* pointer that is to be passed to the method.
                int ProgramX = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)0x00A2F8B0, 4);

                // Load the sound bank into bank3 using C++ P/Invoke and Inline Assembly.
                Invoke_External_Class.Reload_Bank3(ProgramX); 

                // Restore file reload branch instruction.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)BRANCH_CHECK_DO_NOT_RELOAD_FILES, Backup_Array);
            }
            catch (Exception Ex)
            {
                // Send details of exception to the mod loader server.
                Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes("[DEBUG] " + Program.Mod_Name + " | Reload Bank3 Function, Bank Utilities Class, Exception Thrown: " + Ex.Message), false);
            }
        }

        /// <summary>
        /// Retrieves all of the *.pac sound archives preesent within the game folder.
        /// </summary>
        private void Get_All_PAC()
        {
            // Retrieve all of the *.pac files in <Sonic Heroes Directory?/dvdroot/sound.
            string[] File_List = Directory.GetFiles(Environment.CurrentDirectory + @"\dvdroot\sound\", "*.pac");

            for (int x = 0; x < File_List.Length; x++)
            {
                // Read the PAC file.
                byte[] PACFileArray = File.ReadAllBytes(File_List[x]); 

                // Read the amount of sounds from the header of the PAC file.
                int FileCount = BitConverter.ToInt32(PACFileArray, 0);

                // Current Cursor/Caret Position for the reading of the PAC file.
                int CurrentCursorPosition = 0x20; 

                // Allocate Memory for the storage of PAC File Details.
                List_Of_PAC_Files PAC_File = new List_Of_PAC_Files();

                // Get the filename of the PAC file under investigation.
                PAC_File.BANK_NAME = Path.GetFileName(File_List[x]);

                // Read the BANK ID from the file at the current cursor position.
                PAC_File.BANK_ID = PACFileArray[CurrentCursorPosition];

                // Allocate List storing the individual sound IDs for the PAC file, 30 is an estimate.
                PAC_File.SOUND_NUMBER = new List<byte>(30);

                // For the amount of PAC Sounds specified in the header.
                for (int z = 0; z < FileCount; z++)
                {
                    // Read the byte representing the sound number at 0x1 offset in the current sound effect entry header.
                    PAC_File.SOUND_NUMBER.Add(PACFileArray[CurrentCursorPosition + 1]);

                    // Move to the next sound effect entry header.
                    CurrentCursorPosition += 0x10;
                }

                // Add the file to list of files.
                PAC_List.Add(PAC_File);
            }
        }
    }
}
