using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SonicHeroes.Networking.Client_Functions;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Toggles the Bobsled Text On/Off for MinimalHUD
    /// </summary>
    public class Feature_Minimal_HUD_BobsledToggler
    {
        // Disabling Bobsled HUD Text - Overwrite with NOP
        const int ADDRESS_RENDER_BOBSLED_TARGET_ON_HUD_5BYTES = 0x406A3A;
        const int ADDRESS_RENDER_BOBSLED_CURRENTTIME_ON_HUD_5BYTES = 0x406B0A;
        const int ADDRESS_RENDER_BOBSLED_BEST_ON_HUD_5BYTES = 0x4069E4;
        const int ADDRESS_RENDER_BOBSLED_LASTTIME_ON_HUD_5BYTES = 0x406ACA;
        const int ADDRESS_RENDER_BOBSLED_EXIT_ON_HUD_5BYTES = 0x4069E4;
        byte[] OriginalBobsledTargetHUDBytes = Program.Sonic_Heroes_Process.ReadMemory((IntPtr) ADDRESS_RENDER_BOBSLED_TARGET_ON_HUD_5BYTES, 5);
        byte[] OriginalBobsledCurrentTimeHUDBytes = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)ADDRESS_RENDER_BOBSLED_CURRENTTIME_ON_HUD_5BYTES, 5);
        byte[] OriginalBobsledBestONHUDBytes = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)ADDRESS_RENDER_BOBSLED_BEST_ON_HUD_5BYTES, 5);
        byte[] OriginalBobsledLastTimeHUDBytes = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)ADDRESS_RENDER_BOBSLED_LASTTIME_ON_HUD_5BYTES, 5);
        byte[] OriginalBobsledExitOnHUDBytes = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)ADDRESS_RENDER_BOBSLED_EXIT_ON_HUD_5BYTES, 5);

        /// <summary>
        /// Enables the Minimal HUD
        /// </summary>
        public void Disable_Bobsled()
        {
            // Disable Bobsled Text
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_BOBSLED_TARGET_ON_HUD_5BYTES, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); 
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_BOBSLED_CURRENTTIME_ON_HUD_5BYTES, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); 
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_BOBSLED_BEST_ON_HUD_5BYTES, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); 
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_BOBSLED_LASTTIME_ON_HUD_5BYTES, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); 
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_BOBSLED_EXIT_ON_HUD_5BYTES, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 });
        }

        /// <summary>
        /// Disables the Minimal HUD
        /// </summary>
        public void Enable_Bobsled()
        {
            // Enable Bobsled Text
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_BOBSLED_TARGET_ON_HUD_5BYTES, OriginalBobsledTargetHUDBytes);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_BOBSLED_CURRENTTIME_ON_HUD_5BYTES, OriginalBobsledCurrentTimeHUDBytes);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_BOBSLED_BEST_ON_HUD_5BYTES, OriginalBobsledBestONHUDBytes);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_BOBSLED_LASTTIME_ON_HUD_5BYTES, OriginalBobsledLastTimeHUDBytes);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_BOBSLED_EXIT_ON_HUD_5BYTES, OriginalBobsledExitOnHUDBytes);
        }
    }
}
