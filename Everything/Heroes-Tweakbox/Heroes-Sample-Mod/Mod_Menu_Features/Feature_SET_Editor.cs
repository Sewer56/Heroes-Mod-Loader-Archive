using SonicHeroes.Memory;
using System;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Enables the built-in SET editor for Sonic Heroes, method of controlling this at the moment is unknown.
    /// </summary>
    public class Feature_SET_Editor
    {
        // Addresses
        const int SET_Editor_Enable_Instruction_Flag = 0x43C960;

        // Toggle
        bool SET_Editor_Enabled = false;

        /// Overwrites the instruction which decides whether the SET Editor is enabled or not.
        byte[] SET_Editor_Instruction_Overwrite = new byte[] { 0xC7, 0x80, 0x84, 0x0A, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
        byte[] SET_Editor_Instruction_Original = new byte[] { 0xC7, 0x80, 0x84, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// Enables/Disables the SET Editor.
        /// </summary>
        public void SET_Editor_Toggle()
        {
            if (!SET_Editor_Enabled)
            {
                // Write the default.
                int Write_Address_Overlay_State = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)0x00A77848, 4) + 0xA84;
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)Write_Address_Overlay_State, BitConverter.GetBytes(1));

                // Pointer + Offset
                int Write_Address = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)0xA777E4, 4) + 5;
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)Write_Address, new byte[] { 0x01 }); // Write On Screen Display Enabler.

                // Write the instruction overwrite to set the overlay toggle state to 1.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SET_Editor_Enable_Instruction_Flag, SET_Editor_Instruction_Overwrite);

                // SET Editor Enable State
                SET_Editor_Enabled = true;
            }
            else
            {
                // Write the default.
                int Write_Address_Overlay_State = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)0x00A77848, 4) + 0xA84;
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)Write_Address_Overlay_State, BitConverter.GetBytes(0));

                // Pointer + Offset
                int Write_Address = Program.Sonic_Heroes_Process.ReadMemory<int>((IntPtr)0xA777E4, 4) + 5;
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)Write_Address, new byte[] { 0x00 }); // Write On Screen Display Disabler.

                // Write the instruction overwrite to set the overlay toggle state to 0.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SET_Editor_Enable_Instruction_Flag, SET_Editor_Instruction_Original);

                // SET Editor Enable State
                SET_Editor_Enabled = false;
            }
            
        }
    }
}
