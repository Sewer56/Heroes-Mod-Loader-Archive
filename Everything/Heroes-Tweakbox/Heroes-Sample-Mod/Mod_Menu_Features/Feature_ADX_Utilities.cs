using SonicHeroes.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Utilities for working and handling of ADX files.
    /// </summary>
    public class Feature_ADX_Utilities
    {
        /// <summary>
        /// Specifies the ID in the list of the currently selected ADX Track.
        /// </summary>
        int Current_ADX_Track = 0x00;

        /// <summary>
        /// Stores a list of all BGM Files available for usage.
        /// </summary>
        List<List_Of_BGM_Files> BGM_List = new List<List_Of_BGM_Files>(); // Lists of loadable tracks, each is a string.

        /// <summary>
        /// Struct that defines a list of BGMs which can be played at any time.
        /// </summary>
        struct List_Of_BGM_Files
        {
            /// <summary>
            /// The name of the current song that is to be played.
            /// </summary>
            public string Song_Name;

            /// <summary>
            /// Pointer in memory of the song to be played, this is necessary for passsing in, into the function which plays a track.
            /// </summary>
            public int Song_Pointer;
        }

        /// <summary>
        /// Initializes this class for usage.
        /// </summary>
        public Feature_ADX_Utilities() { Get_All_BGM(); }

        /// <summary>
        /// Returns the currently selected ADX Track name.
        /// </summary>
        public string Get_Current_ADX_Track_Name() { return BGM_List[Current_ADX_Track].Song_Name; }

        /// <summary>
        /// Plays the currently selected ADX track. Calls C++ Function with our own pointer to the music track.
        /// </summary>
        public void Play_ADX_Track() { Invoke_External_Class.Play_Song(BGM_List[Current_ADX_Track].Song_Pointer); }

        /// <summary>
        /// Plays ADX which contains in its name the passed in string. Calls C++ Function with our own pointer to the music track.
        /// </summary>
        public void Play_ADX_Track(string ADXName)
        {
            for (int x = 0; x < BGM_List.Count; x++)
            {
                if (BGM_List[x].Song_Name.Contains(ADXName)) { Invoke_External_Class.Play_Song(BGM_List[x].Song_Pointer); break; }  
            }
        }

        /// <summary>
        /// Increments the currently selected BANK file/ID to play.
        /// </summary>
        public void Increment_Current_ADX_Track()
        {
            if (Current_ADX_Track < BGM_List.Count - 1)
                Current_ADX_Track = Current_ADX_Track + 1;
            else Current_ADX_Track = 0;
        }

        /// <summary>
        /// Decrements the currently selected BANK file/ID to play.
        /// </summary>
        public void Decrement_Current_ADX_Track()
        {
            if (Current_ADX_Track > 0)
                Current_ADX_Track = Current_ADX_Track - 1;
            else Current_ADX_Track = (byte)(BGM_List.Count - 1);
        }

        /// <summary>
        /// Obtains a list of BGM.
        /// </summary>
        private void Get_All_BGM()
        {
            // Retrieve list of all .adx files available in game's ADX directory.
            string[] File_List = Directory.GetFiles(Environment.CurrentDirectory + @"\dvdroot\bgm\", "*.adx");

            for (int x = 0; x < File_List.Length; x++)
            {
                // Allocate memory for storing BGM properties.
                List_Of_BGM_Files BGM_File; 

                // Retrieve the file name only of the file.
                string File_Name_Only = Path.GetFileName(File_List[x]);
                
                // Write the file name to memory,
                byte[] Memory_Bytes = Encoding.ASCII.GetBytes(File_Name_Only); // Get bytes to write file name in memory.
                IntPtr Write_Address = Program.Sonic_Heroes_Process.AllocateMemory(Memory_Bytes.Length); // Memory address.
                Program.Sonic_Heroes_Process.WriteMemory(Write_Address, Memory_Bytes); // Write the file name to memory.

                // Add BGM File to List
                BGM_File.Song_Name = File_Name_Only;
                BGM_File.Song_Pointer = (int)Write_Address;
                BGM_List.Add(BGM_File);
            }
        }
    }
}
