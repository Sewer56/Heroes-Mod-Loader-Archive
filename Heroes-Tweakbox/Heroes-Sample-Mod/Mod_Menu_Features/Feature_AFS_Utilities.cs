using SonicHeroes.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Provides various utilities useful for the likes of swapping AFS voice archives in real time.
    /// </summary>
    public class Feature_AFS_Utilities
    {
        /// <summary>
        /// Stores the current list of AFS Files.
        /// </summary>
        string[] AFS_Files;

        /// <summary>
        /// Stores a list of AFS Pointers.
        /// </summary>
        int[] AFS_Pointers;

        /// <summary>
        /// Represents the current index in array of the AFS file to be played.
        /// </summary>
        int AFS_Index = 0;

        /// <summary>
        /// Memory address of where the name of the currently playing ADX song is stored.
        /// </summary>
        const int ADX_CURRENT_SONG_NAME_MEMORY = 0xA6DB8A;

        /// <summary>
        /// Memory location of the jump statement where the game disallows reloading of AFS.
        /// </summary>
        const int AFS_NO_RELOAD_JUMP_STATEMENT = 0x43E32C;

        /// <summary>
        /// Memory location of the AFS File Name Offset.
        /// </summary>
        const int AFS_FILE_NAME_OFFSET_LOCATION_MEMORY = 0x43E333;

        /// <summary>
        /// Main constructor, finds all AFS files available.
        /// </summary>
        public Feature_AFS_Utilities() { Get_All_AFS(); }

        /// <summary>
        /// Returns the current name of the selected AFS File.
        /// </summary>
        /// <returns></returns>
        public string Get_Current_AFS_Name() { return AFS_Files[AFS_Index]; }

        /// <summary>
        /// Increments the currently selected AFS Archive that will be used.
        /// </summary>
        public void Increment_Current_AFS_Archive()
        {
            if  (AFS_Index < AFS_Files.Length - 1)
                 AFS_Index = AFS_Index + 1;
            else AFS_Index = 0;
        }

        /// <summary>
        /// Decrements the currently selected AFS Archive that will be used.
        /// </summary>
        public void Decrement_Current_AFS_Archive()
        {
            if  (AFS_Index > 0)
                 AFS_Index = AFS_Index - 1;
            else AFS_Index = (byte)(AFS_Files.Length - 1); 
        }

        /// <summary>
        /// Sets the AFS File to the current selection.
        /// </summary>
        public void Switch_AFS_File() { Load_AFS_File(AFS_Pointers[AFS_Index]); }

        /// <summary>
        /// Allows you to set AFS file by name, matching a name which contains the given string.
        /// </summary>
        public void Switch_AFS_File(string AFSName)
        {
            for (int x = 0; x < AFS_Files.Length; x++)
            {
                if (AFS_Files[x].Contains(AFSName)) { Load_AFS_File(AFS_Pointers[x]); break; }
            }
        }

        /// <summary>
        /// Loads the AFS file, effectively swapping the voices.
        /// </summary>
        private void Load_AFS_File(int AFS_Pointer)
        {
            // Backup the conditional jump preventing from reloading of AFS file.
            byte[] OriginalCheck = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)AFS_NO_RELOAD_JUMP_STATEMENT, 2);

            // NOP the jump which checks/verifies the AFS reloading.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)AFS_NO_RELOAD_JUMP_STATEMENT, new byte[] { 0x90, 0x90 });

            // Writes the current offset to the new AFS file name into memory.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)AFS_FILE_NAME_OFFSET_LOCATION_MEMORY, BitConverter.GetBytes(AFS_Pointer));

            // Reads the current song name string for backup purposes.
            int String_Length = 0;
            while ((Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)ADX_CURRENT_SONG_NAME_MEMORY + String_Length, 1)) != 0) { String_Length += 1; }
            string Original_Song_Name = Encoding.ASCII.GetString(Program.Sonic_Heroes_Process.ReadMemory((IntPtr)0xA6DB8A, String_Length));

            // Load the currently set AFS File.
            Invoke_External_Class.Load_AFS_Language_File();

            // Rewrite the song name string in memory after it being wiped by the AFS loading function.
            List<byte> Original_Song_Bytes = Encoding.ASCII.GetBytes(Original_Song_Name).ToList();
            Original_Song_Bytes.Add(0x00); // Just in case, add additional null terminator.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADX_CURRENT_SONG_NAME_MEMORY, Original_Song_Bytes.ToArray());

            // Restart the last playing music track.
            Invoke_External_Class.Play_Song(ADX_CURRENT_SONG_NAME_MEMORY);

            // Restore the original check if an AFS file is already loaded.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)AFS_NO_RELOAD_JUMP_STATEMENT, OriginalCheck);
        }

        /// <summary>
        /// Retrieves all of the AFS files.
        /// </summary>
        private void Get_All_AFS()
        {
            // Defines AFS File Location/Directory
            string AFS_Directory = Environment.CurrentDirectory + "\\dvdroot\\bgm\\";

            // Listing of all of the AFS Files.
            string[] AFS_Filepaths = Directory.GetFiles(AFS_Directory, "*.afs");

            // Allocate Memory 
            AFS_Files = new string[AFS_Filepaths.Length];
            AFS_Pointers = new int[AFS_Filepaths.Length];

            for (int x = 0; x < AFS_Filepaths.Length; x++)
            {
                // Add filename of next AFS file into memory.
                AFS_Files[x] = "BGM\\" + Path.GetFileName(AFS_Filepaths[x]);

                // Allocate and write address of AFS file into memory.
                byte[] Memory_Bytes = Encoding.ASCII.GetBytes(AFS_Files[x]); // Get bytes to write file name in memory.
                IntPtr Write_Address = Program.Sonic_Heroes_Process.AllocateMemory(Memory_Bytes.Length); // Memory address.
                Program.Sonic_Heroes_Process.WriteMemory(Write_Address, Memory_Bytes); // Write the file name to memory.

                // Add pointer to array.
                AFS_Pointers[x] = (int)Write_Address;
            }
        }

    }
}
