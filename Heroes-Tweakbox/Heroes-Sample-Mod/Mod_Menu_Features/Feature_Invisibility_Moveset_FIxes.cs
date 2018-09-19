using System;
using SonicHeroes.Variables;
using SonicHeroes.Memory;
using static SonicHeroes.Networking.Client_Functions;
using System.Windows.Forms;
using SonicHeroes.Hooking;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Random note: This is the first class created after the FASM.NET assembler was integrated into the mod loader. Happy assembly!
    /// Reimplementation of Muzzarino's Cheat Engine Shenanigans into Mod Loader form.
    /// </summary>
    public class Feature_Invisbility_Fixes
    {
        // Constructor
        public Feature_Invisbility_Fixes() { }

        /// <summary>
        /// Byte which represents part of a JMP Instruction which checks whether a character can turn invisible.
        /// </summary>
        const int TRANSPARENCY_CHECK_ASM = 0x648B1C;

        /// <summary>
        /// Changes: JE > JMP (Effectively removes check and always jumps).
        /// </summary>
        const byte TRANSPARENCY_CHECK_ASM_BYTE = 0xEB;

        /// <summary>
        /// Changes: JMP > JE (Re-enables check).
        /// </summary>
        const byte TRANSPARENCY_CHECK_ASM_ORIG = 0x74;

        /// <summary>
        /// Part of jumptable address of various blending properties.
        /// </summary>
        const int CHROMA_CAMO_AMY_ASM = 0x00782A44;

        /// <summary>
        /// Disables Chroma Camo Invisibility
        /// </summary>
        const int CHROMA_CAMO_AMY_ASM_DISABLE = 0x00545AE0;

        /// <summary>
        /// Enables Chroma Camo Invisibility
        /// </summary>
        const int CHROMA_CAMO_AMY_ASM_ENABLE = 0x005D29F0;

        /// <summary>
        /// Pointer to byte responsible for setting the near jump length to override if the character is recognized when invisible. (Jumps if not Espio)
        /// </summary>
        const int INVISBILITY_VISIBLE_CHECK = 0x005E2F6D;

        /// <summary>
        /// Overrides jump length to 0, allows everyone to be unrecognizable when invisible.
        /// </summary>
        const byte INVISBILITY_VISIBLE_CHECK_ENABLE = 0;

        /// <summary>
        /// Restores Espio only.
        /// </summary>
        const byte INVISBILITY_VISIBLE_CHECK_DISABLE = 17;

        /// <summary>
        /// x86 Mnemonics which dynamically load the appropriate blending address based on the currently controlling character.
        /// </summary>
        static string[] Injection_ChromaCamo_Mnemonics = new string[]
        {
            "use32",
            "mov eax,[esi]",
            "push ebx",
            "mov ecx,esi",
            "cmp eax,0x782AF0",
            "je IsSonic",
            "cmp eax,0x782A70",
            "jne NotShadow",
            "IsSonic:",
            "test[esi + 0x000001BC],dword 0x400000",
            "je IsShadow",
            "mov eax,0x782970",
            "NotShadow:",
            "IsShadow:",
            "call dword [eax+0x54]"
        };

        /// <summary>
        /// x86 Mnemonics II which dynamically load the appropriate blending address based on the currently controlling character.
        /// </summary>
        static string[] Injection_ChromaCamoII_Mnemonic = new string[]
        {
            "use32",
            "mov eax,[esi]",
            "test[esi + 0x000001BC],dword 0x400000",
            "je visible",
            "mov ecx,[esp + 08]",
            "cmp eax, 0x782A70",
            "je shadowshoes",
            "cmp eax, 0x782AF0",
            "jne isnotsonic",

            "mov ebx, dword 0x005D2C50",
            "call ebx",

            "jmp visible",
            "shadowshoes:",

            "mov ebx, dword 0x005D3250",
            "call ebx",

            "isnotsonic:",
            "visible:",
        };

        /// <summary>
        /// Assembled version of the mnemonics above.
        /// </summary>
        static byte[] Injection_Chroma_Camo_Array_Mnemonics = Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(Injection_ChromaCamo_Mnemonics), true);

        /// <summary>
        /// Assembled version of the mnemonics above.
        /// </summary>
        static byte[] Injection_Chroma_Camo_II_Array_Mnemonics = Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(Injection_ChromaCamoII_Mnemonic), true);

        /// <summary>
        /// Are the various invisibility fixes enabled?
        /// </summary>
        bool Invisibility_Fixes_Enabled = false;

        /// <summary>
        /// The main injection hook used for the enabling of the Chroma Camo.
        /// </summary>
        ASM_Hook Injection_Chroma_Camo_Hook = new ASM_Hook((IntPtr)0x5D1FB8, Injection_Chroma_Camo_Array_Mnemonics, 8, Program.Sonic_Heroes_Networking_Client, true); // Enables/Disables Sonic/Shadow ChromaCamo

        /// <summary>
        /// Secondary injection hook for the modification of various little elements such as glowing shoes when ChromaCamo is enabled.
        /// </summary>
        ASM_Hook Injection_Chroma_Camo_Hook_II = new ASM_Hook((IntPtr)0x5D1FC0, Injection_Chroma_Camo_II_Array_Mnemonics, 7, Program.Sonic_Heroes_Networking_Client, false); // Enables/Disables Sonic/Shadow ChromaCamo

        /// <summary>
        /// Returns whether invisibility fixes are enabled. (Proper invisible glowing characters)
        /// </summary>
        /// <returns></returns>
        public bool Get_Invisibility_Fixes_Status() { return Invisibility_Fixes_Enabled; }

        /// <summary>
        /// Enables or disables the invisibility fixes for the non-espio characters.
        /// </summary>
        public void Toggle_Invisibility_Fixes()
        {
            if (Invisibility_Fixes_Enabled)
            {
                // Disable the hooks.
                Injection_Chroma_Camo_Hook.Deactivate();
                Injection_Chroma_Camo_Hook_II.Deactivate();

                // Disable Amy-specific jump table override.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)CHROMA_CAMO_AMY_ASM, BitConverter.GetBytes(CHROMA_CAMO_AMY_ASM_DISABLE));

                // Disallow anyone to turn invisible.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)TRANSPARENCY_CHECK_ASM, new byte[] { TRANSPARENCY_CHECK_ASM_ORIG });

                // Make Character visible to Enemies.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)INVISBILITY_VISIBLE_CHECK, new byte[] { INVISBILITY_VISIBLE_CHECK_DISABLE });
                Invisibility_Fixes_Enabled = false;
            }
            else
            {
                // Enable the hooks.
                Injection_Chroma_Camo_Hook.Activate();
                Injection_Chroma_Camo_Hook_II.Activate();

                // Enable Amy-specific jump table override.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)CHROMA_CAMO_AMY_ASM, BitConverter.GetBytes(CHROMA_CAMO_AMY_ASM_ENABLE));

                // Allow anyone to turn invisible.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)TRANSPARENCY_CHECK_ASM, new byte[] { TRANSPARENCY_CHECK_ASM_BYTE } );

                // Make Character Invisible to Enemies.
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)INVISBILITY_VISIBLE_CHECK, new byte[] { INVISBILITY_VISIBLE_CHECK_ENABLE });
                Invisibility_Fixes_Enabled = true;
            }
        }

    }
}
