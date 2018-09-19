using SonicHeroes.Memory;
using System;

namespace Heroes_Sample_Mod
{
    public class Feature_Toggle_Super_Metal_Characters
    {

        /// <summary> ////////////////
        /// SUPER AND METAL CHARACTERS
        /// </summary> ///////////////

        // Variables
        bool SuperSonic_Enabled = false;
        bool SuperTails_Enabled = false;
        bool SuperKnuckles_Enabled = false;
        bool MetalCharacters_Enabled = false;

        // Getters
        public bool Get_SuperSonicEnabled() { return SuperSonic_Enabled; }
        public bool Get_SuperTailsEnabled() { return SuperTails_Enabled; }
        public bool Get_SuperKnucklesEnabled() { return SuperKnuckles_Enabled; }
        public bool Get_MetalCharactersEnabled() { return MetalCharacters_Enabled; }

        // ASM Injection
        const int MetalCharacters_Backup_Array_Address = 0x401496; byte[] MetalCharacters_Backup_Array; byte[] MetalCharacters_Replacement_Array = new byte[] { 0xEB };

        // Constant Addresses ## Super Chars
        const int SuperSonic_Backup_Array_Address_1 = 0x5CBEB9; const int SuperSonic_Backup_Array_Address_2 = 0x5CC0EA; const int SuperSonic_Backup_Array_Address_3 = 0x5CBFDF;
        const int SuperTails_Backup_Array_Address_1 = 0x5B7FDD; const int SuperTails_Backup_Array_Address_2 = 0x5B7DE9; const int SuperTails_Backup_Array_Address_3 = 0x5B7ECB;
        const int SuperKnuckles_Backup_Array_Address_1 = 0x5C1D6B; const int SuperKnuckles_Backup_Array_Address_2 = 0x5C1E52; const int SuperKnuckles_Backup_Array_Address_3 = 0x5C1E7A;

        // ASM Injections ## Super Chars
        byte[] SuperSonic_Replacement_Array_Address_1 = new byte[] { 0xC6, 0x86, 0xC2, 0x00, 0x00, 0x00, 0x01 };
        byte[] SuperSonic_Replacement_Array_Address_2 = new byte[] { 0xE8, 0xCA, 0xFD, 0xFF, 0xFF };
        byte[] SuperSonic_Replacement_Array_Address_3 = new byte[] { 0x80, 0xBE, 0xC2, 0x00, 0x00, 0x00, 0x01 };

        byte[] SuperTails_Replacement_Array_Address_1 = new byte[] { 0xE8, 0x07, 0xFE, 0xFF, 0xFF };
        byte[] SuperTails_Replacement_Array_Address_2 = new byte[] { 0xC6, 0x86, 0xC2, 0x00, 0x00, 0x00, 0x01 };
        byte[] SuperTails_Replacement_Array_Address_3 = new byte[] { 0x80, 0xBE, 0xC2, 0x00, 0x00, 0x00, 0x01 };

        byte[] SuperKnuckles_Replacement_Array_Address_1 = new byte[] { 0x80, 0xBE, 0xC2, 0x00, 0x00, 0x00, 0x01 };
        byte[] SuperKnuckles_Replacement_Array_Address_2 = new byte[] { 0xC6, 0x86, 0xC2, 0x00, 0x00, 0x00, 0x01, 0xE9, 0xB2, 0xFD, 0xFF, 0xFF };
        byte[] SuperKnuckles_Replacement_Array_Address_3 = new byte[] { 0xE8, 0xD3, 0xFF, 0xFF, 0xFF };

        // Backup
        byte[] SuperSonic_Backup_Array_1; byte[] SuperSonic_Backup_Array_2; byte[] SuperSonic_Backup_Array_3;
        byte[] SuperTails_Backup_Array_1; byte[] SuperTails_Backup_Array_2; byte[] SuperTails_Backup_Array_3;
        byte[] SuperKnuckles_Backup_Array_1; byte[] SuperKnuckles_Backup_Array_2; byte[] SuperKnuckles_Backup_Array_3;

        /// <summary>
        /// Enables the two button combination for metallic characters to be used in 1P mode.
        /// </summary>
        public void Toggle_Metal_Characters()
        {
            if (!MetalCharacters_Enabled)
            {
                // Backup Original Code & Write ASM Injections
                MetalCharacters_Backup_Array = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)MetalCharacters_Backup_Array_Address, MetalCharacters_Replacement_Array.Length);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)MetalCharacters_Backup_Array_Address, MetalCharacters_Replacement_Array);
                MetalCharacters_Enabled = true;
            }
            else
            {
                // Restore Original Code.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)MetalCharacters_Backup_Array_Address, MetalCharacters_Backup_Array);
                MetalCharacters_Enabled = false;
            }
        }

        /// <summary>
        /// Enables the ASM Injection code which should enable make Super Sonic if the current team is Team Sonic.
        /// </summary>
        public void Toggle_Super_Sonic()
        {
            if (!SuperSonic_Enabled)
            {
                // Backup Original Code
                SuperSonic_Backup_Array_1 = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)SuperSonic_Backup_Array_Address_1, SuperSonic_Replacement_Array_Address_1.Length);
                SuperSonic_Backup_Array_2 = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)SuperSonic_Backup_Array_Address_2, SuperSonic_Replacement_Array_Address_2.Length);
                SuperSonic_Backup_Array_3 = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)SuperSonic_Backup_Array_Address_3, SuperSonic_Replacement_Array_Address_3.Length);

                // Write the necessary ASM Injections
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperSonic_Backup_Array_Address_1, SuperSonic_Replacement_Array_Address_1);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperSonic_Backup_Array_Address_2, SuperSonic_Replacement_Array_Address_2);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperSonic_Backup_Array_Address_3, SuperSonic_Replacement_Array_Address_3);
                SuperSonic_Enabled = true;
            }
            else
            {
                // Restore Original Code
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperSonic_Backup_Array_Address_1, SuperSonic_Backup_Array_1);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperSonic_Backup_Array_Address_2, SuperSonic_Backup_Array_2);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperSonic_Backup_Array_Address_3, SuperSonic_Backup_Array_3);
                SuperSonic_Enabled = false;
            }
        }

        /// <summary>
        /// Enables the ASM Injection code which should enable make Super Tails if the current team is Team Sonic.
        /// </summary>
        public void Toggle_Super_Tails()
        {
            if (!SuperTails_Enabled)
            {
                // Backup Original Code
                SuperTails_Backup_Array_1 = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)SuperTails_Backup_Array_Address_1, SuperTails_Replacement_Array_Address_1.Length);
                SuperTails_Backup_Array_2 = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)SuperTails_Backup_Array_Address_2, SuperTails_Replacement_Array_Address_2.Length);
                SuperTails_Backup_Array_3 = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)SuperTails_Backup_Array_Address_3, SuperTails_Replacement_Array_Address_3.Length);

                // Write the necessary ASM Injections
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperTails_Backup_Array_Address_1, SuperTails_Replacement_Array_Address_1);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperTails_Backup_Array_Address_2, SuperTails_Replacement_Array_Address_2);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperTails_Backup_Array_Address_3, SuperTails_Replacement_Array_Address_3);
                SuperTails_Enabled = true;
            }
            else
            {
                // Restore Original Code
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperTails_Backup_Array_Address_1, SuperTails_Backup_Array_1);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperTails_Backup_Array_Address_2, SuperTails_Backup_Array_2);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperTails_Backup_Array_Address_3, SuperTails_Backup_Array_3);
                SuperTails_Enabled = false;
            }
        }

        /// <summary>
        /// Enables the ASM Injection code which should enable make Super Knuckles if the current team is Team Sonic.
        /// </summary>
        public void Toggle_Super_Knuckles()
        {
            if (!SuperKnuckles_Enabled)
            {
                // Backup Original Code
                SuperKnuckles_Backup_Array_1 = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)SuperKnuckles_Backup_Array_Address_1, SuperKnuckles_Replacement_Array_Address_1.Length);
                SuperKnuckles_Backup_Array_2 = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)SuperKnuckles_Backup_Array_Address_2, SuperKnuckles_Replacement_Array_Address_2.Length);
                SuperKnuckles_Backup_Array_3 = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)SuperKnuckles_Backup_Array_Address_3, SuperKnuckles_Replacement_Array_Address_3.Length);

                // Write the necessary ASM Injections
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperKnuckles_Backup_Array_Address_1, SuperKnuckles_Replacement_Array_Address_1);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperKnuckles_Backup_Array_Address_2, SuperKnuckles_Replacement_Array_Address_2);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperKnuckles_Backup_Array_Address_3, SuperKnuckles_Replacement_Array_Address_3);
                SuperKnuckles_Enabled = true;
            }
            else
            {
                // Restore Original Code
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperKnuckles_Backup_Array_Address_1, SuperKnuckles_Backup_Array_1);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperKnuckles_Backup_Array_Address_2, SuperKnuckles_Backup_Array_2);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SuperKnuckles_Backup_Array_Address_3, SuperKnuckles_Backup_Array_3);
                SuperKnuckles_Enabled = false;
            }
        }

    }
}
