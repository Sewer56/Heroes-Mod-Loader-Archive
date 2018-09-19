using SonicHeroes.Hooking;
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
    /// Allows for the replacement of the normal game's HUD with the 
    /// </summary>
    public class Feature_Minimal_HUD_MaestroToggler
    {
        // Disabling Rendering of HUD Maestros
        const int ADDRESS_RENDER_MAESTRO_ON_HUD_5BYTES = 0x41E239;
        static byte[] OriginalMaestroHUDBytes; 

        // Maestro Enter & Exit Injection
        SonicHeroes.Hooking.Injection MenuEnterInjection = new SonicHeroes.Hooking.Injection((IntPtr)0x402985, (ToggleMaestroDelegate)MaestroEnable, 7, Program.Sonic_Heroes_Networking_Client, false);
        SonicHeroes.Hooking.Injection MenuExitInjection = new SonicHeroes.Hooking.Injection((IntPtr)0x402A96, (ToggleMaestroDelegate)MaestroDisable, 7, Program.Sonic_Heroes_Networking_Client, false);

        // Delegate for toggling maestros.
        delegate void ToggleMaestroDelegate();

        /// <summary>
        /// Enable Maestro Handler
        /// </summary>
        public void Disable_Maestro()
        {
            OriginalMaestroHUDBytes = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)ADDRESS_RENDER_MAESTRO_ON_HUD_5BYTES, 5); // Original Render Maestro Bytes

            // Auto Disable Maestro if not paused.
            byte Is_Currently_Paused = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_CurrentState.IsIngamePauseMenuOpen, 1);
            byte Is_Currently_InLevel = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);

            // If the player is in a stage.
            if (Is_Currently_Paused == 0 && Is_Currently_InLevel == 1) { MaestroDisable(); }

            // Activate Injections
            MenuEnterInjection.Activate();
            MenuExitInjection.Activate();
        }

        /// <summary>
        /// Disable Maestro Handler
        /// </summary>
        public void Enable_Maestro()
        {
            // Deactivate Injections
            MenuEnterInjection.Deactivate();
            MenuExitInjection.Deactivate();

            // Restore Maestro
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_MAESTRO_ON_HUD_5BYTES, OriginalMaestroHUDBytes);
        }

        // Disable/Enable
        private static void MaestroEnable() { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_MAESTRO_ON_HUD_5BYTES, OriginalMaestroHUDBytes); }
        private static void MaestroDisable() { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ADDRESS_RENDER_MAESTRO_ON_HUD_5BYTES, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }); }

        // Quick Hook
        delegate void MaestroDisableDelegate();
    }
}
