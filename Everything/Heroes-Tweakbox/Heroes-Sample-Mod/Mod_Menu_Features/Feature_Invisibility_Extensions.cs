using SonicHeroes.Hooking;
using SonicHeroes.Memory;
using System;
using static SonicHeroes.Networking.Client_Functions;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Various sets of tweaks regarding invisibility.
    /// Another reimplementation of Muzzarino's cheat table.
    /// </summary>
    class Feature_Invisibility_Tweaks
    {
        // Constructor
        public Feature_Invisibility_Tweaks() { }

        /// <summary>
        /// The address storing the starting status of player when starting stage (Visible/Invisible).
        /// </summary>
        const int RESPAWN_INVISIBILITY_STATUS = 0x5ABC6F;

        /// <summary>
        /// Address for the injection which performs, Tornado Hammer = Invisibility
        /// </summary>
        const int TORNADO_HAMMER_INJECTION_ADDRESS = 0x5D426D;

        /// <summary>
        /// Address for the injection which performs, Tornado Spin = Invisibility
        /// </summary>
        const int TORNADO_SPIN_INJECTION_ADDRESS = 0x5D4281;

        /// <summary>
        /// If the RESPAWN_INVISIBILITY_STATUS is set to this value, player spawns visible.
        /// </summary>
        const byte RESPAWN_INVISIBILITY_STATUS_VISIBLE = 0x0;

        /// <summary>
        /// If the RESPAWN_INVISIBILITY_STATUS is set to this value, player spawns invisible.
        /// </summary>
        const byte RESPAWN_INVISIBILITY_STATUS_INVISIBLE = 0x64;

        /// <summary>
        /// Mnemonics for injection of "Tornado Hammer Enables Invisibility"
        /// </summary>
        static string[] Injection_Tornado_Hammer_Enables_Invisibility_Mnemonics_String = new string[]
        {
            "use32",
            "test ecx,dword 0x400000",
            "jne invisible",
            "or ecx,dword 0x00400000",
            "jmp invisiblestatefinish",
            "invisible:",
            "and ecx,dword 0xFFBFFFFF",
            "invisiblestatefinish:",
            "mov [ebp+0x000001BC],ecx",

            "mov ebx, dword 0x00578FF0",
            "call ebx",
        };

        /// <summary>
        /// Mnemonics for injection of "Tornado Spin Enables Invisibility"
        /// </summary>
        static string[] Injection_Tornado_Spin_Enables_Invisibility_Mnemonics_String = new string[]
        {
            "use32",
            "or ecx,0x00000500",
            "test ecx,dword 0x400000",
            "jne invisible",
            "or ecx,dword 0x00400000",
            "jmp invisiblestatefinish",
            "invisible:",
            "and ecx,dword 0xFFBFFFFF",
            "invisiblestatefinish:"
        };

        /// <summary>
        /// Assembled machine code for the mnemonics of "Tornado Hammer Enables Invisibility".
        /// </summary>
        static byte[] Injection_Tornado_Hammer_Enables_Invisibility_Array_Mnemonics = Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(Injection_Tornado_Hammer_Enables_Invisibility_Mnemonics_String), true);

        /// <summary>
        /// Assembled machine code for the mnemonics of "Tornado Spin Enables Invisibility".
        /// </summary>
        static byte[] Injection_Tornado_Spin_Enables_Invisibility_Array_Mnemonics = Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(Injection_Tornado_Spin_Enables_Invisibility_Mnemonics_String), true);

        // Hooks
        ASM_Hook Injection_Tornado_Hammer_Enables_Invisibility_Hook = new ASM_Hook((IntPtr)0x5D426D, Injection_Tornado_Hammer_Enables_Invisibility_Array_Mnemonics, 15, Program.Sonic_Heroes_Networking_Client, true);
        ASM_Hook Injection_Tornado_Spin_Enables_Invisibility_Hook = new ASM_Hook((IntPtr)0x5D4281, Injection_Tornado_Spin_Enables_Invisibility_Array_Mnemonics, 6, Program.Sonic_Heroes_Networking_Client, true);

        // Flags
        bool Tornado_Hammer_Enables_Invisiblity_Flag = false;
        bool Tornado_Spin_Enables_Invisiblity_Flag = false;
        bool Player_Starts_Invisible_Flag = false;

        /// <summary>
        /// Enables or disables "Player Starts/Spawns off Invisible"
        /// </summary>
        public void Toggle_Player_Starts_Invisible()
        {
            if (Player_Starts_Invisible_Flag)
            {
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)RESPAWN_INVISIBILITY_STATUS, new byte[] { RESPAWN_INVISIBILITY_STATUS_VISIBLE });
                Player_Starts_Invisible_Flag = false;
            }
            else
            {
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)RESPAWN_INVISIBILITY_STATUS, new byte[] { RESPAWN_INVISIBILITY_STATUS_INVISIBLE });
                Player_Starts_Invisible_Flag = true;
            }
        }

        /// <summary>
        /// Returns whether "Tornado Hammer Enables Invisibility".
        /// </summary>
        /// <returns></returns>
        public bool Get_Player_Starts_Invisible() { return Player_Starts_Invisible_Flag; }

        /// <summary>
        /// Enables or disables "Tornado Hammer Enables Invisibility"
        /// </summary>
        public void Toggle_Hammer_Enables_Invisiblity()
        {
            if (Tornado_Hammer_Enables_Invisiblity_Flag)
            {
                Injection_Tornado_Hammer_Enables_Invisibility_Hook.Deactivate();
                Tornado_Hammer_Enables_Invisiblity_Flag = false;
            }
            else
            {
                Injection_Tornado_Hammer_Enables_Invisibility_Hook.Activate();
                Tornado_Hammer_Enables_Invisiblity_Flag = true;
            }
        }

        /// <summary>
        /// Returns whether "Tornado Hammer Enables Invisibility".
        /// </summary>
        /// <returns></returns>
        public bool Get_Hammer_Enables_Invisiblity() { return Tornado_Hammer_Enables_Invisiblity_Flag; }

        /// <summary>
        /// Enables or disables "Tornado Hammer Enables Invisibility"
        /// </summary>
        public void Toggle_Spin_Enables_Invisiblity()
        {
            if (Tornado_Spin_Enables_Invisiblity_Flag)
            {
                Injection_Tornado_Spin_Enables_Invisibility_Hook.Deactivate();
                Tornado_Spin_Enables_Invisiblity_Flag = false;
            }
            else
            {
                Injection_Tornado_Spin_Enables_Invisibility_Hook.Activate();
                Tornado_Spin_Enables_Invisiblity_Flag = true;
            }
        }

        /// <summary>
        /// Returns whether "Tornado Hammer Enables Invisibility".
        /// </summary>
        /// <returns></returns>
        public bool Get_Spin_Enables_Invisiblity() { return Tornado_Spin_Enables_Invisiblity_Flag; }
    }
}
