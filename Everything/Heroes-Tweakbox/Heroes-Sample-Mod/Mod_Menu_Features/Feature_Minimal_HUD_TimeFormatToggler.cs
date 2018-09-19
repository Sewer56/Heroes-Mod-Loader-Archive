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
    public class Feature_Minimal_HUD_TimeFormatToggler
    {
        // Time Format
        const int NewTimeFormat_Address = 0x41E427; // Address for overwriting.
        int NewTimeFormatAddress; // Stores the location of the new time format.
        byte[] NewTimeFormat_ASM; // ASM Push Instruction for new time.
        byte[] NewTimeFormat_ASM_Backup = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)NewTimeFormat_Address, 5); // Original ASM Push Instruction for new time.

        /// <summary>
        /// Constructor 
        /// </summary>
        public Feature_Minimal_HUD_TimeFormatToggler()
        {
            // Get bytes for new string format and write to memory.
            byte[] bytes = Encoding.ASCII.GetBytes("h%03d");
            NewTimeFormatAddress = (int)Program.Sonic_Heroes_Process.AllocateMemory(bytes.Length);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)NewTimeFormatAddress, bytes);

            // Assemble code for push from new time string format address.
            string[] Push_Format_String = new string[]
            {
                "use32",
                "push " + NewTimeFormatAddress,
            };
            NewTimeFormat_ASM = Program.Sonic_Heroes_Networking_Client.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(Push_Format_String), true);

        }

        /// <summary>
        /// Toggles the Minimal HUD on and off.
        /// </summary>
        public void Disable_TimeFormat() { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)NewTimeFormat_Address, NewTimeFormat_ASM); }

        /// <summary>
        /// Toggles the Minimal HUD on and off.
        /// </summary>
        public void Enable_TimeFormat() { Program.Sonic_Heroes_Process.WriteMemory((IntPtr)NewTimeFormat_Address, NewTimeFormat_ASM_Backup); }
    }
}
