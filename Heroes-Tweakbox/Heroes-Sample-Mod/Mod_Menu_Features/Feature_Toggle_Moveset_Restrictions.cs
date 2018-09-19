using System;
using SonicHeroes.Variables;
using SonicHeroes.Memory;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// This class provides the control over various movement restrictions that are applied upon the characters, it was written before the
    /// introduction of the FASM.NET Assembler.
    /// </summary>
    public class Feature_Toggle_Moveset_Restrictions
    {
        /// <summary>
        /// Index of current user of Tornado Leaf from the entries in Characters
        /// </summary>
        int Tornado_Leaf_Swirl_Current_Character_Index = 0;

        /// <summary>
        /// Index of current user of Tornado Hammer from the entries in Characters
        /// </summary>
        int Tornado_Hammer_Current_Character_Index = 0;

        /// <summary>
        /// Index of current user of Tornado Leaf from the entries in Characters
        /// </summary>
        int Triangle_Jump_No_Limitations_Character_Index = 0;

        /// <summary>
        /// Index of current user of Tornado Hammer from the entries in Characters
        /// </summary>
        int Triangle_Jump_Disabled_Character_Index = 0;

        /// <summary>
        /// Is Light Speed Dash Enabled?
        /// </summary>
        bool Allow_Light_Speed_Dash_Enabled = false;

        /// <summary>
        /// Is Light Speed Attack Enabled?
        /// </summary>
        bool Allow_Light_Speed_Attack_Enabled = false;

        /// <summary>
        /// Returns true if light speed attack is enabled for all characters, else false.
        /// </summary>
        public bool Get_Light_Speed_Attack_State() { return Allow_Light_Speed_Attack_Enabled; }

        /// <summary>
        /// Returns true if light speed dash is enabled for all characters, else false.
        /// </summary>
        public bool Get_Light_Speed_Dash_State() { return Allow_Light_Speed_Dash_Enabled; }

        // Constant Addresses
        const int TORNADO_LEAF_SWIRL_CHARACTER_ID_CHECK = 0x5D40F7;
        const int TORNADO_HAMMER_CHARACTER_ID_CHECK = 0x5D4256;
        const int TORNADO_JUMP_NO_LIMITATIONS_CHARACTER_ID_CHECK = 0x5D40F7;
        const int TORNADO_JUMP_DISABLED_CHARACTER_ID_CHECK = 0x5D4256;

        // ASM Constant Addresses
        const int ALLOW_LIGHT_SPEED_DASH_ADDRESS = 0x5A67C0;
        const int ALLOW_LIGHT_SPEED_ATTACK_ADDRESS = 0x5A6830;
        const int LIGHT_SPEED_ATTACK_SUPERSONIC_ADDRESS = 0x5A687C;
        const int AMY_DEFAULT_MOVESET_ADDRESS = 0x5AF714;
        const int ESPIO_DEFAULT_MOVESET_ADDRESS = 0x5AF720;

        // ASM Injections
        byte[] ASM_Allow_Light_Speed_Dash_Bytes = new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0xEB };
        byte[] ASM_Allow_Light_Speed_Attack_Bytes = new byte[] { 0xEB, 0x08, 0x90, 0x90, 0x90, 0x90 };
        byte[] ASM_Amy_Espio_Sonic_Moveset_Override = new byte[] { 0x63, 0xF0, 0x5A };
        byte[] ASM_LightSpeedAttack_SuperSonic_Check_Disable = new byte[] { 0x90, 0x90 };

        // ASM Backups
        byte[] ASM_Allow_Light_Speed_Dash_Backup;
        byte[] ASM_Allow_Light_Speed_Attack_Backup;
        byte[] ASM_Amy_Default_Moveset_Backup;
        byte[] ASM_Espio_Default_Moveset_Backup;
        byte[] ASM_LightSpeedAttack_SuperSonic_Check_Backup;

        /// <summary>
        /// Constructor, finds current leaf swirl and tornado hammer users as well as backing up the necessary jump table pointers.
        /// </summary>
        public Feature_Toggle_Moveset_Restrictions()
        {
            int Tornado_Hammer_User = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Tornado_Hammer_User_ID, 1);
            int Leaf_Swirl_User = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Leaf_Swirl_User_ID, 1);
            int Unlimited_Jump_User = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Unlimited_Triangle_Jump_User, 1);
            int Disabled_Jump_User = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Disabled_Triangle_Jump_User, 1);

            // Loop over all characters to find current technique users.
            for (int x = 0; x < Characters.Length; x++)
            {
                if (Characters[x].Character_ID == Tornado_Hammer_User) { Tornado_Hammer_Current_Character_Index = x; }
                if (Characters[x].Character_ID == Leaf_Swirl_User) { Tornado_Leaf_Swirl_Current_Character_Index = x; }
                if (Characters[x].Character_ID == Unlimited_Jump_User) { Triangle_Jump_No_Limitations_Character_Index = x; }
                if (Characters[x].Character_ID == Disabled_Jump_User) { Triangle_Jump_Disabled_Character_Index = x; }
            }

            // Back up all of the necessary bytes with character movesets.
            ASM_Allow_Light_Speed_Dash_Backup = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)ALLOW_LIGHT_SPEED_DASH_ADDRESS, ASM_Allow_Light_Speed_Dash_Bytes.Length);
            ASM_Allow_Light_Speed_Attack_Backup = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)ALLOW_LIGHT_SPEED_ATTACK_ADDRESS, ASM_Allow_Light_Speed_Attack_Bytes.Length);
            ASM_Amy_Default_Moveset_Backup = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)AMY_DEFAULT_MOVESET_ADDRESS, ASM_Amy_Espio_Sonic_Moveset_Override.Length);
            ASM_Espio_Default_Moveset_Backup = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)ESPIO_DEFAULT_MOVESET_ADDRESS, ASM_Amy_Espio_Sonic_Moveset_Override.Length);
            ASM_LightSpeedAttack_SuperSonic_Check_Backup = Program.Sonic_Heroes_Process.ReadMemory((IntPtr)LIGHT_SPEED_ATTACK_SUPERSONIC_ADDRESS, ASM_LightSpeedAttack_SuperSonic_Check_Disable.Length);
        }

        // Constant Character Pool
        Speed_Character_Moveset_Details[] Characters = new Speed_Character_Moveset_Details[]
        {
            new Speed_Character_Moveset_Details("Sonic",(int)Speed_Character_IDs.Sonic),
            new Speed_Character_Moveset_Details("Knuckles",(int)Speed_Character_IDs.Knuckles),
            new Speed_Character_Moveset_Details("Tails",(int)Speed_Character_IDs.Tails),
            new Speed_Character_Moveset_Details("Shadow",(int)Speed_Character_IDs.Shadow),
            new Speed_Character_Moveset_Details("Omega",(int)Speed_Character_IDs.Omega),
            new Speed_Character_Moveset_Details("Rouge",(int)Speed_Character_IDs.Rouge),
            new Speed_Character_Moveset_Details("Amy",(int)Speed_Character_IDs.Amy),
            new Speed_Character_Moveset_Details("Big",(int)Speed_Character_IDs.Big),
            new Speed_Character_Moveset_Details("Cream",(int)Speed_Character_IDs.Cream),
            new Speed_Character_Moveset_Details("Espio",(int)Speed_Character_IDs.Espio),
            new Speed_Character_Moveset_Details("Vector",(int)Speed_Character_IDs.Vector),
            new Speed_Character_Moveset_Details("Charmy",(int)Speed_Character_IDs.Charmy),
            new Speed_Character_Moveset_Details("Null",(int)69),
        }; // List of characters from which character IDs can be read from.

        /// <summary> 
        /// A list of character IDs to use for tornado move checking.
        /// </summary> 
        private enum Speed_Character_IDs
        {
            Sonic = 0x0,
            Knuckles = 0x1,
            Tails = 0x2,
            Shadow = 0x3,
            Omega = 0x4,
            Rouge = 0x5,
            Amy = 0x6,
            Big = 0x7,
            Cream = 0x8,
            Espio = 0x9,
            Vector = 0xA,
            Charmy = 0xB
        }

        /// <summary>
        /// Defines a move to be performed by a speed character.
        /// </summary>
        public struct Speed_Character_Moveset_Details
        {
            // Variables
            public string Character_Name;
            public byte Character_ID;

            // Constructor.
            public Speed_Character_Moveset_Details(string Character_Name_X, byte Character_ID_X) { Character_Name = Character_Name_X; Character_ID = Character_ID_X; }
        }

        /// <summary>
        /// Increments and sets the current Tornado Leaf user.
        /// </summary>
        public void Increment_Tornado_Leaf_Swirl_User()
        { 
            if (Tornado_Leaf_Swirl_Current_Character_Index < Characters.Length - 1) { Tornado_Leaf_Swirl_Current_Character_Index = Tornado_Leaf_Swirl_Current_Character_Index + 1; }
            else { Tornado_Leaf_Swirl_Current_Character_Index = 0; }

            // Write Swirl user to Memory.
            byte Character_ID = Convert.ToByte(Characters[Tornado_Leaf_Swirl_Current_Character_Index].Character_ID);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Leaf_Swirl_User_ID, new byte[] { Character_ID });
        }

        /// <summary>
        /// Decrements and sets the current Tornado Leaf user.
        /// </summary>
        public void Decrement_Tornado_Leaf_Swirl_User()
        {
            if (Tornado_Leaf_Swirl_Current_Character_Index > 0) { Tornado_Leaf_Swirl_Current_Character_Index = Tornado_Leaf_Swirl_Current_Character_Index - 1; }
            else { Tornado_Leaf_Swirl_Current_Character_Index = (byte)(Characters.Length - 1); }

            // Write Swirl user to Memory.
            byte Character_ID = Convert.ToByte(Characters[Tornado_Leaf_Swirl_Current_Character_Index].Character_ID);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Leaf_Swirl_User_ID, new byte[] { Character_ID });
        }

        /// <summary>
        /// Increments and sets the current user which has disabled triangle jump.
        /// </summary>
        public void Increment_No_Triangle_Jump_User()
        {
            if (Triangle_Jump_Disabled_Character_Index < Characters.Length - 1) { Triangle_Jump_Disabled_Character_Index = Triangle_Jump_Disabled_Character_Index + 1; }
            else { Triangle_Jump_Disabled_Character_Index = 0; }

            // Write Swirl user to Memory.
            byte Character_ID = Convert.ToByte(Characters[Triangle_Jump_Disabled_Character_Index].Character_ID);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Disabled_Triangle_Jump_User, new byte[] { Character_ID });
        }

        /// <summary>
        /// Decrements and sets the current user which has disabled triangle jump.
        /// </summary>
        public void Decrement_No_Triangle_Jump_User()
        {
            if (Triangle_Jump_Disabled_Character_Index > 0) { Triangle_Jump_Disabled_Character_Index = Triangle_Jump_Disabled_Character_Index - 1; }
            else { Triangle_Jump_Disabled_Character_Index = (byte)(Characters.Length - 1); }

            // Write Swirl user to Memory.
            byte Character_ID = Convert.ToByte(Characters[Triangle_Jump_Disabled_Character_Index].Character_ID);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Disabled_Triangle_Jump_User, new byte[] { Character_ID });
        }

        /// <summary>
        /// Increments and sets the current user whom has unlimited time wall clinging during triangle jump. (Default:Espio)
        /// </summary>
        public void Increment_Unlimited_Triangle_Jump_User()
        {
            if (Triangle_Jump_No_Limitations_Character_Index < Characters.Length - 1) { Triangle_Jump_No_Limitations_Character_Index = Triangle_Jump_No_Limitations_Character_Index + 1; }
            else { Triangle_Jump_No_Limitations_Character_Index = 0; }

            // Write Swirl user to Memory.
            byte Character_ID = Convert.ToByte(Characters[Triangle_Jump_No_Limitations_Character_Index].Character_ID);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Unlimited_Triangle_Jump_User, new byte[] { Character_ID });
        }

        /// <summary>
        /// Decrements and sets the current user whom has unlimited time wall clinging during triangle jump. (Default:Espio)
        /// </summary>
        public void Decrement_Unlimited_Triangle_Jump_User()
        {
            if (Triangle_Jump_No_Limitations_Character_Index > 0) { Triangle_Jump_No_Limitations_Character_Index = Triangle_Jump_No_Limitations_Character_Index - 1; }
            else { Triangle_Jump_No_Limitations_Character_Index = (byte)(Characters.Length - 1); }

            // Write Swirl user to Memory.
            byte Character_ID = Convert.ToByte(Characters[Triangle_Jump_No_Limitations_Character_Index].Character_ID);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Unlimited_Triangle_Jump_User, new byte[] { Character_ID });
        }

        /// <summary>
        /// Increments and sets the current Tornado Hammer user.
        /// </summary>
        public void Increment_Tornado_Hammer_User()
        {
            if (Tornado_Hammer_Current_Character_Index < Characters.Length - 1) { Tornado_Hammer_Current_Character_Index = Tornado_Hammer_Current_Character_Index + 1; }
            else { Tornado_Hammer_Current_Character_Index = 0; }

            // Write Hammer user to Memory.
            byte Character_ID = Convert.ToByte(Characters[Tornado_Hammer_Current_Character_Index].Character_ID);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Tornado_Hammer_User_ID, new byte[] { Character_ID });
        }

        /// <summary>
        /// Decrements and sets the current Tornado Hammer user.
        /// </summary>
        public void Decrement_Tornado_Hammer_User()
        {
            if (Tornado_Hammer_Current_Character_Index > 0) { Tornado_Hammer_Current_Character_Index = Tornado_Hammer_Current_Character_Index - 1; }
            else { Tornado_Hammer_Current_Character_Index = (byte)(Characters.Length - 1); }

            // Write Hammer user to Memory.
            byte Character_ID = Convert.ToByte(Characters[Tornado_Hammer_Current_Character_Index].Character_ID);
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Movement_Restrictions_Users.Tornado_Hammer_User_ID, new byte[] { Character_ID });
        }

        /// <summary>
        /// Toggles the light speed dash enabler for all Speed Characters.
        /// </summary>
        public void Toggle_Light_Speed_Dash()
        {
            if (Allow_Light_Speed_Dash_Enabled)
            {
                if (!Allow_Light_Speed_Attack_Enabled)
                {
                    Program.Sonic_Heroes_Process.WriteMemory((IntPtr)AMY_DEFAULT_MOVESET_ADDRESS, ASM_Amy_Default_Moveset_Backup);
                    Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ESPIO_DEFAULT_MOVESET_ADDRESS, ASM_Espio_Default_Moveset_Backup);
                }

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ALLOW_LIGHT_SPEED_DASH_ADDRESS, ASM_Allow_Light_Speed_Dash_Backup);
                Allow_Light_Speed_Dash_Enabled = false;
            }
            else
            {
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)AMY_DEFAULT_MOVESET_ADDRESS, ASM_Amy_Espio_Sonic_Moveset_Override);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ESPIO_DEFAULT_MOVESET_ADDRESS, ASM_Amy_Espio_Sonic_Moveset_Override);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ALLOW_LIGHT_SPEED_DASH_ADDRESS, ASM_Allow_Light_Speed_Dash_Bytes);

                Allow_Light_Speed_Dash_Enabled = true;
            }
        }

        /// <summary>
        /// Toggles the light speed attack enabler for all Speed Characters.
        /// </summary>
        public void Toggle_Light_Speed_Attack()
        {
            if (Allow_Light_Speed_Attack_Enabled)
            {
                if (!Allow_Light_Speed_Dash_Enabled)
                {
                    Program.Sonic_Heroes_Process.WriteMemory((IntPtr)AMY_DEFAULT_MOVESET_ADDRESS, ASM_Amy_Default_Moveset_Backup);
                    Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ESPIO_DEFAULT_MOVESET_ADDRESS, ASM_Espio_Default_Moveset_Backup);
                }

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ALLOW_LIGHT_SPEED_ATTACK_ADDRESS, ASM_Allow_Light_Speed_Attack_Backup);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)LIGHT_SPEED_ATTACK_SUPERSONIC_ADDRESS, ASM_LightSpeedAttack_SuperSonic_Check_Backup);
                Allow_Light_Speed_Attack_Enabled = false;
            }
            else
            {
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)AMY_DEFAULT_MOVESET_ADDRESS, ASM_Amy_Espio_Sonic_Moveset_Override);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ESPIO_DEFAULT_MOVESET_ADDRESS, ASM_Amy_Espio_Sonic_Moveset_Override);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)ALLOW_LIGHT_SPEED_ATTACK_ADDRESS, ASM_Allow_Light_Speed_Attack_Bytes);
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)LIGHT_SPEED_ATTACK_SUPERSONIC_ADDRESS, ASM_LightSpeedAttack_SuperSonic_Check_Disable);

                Allow_Light_Speed_Attack_Enabled = true;
            }
        }

        /// <summary>
        /// Retrieves the current user details of the user of the Leaf Swirl technique (Default: Espio).
        /// </summary>
        /// <returns></returns>
        public Speed_Character_Moveset_Details Get_Current_Tornado_Leaf_Swirl_User() { return Characters[Tornado_Leaf_Swirl_Current_Character_Index]; }

        /// <summary>
        /// Retrieves the current user details of the user of the Tornado Hammer technique (Default: Amy).
        /// </summary>
        /// <returns></returns>
        public Speed_Character_Moveset_Details Get_Current_Tornado_Hammer_User() { return Characters[Tornado_Hammer_Current_Character_Index]; }

        /// <summary>
        /// Retrieves the current user details of the user of the Leaf Swirl technique (Default: Espio).
        /// </summary>
        /// <returns></returns>
        public Speed_Character_Moveset_Details Get_Current_Unlimited_Triangle_Jump_User() { return Characters[Triangle_Jump_No_Limitations_Character_Index]; }

        /// <summary>
        /// Retrieves the current user details of the user of the Tornado Hammer technique (Default: Amy).
        /// </summary>
        /// <returns></returns>
        public Speed_Character_Moveset_Details Get_Current_No_Triangle_Jump_User() { return Characters[Triangle_Jump_Disabled_Character_Index]; }
    }
}
