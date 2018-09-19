using SharpDX.Direct2D1;
using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SonicHeroes.Networking.Client_Functions;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Enables Station Square Specific Level Warping.
    /// </summary>
    public class Feature_Station_Square : Feature_Inject_Stage
    {
        /// <summary>
        /// Has the payload been triggered once.
        /// </summary>
        public bool Emerald_Coast_Trigger_Activated = false;

        /// <summary>
        /// Initializes the Hub World Payload
        /// </summary>
        public Feature_Station_Square(string StageDirectoryX) : base(StageDirectoryX)
        {
            // Set Payload
            SetCollisionDelegate(0, Payload);
            SetCollisionDelegate(1, Payload_II);
        }

        /// <summary>
        /// Hit the payload.
        /// </summary>
        private void Payload()
        {
            if (!Emerald_Coast_Trigger_Activated)
            {
                Program.Feature_Force_Load_Level_X.Load_Level_ID((int)SonicHeroesVariables.Stage_StageIDs.OceanPalace);
                Emerald_Coast_Trigger_Activated = true;
            }
        }

        /// <summary>
        /// Hit the payload.
        /// </summary>
        private void Payload_II()
        {
            if (!Emerald_Coast_Trigger_Activated)
            {
                // Secretly put us back in the main menu if we are in a stage.
                Invoke_External_Class.Exit_Stage_X();
                Emerald_Coast_Trigger_Activated = true;
            }
        }

        /// <summary>
        /// Checks for collision with all boxes and if true, invokes the necessary delegates.
        /// </summary>
        public override void HandleCollision_OnFrame(WindowRenderTarget nulltarget)
        {
            // Retrieve Current Stage
            byte CurrentStage = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Stage_CurrentStage.PlayerStageIDToLoad, 1);

            // If stage is Seaside Hill, process the triggers, else do nothing..
            if (CurrentStage == (int)SonicHeroesVariables.Stage_StageIDs.SeasideHill && Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroes.Variables.SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1) == 1)
            { base.HandleCollision_OnFrame(nulltarget); }
            else { Emerald_Coast_Trigger_Activated = false; } // Reset trigger if another stage.
        }

    }
}
