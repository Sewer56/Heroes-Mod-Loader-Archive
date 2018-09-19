using SonicHeroes.Memory;
using System;
using System.Collections.Generic;

namespace Heroes_Sample_Mod
{
    public class Feature_Physics_Swap
    {
        /// <summary>
        /// Costant which defines the start of the physics entry table within game memory.
        /// </summary>
        const int PHYSICS_ENTRY_ADDRESS = 0x008BE550; 
        
        /// <summary>
        /// Represents the amount of entries for physics in Sonic Heroes.
        /// </summary>
        const int Physics_Entry_Count = 0x12;

        /// <summary>
        /// Represents the length of each unique physics entry within the game memory.
        /// </summary>
        const int Physics_Entry_Length = 0x84;

        /// <summary>
        /// Abstract an index by using an object, this allows us to set a to b by reference.
        /// </summary>
        public class Index_Abstraction_Class { public int Index { get; set; } public Index_Abstraction_Class(int Index_X) { Index = Index_X; } }

        /// <summary>
        /// Enumberables for specifying the target of the physics swapper.
        /// </summary>
        public enum Characters_Heroes
        {
            Sonic,
            Knuckles,
            Tails,
            Shadow,
            Omega,
            Rouge,
            Amy,
            Big,
            Cream,
            Espio,
            Vector,
            Charmy
        }

        // List of Physics Entry Indexes for each character.
        Index_Abstraction_Class Character_Sonic_Physics_Entry_Index = new Index_Abstraction_Class(0);
        Index_Abstraction_Class Character_Knuckles_Physics_Entry_Index = new Index_Abstraction_Class(1);
        Index_Abstraction_Class Character_Tails_Physics_Entry_Index = new Index_Abstraction_Class(2);

        Index_Abstraction_Class Character_Shadow_Physics_Entry_Index = new Index_Abstraction_Class(3);
        Index_Abstraction_Class Character_Omega_Physics_Entry_Index = new Index_Abstraction_Class(4);
        Index_Abstraction_Class Character_Rouge_Physics_Entry_Index = new Index_Abstraction_Class(5);

        Index_Abstraction_Class Character_Amy_Physics_Entry_Index = new Index_Abstraction_Class(6);
        Index_Abstraction_Class Character_Big_Physics_Entry_Index = new Index_Abstraction_Class(7);
        Index_Abstraction_Class Character_Cream_Physics_Entry_Index = new Index_Abstraction_Class(8);

        Index_Abstraction_Class Character_Espio_Physics_Entry_Index = new Index_Abstraction_Class(9);
        Index_Abstraction_Class Character_Vector_Physics_Entry_Index = new Index_Abstraction_Class(10);
        Index_Abstraction_Class Character_Charmy_Physics_Entry_Index = new Index_Abstraction_Class(11);

        // List of all physics entries.
        List<Character_Physics_SonicHeroes> Characters_Physics_All = new List<Character_Physics_SonicHeroes>();

        /// <summary>
        /// Constructor, populates all of the physics profiles.
        /// </summary>
        public Feature_Physics_Swap()
        {
            // Add every physics entry onto list.
            Populate_Physics_Entries();
        }

        /// <summary>
        /// Increments the currently selected character physics index.
        /// </summary>
        public void Increment_Current_Physics_Character(Characters_Heroes Character)
        {
            // Creates an instance of the Index Class with Character Index.
            Index_Abstraction_Class Character_Index = Get_Character_Index(Character);

            // Cycling Menu Item Switching.
            if (Character_Index.Index < Characters_Physics_All.Count - 1) { Character_Index.Index = Character_Index.Index + 1; }
            else { Character_Index.Index = 0; }

            // Calculate Character Physics Write Address 
            int Write_Address = PHYSICS_ENTRY_ADDRESS + (Physics_Entry_Length * Get_Character_Address_Entry(Character));

            // Writes the character's physics values.
            Write_Physics_Values(Write_Address, Character_Index.Index); 
        }

        /// <summary>
        /// Decrements the currently selected character physics index.
        /// </summary>
        public void Decrement_Current_Physics_Character(Characters_Heroes Character)
        {
            // Creates an instance of the Index Class with Character Index.
            Index_Abstraction_Class Character_Index = Get_Character_Index(Character);

            // Cycling Menu Item Switching.
            if (Character_Index.Index > 0) { Character_Index.Index = Character_Index.Index - 1; }
            else { Character_Index.Index = (byte)(Characters_Physics_All.Count - 1); }

            // Calculate Character Physics Write Address
            int Write_Address = PHYSICS_ENTRY_ADDRESS + (Physics_Entry_Length * Get_Character_Address_Entry(Character));

            // Writes the character's physics values.
            Write_Physics_Values(Write_Address, Character_Index.Index);
        }

        /// <summary>
        /// Retrieves the name of the currently assigned character to an index slot.
        /// </summary>
        public string Get_Current_Character_Assignment_Name(Characters_Heroes Character)
        {
            Index_Abstraction_Class Character_Index = Get_Character_Index(Character);
            return Characters_Physics_All[Character_Index.Index].Physics_Name;
        }

        /// <summary>
        /// Writes the physics properties to the currently set character.
        /// </summary>
        private void Write_Physics_Values(int Physics_Entry_Address, int Character_Index)
        {
            Character_Physics_SonicHeroes Character_To_Write = Characters_Physics_All[Character_Index];

            // Arrange a list of bytes.
            List<byte> Physics_Data_To_Write = new List<byte>(Physics_Entry_Length);

            // Append bytes to list of bytes.
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Hang_Time));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Floor_Grip));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Horizontal_Speed_Cap));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Vertical_Speed_Cap));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Unknown_Accel_Related));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Unknown_I));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Initial_Jump_Speed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Spring_Control));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Unknown_II));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Rolling_Minimum_Speed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Rolling_End_Speed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Action_I_Speed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Min_Wall_Hit_Knockback_Speed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Action_II_Speed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Jump_Hold_AddSpeed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Ground_Starting_Acceleration));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Air_Acceleration));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Ground_Deceleration));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Brake_Speed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Air_Brake_Speed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Air_Deceleration));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Rolling_Deceleration));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Gravity_Offset_Speed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Mid_Air_Swerve_Speed));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Min_Speed_Before_Stopping));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Constant_Speed_Offset));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Unknown_Off_Road));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Unknown_III));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Unknown_IV));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Collision_Size));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Gravitational_Pull));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Y_Offset_Camera));
            Physics_Data_To_Write.AddRange(BitConverter.GetBytes(Character_To_Write.Y_Offset));

            // Write to memory.
            Program.Sonic_Heroes_Process.WriteMemory((IntPtr)Physics_Entry_Address, Physics_Data_To_Write.ToArray());
        }

        /// <summary>
        /// Retrieves a character index object.
        /// </summary>
        /// <param name="Character"></param>
        /// <returns></returns>
        private Index_Abstraction_Class Get_Character_Index(Characters_Heroes Character)
        {
            Index_Abstraction_Class Character_Index = new Index_Abstraction_Class(-1);

            // Get Index Object of Character.
            switch (Character)
            {
                case Characters_Heroes.Sonic:
                    Character_Index = Character_Sonic_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Knuckles:
                    Character_Index = Character_Knuckles_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Tails:
                    Character_Index = Character_Tails_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Shadow:
                    Character_Index = Character_Shadow_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Omega:
                    Character_Index = Character_Omega_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Rouge:
                    Character_Index = Character_Rouge_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Amy:
                    Character_Index = Character_Amy_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Big:
                    Character_Index = Character_Big_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Cream:
                    Character_Index = Character_Cream_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Espio:
                    Character_Index = Character_Espio_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Vector:
                    Character_Index = Character_Vector_Physics_Entry_Index;
                    break;

                case Characters_Heroes.Charmy:
                    Character_Index = Character_Charmy_Physics_Entry_Index;
                    break;
            }
            return Character_Index;
        }

        /// <summary>
        /// Retrieves the entry number for the character physics entry. (i.e. which entry is it out of the 12 entries)
        /// </summary>
        /// <param name="Character"></param>
        /// <returns></returns>
        private int Get_Character_Address_Entry(Characters_Heroes Character)
        {
            // Get Index Object of Character.
            switch (Character)
            {
                case Characters_Heroes.Sonic:
                    return 0;

                case Characters_Heroes.Knuckles:
                    return 1;

                case Characters_Heroes.Tails:
                    return 2;

                case Characters_Heroes.Shadow:
                    return 3;

                case Characters_Heroes.Omega:
                    return 4;

                case Characters_Heroes.Rouge:
                    return 5;

                case Characters_Heroes.Amy:
                    return 6;

                case Characters_Heroes.Big:
                    return 7;

                case Characters_Heroes.Cream:
                    return 8;

                case Characters_Heroes.Espio:
                    return 9;

                case Characters_Heroes.Vector:
                    return 10;

                case Characters_Heroes.Charmy:
                    return 11;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Adds every single physics entry onto the list.
        /// </summary>
        private void Populate_Physics_Entries()
        {
            // Heroes
            Characters_Physics_All.Add(Character_Physics_Sonic_Heroes);
            Characters_Physics_All.Add(Character_Physics_Knuckles_Heroes);
            Characters_Physics_All.Add(Character_Physics_Tails_Heroes);

            Characters_Physics_All.Add(Character_Physics_Shadow_Heroes);
            Characters_Physics_All.Add(Character_Physics_Omega_Heroes);
            Characters_Physics_All.Add(Character_Physics_Rouge_Heroes);

            Characters_Physics_All.Add(Character_Physics_Amy_Heroes);
            Characters_Physics_All.Add(Character_Physics_Big_Heroes);
            Characters_Physics_All.Add(Character_Physics_Cream_Heroes);

            Characters_Physics_All.Add(Character_Physics_Espio_Heroes);
            Characters_Physics_All.Add(Character_Physics_Vector_Heroes);
            Characters_Physics_All.Add(Character_Physics_Charmy_Heroes);

            // SADX
            Characters_Physics_All.Add(Character_Physics_Sonic_SADX);
            Characters_Physics_All.Add(Character_Physics_Eggman_SADX);
            Characters_Physics_All.Add(Character_Physics_Tails_SADX);

            Characters_Physics_All.Add(Character_Physics_Knuckles_SADX);
            Characters_Physics_All.Add(Character_Physics_Tikal_SADX);
            Characters_Physics_All.Add(Character_Physics_Amy_SADX);

            Characters_Physics_All.Add(Character_Physics_Gamma_SADX);
            Characters_Physics_All.Add(Character_Physics_Big_SADX);
            Characters_Physics_All.Add(Character_Physics_SuperSonic_SADX);

            // SA2B
            Characters_Physics_All.Add(Character_Physics_Sonic_SA2B);
            Characters_Physics_All.Add(Character_Physics_Shadow_SA2B);
            Characters_Physics_All.Add(Character_Physics_Tails_SA2B);

            Characters_Physics_All.Add(Character_Physics_Eggman_SA2B);
            Characters_Physics_All.Add(Character_Physics_Knuckles_SA2B);
            Characters_Physics_All.Add(Character_Physics_Rouge_SA2B);

            Characters_Physics_All.Add(Character_Physics_Tails_Mech_SA2B);
            Characters_Physics_All.Add(Character_Physics_Eggman_Mech_SA2B);
            Characters_Physics_All.Add(Character_Physics_Amy_SA2B);

            Characters_Physics_All.Add(Character_Physics_SuperSonic_SA2B);
            Characters_Physics_All.Add(Character_Physics_SuperShadow_SA2B);
            Characters_Physics_All.Add(Character_Physics_Unused_SA2B);

            Characters_Physics_All.Add(Character_Physics_MetalSonic_SA2B);
            Characters_Physics_All.Add(Character_Physics_ChaoWalker_SA2B);
            Characters_Physics_All.Add(Character_Physics_DarkChaoWalker_SA2B);

            Characters_Physics_All.Add(Character_Physics_Tikal_SA2B);
            Characters_Physics_All.Add(Character_Physics_Chaos_SA2B);
        }

        public struct Character_Physics_SonicHeroes
        {
            /// <summary>
            /// Name of the physics profile.
            /// </summary>
            public string Physics_Name;

            /// <summary>
            /// Amount of frames in which the character will fall at a decreased rate when rolling off a ledge.
            /// </summary>
            public int Hang_Time;

            /// <summary>
            /// Higher values indicate easier acceleration on rough surfaces, rarely used in Heroes.
            /// </summary>
            public float Floor_Grip;

            /// <summary>
            /// The horizontal speed cap, maximum speed in X/Z axis.
            /// </summary>
            public float Horizontal_Speed_Cap;

            /// <summary>
            /// The vertical speed cap, maximum speed in Y axis.
            /// </summary>
            public float Vertical_Speed_Cap;

            /// <summary>
            /// Affects character acceleration.
            /// </summary>
            public float Unknown_Accel_Related;

            /// <summary>
            /// Unknown. This value is same as in SADX.
            /// </summary>
            public float Unknown_I;

            /// <summary>
            /// The initial vertical speed set by the game when the player presses the "Jump" button.
            /// </summary>
            public float Initial_Jump_Speed;

            /// <summary>
            /// Seems related to springs (at least in SADX), unknown.
            /// </summary>
            public float Spring_Control;

            /// <summary>
            /// Unknown value.
            /// </summary>
            public float Unknown_II;

            /// <summary>
            /// If the character is below this speed, the roll will be cancelled.
            /// </summary>
            public float Rolling_Minimum_Speed;

            /// <summary>
            /// Unknown value. SADX/SA2: Rolling End Speed
            /// </summary>
            public float Rolling_End_Speed;

            /// <summary>
            /// Speed after starting to roll as Sonic or punching as Knuckles. SADX: Running_I Speed
            /// </summary>
            public float Action_I_Speed;

            /// <summary>
            /// The minimum speed of knockback/push in another direction when making contact with a wall.
            /// </summary>
            public float Min_Wall_Hit_Knockback_Speed;

            /// <summary>
            /// Speed after kickingc as Sonic or punching as Knuckles. SADX: Running_I Speed
            /// </summary>
            public float Action_II_Speed;

            /// <summary>
            /// While jump is held, this is added to speed. Allows for higher jumps when holding jump.
            /// </summary>
            public float Jump_Hold_AddSpeed;

            /// <summary>
            /// The character's ground horizontal acceleration. Speed is set to this value when starting from a neutral position and is applied constantly until the character reaches a set speed.
            /// </summary>
            public float Ground_Starting_Acceleration;

            /// <summary>
            /// How fast the character gains speed in the air.
            /// </summary>
            public float Air_Acceleration;

            /// <summary>
            /// How fast the character decelerates naturally on the ground when no buttons are held.
            /// </summary>
            public float Ground_Deceleration;

            /// <summary>
            /// How fast the character can halt on the ground after holding the direction opposite to direction of travel after running.
            /// </summary>
            public float Brake_Speed;

            /// <summary>
            /// How fast the character can halt in the air when holding the direction opposite to direction of travel.
            /// </summary>
            public float Air_Brake_Speed;

            /// <summary>
            /// How fast the character decelerates naturally in the air when no buttons are held.
            /// </summary>
            public float Air_Deceleration;

            /// <summary>
            /// How fast the character decelerates naturally in the air when no buttons are held.
            /// </summary>
            public float Rolling_Deceleration;

            /// <summary>
            /// This speed is added every frame in the direction that the character is travelling in the Y Axis. e.g. If you are going up, a positive value will push you up but will push you down when falling.
            /// </summary>
            public float Gravity_Offset_Speed;

            /// <summary>
            /// (Seems to be tied with collision?) The speed applied to Sonic when he tried to alter the course of his original trajectory by pushing left or right in mid air.
            /// </summary>
            public float Mid_Air_Swerve_Speed;

            /// <summary>
            /// The minimum speed until the character will stop moving completely.
            /// </summary>
            public float Min_Speed_Before_Stopping;

            /// <summary>
            /// Constant force applied to the left of the character, used to make characters appear to run sideways. NOT IN OTHER SANIC GAMES.
            /// </summary>
            public float Constant_Speed_Offset;

            /// <summary>
            /// Unknown value, affects off road acceleration. The closer to -0 on the negative, the slower the offroad acceleration.
            /// </summary>
            public float Unknown_Off_Road;

            public float Unknown_III;
            public float Unknown_IV;

            /// <summary>
            /// Represents the radius (or diameter) of the collision cylinder which represents Sonic.
            /// </summary>
            public float Collision_Size;

            /// <summary>
            /// Gravity Constant for the character.
            /// </summary>
            public float Gravitational_Pull;

            /// <summary>
            /// (Only affects when playing as partner?) Y_Offset for camera.
            /// </summary>
            public float Y_Offset_Camera;

            /// <summary>
            /// (Only affects when playing as partner?) Physical location Y_Offset
            /// </summary>
            public float Y_Offset;

            /// <summary>
            /// A HUGE CONSTRUCTOR.
            /// </summary>
            public Character_Physics_SonicHeroes
            (
                string Physics_Name_X,
                int Hang_Time_X,
                float Floor_Grip_X,
                float Horizontal_Speed_Cap_X,
                float Vertical_Speed_Cap_X,
                float Unknown_Accel_Related_X,
                float Unknown_I_X,
                float Initial_Jump_Speed_X,
                float Spring_Control_X,
                float Unknown_II_X,
                float Rolling_Minimum_Speed_X,
                float Rolling_End_Speed_X,
                float Action_I_Speed_X,
                float Min_Wall_Hit_Knockback_Speed_X,
                float Action_II_Speed_X,
                float Jump_Hold_AddSpeed_X,
                float Ground_Starting_Acceleration_X,
                float Air_Acceleration_X,
                float Ground_Deceleration_X,
                float Brake_Speed_X,
                float Air_Brake_Speed_X,
                float Air_Deceleration_X,
                float Rolling_Deceleration_X,
                float Gravity_Offset_Speed_X,
                float Mid_Air_Swerve_Speed_X,
                float Min_Speed_Before_Stopping_X,
                float Constant_Speed_Offset_X,
                float Unknown_Off_Road_X,
                float Unknown_III_X,
                float Unknown_IV_X,
                float Collision_Size_X,
                float Gravitational_Pull_X,
                float Y_Offset_Camera_X,
                float Y_Offset_X
            )
            {
                Physics_Name = Physics_Name_X;
                Hang_Time = Hang_Time_X;
                Floor_Grip = Floor_Grip_X;
                Horizontal_Speed_Cap = Horizontal_Speed_Cap_X;
                Vertical_Speed_Cap = Vertical_Speed_Cap_X;
                Unknown_Accel_Related = Unknown_Accel_Related_X;
                Unknown_I = Unknown_I_X;
                Initial_Jump_Speed = Initial_Jump_Speed_X;
                Spring_Control = Spring_Control_X;
                Unknown_II = Unknown_II_X;
                Rolling_Minimum_Speed = Rolling_Minimum_Speed_X;
                Rolling_End_Speed = Rolling_End_Speed_X;
                Action_I_Speed = Action_I_Speed_X;
                Min_Wall_Hit_Knockback_Speed = Min_Wall_Hit_Knockback_Speed_X;
                Action_II_Speed = Action_II_Speed_X;
                Jump_Hold_AddSpeed = Jump_Hold_AddSpeed_X;
                Ground_Starting_Acceleration = Ground_Starting_Acceleration_X;
                Air_Acceleration = Air_Acceleration_X;
                Ground_Deceleration = Ground_Deceleration_X;
                Brake_Speed = Brake_Speed_X;
                Air_Brake_Speed = Air_Brake_Speed_X;
                Air_Deceleration = Air_Deceleration_X;
                Rolling_Deceleration = Rolling_Deceleration_X;
                Gravity_Offset_Speed = Gravity_Offset_Speed_X;
                Mid_Air_Swerve_Speed = Mid_Air_Swerve_Speed_X;
                Min_Speed_Before_Stopping = Min_Speed_Before_Stopping_X;
                Constant_Speed_Offset = Constant_Speed_Offset_X;
                Unknown_Off_Road = Unknown_Off_Road_X;
                Unknown_III = Unknown_III_X;
                Unknown_IV = Unknown_IV_X;
                Collision_Size = Collision_Size_X;
                Gravitational_Pull = Gravitational_Pull_X;
                Y_Offset_Camera = Y_Offset_Camera_X;
                Y_Offset = Y_Offset_X;
            }
        }

        //
        //
        // PHYSICS PROFILES
        //
        //
        Character_Physics_SonicHeroes Character_Physics_Sonic_SADX = new Character_Physics_SonicHeroes
        (
            "Sonic (Sonic Adventure DX)",
            60, // Hang Time
            2, // Floor Grip
            16, // Horz Speed Cap
            16, // Vert Speed Cap
            3, // Unknown Accel Related
            0.6F, // Unknown_I
            1.66F, // Initial Jump Speed
            3, // Spring_Control_X 
            0.23F, // Unknown_II
            0.46F, // Rolling_Minimum_Speed
            1.39F, // Rolling_End_Speed
            2.3F, // Action_I Speed
            3.7F, // Minimum Knockback Speed from Wall Hit
            5.09F, // Action_II Speed
            0.076F, // Jump_Hold_AddSpeed
            0.06F, // Ground_Starting_Acceleration
            0.031F, // Air_Acceleration
            -0.06F, // Ground_Deceleration
            -0.18F, // Break_Speed
            -0.17F, // Air_Break Speed
            -0.028F, // Air_Deceleration
            -0.008F, // Rolling_Deceleration
            -0.01F, // Gravity_Offset_Speed
            -0.4F, // Mid_Air_Swerve_Speed
            -0.1F, // Min_Speed_Before_Stopping
            -0.6F, // Constant_Speed_Offset_X
            -0.2825F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
            0.3F, // Unknown_III
            4F, // Unknown_IV
            10F, // Collision_Size (Radius)
            0.08F, // Gravitational_Constant
            7F, // Y Offset Camera
            5.4F // Y Offset
        );
        Character_Physics_SonicHeroes Character_Physics_Eggman_SADX = new Character_Physics_SonicHeroes
        (
            "Eggman (Sonic Adventure DX - Unused)",
            60, // Hang Time
            3, // Floor Grip
            16, // Horz Speed Cap
            16, // Vert Speed Cap
            3, // Unknown Accel Related
            1.0F, // Unknown_I
            1.66F, // Initial Jump Speed
            3, // Spring_Control_X 
            0.23F, // Unknown_II
            0.46F, // Rolling_Minimum_Speed
            1.39F, // Rolling_End_Speed
            2.3F, // Action_I Speed
            3.7F, // Minimum Knockback Speed from Wall Hit
            5.09F, // Action_II Speed
            0.076F, // Jump_Hold_AddSpeed
            0.06F, // Ground_Starting_Acceleration
            0.031F, // Air_Acceleration
            -0.06F, // Ground_Deceleration
            -0.18F, // Break_Speed
            -0.17F, // Air_Break Speed
            -0.028F, // Air_Deceleration
            -0.008F, // Rolling_Deceleration
            -0.01F, // Gravity_Offset_Speed
            -0.4F, // Mid_Air_Swerve_Speed
            -0.1F, // Min_Speed_Before_Stopping
            -0.6F, // Constant_Speed_Offset_X
            -0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
            0.3F, // Unknown_III
            8.5F, // Unknown_IV
            18F, // Collision_Size (Radius)
            0.08F, // Gravitational_Constant
            7F, // Y Offset Camera
            5.3F // Y Offset
        );
        Character_Physics_SonicHeroes Character_Physics_Tails_SADX = new Character_Physics_SonicHeroes
(
    "Tails (Sonic Adventure DX)",
    60, // Hang Time
    1.5F, // Floor Grip
    16, // Horz Speed Cap
    16, // Vert Speed Cap
    2, // Unknown Accel Related
    0.6F, // Unknown_I
    1.66F, // Initial Jump Speed
    3, // Spring_Control_X 
    0.23F, // Unknown_II
    0.49F, // Rolling_Minimum_Speed
    1.39F, // Rolling_End_Speed
    2.8F, // Action_I Speed
    3.7F, // Minimum Knockback Speed from Wall Hit
    5.09F, // Action_II Speed
    0.076F, // Jump_Hold_AddSpeed
    0.06F, // Ground_Starting_Acceleration
    0.031F, // Air_Acceleration
    -0.06F, // Ground_Deceleration
    -0.18F, // Break_Speed
    -0.17F, // Air_Break Speed
    -0.028F, // Air_Deceleration
    -0.008F, // Rolling_Deceleration
    -0.01F, // Gravity_Offset_Speed
    -0.4F, // Mid_Air_Swerve_Speed
    -0.1F, // Min_Speed_Before_Stopping
    -0.6F, // Constant_Speed_Offset_X
    -0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
    0.3F, // Unknown_III
    3.5F, // Unknown_IV
    6F, // Collision_Size (Radius)
    0.08F, // Gravitational_Constant
    6F, // Y Offset Camera
    4.5F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Knuckles_SADX = new Character_Physics_SonicHeroes
(
"Knuckles (Sonic Adventure DX)",
60, // Hang Time
2.0F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
2.5F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed
3, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
3.1F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.06F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration
-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
0.3F, // Unknown_III
4.0F, // Unknown_IV
11.4F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
9F, // Y Offset Camera
5.7F // Y Offset
);
        Character_Physics_SonicHeroes Character_Physics_Tikal_SADX = new Character_Physics_SonicHeroes
(
"Tikal (Sonic Adventure DX - Unused)",
60, // Hang Time
2.0F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
3.0F, // Unknown Accel Related
1.0F, // Unknown_I
1.66F, // Initial Jump Speed
3, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.06F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration
-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
0.3F, // Unknown_III
4.0F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5F // Y Offset
);
        Character_Physics_SonicHeroes Character_Physics_Amy_SADX = new Character_Physics_SonicHeroes
(
"Amy (Sonic Adventure DX)",
60, // Hang Time
1.5F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
0.05F, // Unknown Accel Related
0.5F, // Unknown_I
1.3F, // Initial Jump Speed
3, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.013F, // Jump_Hold_AddSpeed
0.045F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration
-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
0.3F, // Unknown_III
3.5F, // Unknown_IV
10F, // Collision_Size (Radius)
0.04F, // Gravitational_Constant
7F, // Y Offset Camera
5F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Gamma_SADX = new Character_Physics_SonicHeroes
(
"Gamma (Sonic Adventure DX)",
60, // Hang Time
2.0F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
2.5F, // Unknown Accel Related
0.6F, // Unknown_I
2.0F, // Initial Jump Speed
3, // Spring_Control_X 
0.23F, // Unknown_II
1.00F, // Rolling_Minimum_Speed
2.00F, // Rolling_End_Speed
2.5F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.09F, // Jump_Hold_AddSpeed
0.06F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration
-0.06F, // Ground_Deceleration
-0.25F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.9F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
0.3F, // Unknown_III
4F, // Unknown_IV
21F, // Collision_Size (Radius)
0.1F, // Gravitational_Constant
20F, // Y Offset Camera
14F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Big_SADX = new Character_Physics_SonicHeroes
(
"Big (Sonic Adventure DX)",
60, // Hang Time
2.0F, // Floor Grip
5, // Horz Speed Cap
8, // Vert Speed Cap
1F, // Unknown Accel Related
0.2F, // Unknown_I
2F, // Initial Jump Speed
3, // Spring_Control_X 
0.23F, // Unknown_II
0.32F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.1F, // Jump_Hold_AddSpeed
0.08F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration
-0.08F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.04F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.2F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
0.3F, // Unknown_III
9.5F, // Unknown_IV
17F, // Collision_Size (Radius)
0.135F, // Gravitational_Constant
15F, // Y Offset Camera
8F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_SuperSonic_SADX = new Character_Physics_SonicHeroes
(
"Super Sonic (Sonic Adventure DX)",
60, // Hang Time
2.0F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
3F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed
3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.05F, // Ground_Starting_Acceleration
0.05F, // Air_Acceleration
-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.002F, // Air_Deceleration
-0.001F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.2825F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Sonic_SA2B = new Character_Physics_SonicHeroes
(
"Sonic (Sonic Adventure 2 Battle)",
60, // Hang Time
2.0F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
3F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed
3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.05F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration
-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.2825F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Shadow_SA2B = new Character_Physics_SonicHeroes
(
"Shadow (Sonic Adventure 2 Battle)",
60, // Hang Time
2.0F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
3F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed
3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.05F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration
-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.2825F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Tails_SA2B = new Character_Physics_SonicHeroes
(
"Tails (Sonic Adventure 2 Battle - Mechless)",
40, // Hang Time
1.5F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
0.6F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed
3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
4F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.048F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration
-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.01F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
0.3F, // Unknown_III
3.5F, // Unknown_IV
9F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
6F, // Y Offset Camera
4.5F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Eggman_SA2B = new Character_Physics_SonicHeroes
(
"Eggman (Sonic Adventure 2 Battle - Mechless)",
0, // Hang Time
3F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
3F, // Unknown Accel Related
1F, // Unknown_I
1.66F, // Initial Jump Speed
3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.06F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration
-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)
0.3F, // Unknown_III
8.5F, // Unknown_IV
15.5F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
14.5F, // Y Offset Camera
10F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Knuckles_SA2B = new Character_Physics_SonicHeroes
(
"Knuckles (Sonic Adventure 2 Battle)",
60, // Hang Time
2F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
2.5F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed
3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
3.1F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.05F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
11.4F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
9F, // Y Offset Camera
5.7F // Y Offset
);
        Character_Physics_SonicHeroes Character_Physics_Rouge_SA2B = new Character_Physics_SonicHeroes
(
"Rouge (Sonic Adventure 2 Battle)",
60, // Hang Time
2F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
2.5F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed
3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
3.1F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.05F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
11.4F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
9F, // Y Offset Camera
5.7F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Tails_Mech_SA2B = new Character_Physics_SonicHeroes
(
"Tails (Sonic Adventure 2 Battle - Mech)",
16, // Hang Time
3F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
1F, // Unknown Accel Related
1F, // Unknown_I
2.6F, // Initial Jump Speed

3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
1.8F, // Action_I Speed
2.0F, // Minimum Knockback Speed from Wall Hit
3.0F, // Action_II Speed
0.19F, // Jump_Hold_AddSpeed
0.1F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.1F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.014F, // Rolling_Deceleration
-0.02F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
8F, // Unknown_IV
21F, // Collision_Size (Radius)
0.2F, // Gravitational_Constant
20F, // Y Offset Camera
15F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Eggman_Mech_SA2B = new Character_Physics_SonicHeroes
(
"Eggman (Sonic Adventure 2 Battle - Mech)",
16, // Hang Time
3F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
1F, // Unknown Accel Related
1F, // Unknown_I
2.6F, // Initial Jump Speed

3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
1.8F, // Action_I Speed
2.0F, // Minimum Knockback Speed from Wall Hit
3.0F, // Action_II Speed
0.19F, // Jump_Hold_AddSpeed
0.1F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.1F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.014F, // Rolling_Deceleration
-0.02F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
8F, // Unknown_IV
21F, // Collision_Size (Radius)
0.2F, // Gravitational_Constant
20F, // Y Offset Camera
15F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Amy_SA2B = new Character_Physics_SonicHeroes
(
"Amy (Sonic Adventure 2 Battle)",
60, // Hang Time
2F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
1.3F, // Unknown Accel Related
0.6F, // Unknown_I
1.3F, // Initial Jump Speed

3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.048F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.2825F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);
        Character_Physics_SonicHeroes Character_Physics_SuperSonic_SA2B = new Character_Physics_SonicHeroes
(
"Super Sonic (Sonic Adventure 2 Battle)",
60, // Hang Time
2F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
3F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.05F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.2825F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_SuperShadow_SA2B = new Character_Physics_SonicHeroes
(
"Super Shadow (Sonic Adventure 2 Battle)",
60, // Hang Time
2F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
3F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.05F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.2825F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Unused_SA2B = new Character_Physics_SonicHeroes
(
"Unused (Sonic Adventure 2 Battle)",
60, // Hang Time
2F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
3F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.05F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.2825F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_MetalSonic_SA2B = new Character_Physics_SonicHeroes
(
"Metal Sonic (Sonic Adventure 2 Battle)",
60, // Hang Time
2F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
3F, // Unknown Accel Related
0.6F, // Unknown_I
2.52F, // Initial Jump Speed

3F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.2F, // Ground_Starting_Acceleration
0.05F, // Air_Acceleration

-0.01F, // Ground_Deceleration
-0.02F, // Break_Speed
-0.03F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.4F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.2825F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_ChaoWalker_SA2B = new Character_Physics_SonicHeroes
(
"Chao Walker (Sonic Adventure 2 Battle)",
16, // Hang Time
3F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
1.4F, // Unknown Accel Related
1.0F, // Unknown_I
2.6F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
1.8F, // Action_I Speed
2.0F, // Minimum Knockback Speed from Wall Hit
3.0F, // Action_II Speed
0.19F, // Jump_Hold_AddSpeed
0.2F, // Ground_Starting_Acceleration
0.046F, // Air_Acceleration

-0.1F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.014F, // Rolling_Deceleration
-0.02F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.4F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
8F, // Unknown_IV
21F, // Collision_Size (Radius)
0.2F, // Gravitational_Constant
20F, // Y Offset Camera
15F // Y Offset
);
        Character_Physics_SonicHeroes Character_Physics_DarkChaoWalker_SA2B = new Character_Physics_SonicHeroes
(
"Dark Chao Walker (Sonic Adventure 2 Battle)",
16, // Hang Time
3F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
0.7F, // Unknown Accel Related
1.0F, // Unknown_I
2.6F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
1.8F, // Action_I Speed
2.0F, // Minimum Knockback Speed from Wall Hit
3.0F, // Action_II Speed
0.19F, // Jump_Hold_AddSpeed
0.07F, // Ground_Starting_Acceleration
0.025F, // Air_Acceleration

-0.1F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.014F, // Rolling_Deceleration
-0.02F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.07F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
8F, // Unknown_IV
21F, // Collision_Size (Radius)
0.2F, // Gravitational_Constant
20F, // Y Offset Camera
15F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Tikal_SA2B = new Character_Physics_SonicHeroes
(
"Tikal (Sonic Adventure 2 Battle)",
60, // Hang Time
2F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
2.5F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
3.1F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.06F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.2F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.3F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.45F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
11.4F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
9F, // Y Offset Camera
5.7F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Chaos_SA2B = new Character_Physics_SonicHeroes
(
"Chaos (Sonic Adventure 2 Battle)",
60, // Hang Time
2F, // Floor Grip
16, // Horz Speed Cap
16, // Vert Speed Cap
1.0F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
3.1F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.05F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.09F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
11.4F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
9F, // Y Offset Camera
5.7F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Sonic_Heroes = new Character_Physics_SonicHeroes
(
"Sonic (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
6.0F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.1F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.09F, // Ground_Deceleration
-0.15F, // Break_Speed
-0.14F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.25F, // Min_Speed_Before_Stopping
-0.8F, // Constant_Speed_Offset_X
-0.6F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Knuckles_Heroes = new Character_Physics_SonicHeroes
(
"Knuckles (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
2.5F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.18F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.048F, // Ground_Starting_Acceleration
0.035F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.35F, // Min_Speed_Before_Stopping
-0.85F, // Constant_Speed_Offset_X
-0.35F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.5F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Tails_Heroes = new Character_Physics_SonicHeroes
(
"Tails (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
2F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.11F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.071F, // Ground_Starting_Acceleration
0.051F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.22F, // Break_Speed
-0.21F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.01F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.125F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.45F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.28F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Shadow_Heroes = new Character_Physics_SonicHeroes
(
"Shadow (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
6F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.1F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.09F, // Ground_Deceleration
-0.15F, // Break_Speed
-0.14F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.25F, // Min_Speed_Before_Stopping
-0.8F, // Constant_Speed_Offset_X
-0.6F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Omega_Heroes = new Character_Physics_SonicHeroes
(
"Omega (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
2.5F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.18F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.048F, // Ground_Starting_Acceleration
0.035F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.35F, // Min_Speed_Before_Stopping
-0.85F, // Constant_Speed_Offset_X
-0.35F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.5F, // Unknown_III
6F, // Unknown_IV
15F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
14F, // Y Offset Camera
7.5F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Rouge_Heroes = new Character_Physics_SonicHeroes
(
"Rouge (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
2F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.11F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.071F, // Ground_Starting_Acceleration
0.051F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.22F, // Break_Speed
-0.21F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.01F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.125F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.45F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.28F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.7F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Amy_Heroes = new Character_Physics_SonicHeroes
(
"Amy (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
4.5F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.09F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.14F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.2825F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Big_Heroes = new Character_Physics_SonicHeroes
(
"Big (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
2.5F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
3.1F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.05F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.1F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.3375F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
8.5F, // Unknown_IV
18F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
18F, // Y Offset Camera
9F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Cream_Heroes = new Character_Physics_SonicHeroes
(
"Cream (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
2F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.11F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.071F, // Ground_Starting_Acceleration
0.051F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.22F, // Break_Speed
-0.21F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.01F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.125F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.45F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.28F, // Unknown_III
3.5F, // Unknown_IV
9F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
6F, // Y Offset Camera
4.5F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Espio_Heroes = new Character_Physics_SonicHeroes
(
"Espio (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
6.0F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.23F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.1F, // Ground_Starting_Acceleration
0.031F, // Air_Acceleration

-0.09F, // Ground_Deceleration
-0.15F, // Break_Speed
-0.14F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.25F, // Min_Speed_Before_Stopping
-0.8F, // Constant_Speed_Offset_X
-0.6F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.3F, // Unknown_III
4F, // Unknown_IV
10F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
7F, // Y Offset Camera
5.4F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Vector_Heroes = new Character_Physics_SonicHeroes
(
"Vector (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
2.5F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.18F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.048F, // Ground_Starting_Acceleration
0.035F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.18F, // Break_Speed
-0.17F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.008F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.35F, // Min_Speed_Before_Stopping
-0.85F, // Constant_Speed_Offset_X
-0.35F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.5F, // Unknown_III
8F, // Unknown_IV
17F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
16F, // Y Offset Camera
8.5F // Y Offset
);

        Character_Physics_SonicHeroes Character_Physics_Charmy_Heroes = new Character_Physics_SonicHeroes
(
"Charmy (Sonic Heroes)",
60, // Hang Time
2F, // Floor Grip
32, // Horz Speed Cap
32, // Vert Speed Cap
2F, // Unknown Accel Related
0.6F, // Unknown_I
1.66F, // Initial Jump Speed

3.0F, // Spring_Control_X 
0.11F, // Unknown_II
0.46F, // Rolling_Minimum_Speed
1.39F, // Rolling_End_Speed
2.3F, // Action_I Speed
3.7F, // Minimum Knockback Speed from Wall Hit
5.09F, // Action_II Speed
0.076F, // Jump_Hold_AddSpeed
0.071F, // Ground_Starting_Acceleration
0.051F, // Air_Acceleration

-0.06F, // Ground_Deceleration
-0.22F, // Break_Speed
-0.21F, // Air_Break Speed
-0.028F, // Air_Deceleration
-0.01F, // Rolling_Deceleration
-0.01F, // Gravity_Offset_Speed
-0.4F, // Mid_Air_Swerve_Speed
-0.125F, // Min_Speed_Before_Stopping
-0.6F, // Constant_Speed_Offset_X
-0.45F, // Unknown_Off_Road_X (Affects Ground Accel, to slow down get as close to 0 but not 0)

0.28F, // Unknown_III
3.5F, // Unknown_IV
9F, // Collision_Size (Radius)
0.08F, // Gravitational_Constant
6F, // Y Offset Camera
4.5F // Y Offset
);
    }
}
