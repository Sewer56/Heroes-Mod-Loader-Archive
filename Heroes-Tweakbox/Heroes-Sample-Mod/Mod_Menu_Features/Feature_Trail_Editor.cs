using SonicHeroes.Memory;
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
    /// Mod Menu's Trail Editor
    /// </summary>
    public class Feature_Trail_Editor
    {
        public Feature_Trail_Editor()
        {
            // Load Default Values for Character Colours
            Populate_All_Characters();

            // Load Current Character.
            Load_Current_Character();

            // Load all Character Settings
            Load_All_Character_Settings(false);
        }

        /// 
        /// Variables!
        /// 

        // List of Sonic Heroes Characters.
        public List<Characters> Sonic_Heroes_Characters = new List<Characters>();

        // Current Character Index (Character Selected)
        public int Character_Index = 0;

        // Used for holding colours.
        public struct Character_Ball_Colours
        {
            public RGBA_Colour Jump_Ball;
            public RGBA_Colour Trail;
            public RGBA_Colour Tornado;
            public RGBA_Colour Misc;
        }

        // Used for holding RGBA Values
        public struct RGBA_Colour
        {
            public byte R;
            public byte G;
            public byte B;
            public byte A;
        }

        // Used for holding individual characters.
        public struct Characters
        {
            public string Character_Name;
            public string Misc_Name;
            public Character_Ball_Colours Character_Ball_Colours;
            public IntPtr Jump_Ball_Address;
            public IntPtr Trail_Address;
            public IntPtr Tornado_Address;
            public IntPtr Misc_Address;
        }

        // Items contained in this menu.
        public enum Character_List
        {
            Amy = 0x00,
            Big = 0x01,
            Charmy = 0x02,
            Cream = 0x03,
            Espio = 0x04,
            Knuckles = 0x05,
            Omega = 0x06,
            Rouge = 0x07,
            Shadow = 0x08,
            Sonic = 0x09,
            Tails = 0x10,
            Vector = 0x11,
        }

        /// <summary>
        /// Increments the index of the current menu.
        /// </summary>
        public void Increment_Character_Index()
        {
            if (Character_Index == Sonic_Heroes_Characters.Count - 1) { Character_Index = 0; }
            else { Character_Index = Character_Index + 1; }
        }

        /// <summary>
        /// Decrements the index of the current menu.
        /// </summary>
        public void Decrement_Character_Index()
        {
            if (Character_Index == 0) { Character_Index = Sonic_Heroes_Characters.Count - 1; }
            else { Character_Index = Character_Index - 1; }
        }

        /// <summary>
        /// Iterates over each character and saves all settings.
        /// </summary>
        public void Save_All_Character_Settings()
        {
            // Mod Directory
            string Mod_Folder = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);

            // List of strings for the config file.
            List<string> Character_Ball_Config_Strings = new List<string>();

            // Tweakbox
            Character_Ball_Config_Strings.Add("# Heroes Tweakbox Character Colour Configuration File");

            // For each character, save their properties.
            for (int x = 0; x < Sonic_Heroes_Characters.Count; x++)
            {
                // Rotate the character Index.
                Increment_Character_Index();

                // Load the current character.
                Load_Current_Character();

                // Dump Necessary Character Information :)
                Character_Ball_Config_Strings.Add("\n# Character Entry for " + Sonic_Heroes_Characters[x].Character_Name + " #");
                Character_Ball_Config_Strings.Add("Character Name=" + Sonic_Heroes_Characters[x].Character_Name);

                // Dump all of the RGB Values.
                if (Sonic_Heroes_Characters[x].Jump_Ball_Address != IntPtr.Zero)
                {
                    Character_Ball_Config_Strings.Add("Jump Ball Colour | R=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Jump_Ball.R);
                    Character_Ball_Config_Strings.Add("Jump Ball Colour | G=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Jump_Ball.G);
                    Character_Ball_Config_Strings.Add("Jump Ball Colour | B=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Jump_Ball.B);
                    Character_Ball_Config_Strings.Add("Jump Ball Colour | A=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Jump_Ball.A);
                }

                // Dump all of the RGB Values.
                if (Sonic_Heroes_Characters[x].Tornado_Address != IntPtr.Zero)
                {
                    Character_Ball_Config_Strings.Add("Tornado Colour | R=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Tornado.R);
                    Character_Ball_Config_Strings.Add("Tornado Colour | G=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Tornado.G);
                    Character_Ball_Config_Strings.Add("Tornado Colour | B=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Tornado.B);
                    Character_Ball_Config_Strings.Add("Tornado Colour | A=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Tornado.A);
                }

                // Dump all of the RGB Values.
                if (Sonic_Heroes_Characters[x].Trail_Address != IntPtr.Zero)
                {
                    Character_Ball_Config_Strings.Add("Trail Colour | R=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Trail.R);
                    Character_Ball_Config_Strings.Add("Trail Colour | G=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Trail.G);
                    Character_Ball_Config_Strings.Add("Trail Colour | B=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Trail.B);
                    Character_Ball_Config_Strings.Add("Trail Colour | A=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Trail.A);
                }

                // Dump all of the RGB Values.
                if (Sonic_Heroes_Characters[x].Misc_Address != IntPtr.Zero)
                {
                    Character_Ball_Config_Strings.Add("Misc Colour | R=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Misc.R);
                    Character_Ball_Config_Strings.Add("Misc Colour | G=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Misc.G);
                    Character_Ball_Config_Strings.Add("Misc Colour | B=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Misc.B);
                    Character_Ball_Config_Strings.Add("Misc Colour | A=" + Sonic_Heroes_Characters[x].Character_Ball_Colours.Misc.A);
                }

            }

            // Create config file.
            File.WriteAllLines(Mod_Folder + "\\Character-Ball-Configuration.txt", Character_Ball_Config_Strings);
        }

        /// <summary>
        /// Iterates over each character and reads all settings.
        /// </summary>
        public void Load_All_Character_Settings(bool Read_Default_Configuration)
        {
            try
            {
                // Mod Directory
                string Mod_Folder = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);

                // Read the file.
                StreamReader Character_Ball_Config_Stream;
                if (Read_Default_Configuration) { Character_Ball_Config_Stream = new StreamReader(Mod_Folder + "\\Default-Ball-Configuration.txt"); }
                else { Character_Ball_Config_Stream = new StreamReader(Mod_Folder + "\\Character-Ball-Configuration.txt"); }

                // Current Line
                string Current_Line;

                // Current Character Array Index
                int Current_Character_Index = 0;

                // Temporary copy of all characters.
                Characters[] Temp_Heroes_Characters = Sonic_Heroes_Characters.ToArray();

                // Read the file
                while ((Current_Line = Character_Ball_Config_Stream.ReadLine()) != null)
                {
                    // Value of the individual element
                    string value = Current_Line.Substring(Current_Line.IndexOf("=") + 1);

                    if (Current_Line.StartsWith("#")) { }
                    // Get index of character
                    else if (Current_Line.StartsWith("Character Name")) { Current_Character_Index = Sonic_Heroes_Characters.TakeWhile(x => x.Character_Name != value).Count(); }
                    else if (Current_Line.StartsWith("Jump Ball Colour | R")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Jump_Ball.R = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Jump Ball Colour | G")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Jump_Ball.G = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Jump Ball Colour | B")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Jump_Ball.B = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Jump Ball Colour | A")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Jump_Ball.A = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Tornado Colour | R")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Tornado.R = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Tornado Colour | G")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Tornado.G = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Tornado Colour | B")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Tornado.B = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Tornado Colour | A")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Tornado.A = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Trail Colour | R")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Trail.R = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Trail Colour | G")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Trail.G = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Trail Colour | B")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Trail.B = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Trail Colour | A")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Trail.A = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Misc Colour | R")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Misc.R = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Misc Colour | G")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Misc.G = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Misc Colour | B")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Misc.B = Convert.ToByte(value); }
                    else if (Current_Line.StartsWith("Misc Colour | A")) { Temp_Heroes_Characters[Current_Character_Index].Character_Ball_Colours.Misc.A = Convert.ToByte(value); }
                }

                // Close FileStream
                Character_Ball_Config_Stream.Close();

                // Retrieve List
                Sonic_Heroes_Characters = Temp_Heroes_Characters.ToList();

                // Write all colours to memory.
                foreach (Characters Character in Sonic_Heroes_Characters)
                {
                    Write_Character_Colours(Character);
                }
            }
            catch (Exception Ex)
            {
                Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Send_Message, Encoding.ASCII.GetBytes("Tweakbox: Default Character Ball Colour settings are missing or are invalid, your culprit is either Mod-Loader-Mods/Tweakbox/Character-Ball-Configuration.txt or Default-Ball-Configuration.txt"), false);
            }
        }

        /// <summary>
        /// Loads the character at the currently selected index.
        /// </summary>
        public void Load_Current_Character()
        {
            // Get Current Character in a More Accessible Manner
            Characters Current_Character = Sonic_Heroes_Characters[Character_Index];

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Current_Character.Jump_Ball_Address != IntPtr.Zero)
            {
                // Load Jump Ball Colour
                RGBA_Colour Ball_Colour = new RGBA_Colour();
                Ball_Colour.R = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Jump_Ball_Address, 1);
                Ball_Colour.G = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Jump_Ball_Address + 1, 1);
                Ball_Colour.B = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Jump_Ball_Address + 2, 1);
                Ball_Colour.A = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Jump_Ball_Address + 3, 1);
                Current_Character.Character_Ball_Colours.Jump_Ball = Ball_Colour;
            }

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Current_Character.Trail_Address != IntPtr.Zero)
            {
                // Load Jump Ball Colour
                RGBA_Colour Ball_Colour = new RGBA_Colour();
                Ball_Colour.R = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Trail_Address, 1);
                Ball_Colour.G = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Trail_Address + 1, 1);
                Ball_Colour.B = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Trail_Address + 2, 1);
                Ball_Colour.A = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Trail_Address + 3, 1);
                Current_Character.Character_Ball_Colours.Trail = Ball_Colour;
            }

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Current_Character.Tornado_Address != IntPtr.Zero)
            {
                // Load Jump Ball Colour
                RGBA_Colour Ball_Colour = new RGBA_Colour();
                Ball_Colour.R = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Tornado_Address, 1);
                Ball_Colour.G = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Tornado_Address + 1, 1);
                Ball_Colour.B = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Tornado_Address + 2, 1);
                Ball_Colour.A = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Tornado_Address + 3, 1);
                Current_Character.Character_Ball_Colours.Tornado = Ball_Colour;
            }

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Current_Character.Misc_Address != IntPtr.Zero)
            {
                // Load Jump Ball Colour
                RGBA_Colour Ball_Colour = new RGBA_Colour();
                Ball_Colour.R = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Misc_Address, 1);
                Ball_Colour.G = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Misc_Address + 1, 1);
                Ball_Colour.B = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Misc_Address + 2, 1);
                Ball_Colour.A = Program.Sonic_Heroes_Process.ReadMemory<byte>(Current_Character.Misc_Address + 3, 1);
                Current_Character.Character_Ball_Colours.Misc = Ball_Colour;
            }

            // Replace Current Character Back into The List
            Sonic_Heroes_Characters[Character_Index] = Current_Character;
        }

        /// <summary>
        /// Writes all of the Specific Colours of Ball/Trail to the current characters in Sonic Heroes' Memory.
        /// </summary>
        public void Write_Character_Colours(Characters Sonic_Heroes_Character)
        {
            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Sonic_Heroes_Characters[Character_Index].Jump_Ball_Address != IntPtr.Zero)
            {
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Jump_Ball_Address, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Jump_Ball.R });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Jump_Ball_Address + 1, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Jump_Ball.G });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Jump_Ball_Address + 2, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Jump_Ball.B });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Jump_Ball_Address + 3, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Jump_Ball.A });
            }

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Sonic_Heroes_Character.Trail_Address != IntPtr.Zero)
            {
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Trail_Address, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Trail.R });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Trail_Address + 1, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Trail.G });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Trail_Address + 2, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Trail.B });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Trail_Address + 3, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Trail.A });
            }

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Sonic_Heroes_Character.Tornado_Address != IntPtr.Zero)
            {
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Tornado_Address, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Tornado.R });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Tornado_Address + 1, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Tornado.G });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Tornado_Address + 2, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Tornado.B });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Tornado_Address + 3, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Tornado.A });
            }

            // Check if character has X/Y/Z Property. If they do, load them into the menu.
            if (Sonic_Heroes_Character.Misc_Address != IntPtr.Zero)
            {
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Misc_Address, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Misc.R });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Misc_Address + 1, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Misc.G });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Misc_Address + 2, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Misc.B });
                Program.Sonic_Heroes_Process.WriteMemory(Sonic_Heroes_Character.Misc_Address + 3, new byte[] { Sonic_Heroes_Character.Character_Ball_Colours.Misc.A });
            }
        }

        /// <summary>
        /// Populates all of the characters' values.
        /// </summary>
        public void Populate_All_Characters()
        {
            // Populate list of Sonic_Heroes_Characters
            Characters Amy = new Characters();
            Characters Big = new Characters();
            Characters Charmy = new Characters();
            Characters Cream = new Characters();
            Characters Espio = new Characters();
            Characters Knuckles = new Characters();
            Characters Omega = new Characters();
            Characters Rouge = new Characters();
            Characters Shadow = new Characters();
            Characters Sonic = new Characters();
            Characters Tails = new Characters();
            Characters Vector = new Characters();

            // Populate Character Properties.
            Amy.Character_Name = "Amy";
            Amy.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Jump_Ball;
            Amy.Tornado_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Tornado;
            Amy.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Trails;

            Big.Character_Name = "Big";
            Big.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Jump_Ball;
            Big.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Trails;

            Charmy.Character_Name = "Charmy";
            Charmy.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Jump_Ball;
            Charmy.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Trails;

            Cream.Character_Name = "Cream";
            Cream.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Jump_Ball;
            Cream.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Trails;

            Espio.Character_Name = "Espio";
            Espio.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Jump_Ball;
            Espio.Tornado_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Tornado;
            Espio.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Trails;

            Knuckles.Character_Name = "Knuckles";
            Knuckles.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Jump_Ball;
            Knuckles.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Trails;

            Omega.Character_Name = "Omega";
            Omega.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Jump_Ball;
            Omega.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Trails;

            Rouge.Character_Name = "Rouge";
            Rouge.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Jump_Ball;
            Rouge.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Trails;

            Shadow.Character_Name = "Shadow";
            Shadow.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Jump_Ball;
            Shadow.Tornado_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Tornado;
            Shadow.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Trails;

            Sonic.Character_Name = "Sonic";
            Sonic.Misc_Name = "Sonic Overdrive Ball";
            Sonic.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Jump_Ball;
            Sonic.Tornado_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Tornado;
            Sonic.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Trails;
            Sonic.Misc_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Overdrive_Ball;

            Tails.Character_Name = "Tails";
            Tails.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Jump_Ball;
            Tails.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Trails;

            Vector.Character_Name = "Vector";
            Vector.Jump_Ball_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Jump_Ball;
            Vector.Trail_Address = (IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Trails;

            // Add all of the characters to the list.
            Sonic_Heroes_Characters.Add(Amy);
            Sonic_Heroes_Characters.Add(Big);
            Sonic_Heroes_Characters.Add(Charmy);
            Sonic_Heroes_Characters.Add(Cream);
            Sonic_Heroes_Characters.Add(Espio);
            Sonic_Heroes_Characters.Add(Knuckles);
            Sonic_Heroes_Characters.Add(Omega);
            Sonic_Heroes_Characters.Add(Rouge);
            Sonic_Heroes_Characters.Add(Shadow);
            Sonic_Heroes_Characters.Add(Sonic);
            Sonic_Heroes_Characters.Add(Tails);
            Sonic_Heroes_Characters.Add(Vector);
        }
    }
}
