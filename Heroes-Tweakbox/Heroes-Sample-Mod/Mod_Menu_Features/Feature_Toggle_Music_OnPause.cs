using SonicHeroes.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Controls whether the music continues playing or not whenever the game is paused.
    /// </summary>
    public class Feature_Toggle_Music_OnPause
    {
        /// <summary>
        /// Stores the original bytes which stop the music playback when the player pauses the game.
        /// </summary>
        byte[] OriginalBytes;

        /// <summary>
        /// Is the disabling of the Music pausing enabled?
        /// </summary>
        public bool Enabled = false; // True if function disabled.

        /// <summary>
        /// Address in memory of instruction which pauses the music playback, 5 bytes.
        /// </summary>
        int MUSIC_PAUSE_INSTRUCTION_ADDRESS = 0x006C76F9;

        /// <summary>
        /// Back up the old opcodes upon class instantiation.
        /// </summary>
        public Feature_Toggle_Music_OnPause() { OriginalBytes = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)MUSIC_PAUSE_INSTRUCTION_ADDRESS, 5); }

        /// <summary>
        /// Toggles whether music is paused or not when game is paused.
        /// </summary>
        public void Toggle_Music_OnPause()
        {
            if (Enabled)
            {
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)MUSIC_PAUSE_INSTRUCTION_ADDRESS, OriginalBytes);
                Enabled = false;
            }
            else
            {
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)MUSIC_PAUSE_INSTRUCTION_ADDRESS, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 } ); // NOP
                Enabled = true;
            }
        }
    }
}
