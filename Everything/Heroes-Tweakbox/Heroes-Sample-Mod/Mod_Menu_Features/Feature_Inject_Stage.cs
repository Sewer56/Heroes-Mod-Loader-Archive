using SharpDX.Direct2D1;
using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SonicHeroes.Networking.Client_Functions;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Injects Emerald Coast live during runtime.
    /// </summary>
    public class Feature_Inject_Stage
    {
        /// <summary>
        /// List of pointers to all of the splines.
        /// </summary>
        List<int> SplinePointers = new List<int>();

        /// <summary>
        /// List to the written pointers to all of the splines.
        /// </summary>
        int SplinePointerTable;

        /// <summary>
        /// Assembly Push Instruction Specifying Pointer to Spline Table.
        /// </summary>
        byte[] ASM_PushPointerTable;

        /// <summary>
        /// Backup of Assembly Push Instruction Specifying Pointer to Spline Table.
        /// </summary>
        byte[] ASM_PushPointerTable_Old;

        /// <summary>
        /// PUSH Instruction with spline list to overwrite.
        /// </summary>
        int PUSH_INSTRUCTION_POINTER; // Ocean Palace Spline List

        /// <summary>
        /// Start Position Pointer of the Stage we are Overwriting.
        /// </summary>
        int STAGE_START_POSITION_POINTER; // Ocean Palace 

        /// <summary>
        /// End Position Pointer of the Stage we are Overwriting.
        /// </summary>
        int STAGE_END_POSITION_POINTER; // Ocean Palace

        byte[] STAGE_START_ORIGINAL_BYTES;
        byte[] STAGE_END_ORIGINAL_BYTES;

        /// <summary>
        /// Is the level enabled or disabled.
        /// </summary>
        public bool Enabled = false;

        /// <summary>
        /// The Directory of the Sonic Heroes Game.
        /// </summary>
        string Heroes_Game_Directory;

        /// <summary>
        /// The folder of the mod loader specific mod.
        /// </summary>
        string Mod_Folder;

        /// <summary>
        /// List of all start positions.
        /// </summary>
        List<StartPositionStruct> Start_Positions = new List<StartPositionStruct>();

        /// <summary>
        /// List of all start positions.
        /// </summary>
        List<EndPositionStruct> End_Positions = new List<EndPositionStruct>();

        /// <summary>
        /// Subdirectory of the mod directory under which the stage is contained in.
        /// </summary>
        string StageDirectory;

        /// <summary>
        /// Holds the collision handlers which are used for executing events if a collision is made.
        /// </summary>
        public List<CollisionHandler> FrameDelegates = new List<CollisionHandler>();

        /// <summary>
        /// Defines a struct which is used for collision checking and if condition is met, executing a delegate.
        /// </summary>
        public struct CollisionHandler
        {
            /// <summary>
            /// Called upon collision with a box found.
            /// </summary>
            public OnFrameDelegate collisionDelegate;
            public List<BoxStruct> collisionBoxes;
        }
        public delegate void OnFrameDelegate();

        /// <summary>
        /// Adds a delegate to the collision handler.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="yourdelegate"></param>
        public void SetCollisionDelegate(int index, OnFrameDelegate yourdelegate)
        {
            CollisionHandler collisionHandler = FrameDelegates[index];
            collisionHandler.collisionDelegate += yourdelegate;
            FrameDelegates[index] = collisionHandler;
        }

        /// <summary>
        /// Struct that defines a spline header.
        /// </summary>
        struct SplineHeader
        {
            /// <summary>
            /// Always 1
            /// </summary>
            public ushort Enabler;
            public ushort Number_Of_Vertices;
            /// <summary>
            /// Purpose Unknown, Set 1000F
            /// </summary>
            public float Total_Spline_Length;
            public int Pointer_Vertex_List;
            /// <summary>
            /// Cast Spline_Type
            /// </summary>
            public int Spline_Type;

            /// <summary>
            /// Individual Vertices of the Spline File.
            /// </summary>
            public List<HeroesVertexStruct> Vertices;
        }

        /// <summary>
        /// Type of Spline.
        /// </summary>
        enum Spline_Type
        {
            Autoloop = 0x433970,
            Rail = 0x4343F0
        }

        /// <summary>
        /// Struct that defines an individual vertex for Sonic Heroes Splines.
        /// </summary>
        struct HeroesVertexStruct
        {
            public int UnknownFlags;
            public float DistanceToNextVertex;
            public float PositionX;
            public float PositionY;
            public float PositionZ;
        }

        /// <summary>
        /// Struct that defines an individual vertex.
        /// </summary>
        public struct VertexStruct
        {
            public float PositionX;
            public float PositionY;
            public float PositionZ;
        }

        /// <summary>
        /// Struct that defines a box.
        /// </summary>
        public struct BoxStruct
        {
            public float MinX;
            public float MinY;
            public float MinZ;
            public float MaxX;
            public float MaxY;
            public float MaxZ;
        }

        /// <summary>
        /// Struct that defines the starting position within the stage.
        /// </summary>
        struct StartPositionStruct
        {
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            public ushort Pitch;
            public byte Mode;
            public int HoldTime; 
        }

        /// <summary>
        /// Struct that defines the end character position.
        /// </summary>
        struct EndPositionStruct
        {
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            public ushort Pitch;
        }

        /// <summary>
        /// Directory under which the stage is contained.
        /// </summary>
        /// <param name="StageDirectoryX">Subdirectory of the mod folder where the stage is contained.</param>
        public Feature_Inject_Stage(string StageDirectoryX)
        {
            // Get folder locations.
            Mod_Folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Heroes_Game_Directory = Environment.CurrentDirectory;
            StageDirectory = StageDirectoryX;

            // Get Stage Properties based off of level configuration.
            Read_Config_File();

            // Read Original Memory Stuff.
            ASM_PushPointerTable_Old = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)PUSH_INSTRUCTION_POINTER, 5);
            STAGE_START_ORIGINAL_BYTES = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)STAGE_START_POSITION_POINTER + 4, 0x8C);
            STAGE_END_ORIGINAL_BYTES = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)STAGE_END_POSITION_POINTER + 4, 0x64);

            // Get all trigger files and construct them.
            try
            {
                string[] TriggerFiles = Directory.GetFiles(Mod_Folder + "\\" + StageDirectory + "\\Triggers\\", "*.obj");
                for (int x = 0; x < TriggerFiles.Length; x++) { FrameDelegates.Add(ConstructBox(TriggerFiles[x])); }
            }
            catch { } // No trigger files.

            // Get All Files & Construct All Splines.
            // If your stage has no splines, place one offscreen.
            string[] SplineFiles = Directory.GetFiles(Mod_Folder + "\\" + StageDirectory + "\\Splines\\", "*.obj");
            for (int x = 0; x < SplineFiles.Length; x++) { SplineHeader Spline = ConstructSpline(SplineFiles[x]); WriteSpline(Spline); }

            // Generate Spline Pointer Table
            Generate_Pointer_Table();

            // Backup Old Files
            BackupFiles();
        }

        /// <summary>
        /// Enables/Disables the Stage Swap
        /// </summary>
        public void Toggle_Stage_Swap()
        {
            if (Enabled)
            {
                ReplaceFiles(Mod_Folder + "\\" + StageDirectory + "\\dvdroot_old\\");
                Program.Sonic_Heroes_Overlay.direct2DRenderMethod -= HandleCollision_OnFrame;
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)PUSH_INSTRUCTION_POINTER, ASM_PushPointerTable_Old);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)STAGE_START_POSITION_POINTER + 0x4, STAGE_START_ORIGINAL_BYTES);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)STAGE_END_POSITION_POINTER + 0x4, STAGE_END_ORIGINAL_BYTES);
                Enabled = false;
            }
            else
            {
                ReplaceFiles(Mod_Folder + "\\" + StageDirectory + "\\dvdroot_new\\");
                Program.Sonic_Heroes_Overlay.direct2DRenderMethod += HandleCollision_OnFrame;
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)PUSH_INSTRUCTION_POINTER, ASM_PushPointerTable);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)STAGE_START_POSITION_POINTER + 0x4, Build_Start_Positions().ToArray());
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)STAGE_END_POSITION_POINTER + 0x4, Build_End_Positions().ToArray());
                Enabled = true;
            }
        }

        /// <summary>
        /// Checks for collision with all boxes and if true, invokes the necessary delegates.
        /// </summary>
        public virtual void HandleCollision_OnFrame(WindowRenderTarget nulltarget)
        {
            // Get Player Postion.
            VertexStruct PlayerPosition = GetPlayerPosition();

            // Iterate over all frame delegates.
            for (int x = 0; x < FrameDelegates.Count; x++)
            {
                // Check Collision for Each Set of Delegates.
                for (int y = 0; y < FrameDelegates[x].collisionBoxes.Count; y++)
                {
                    // Checks Collision
                    bool PlayerInsideBox = CheckCollision(PlayerPosition, FrameDelegates[x].collisionBoxes[y]);

                    // Invoke if collision set.
                    if (PlayerInsideBox) { FrameDelegates[x].collisionDelegate?.Invoke(); break; }
                }
            }
        }

        /// <summary>
        /// Returns the player position for collision checking.
        /// </summary>
        /// <returns></returns>
        protected VertexStruct GetPlayerPosition()
        {
            // Get Player Position.
            VertexStruct PlayerPosition = new VertexStruct();

            // Get XYZ
            int Character_Pointer = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)SonicHeroesVariables.Characters_Addresses.CurrentPlayerControlledCharacter_Pointer, 4);
            int Character_Memory_Position_X = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.XPosition;
            int Character_Memory_Position_Y = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.YPosition;
            int Character_Memory_Position_Z = Character_Pointer + (int)SonicHeroesVariables.Characters_Addresses_Offsets.ZPosition;

            // Gets the character's position coordinates.
            PlayerPosition.PositionX = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)Character_Memory_Position_X, 4);
            PlayerPosition.PositionY = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)Character_Memory_Position_Y, 4);
            PlayerPosition.PositionZ = Program.Sonic_Heroes_Process.ReadMemory<float>((IntPtr)Character_Memory_Position_Z, 4);

            // Return Player Position.
            return PlayerPosition;
        }

        /// <summary>
        /// Builds the Start Position Bytes
        /// </summary>
        private List<byte> Build_Start_Positions()
        {
            List<byte> StartPositionBytes = new List<byte>();

            for (int x = 0; x < Start_Positions.Count; x++)
            {
                StartPositionBytes.AddRange(BitConverter.GetBytes(Start_Positions[x].PositionX));
                StartPositionBytes.AddRange(BitConverter.GetBytes(Start_Positions[x].PositionY));
                StartPositionBytes.AddRange(BitConverter.GetBytes(Start_Positions[x].PositionZ));
                StartPositionBytes.AddRange(BitConverter.GetBytes(Start_Positions[x].Pitch));
                StartPositionBytes.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                StartPositionBytes.Add(Start_Positions[x].Mode);
                StartPositionBytes.AddRange(new byte[] { 0x00, 0x00, 0x00 });
                StartPositionBytes.AddRange(BitConverter.GetBytes(Start_Positions[x].HoldTime));
            }

            return StartPositionBytes;
        }

        /// <summary>
        /// Builds the End Position Bytes
        /// </summary>
        private List<byte> Build_End_Positions()
        {
            List<byte> EndPositionBytes = new List<byte>();

            for (int x = 0; x < Start_Positions.Count; x++)
            {
                EndPositionBytes.AddRange(BitConverter.GetBytes(End_Positions[x].PositionX));
                EndPositionBytes.AddRange(BitConverter.GetBytes(End_Positions[x].PositionY));
                EndPositionBytes.AddRange(BitConverter.GetBytes(End_Positions[x].PositionZ));
                EndPositionBytes.AddRange(BitConverter.GetBytes(End_Positions[x].Pitch));
                EndPositionBytes.AddRange(new byte[] { 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00 });
            }

            return EndPositionBytes;
        }

        /// <summary>
        /// Generates the spline pointer table.
        /// </summary>
        private void Generate_Pointer_Table()
        {
            // Yup
            List<byte> SplinePointerTableData = new List<byte>();
            for (int x = 0; x < SplinePointers.Count; x++) { SplinePointerTableData.AddRange(BitConverter.GetBytes(SplinePointers[x])); }
            SplinePointerTable = (int)Program.Sonic_Heroes_Process.AllocateMemory(SplinePointerTableData.Count());
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SplinePointerTable, SplinePointerTableData.ToArray());

            // Assemble instructions to new pointer table.
            string[] Push_Format_String = new string[] { "use32", "push " + SplinePointerTable, };
            ASM_PushPointerTable = Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(Push_Format_String), true);
        }

        /// <summary>
        /// Reads the Stage Configuration File. This code is crappily written, it could be much better. Do not use this as an example.
        /// </summary>
        private void Read_Config_File()
        {
            // I wouldn't forgive myself for writing this, thankfully it's only ran once.
            string[] ConfigFile = File.ReadAllLines(Mod_Folder + "\\" + StageDirectory + "\\Config.cc");

            // Start and ending structs.
            StartPositionStruct SonicStartPosition = new StartPositionStruct(); EndPositionStruct SonicEndPosition = new EndPositionStruct();
            StartPositionStruct DarkStartPosition = new StartPositionStruct(); EndPositionStruct DarkEndPosition = new EndPositionStruct();
            StartPositionStruct RoseStartPosition = new StartPositionStruct(); EndPositionStruct RoseEndPosition = new EndPositionStruct();
            StartPositionStruct ChaotixStartPosition = new StartPositionStruct(); EndPositionStruct ChaotixEndPosition = new EndPositionStruct();
            StartPositionStruct ForeditStartPosition = new StartPositionStruct(); EndPositionStruct ForeditEndPosition = new EndPositionStruct();

            // Iterate over all lines.
            for (int x = 0; x < ConfigFile.Length; x++)
            {
                try
                {
                    string Value = ConfigFile[x].Substring(ConfigFile[x].IndexOf("=") + 1);

                    /// Stage Spline and Start/End Settings
                    if (ConfigFile[x].StartsWith("PUSH_")) { PUSH_INSTRUCTION_POINTER = Convert.ToInt32(Value, 16); }
                    else if (ConfigFile[x].StartsWith("START_")) { STAGE_START_POSITION_POINTER = Convert.ToInt32(Value, 16); }
                    else if (ConfigFile[x].StartsWith("END_")) { STAGE_END_POSITION_POINTER = Convert.ToInt32(Value, 16); }

                    ///////////////////////////////////////////////////
                    /// Check team, then if start and end then complete
                    ///////////////////////////////////////////////////

                    else if (ConfigFile[x].StartsWith("SONIC_"))
                    {
                        if (ConfigFile[x].Contains("END_"))
                        {
                            if (ConfigFile[x].Contains("POSITIONX")) { SonicEndPosition.PositionX = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONY")) { SonicEndPosition.PositionY = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONZ")) { SonicEndPosition.PositionZ = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("PITCH")) { SonicEndPosition.Pitch = Convert.ToUInt16(Value); continue; }
                        }
                        else
                        {
                            if (ConfigFile[x].Contains("POSITIONX")) { SonicStartPosition.PositionX = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONY")) { SonicStartPosition.PositionY = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONZ")) { SonicStartPosition.PositionZ = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("PITCH")) { SonicStartPosition.Pitch = Convert.ToUInt16(Value); continue; }
                            else if (ConfigFile[x].Contains("MODE")) { SonicStartPosition.Mode = Convert.ToByte(Value); continue; }
                            else if (ConfigFile[x].Contains("RUNNING")) { SonicStartPosition.HoldTime = Convert.ToInt32(Value); continue; }
                        }
                    }
                    else if (ConfigFile[x].StartsWith("DARK_"))
                    {
                        if (ConfigFile[x].Contains("END_"))
                        {
                            if (ConfigFile[x].Contains("POSITIONX")) { DarkEndPosition.PositionX = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONY")) { DarkEndPosition.PositionY = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONZ")) { DarkEndPosition.PositionZ = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("PITCH")) { DarkEndPosition.Pitch = Convert.ToUInt16(Value); continue; }
                        }
                        else
                        {
                            if (ConfigFile[x].Contains("POSITIONX")) { DarkStartPosition.PositionX = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONY")) { DarkStartPosition.PositionY = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONZ")) { DarkStartPosition.PositionZ = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("PITCH")) { DarkStartPosition.Pitch = Convert.ToUInt16(Value); continue; }
                            else if (ConfigFile[x].Contains("MODE")) { DarkStartPosition.Mode = Convert.ToByte(Value); continue; }
                            else if (ConfigFile[x].Contains("RUNNING")) { DarkStartPosition.HoldTime = Convert.ToInt32(Value); continue; }
                        }
                    }
                    else if (ConfigFile[x].StartsWith("ROSE_"))
                    {
                        if (ConfigFile[x].Contains("END_"))
                        {
                            if (ConfigFile[x].Contains("POSITIONX")) { RoseEndPosition.PositionX = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONY")) { RoseEndPosition.PositionY = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONZ")) { RoseEndPosition.PositionZ = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("PITCH")) { RoseEndPosition.Pitch = Convert.ToUInt16(Value); continue; }
                        }
                        else
                        {
                            if (ConfigFile[x].Contains("POSITIONX")) { RoseStartPosition.PositionX = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONY")) { RoseStartPosition.PositionY = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONZ")) { RoseStartPosition.PositionZ = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("PITCH")) { RoseStartPosition.Pitch = Convert.ToUInt16(Value); continue; }
                            else if (ConfigFile[x].Contains("MODE")) { RoseStartPosition.Mode = Convert.ToByte(Value); continue; }
                            else if (ConfigFile[x].Contains("RUNNING")) { RoseStartPosition.HoldTime = Convert.ToInt32(Value); continue; }
                        }
                    }
                    else if (ConfigFile[x].StartsWith("CHAOTIX_"))
                    {
                        if (ConfigFile[x].Contains("END_"))
                        {
                            if (ConfigFile[x].Contains("POSITIONX")) { ChaotixEndPosition.PositionX = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONY")) { ChaotixEndPosition.PositionY = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONZ")) { ChaotixEndPosition.PositionZ = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("PITCH")) { ChaotixEndPosition.Pitch = Convert.ToUInt16(Value); continue; }
                        }
                        else
                        {
                            if (ConfigFile[x].Contains("POSITIONX")) { ChaotixStartPosition.PositionX = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONY")) { ChaotixStartPosition.PositionY = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONZ")) { ChaotixStartPosition.PositionZ = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("PITCH")) { ChaotixStartPosition.Pitch = Convert.ToUInt16(Value); continue; }
                            else if (ConfigFile[x].Contains("MODE")) { ChaotixStartPosition.Mode = Convert.ToByte(Value); continue; }
                            else if (ConfigFile[x].Contains("RUNNING")) { ChaotixStartPosition.HoldTime = Convert.ToInt32(Value); continue; }
                        }
                    }
                    else if (ConfigFile[x].StartsWith("FOREDIT_"))
                    {
                        if (ConfigFile[x].Contains("END_"))
                        {
                            if (ConfigFile[x].Contains("POSITIONX")) { ForeditEndPosition.PositionX = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONY")) { ForeditEndPosition.PositionY = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONZ")) { ForeditEndPosition.PositionZ = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("PITCH")) { ForeditEndPosition.Pitch = Convert.ToUInt16(Value); continue; }
                        }
                        else
                        {
                            if (ConfigFile[x].Contains("POSITIONX")) { ForeditStartPosition.PositionX = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONY")) { ForeditStartPosition.PositionY = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("POSITIONZ")) { ForeditStartPosition.PositionZ = Convert.ToSingle(Value); continue; }
                            else if (ConfigFile[x].Contains("PITCH")) { ForeditStartPosition.Pitch = Convert.ToUInt16(Value); continue; }
                            else if (ConfigFile[x].Contains("MODE")) { ForeditStartPosition.Mode = Convert.ToByte(Value); continue; }
                            else if (ConfigFile[x].Contains("RUNNING")) { ForeditStartPosition.HoldTime = Convert.ToInt32(Value); continue; }
                        }
                    }
                } catch { }
            }

            // Add All Character Entries
            Start_Positions.Add(SonicStartPosition);
            Start_Positions.Add(DarkStartPosition);
            Start_Positions.Add(RoseStartPosition);
            Start_Positions.Add(ChaotixStartPosition);
            Start_Positions.Add(ForeditStartPosition);

            End_Positions.Add(SonicEndPosition);
            End_Positions.Add(DarkEndPosition);
            End_Positions.Add(RoseEndPosition);
            End_Positions.Add(ChaotixEndPosition);
            End_Positions.Add(ForeditEndPosition);
        }

        /// <summary>
        /// Backs up all files into /dvdroot_new
        /// </summary>
        /// <param name="Stage_Directory"></param>
        private void ReplaceFiles(string Stage_Directory)
        {
            // Get all Sonic Heroes Mod Files
            DirectoryInfo Heroes_Stage_Directory_Info = new DirectoryInfo(Stage_Directory);
            FileInfo[] Heroes_Stage_All_Files_List = Heroes_Stage_Directory_Info.EnumerateFiles("*.*", System.IO.SearchOption.AllDirectories).ToArray();

            for (int z = 0; z < Heroes_Stage_All_Files_List.Length; z++)
            {
                // Relative File Location
                string Relative_File_Location = Heroes_Stage_All_Files_List[z].FullName.Substring(Stage_Directory.Length);

                // Copy the new file from the mod to replace the original.
                File.Copy(Stage_Directory + Relative_File_Location, Heroes_Game_Directory + "\\dvdroot\\" + Relative_File_Location, true);
            }
        }

        /// <summary>
        /// Backs up all files into /dvdroot_old
        /// </summary>
        /// <param name="Stage_Directory"></param>
        private void BackupFiles()
        {
            // If old stage directory exists:
            if (Directory.Exists(Mod_Folder + "\\" + StageDirectory + "\\dvdroot_old\\")) { Directory.Delete(Mod_Folder + "\\" + StageDirectory + "\\dvdroot_old\\", true); }
            
            // Get all Sonic Heroes Mod Files
            string Stage_Directory = Mod_Folder + "\\" + StageDirectory + "\\dvdroot_new\\";
            DirectoryInfo Heroes_Stage_Directory_Info = new DirectoryInfo(Stage_Directory);
            FileInfo[] Heroes_Stage_All_Files_List = Heroes_Stage_Directory_Info.EnumerateFiles("*.*", System.IO.SearchOption.AllDirectories).ToArray();

            for (int z = 0; z < Heroes_Stage_All_Files_List.Length; z++)
            {
                // Relative File Location
                string Relative_File_Location = Heroes_Stage_All_Files_List[z].FullName.Substring(Stage_Directory.Length);

                // Copy the new file from the mod to replace the original.
                Directory.CreateDirectory(Path.GetDirectoryName(Stage_Directory.Replace("dvdroot_new", "dvdroot_old") + Relative_File_Location)); // Create directory if it doesn't exist.
                File.Copy(Heroes_Game_Directory + "\\dvdroot\\" + Relative_File_Location, Stage_Directory.Replace("dvdroot_new", "dvdroot_old") + Relative_File_Location, true);
            }
        }

        /// <summary>
        /// Constructs a spline from scratch.
        /// </summary>
        /// <param name="SplinePath"></param>
        private SplineHeader ConstructSpline(string SplinePath)
        {
            SplineHeader SplineFile = new SplineHeader();

            // Properties
            bool CalculateSplineDistanceFlag = false;
            float SplineVertexDistance = 0.0F;
            int SplineVertexFlags = 0;

            // Defaults
            SplineFile.Spline_Type = (int)Spline_Type.Autoloop;

            // Read Spline File
            List<HeroesVertexStruct> Vertices = new List<HeroesVertexStruct>(); // List of vertices for spline.
            string[] OBJ_File = File.ReadAllLines(SplinePath); // Read all of the lines.
            string[] Vertex_XYZ; // Used when splitting current lines of OBJ Files.

            // Iterate over all lines.
            for (int x = 0; x < OBJ_File.Length; x++)
            {
                try
                {
                    string Value = OBJ_File[x].Substring(OBJ_File[x].IndexOf("=") + 1);

                    // Check Spline Type
                    if (OBJ_File[x].StartsWith("SPLINE_TYPE"))
                    {
                        if (Value == "Loop") { SplineFile.Spline_Type = (int)Spline_Type.Autoloop; }
                        else if (Value == "Rail") { SplineFile.Spline_Type = (int)Spline_Type.Rail; }
                    }
                    else if (OBJ_File[x].StartsWith("SPLINE_VERTEX_FLAGS"))
                    {
                        SplineVertexFlags = Convert.ToInt32(Value, 16);
                    }
                    else if (OBJ_File[x].StartsWith("DISTANCE_TO_NEXT_POINT"))
                    {
                        if (Value == "Auto") { CalculateSplineDistanceFlag = true; }
                        else { SplineVertexDistance = Convert.ToSingle(Value); }
                    }
                    else if (OBJ_File[x].StartsWith("v"))
                    {
                        // Remove spaces, place strings to array.
                        Vertex_XYZ = OBJ_File[x].Split(' ');

                        // Define a new Vertex.
                        HeroesVertexStruct TempVertex;

                        // Set Flags
                        TempVertex.UnknownFlags = SplineVertexFlags;

                        // Set Distance between Verts.
                        if (!CalculateSplineDistanceFlag) { TempVertex.DistanceToNextVertex = SplineVertexDistance; }
                        else { TempVertex.DistanceToNextVertex = 0.0F; }

                        // Get coordinates.
                        TempVertex.PositionX = Convert.ToSingle(Vertex_XYZ[1]);
                        TempVertex.PositionY = Convert.ToSingle(Vertex_XYZ[2]);
                        TempVertex.PositionZ = Convert.ToSingle(Vertex_XYZ[3]);
                        Vertices.Add(TempVertex);
                    }
                } catch { }
            }

            // Read all vertices.
            SplineFile.Vertices = Vertices;

            // Set properties
            SplineFile.Enabler = 1;
            SplineFile.Number_Of_Vertices = (ushort)SplineFile.Vertices.Count;

            // Calculate Distance to Next Vertex if Auto
            if (CalculateSplineDistanceFlag)
            {
                for (int x = 0; x < SplineFile.Vertices.Count; x++)
                {
                    try
                    {
                        HeroesVertexStruct TempVertex = SplineFile.Vertices[x];

                        // Get the difference in current and location of next spline.
                        float X_Delta_Position = SplineFile.Vertices[x].PositionX - SplineFile.Vertices[x + 1].PositionX;
                        float Y_Delta_Position = SplineFile.Vertices[x].PositionY - SplineFile.Vertices[x + 1].PositionY;
                        float Z_Delta_Position = SplineFile.Vertices[x].PositionZ - SplineFile.Vertices[x + 1].PositionZ;

                        float Length_Squared = (X_Delta_Position * X_Delta_Position) + (Y_Delta_Position * Y_Delta_Position) + (Z_Delta_Position * Z_Delta_Position);
                        TempVertex.DistanceToNextVertex = (float)Math.Sqrt(Length_Squared);
                        SplineFile.Vertices[x] = TempVertex;
                    } catch { } // Throws exception on last vertex, leaving it untouched.
                }
            }

            // Calculate Spline Length
            SplineFile.Total_Spline_Length = 0F;
            for (int x = 0; x < SplineFile.Vertices.Count; x++) { SplineFile.Total_Spline_Length += SplineFile.Vertices[x].DistanceToNextVertex; }

            return SplineFile;
        }

        /// <summary>
        /// Constructs collision handlers used for collision detection.
        /// </summary>
        /// <param name="BoxPath">Path to the .OBJ file defining multiple boxes.</param>
        private CollisionHandler ConstructBox(string BoxPath)
        {
            // Define Handler
            CollisionHandler Collision_Handler = new CollisionHandler();

            // Allocate Memory
            Collision_Handler.collisionBoxes = new List<BoxStruct>();

            // Read The Box Collision Files.
            List<VertexStruct> Vertices = new List<VertexStruct>(); // List of vertices for spline.
            string[] OBJ_File = File.ReadAllLines(BoxPath); // Read all of the lines.
            string[] Vertex_XYZ; // Used when splitting current lines of OBJ Files.
            BoxStruct CollisionBox = new BoxStruct();

            // Iterate over all lines.
            for (int x = 0; x < OBJ_File.Length; x++)
            {
                try
                {
                    // Convert 3DS Notation
                    if (OBJ_File[x].StartsWith("v  ")) { OBJ_File[x] = OBJ_File[x].Replace("v  ", "v "); }

                    if (OBJ_File[x].Contains("object"))
                    {
                        // Get largest and smallest vertex in each axis from a list of structs containing XYZ.
                        CollisionBox.MinX = Vertices.Min(z => z.PositionX);
                        CollisionBox.MinY = Vertices.Min(z => z.PositionY);
                        CollisionBox.MinZ = Vertices.Min(z => z.PositionZ);
                        CollisionBox.MaxX = Vertices.Max(z => z.PositionX);
                        CollisionBox.MaxY = Vertices.Max(z => z.PositionY);
                        CollisionBox.MaxZ = Vertices.Max(z => z.PositionZ);

                        // Add current Collision Box.
                        Collision_Handler.collisionBoxes.Add(CollisionBox);

                        // Reset the box struct and vertex struct.
                        Vertices = new List<VertexStruct>();
                        CollisionBox = new BoxStruct();
                    }
                    else if (OBJ_File[x].StartsWith("v "))
                    {
                        // Remove spaces, place strings to array.
                        Vertex_XYZ = OBJ_File[x].Split(' ');

                        // Define a new Vertex.
                        VertexStruct TempVertex;

                        // Get coordinates.
                        TempVertex.PositionX = Convert.ToSingle(Vertex_XYZ[1]);
                        TempVertex.PositionY = Convert.ToSingle(Vertex_XYZ[2]);
                        TempVertex.PositionZ = Convert.ToSingle(Vertex_XYZ[3]);

                        // Add vertex to list of vertices.
                        Vertices.Add(TempVertex);
                    }
                }
                catch { }
            }

            // Spaghetti Code Warning - Add Last/First (if only) Box
            try
            {
                CollisionBox.MinX = Vertices.Min(z => z.PositionX); CollisionBox.MinY = Vertices.Min(z => z.PositionY); CollisionBox.MinZ = Vertices.Min(z => z.PositionZ); CollisionBox.MaxX = Vertices.Max(z => z.PositionX); CollisionBox.MaxY = Vertices.Max(z => z.PositionY);  CollisionBox.MaxZ = Vertices.Max(z => z.PositionZ);
                Collision_Handler.collisionBoxes.Add(CollisionBox);
            } catch { }

            return Collision_Handler;
        }

        /// <summary>
        /// Writes a spline onto game memory.
        /// </summary>
        /// <param name="Spline"></param>
        private void WriteSpline(SplineHeader Spline)
        {
            // Write Spline Vertices to Memory
            List<byte> VertexData = new List<byte>();

            // Add data fo all vertices.
            for (int x = 0; x < Spline.Vertices.Count(); x++)
            {
                VertexData.AddRange(BitConverter.GetBytes(Spline.Vertices[x].UnknownFlags));
                VertexData.AddRange(BitConverter.GetBytes(Spline.Vertices[x].DistanceToNextVertex));
                VertexData.AddRange(BitConverter.GetBytes(Spline.Vertices[x].PositionX));
                VertexData.AddRange(BitConverter.GetBytes(Spline.Vertices[x].PositionY));
                VertexData.AddRange(BitConverter.GetBytes(Spline.Vertices[x].PositionZ));
            }

            // Write Vertices to Memory
            Spline.Pointer_Vertex_List = (int)Program.Sonic_Heroes_Process.AllocateMemory(VertexData.Count);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)Spline.Pointer_Vertex_List, VertexData.ToArray());

            // Construct Header File and Add to Header List.
            List<byte> HeaderData = new List<byte>();
            HeaderData.AddRange(BitConverter.GetBytes(Spline.Enabler));
            HeaderData.AddRange(BitConverter.GetBytes(Spline.Number_Of_Vertices));
            HeaderData.AddRange(BitConverter.GetBytes(Spline.Total_Spline_Length));
            HeaderData.AddRange(BitConverter.GetBytes(Spline.Pointer_Vertex_List));
            HeaderData.AddRange(BitConverter.GetBytes(Spline.Spline_Type));

            // Add spline to pointer list.
            int SplineHeaderPointer = (int)Program.Sonic_Heroes_Process.AllocateMemory(HeaderData.Count);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SplineHeaderPointer, HeaderData.ToArray());

            // Add Spline Header Pointer to List
            SplinePointers.Add(SplineHeaderPointer);
        }

        /// <summary>
        /// Checks collision between box A and the supplied coordinates. If they collide, return true.
        /// </summary>
        protected bool CheckCollision(VertexStruct PlayerPosition, BoxStruct Box)
        {
            if 
            (
                (PlayerPosition.PositionX >= Box.MinX && PlayerPosition.PositionX <= Box.MaxX) &&
                (PlayerPosition.PositionY >= Box.MinY && PlayerPosition.PositionY <= Box.MaxY) &&
                (PlayerPosition.PositionZ >= Box.MinZ && PlayerPosition.PositionZ <= Box.MaxZ)
            ) { return true; }
            else { return false; }
        }
    }
}
