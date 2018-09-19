using SonicHeroes.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Toggles character chatter.
    /// </summary>
    public class Feature_Toggle_Character_Chatter
    {
        /// <summary>
        /// Address to a 0/1 boolean flag which determins whether the characters perform their action sounds.
        /// </summary>
        const int CHARACTER_CHATTER_FLAG_ADDRESS = 0x8CAEF8;

        /// <summary>
        /// Address to a conditional branch which decides whether character stage comments should play.
        /// </summary>
        const int CHARACTER_STAGE_COMMENTS_BRANCH_ADDRESS = 0x43EC8C;

        /// <summary>
        /// Is Character Action Chatter Enabled?
        /// </summary>
        bool CHARACTER_CHATTER_ENABLED = true;

        /// <summary>
        /// Are Character Stage-Triggered Comments enabled?
        /// </summary>
        bool CHARACTER_Stage_COMMENTS_ENABLED = true;

        /// <summary>
        /// Returns true if Character Action Chatter is Enabled.
        /// </summary>
        public bool Get_CharacterChatter() { return CHARACTER_CHATTER_ENABLED; }

        /// <summary>
        /// Returns true if Character Stage-triggered Comment Chatter is Enabled.
        /// </summary>
        public bool Get_VoiceComments() { return CHARACTER_Stage_COMMENTS_ENABLED; }

        /// <summary>
        /// Toggles Character Action Chatter On/Off.
        /// </summary>
        public void Toggle_CharacterChatter()
        {
            if (CHARACTER_CHATTER_ENABLED) { DisableCharacterChatter(); CHARACTER_CHATTER_ENABLED = false; }
            else { EnableCharacterChatter(); CHARACTER_CHATTER_ENABLED = true; }
        }

        /// <summary>
        /// Toggles Character Stage-triggered Chatter On/Off.
        /// </summary>
        public void Toggle_CharacterCommentChatter()
        {
            if (CHARACTER_Stage_COMMENTS_ENABLED) { DisableCharacterVoiceChatter(); CHARACTER_Stage_COMMENTS_ENABLED = false; }
            else { EnableCharacterVoiceChatter(); CHARACTER_Stage_COMMENTS_ENABLED = true; }
        }

        // Enablers/Disablers
        private void DisableCharacterChatter() { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)CHARACTER_CHATTER_FLAG_ADDRESS, BitConverter.GetBytes((int)1)); } // CHARMY_SHUTUP 1
        private void EnableCharacterChatter() { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)CHARACTER_CHATTER_FLAG_ADDRESS, BitConverter.GetBytes((int)0)); } // CHARMY_SHUTUP 0

        private void EnableCharacterVoiceChatter() { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)CHARACTER_STAGE_COMMENTS_BRANCH_ADDRESS, new byte[] { 0x74 } ); } // JMP if Zero/Equal
        private void DisableCharacterVoiceChatter() { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)CHARACTER_STAGE_COMMENTS_BRANCH_ADDRESS, new byte[] { 0xEB }); } // JMP
    }
}
