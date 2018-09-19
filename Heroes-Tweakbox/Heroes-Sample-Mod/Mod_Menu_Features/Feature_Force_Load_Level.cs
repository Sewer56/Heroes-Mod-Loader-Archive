using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System;
using System.Collections.Generic;
using System.Threading;
using static SonicHeroes.Networking.Client_Functions;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Allows for forceful loading of levels.
    /// </summary>
    public class Feature_Force_Load_Level
    {
        public Feature_Force_Load_Level()
        {
            // Retrieves all of the available possible levels that could be loaded, as well as their individual Stage IDs.
            Get_All_Levels();

            /// <summary>
            /// x86 Mnemonics which dynamically load the appropriate blending address based on the currently controlling character.
            /// </summary>
            string[] Menu_Enter_Hook_ASM_Mnemonics = new string[]
            {
                "use32",
                "mov eax,[edi+0x58]",
                "push dword 0x40578D",
                "ret"
            };

            // Get Assembly Code for the Mnemonics
            byte[] Menu_Enter_Hook_ASM_Bytes = Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(Menu_Enter_Hook_ASM_Mnemonics), true);

            // Set Hook - Hooked at random address triggered post loading of main menu.
            Menu_Enter_Hook_ASM = new SonicHeroes.Hooking.ASM_Hook((IntPtr)0x405785, Menu_Enter_Hook_ASM_Bytes, 8, Program.Sonic_Heroes_Networking_Client, true);

            // Activate Hook Such that Menu_Enter_Hook Picks up the New Bytes.
            Menu_Enter_Hook_ASM.Activate();

            // Set Hook - Hooked at random address triggered post loading of main menu.
            Menu_Enter_Hook = new SonicHeroes.Hooking.Injection((IntPtr)0x405785, (Force_Level_Load_Hook)Force_Load_Level_Method, 8, Program.Sonic_Heroes_Networking_Client, false);

            // Deactivate Hook.
            Menu_Enter_Hook_ASM.Deactivate();

            // At startup set initial teams to be Team Sonic for P1 and disable others
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x8D6920, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        // Variables
        public int Stage_Enum_Index = 0;
        public List<Stage_ID_Entry> Stage_Entry_List = new List<Stage_ID_Entry>();

        // Describes an entry.
        public struct Stage_ID_Entry
        {
            public string Stage_Name;
            public int Stage_ID;
        }

        /// <summary>
        /// Retrieves all of the available possible levels that could be loaded, as well as their individual Stage IDs.
        /// </summary>
        private void Get_All_Levels()
        {
            foreach (SonicHeroesVariables.Stage_StageIDs Stage_ID in Enum.GetValues(typeof(SonicHeroesVariables.Stage_StageIDs)))
            {
                Stage_ID_Entry New_Entry = new Stage_ID_Entry();
                New_Entry.Stage_Name = Enum.GetName(typeof(SonicHeroesVariables.Stage_StageIDs), Stage_ID);
                New_Entry.Stage_ID = (int)Stage_ID;
                Stage_Entry_List.Add(New_Entry);
            }
        }

        /// <summary>
        /// Increments the currently selected STAGE ID to load.
        /// </summary>
        public void Increment_StageID_Index()
        {
            if (Stage_Enum_Index == Stage_Entry_List.Count - 1) { Stage_Enum_Index = 0; }
            else { Stage_Enum_Index = Stage_Enum_Index + 1; }
        }

        /// <summary>
        /// Decrements the currently selected STAGE ID to load.
        /// </summary>
        public void Decrement_StageID_Index()
        {
            if (Stage_Enum_Index == 0) { Stage_Enum_Index = Stage_Entry_List.Count - 1; }
            else { Stage_Enum_Index = Stage_Enum_Index - 1; }
        }

        SonicHeroes.Hooking.Injection Menu_Enter_Hook;
        SonicHeroes.Hooking.ASM_Hook Menu_Enter_Hook_ASM;
        public delegate void Force_Level_Load_Hook(); 

        /// <summary>
        /// Attempts to forcefully load a level into the current game.
        /// </summary>
        public void Force_Load_Level()
        {
            // Write requested stage to memory.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Stage_CurrentStage.PlayerStageIDToLoad, BitConverter.GetBytes(Stage_Entry_List[Stage_Enum_Index].Stage_ID));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Stage_CurrentStage.PlayerStageChoiceMainMenu, BitConverter.GetBytes(Stage_Entry_List[Stage_Enum_Index].Stage_ID));

            // Check if in level.
            byte In_Level = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);
            if (In_Level == 1)
            {
                // Secretly put us back in the main menu if we are in a stage.
                Invoke_External_Class.Exit_Stage_X();
                Menu_Enter_Hook_ASM.Activate();
                Menu_Enter_Hook.Activate();
            }
            else
            {
                Force_Load_Level_Method();
            }
        }

        /// <summary>
        /// Loads a level with a specified ID into the game.
        /// </summary>
        public void Load_Level_ID(int LevelID)
        {
            // Write Passed in Level ID
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Stage_CurrentStage.PlayerStageIDToLoad, BitConverter.GetBytes(LevelID));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Stage_CurrentStage.PlayerStageChoiceMainMenu, BitConverter.GetBytes(LevelID));

            // Check if in level.
            byte In_Level = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);
            if (In_Level == 1)
            {
                // Secretly put us back in the main menu if we are in a stage.
                Invoke_External_Class.Exit_Stage_X();
                Menu_Enter_Hook_ASM.Activate();
                Menu_Enter_Hook.Activate();
            }
            else
            {
                Force_Load_Level_Method();
            }
        }

        /// <summary>
        /// Loads the level with the level ID through the use of the hook rather than normally.
        /// </summary>
        /// <param name="LevelID"></param>
        public void Load_Level_ID_Via_Hook(int LevelID)
        {
            // Write Passed in Level ID
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Stage_CurrentStage.PlayerStageIDToLoad, BitConverter.GetBytes(LevelID));
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Stage_CurrentStage.PlayerStageChoiceMainMenu, BitConverter.GetBytes(LevelID));

            // Set Hook
            Menu_Enter_Hook_ASM.Activate();
            Menu_Enter_Hook.Activate();
        }

        /// <summary>
        /// Forcefully loads a level once player is in main menu.
        /// </summary>
        private void Force_Load_Level_Method()
        {
            // Commit to the writing of the stage IDs to memory.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x8D66F0, BitConverter.GetBytes((int)2)); // Set Game State to 2 (In Level)

            // Set the game load status to -1 to force a stage load.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0xA81FFC, BitConverter.GetBytes((int)-1));

            // Disable hook
            Menu_Enter_Hook.Deactivate();
            Menu_Enter_Hook_ASM.Deactivate();
        }

        ////////////////////////////////////////////
        ////////////////////////////////////////////
        ////////////////////////////////////////////
        ////////////////////////////////////////////
        ////////////////////////////////////////////

        /// <summary>
        /// Forcefully tries to load a debug menu.
        /// </summary>
        public void Force_Load_Debug_Menu()
        {
            // Jump to debug menu ASM.
            byte[] Jump_004273D5 = new byte[] { 0xE9, 0x68, 0x02, 0x00, 0x00, 0x90, 0x90 };
            byte[] Jump_0042723A = new byte[] { 0xE9, 0xCD, 0x00, 0x00, 0x00, 0x90, 0x90 };
            byte[] Jump_00427355 = new byte[] { 0xE9, 0xE8, 0x01, 0x00, 0x00, 0x90, 0x90 };

            // Override where to go on next load to debug menu.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x427168, Jump_00427355);

            // Tell the game to re-load current state
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0xA81FFC, BitConverter.GetBytes((int)1));
            Thread.Sleep(32);

            // Override where to go on next load to debug menu.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x427168, Jump_0042723A);
            Thread.Sleep(64);

            // Activate Level Enter Hook
            DebugJumpResetInjection.Activate();
        }

        // Write Original Instructions
        public static void Reset_Jump() { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)0x427168, Original_Debug_Jump_Instructions); DebugJumpResetInjection.Deactivate(); }
        static byte[] Original_Debug_Jump_Instructions = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)0x427168, 7);
        private delegate void Debug_Menu_Reset_Jump_Delegate();
        static SonicHeroes.Hooking.Injection DebugJumpResetInjection = new SonicHeroes.Hooking.Injection((IntPtr)0x427244, (Debug_Menu_Reset_Jump_Delegate)Reset_Jump, 6, Program.Sonic_Heroes_Networking_Client, false);

    }
}
