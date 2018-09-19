using Colorspace;
using SonicHeroes.Memory;
using SonicHeroes.Variables;
using System;
using System.Threading;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// A features that allows
    /// </summary>
    public class Feature_Cycle_RGB_Colours
    {
        /// <summary>
        /// True if currently cycling the HUE of the formation barriers.
        /// </summary>
        static bool Currently_Cycling_Barrier_Hue = false;

        /// <summary>
        /// True if currently cycling the HUE of the character ball aura.
        /// </summary>
        static bool Currently_Cycling_Ball_Hue = false;

        /// <summary>
        /// True if currently cycling the HUE of the character trail.
        /// </summary>
        static bool Currently_Cycling_Trail_Hue = false;

        /// <summary>
        /// True if currently cycling the HUE of the character tornado.
        /// </summary>
        static bool Currently_Cycling_Tornado_Hue = false;

        /// <summary>
        /// True if cycling all of the cycleable items at once.
        /// </summary>
        static bool Currently_Cycling_All_Items = false;

        /// <summary>
        /// Defines the amount of colour switches of the colour to use of the aura per second.
        /// </summary>
        private int Colours_Per_Second = 120;

        /// <summary>
        /// The speed delay is the delay for which the thread sleeps while cycling ball aura. It is defines as 1000/Colours_Per_Second.
        /// </summary>
        private int Sleep_Delay = 8;

        /// <summary>
        /// Returns true if the formation barriers are currently being hue cycled.
        /// </summary>
        public bool Get_CyclingBarrierHue() { return Currently_Cycling_Barrier_Hue; }

        /// <summary>
        /// Returns true if the character ball auras are currently being hue cycled.
        /// </summary>
        public bool Get_CyclingBallHue() { return Currently_Cycling_Ball_Hue; }

        /// <summary>
        /// Returns true if the character trails are currently being hue cycled.
        /// </summary>
        public bool Get_CyclingTrailHue() { return Currently_Cycling_Trail_Hue; }

        /// <summary>
        /// Returns true if the tornado colours are currently being hue cycled.
        /// </summary>
        public bool Get_CyclingTornadoHue() { return Currently_Cycling_Tornado_Hue; }

        /// <summary>
        /// Returns true if the all of the things at once are being hue cycled.
        /// </summary>
        public bool Get_CyclingAllHue() { return Currently_Cycling_All_Items; }

        /// <summary>
        /// Retrieves the current approximate amount of colours switched per second.
        /// </summary>
        public int Get_Colours_Per_Second() { return Colours_Per_Second; }

        /// <summary>
        /// Retrieves the actual set sleep delay.
        /// </summary>
        public int Get_Sleep_Delay() { return Sleep_Delay; }

        /// <summary>
        /// Increments the current amount of colours per second.
        /// </summary>
        public void Increment_Colours_Per_Second()
        {
            if (Colours_Per_Second + 1 < 1001) { Colours_Per_Second += 1; }
            Sleep_Delay = 1000 / Colours_Per_Second;
        }

        /// <summary>
        /// Decrements the current amount of colours per second.
        /// </summary>
        public void Decrement_Colours_Per_Second()
        {
            if (Colours_Per_Second - 1 > 0) { Colours_Per_Second -= 1; }
            Sleep_Delay = 1000 / Colours_Per_Second;
        }

        /// <summary>
        /// Toggles the live Formation Barrier Cycle State
        /// </summary>
        public void Toggle_HUE_Cycle_Formation_Barriers()
        {
            if (!Currently_Cycling_Barrier_Hue) {  Currently_Cycling_Barrier_Hue = true; Cycle_HUE_Formation_Barriers(); }
            else { Currently_Cycling_Barrier_Hue = false; }
        }

        /// <summary>
        /// Toggles the live Character Ball Colour Cycle State
        /// </summary>
        public void Toggle_HUE_Cycle_Character_Balls()
        {
            if (!Currently_Cycling_Ball_Hue) { Currently_Cycling_Ball_Hue = true; Cycle_HUE_Character_Balls(); }
            else { Currently_Cycling_Ball_Hue = false; }
        }

        /// <summary>
        /// Toggles the live Character Trail Cycle State
        /// </summary>
        public void Toggle_HUE_Cycle_Character_Trails()
        {
            if (!Currently_Cycling_Trail_Hue) { Currently_Cycling_Trail_Hue = true; Cycle_HUE_Character_Trails(); }
            else { Currently_Cycling_Trail_Hue = false; }
        }

        /// <summary>
        /// Toggles the live Character Tornado Cycle State
        /// </summary>
        public void Toggle_HUE_Cycle_Character_Tornadoes()
        {
            if (!Currently_Cycling_Tornado_Hue) {  Currently_Cycling_Tornado_Hue = true; Cycle_HUE_Character_Tornadoes(); }
            else { Currently_Cycling_Tornado_Hue = false; }
        }

        /// <summary>
        /// Enables the cycling of all of the items at once.
        /// </summary>
        public void Toggle_HUE_Cycle_All_Combined()
        {
            if (Currently_Cycling_All_Items)
            {
                // Disable all of the current cycles.
                if (Currently_Cycling_Barrier_Hue) { Currently_Cycling_Barrier_Hue = false; }
                if (Currently_Cycling_Ball_Hue) { Currently_Cycling_Ball_Hue = false; }
                if (Currently_Cycling_Trail_Hue) { Currently_Cycling_Trail_Hue = false; }
                if (Currently_Cycling_Tornado_Hue) { Currently_Cycling_Tornado_Hue = false; }

                Currently_Cycling_All_Items = false;
            }
            else
            {
                // Disable all of the current cycles.
                if (Currently_Cycling_Barrier_Hue) { Currently_Cycling_Barrier_Hue = false; }
                if (Currently_Cycling_Ball_Hue) { Currently_Cycling_Ball_Hue = false; }
                if (Currently_Cycling_Trail_Hue) { Currently_Cycling_Trail_Hue = false; }
                if (Currently_Cycling_Tornado_Hue) { Currently_Cycling_Tornado_Hue = false; }

                // Wait for sync purposes.
                Thread.Sleep(32);

                // Enable everything approximately at once.
                Toggle_HUE_Cycle_Formation_Barriers();
                Toggle_HUE_Cycle_Character_Balls();
                Toggle_HUE_Cycle_Character_Trails();
                Toggle_HUE_Cycle_Character_Tornadoes();

                Currently_Cycling_All_Items = true;
            }
        }

        /// <summary>
        /// Cycles the HUE of Formation Barriers.
        /// </summary>
        private void Cycle_HUE_Formation_Barriers()
        {
            // Setup the thread.
            Thread HUE_Cycle_Thread = new Thread(x =>
            {
                // Retrieve Original Barrier Colours.
                Colorspace.ColorRGB Original_Red_Barrier_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Red_R, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Red_G, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Red_B, 1) / 255.0F)
                );
                Colorspace.ColorRGB Original_Blue_Barrier_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Blue_R, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Blue_G, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Blue_B, 1) / 255.0F)
                );
                Colorspace.ColorRGB Original_Yellow_Barrier_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Yellow_R, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Yellow_G, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Yellow_B, 1) / 255.0F)
                );

                // Original Colours as HSL for Cycle Value Calculation.
                ColorHSV HSL_Original_Red_Barrier_Colour = new ColorHSV(Original_Red_Barrier_Colour);
                ColorHSV HSL_Original_Yellow_Barrier_Colour = new ColorHSV(Original_Yellow_Barrier_Colour);
                ColorHSV HSL_Original_Blue_Barrier_Colour = new ColorHSV(Original_Blue_Barrier_Colour);

                // Arrays of HUEs of colours.
                ColorRGB[] RGB_Red_Barrier_Colours = new ColorRGB[255];
                ColorRGB[] RGB_Blue_Barrier_Colours = new ColorRGB[255];
                ColorRGB[] RGB_Yellow_Barrier_Colours = new ColorRGB[255];

                // Precalculate cycle values (CPU Saving)
                for (int z = 0; z < 255; z++)
                {
                    // Obtain HUE Values
                    byte Red_Barrier_Hue = (byte)((HSL_Original_Red_Barrier_Colour.H * 255.0F) + z);
                    byte Yellow_Barrier_Hue = (byte)((HSL_Original_Yellow_Barrier_Colour.H * 255.0F) + z);
                    byte Blue_Barrier_Hue = (byte)((HSL_Original_Blue_Barrier_Colour.H * 255.0F) + z);

                    // Cycle Hue
                    ColorHSV Current_Red_Barrier_Colour = new ColorHSV(Red_Barrier_Hue / 255.0F, HSL_Original_Red_Barrier_Colour.S, HSL_Original_Red_Barrier_Colour.V);
                    ColorHSV Current_Yellow_Barrier_Colour = new ColorHSV(Yellow_Barrier_Hue / 255.0F, HSL_Original_Yellow_Barrier_Colour.S, HSL_Original_Yellow_Barrier_Colour.V);
                    ColorHSV Current_Blue_Barrier_Colour = new ColorHSV(Blue_Barrier_Hue / 255.0F, HSL_Original_Blue_Barrier_Colour.S, HSL_Original_Blue_Barrier_Colour.V);

                    // Convert back to RGB
                    RGB_Red_Barrier_Colours[z] = new ColorRGB(Current_Red_Barrier_Colour);
                    RGB_Blue_Barrier_Colours[z] = new ColorRGB(Current_Blue_Barrier_Colour);
                    RGB_Yellow_Barrier_Colours[z] = new ColorRGB(Current_Yellow_Barrier_Colour);
                }

                while (Currently_Cycling_Barrier_Hue)
                {
                    for (int c = 0; c < 255; c++) // C++, lol
                    {
                        // Write to Game Memory
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Blue_B, new byte[] { Convert.ToByte(RGB_Blue_Barrier_Colours[c].B * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Blue_G, new byte[] { Convert.ToByte(RGB_Blue_Barrier_Colours[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Blue_R, new byte[] { Convert.ToByte(RGB_Blue_Barrier_Colours[c].R * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Yellow_B, new byte[] { Convert.ToByte(RGB_Yellow_Barrier_Colours[c].B * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Yellow_G, new byte[] { Convert.ToByte(RGB_Yellow_Barrier_Colours[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Yellow_R, new byte[] { Convert.ToByte(RGB_Yellow_Barrier_Colours[c].R * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Red_B, new byte[] { Convert.ToByte(RGB_Red_Barrier_Colours[c].B * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Red_G, new byte[] { Convert.ToByte(RGB_Red_Barrier_Colours[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Red_R, new byte[] { Convert.ToByte(RGB_Red_Barrier_Colours[c].R * 255.0F) });
                        Thread.Sleep(Sleep_Delay);
                    }
                }

                // Restore Original Values to Game Memory
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Blue_B, new byte[] { Convert.ToByte(Original_Blue_Barrier_Colour.B * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Blue_G, new byte[] { Convert.ToByte(Original_Blue_Barrier_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Blue_R, new byte[] { Convert.ToByte(Original_Blue_Barrier_Colour.R * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Yellow_B, new byte[] { Convert.ToByte(Original_Yellow_Barrier_Colour.B * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Yellow_G, new byte[] { Convert.ToByte(Original_Yellow_Barrier_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Yellow_R, new byte[] { Convert.ToByte(Original_Yellow_Barrier_Colour.R * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Red_B, new byte[] { Convert.ToByte(Original_Red_Barrier_Colour.B * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Red_G, new byte[] { Convert.ToByte(Original_Red_Barrier_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Object_Colours.Formation_Gate_Lasers_Red_R, new byte[] { Convert.ToByte(Original_Red_Barrier_Colour.R * 255.0F) });
            });

            // Run the Thread.
            HUE_Cycle_Thread.Start();
        }

        /// <summary>
        /// Cycles the HUE of Formation Barriers.
        /// </summary>
        private void Cycle_HUE_Character_Balls()
        {
            // Setup the thread.
            Thread HUE_Ball_Cycle_Thread = new Thread(x =>
            {

                // Retrieve Original Barrier Colours.
                Colorspace.ColorRGB Amy_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Big_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Charmy_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Cream_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Espio_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Knuckles_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Omega_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Rouge_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Shadow_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Sonic_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Tails_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Jump_Ball + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Vector_Ball_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Jump_Ball, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Jump_Ball + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Jump_Ball + 2, 1) / 255.0F)
                );

                // Original Colours as HSL for Cycle Value Calculation.
                ColorHSV HSL_Amy_Ball_Colour = new ColorHSV(Amy_Ball_Colour);
                ColorHSV HSL_Big_Ball_Colour = new ColorHSV(Big_Ball_Colour);
                ColorHSV HSL_Charmy_Ball_Colour = new ColorHSV(Charmy_Ball_Colour);
                ColorHSV HSL_Cream_Ball_Colour = new ColorHSV(Cream_Ball_Colour);
                ColorHSV HSL_Espio_Ball_Colour = new ColorHSV(Espio_Ball_Colour);
                ColorHSV HSL_Knuckles_Ball_Colour = new ColorHSV(Knuckles_Ball_Colour);
                ColorHSV HSL_Omega_Ball_Colour = new ColorHSV(Omega_Ball_Colour);
                ColorHSV HSL_Rouge_Ball_Colour = new ColorHSV(Rouge_Ball_Colour);
                ColorHSV HSL_Shadow_Ball_Colour = new ColorHSV(Shadow_Ball_Colour);
                ColorHSV HSL_Sonic_Ball_Colour = new ColorHSV(Sonic_Ball_Colour);
                ColorHSV HSL_Tails_Ball_Colour = new ColorHSV(Tails_Ball_Colour);
                ColorHSV HSL_Vector_Ball_Colour = new ColorHSV(Vector_Ball_Colour);

                // Arrays of HUEs of colours.
                ColorRGB[] RGB_Amy_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Big_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Charmy_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Cream_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Espio_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Knuckles_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Omega_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Rouge_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Shadow_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Sonic_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Tails_Ball_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Vector_Ball_Colour = new ColorRGB[255];

                // Precalculate cycle values (CPU Saving)
                for (int z = 0; z < 255; z++)
                {
                    // Obtain HUE Values
                    byte Amy_Ball_Hue = (byte)((HSL_Amy_Ball_Colour.H * 255.0F) + z);
                    byte Big_Ball_Hue = (byte)((HSL_Big_Ball_Colour.H * 255.0F) + z);
                    byte Charmy_Ball_Hue = (byte)((HSL_Charmy_Ball_Colour.H * 255.0F) + z);
                    byte Cream_Ball_Hue = (byte)((HSL_Cream_Ball_Colour.H * 255.0F) + z);
                    byte Espio_Ball_Hue = (byte)((HSL_Espio_Ball_Colour.H * 255.0F) + z);
                    byte Knuckles_Ball_Hue = (byte)((HSL_Knuckles_Ball_Colour.H * 255.0F) + z);
                    byte Omega_Ball_Hue = (byte)((HSL_Omega_Ball_Colour.H * 255.0F) + z);
                    byte Rouge_Ball_Hue = (byte)((HSL_Rouge_Ball_Colour.H * 255.0F) + z);
                    byte Shadow_Ball_Hue = (byte)((HSL_Shadow_Ball_Colour.H * 255.0F) + z);
                    byte Sonic_Ball_Hue = (byte)((HSL_Sonic_Ball_Colour.H * 255.0F) + z);
                    byte Tails_Ball_Hue = (byte)((HSL_Tails_Ball_Colour.H * 255.0F) + z);
                    byte Vector_Ball_Hue = (byte)((HSL_Vector_Ball_Colour.H * 255.0F) + z);

                    // Cycle Hue
                    ColorHSV Amy_Ball_Colour_New = new ColorHSV(Amy_Ball_Hue / 255.0F, HSL_Amy_Ball_Colour.S, HSL_Amy_Ball_Colour.V);
                    ColorHSV Big_Ball_Colour_New = new ColorHSV(Big_Ball_Hue / 255.0F, HSL_Big_Ball_Colour.S, HSL_Big_Ball_Colour.V);
                    ColorHSV Charmy_Ball_Colour_New = new ColorHSV(Charmy_Ball_Hue / 255.0F, HSL_Charmy_Ball_Colour.S, HSL_Charmy_Ball_Colour.V);
                    ColorHSV Cream_Ball_Colour_New = new ColorHSV(Cream_Ball_Hue / 255.0F, HSL_Cream_Ball_Colour.S, HSL_Cream_Ball_Colour.V);
                    ColorHSV Espio_Ball_Colour_New = new ColorHSV(Espio_Ball_Hue / 255.0F, HSL_Espio_Ball_Colour.S, HSL_Espio_Ball_Colour.V);
                    ColorHSV Knuckles_Ball_Colour_New = new ColorHSV(Knuckles_Ball_Hue / 255.0F, HSL_Knuckles_Ball_Colour.S, HSL_Knuckles_Ball_Colour.V);
                    ColorHSV Omega_Ball_Colour_New = new ColorHSV(Omega_Ball_Hue / 255.0F, HSL_Omega_Ball_Colour.S, HSL_Omega_Ball_Colour.V);
                    ColorHSV Rouge_Ball_Colour_New = new ColorHSV(Rouge_Ball_Hue / 255.0F, HSL_Rouge_Ball_Colour.S, HSL_Rouge_Ball_Colour.V);
                    ColorHSV Shadow_Ball_Colour_New = new ColorHSV(Shadow_Ball_Hue / 255.0F, HSL_Shadow_Ball_Colour.S, HSL_Shadow_Ball_Colour.V);
                    ColorHSV Sonic_Ball_Colour_New = new ColorHSV(Sonic_Ball_Hue / 255.0F, HSL_Sonic_Ball_Colour.S, HSL_Sonic_Ball_Colour.V);
                    ColorHSV Tails_Ball_Colour_New = new ColorHSV(Tails_Ball_Hue / 255.0F, HSL_Tails_Ball_Colour.S, HSL_Tails_Ball_Colour.V);
                    ColorHSV Vector_Ball_Colour_New = new ColorHSV(Vector_Ball_Hue / 255.0F, HSL_Vector_Ball_Colour.S, HSL_Vector_Ball_Colour.V);

                    // Convert back to RGB
                    RGB_Amy_Ball_Colour[z] = new ColorRGB(Amy_Ball_Colour_New);
                    RGB_Big_Ball_Colour[z] = new ColorRGB(Big_Ball_Colour_New);
                    RGB_Charmy_Ball_Colour[z] = new ColorRGB(Charmy_Ball_Colour_New);
                    RGB_Cream_Ball_Colour[z] = new ColorRGB(Cream_Ball_Colour_New);
                    RGB_Espio_Ball_Colour[z] = new ColorRGB(Espio_Ball_Colour_New);
                    RGB_Knuckles_Ball_Colour[z] = new ColorRGB(Knuckles_Ball_Colour_New);
                    RGB_Omega_Ball_Colour[z] = new ColorRGB(Omega_Ball_Colour_New);
                    RGB_Rouge_Ball_Colour[z] = new ColorRGB(Rouge_Ball_Colour_New);
                    RGB_Shadow_Ball_Colour[z] = new ColorRGB(Shadow_Ball_Colour_New);
                    RGB_Sonic_Ball_Colour[z] = new ColorRGB(Sonic_Ball_Colour_New);
                    RGB_Tails_Ball_Colour[z] = new ColorRGB(Tails_Ball_Colour_New);
                    RGB_Vector_Ball_Colour[z] = new ColorRGB(Vector_Ball_Colour_New);
                }

                while (Currently_Cycling_Ball_Hue)
                {
                    for (int c = 0; c < 255; c++) // C++, lol
                    {
                        // Write to Game Memory
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Jump_Ball, new byte[] { Convert.ToByte(RGB_Amy_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Amy_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Amy_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Jump_Ball, new byte[] { Convert.ToByte(RGB_Big_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Big_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Big_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Jump_Ball, new byte[] { Convert.ToByte(RGB_Charmy_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Charmy_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Charmy_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Jump_Ball, new byte[] { Convert.ToByte(RGB_Cream_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Cream_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Cream_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Jump_Ball, new byte[] { Convert.ToByte(RGB_Espio_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Espio_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Espio_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Jump_Ball, new byte[] { Convert.ToByte(RGB_Knuckles_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Knuckles_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Knuckles_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Jump_Ball, new byte[] { Convert.ToByte(RGB_Omega_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Omega_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Omega_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Jump_Ball, new byte[] { Convert.ToByte(RGB_Rouge_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Rouge_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Rouge_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Jump_Ball, new byte[] { Convert.ToByte(RGB_Shadow_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Shadow_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Shadow_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Jump_Ball, new byte[] { Convert.ToByte(RGB_Sonic_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Sonic_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Sonic_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Jump_Ball, new byte[] { Convert.ToByte(RGB_Tails_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Tails_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Tails_Ball_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Jump_Ball, new byte[] { Convert.ToByte(RGB_Vector_Ball_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Jump_Ball + 1, new byte[] { Convert.ToByte(RGB_Vector_Ball_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Jump_Ball + 2, new byte[] { Convert.ToByte(RGB_Vector_Ball_Colour[c].B * 255.0F) });
                        Thread.Sleep(Sleep_Delay);
                    }
                }

                // Restore Original Values to Game Memory
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Jump_Ball, new byte[] { Convert.ToByte(Amy_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Jump_Ball + 1, new byte[] { Convert.ToByte(Amy_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Jump_Ball + 2, new byte[] { Convert.ToByte(Amy_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Jump_Ball, new byte[] { Convert.ToByte(Big_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Jump_Ball + 1, new byte[] { Convert.ToByte(Big_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Jump_Ball + 2, new byte[] { Convert.ToByte(Big_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Jump_Ball, new byte[] { Convert.ToByte(Charmy_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Jump_Ball + 1, new byte[] { Convert.ToByte(Charmy_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Jump_Ball + 2, new byte[] { Convert.ToByte(Charmy_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Jump_Ball, new byte[] { Convert.ToByte(Cream_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Jump_Ball + 1, new byte[] { Convert.ToByte(Cream_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Jump_Ball + 2, new byte[] { Convert.ToByte(Cream_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Jump_Ball, new byte[] { Convert.ToByte(Espio_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Jump_Ball + 1, new byte[] { Convert.ToByte(Espio_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Jump_Ball + 2, new byte[] { Convert.ToByte(Espio_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Jump_Ball, new byte[] { Convert.ToByte(Knuckles_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Jump_Ball + 1, new byte[] { Convert.ToByte(Knuckles_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Jump_Ball + 2, new byte[] { Convert.ToByte(Knuckles_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Jump_Ball, new byte[] { Convert.ToByte(Omega_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Jump_Ball + 1, new byte[] { Convert.ToByte(Omega_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Jump_Ball + 2, new byte[] { Convert.ToByte(Omega_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Jump_Ball, new byte[] { Convert.ToByte(Rouge_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Jump_Ball + 1, new byte[] { Convert.ToByte(Rouge_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Jump_Ball + 2, new byte[] { Convert.ToByte(Rouge_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Jump_Ball, new byte[] { Convert.ToByte(Shadow_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Jump_Ball + 1, new byte[] { Convert.ToByte(Shadow_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Jump_Ball + 2, new byte[] { Convert.ToByte(Shadow_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Jump_Ball, new byte[] { Convert.ToByte(Sonic_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Jump_Ball + 1, new byte[] { Convert.ToByte(Sonic_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Jump_Ball + 2, new byte[] { Convert.ToByte(Sonic_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Jump_Ball, new byte[] { Convert.ToByte(Tails_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Jump_Ball + 1, new byte[] { Convert.ToByte(Tails_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Jump_Ball + 2, new byte[] { Convert.ToByte(Tails_Ball_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Jump_Ball, new byte[] { Convert.ToByte(Vector_Ball_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Jump_Ball + 1, new byte[] { Convert.ToByte(Vector_Ball_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Jump_Ball + 2, new byte[] { Convert.ToByte(Vector_Ball_Colour.B * 255.0F) });
            });

            // Run the Thread.
            HUE_Ball_Cycle_Thread.Start();
        }

        /// <summary>
        /// Cycles the HUE of Formation Barriers.
        /// </summary>
        private void Cycle_HUE_Character_Trails()
        {
            // Setup the thread.
            Thread HUE_Trail_Cycle_Thread = new Thread(x =>
            {
                // Retrieve Original Barrier Colours.
                Colorspace.ColorRGB Amy_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Big_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Charmy_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Cream_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Espio_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Knuckles_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Omega_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Rouge_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Shadow_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Sonic_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Tails_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Trails + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Vector_Trail_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Trails, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Trails + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Trails + 2, 1) / 255.0F)
                );

                // Original Colours as HSL for Cycle Value Calculation.
                ColorHSV HSL_Amy_Trail_Colour = new ColorHSV(Amy_Trail_Colour);
                ColorHSV HSL_Big_Trail_Colour = new ColorHSV(Big_Trail_Colour);
                ColorHSV HSL_Charmy_Trail_Colour = new ColorHSV(Charmy_Trail_Colour);
                ColorHSV HSL_Cream_Trail_Colour = new ColorHSV(Cream_Trail_Colour);
                ColorHSV HSL_Espio_Trail_Colour = new ColorHSV(Espio_Trail_Colour);
                ColorHSV HSL_Knuckles_Trail_Colour = new ColorHSV(Knuckles_Trail_Colour);
                ColorHSV HSL_Omega_Trail_Colour = new ColorHSV(Omega_Trail_Colour);
                ColorHSV HSL_Rouge_Trail_Colour = new ColorHSV(Rouge_Trail_Colour);
                ColorHSV HSL_Shadow_Trail_Colour = new ColorHSV(Shadow_Trail_Colour);
                ColorHSV HSL_Sonic_Trail_Colour = new ColorHSV(Sonic_Trail_Colour);
                ColorHSV HSL_Tails_Trail_Colour = new ColorHSV(Tails_Trail_Colour);
                ColorHSV HSL_Vector_Trail_Colour = new ColorHSV(Vector_Trail_Colour);

                // Arrays of HUEs of colours.
                ColorRGB[] RGB_Amy_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Big_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Charmy_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Cream_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Espio_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Knuckles_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Omega_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Rouge_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Shadow_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Sonic_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Tails_Trail_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Vector_Trail_Colour = new ColorRGB[255];

                // Precalculate cycle values (CPU Saving)
                for (int z = 0; z < 255; z++)
                {
                    // Obtain HUE Values
                    byte Amy_Ball_Hue = (byte)((HSL_Amy_Trail_Colour.H * 255.0F) + z);
                    byte Big_Ball_Hue = (byte)((HSL_Big_Trail_Colour.H * 255.0F) + z);
                    byte Charmy_Ball_Hue = (byte)((HSL_Charmy_Trail_Colour.H * 255.0F) + z);
                    byte Cream_Ball_Hue = (byte)((HSL_Cream_Trail_Colour.H * 255.0F) + z);
                    byte Espio_Ball_Hue = (byte)((HSL_Espio_Trail_Colour.H * 255.0F) + z);
                    byte Knuckles_Ball_Hue = (byte)((HSL_Knuckles_Trail_Colour.H * 255.0F) + z);
                    byte Omega_Ball_Hue = (byte)((HSL_Omega_Trail_Colour.H * 255.0F) + z);
                    byte Rouge_Ball_Hue = (byte)((HSL_Rouge_Trail_Colour.H * 255.0F) + z);
                    byte Shadow_Ball_Hue = (byte)((HSL_Shadow_Trail_Colour.H * 255.0F) + z);
                    byte Sonic_Ball_Hue = (byte)((HSL_Sonic_Trail_Colour.H * 255.0F) + z);
                    byte Tails_Ball_Hue = (byte)((HSL_Tails_Trail_Colour.H * 255.0F) + z);
                    byte Vector_Ball_Hue = (byte)((HSL_Vector_Trail_Colour.H * 255.0F) + z);

                    // Cycle Hue
                    ColorHSV Amy_Trail_Colour_New = new ColorHSV(Amy_Ball_Hue / 255.0F, HSL_Amy_Trail_Colour.S, HSL_Amy_Trail_Colour.V);
                    ColorHSV Big_Trail_Colour_New = new ColorHSV(Big_Ball_Hue / 255.0F, HSL_Big_Trail_Colour.S, HSL_Big_Trail_Colour.V);
                    ColorHSV Charmy_Trail_Colour_New = new ColorHSV(Charmy_Ball_Hue / 255.0F, HSL_Charmy_Trail_Colour.S, HSL_Charmy_Trail_Colour.V);
                    ColorHSV Cream_Trail_Colour_New = new ColorHSV(Cream_Ball_Hue / 255.0F, HSL_Cream_Trail_Colour.S, HSL_Cream_Trail_Colour.V);
                    ColorHSV Espio_Trail_Colour_New = new ColorHSV(Espio_Ball_Hue / 255.0F, HSL_Espio_Trail_Colour.S, HSL_Espio_Trail_Colour.V);
                    ColorHSV Knuckles_Trail_Colour_New = new ColorHSV(Knuckles_Ball_Hue / 255.0F, HSL_Knuckles_Trail_Colour.S, HSL_Knuckles_Trail_Colour.V);
                    ColorHSV Omega_Trail_Colour_New = new ColorHSV(Omega_Ball_Hue / 255.0F, HSL_Omega_Trail_Colour.S, HSL_Omega_Trail_Colour.V);
                    ColorHSV Rouge_Trail_Colour_New = new ColorHSV(Rouge_Ball_Hue / 255.0F, HSL_Rouge_Trail_Colour.S, HSL_Rouge_Trail_Colour.V);
                    ColorHSV Shadow_Trail_Colour_New = new ColorHSV(Shadow_Ball_Hue / 255.0F, HSL_Shadow_Trail_Colour.S, HSL_Shadow_Trail_Colour.V);
                    ColorHSV Sonic_Trail_Colour_New = new ColorHSV(Sonic_Ball_Hue / 255.0F, HSL_Sonic_Trail_Colour.S, HSL_Sonic_Trail_Colour.V);
                    ColorHSV Tails_Trail_Colour_New = new ColorHSV(Tails_Ball_Hue / 255.0F, HSL_Tails_Trail_Colour.S, HSL_Tails_Trail_Colour.V);
                    ColorHSV Vector_Trail_Colour_New = new ColorHSV(Vector_Ball_Hue / 255.0F, HSL_Vector_Trail_Colour.S, HSL_Vector_Trail_Colour.V);

                    // Convert back to RGB
                    RGB_Amy_Trail_Colour[z] = new ColorRGB(Amy_Trail_Colour_New);
                    RGB_Big_Trail_Colour[z] = new ColorRGB(Big_Trail_Colour_New);
                    RGB_Charmy_Trail_Colour[z] = new ColorRGB(Charmy_Trail_Colour_New);
                    RGB_Cream_Trail_Colour[z] = new ColorRGB(Cream_Trail_Colour_New);
                    RGB_Espio_Trail_Colour[z] = new ColorRGB(Espio_Trail_Colour_New);
                    RGB_Knuckles_Trail_Colour[z] = new ColorRGB(Knuckles_Trail_Colour_New);
                    RGB_Omega_Trail_Colour[z] = new ColorRGB(Omega_Trail_Colour_New);
                    RGB_Rouge_Trail_Colour[z] = new ColorRGB(Rouge_Trail_Colour_New);
                    RGB_Shadow_Trail_Colour[z] = new ColorRGB(Shadow_Trail_Colour_New);
                    RGB_Sonic_Trail_Colour[z] = new ColorRGB(Sonic_Trail_Colour_New);
                    RGB_Tails_Trail_Colour[z] = new ColorRGB(Tails_Trail_Colour_New);
                    RGB_Vector_Trail_Colour[z] = new ColorRGB(Vector_Trail_Colour_New);
                }

                while (Currently_Cycling_Trail_Hue)
                {
                    for (int c = 0; c < 255; c++) // C++, lol
                    {
                        // Write to Game Memory
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Trails, new byte[] { Convert.ToByte(RGB_Amy_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Trails + 1, new byte[] { Convert.ToByte(RGB_Amy_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Trails + 2, new byte[] { Convert.ToByte(RGB_Amy_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Trails, new byte[] { Convert.ToByte(RGB_Big_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Trails + 1, new byte[] { Convert.ToByte(RGB_Big_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Trails + 2, new byte[] { Convert.ToByte(RGB_Big_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Trails, new byte[] { Convert.ToByte(RGB_Charmy_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Trails + 1, new byte[] { Convert.ToByte(RGB_Charmy_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Trails + 2, new byte[] { Convert.ToByte(RGB_Charmy_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Trails, new byte[] { Convert.ToByte(RGB_Cream_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Trails + 1, new byte[] { Convert.ToByte(RGB_Cream_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Trails + 2, new byte[] { Convert.ToByte(RGB_Cream_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Trails, new byte[] { Convert.ToByte(RGB_Espio_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Trails + 1, new byte[] { Convert.ToByte(RGB_Espio_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Trails + 2, new byte[] { Convert.ToByte(RGB_Espio_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Trails, new byte[] { Convert.ToByte(RGB_Knuckles_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Trails + 1, new byte[] { Convert.ToByte(RGB_Knuckles_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Trails + 2, new byte[] { Convert.ToByte(RGB_Knuckles_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Trails, new byte[] { Convert.ToByte(RGB_Omega_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Trails + 1, new byte[] { Convert.ToByte(RGB_Omega_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Trails + 2, new byte[] { Convert.ToByte(RGB_Omega_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Trails, new byte[] { Convert.ToByte(RGB_Rouge_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Trails + 1, new byte[] { Convert.ToByte(RGB_Rouge_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Trails + 2, new byte[] { Convert.ToByte(RGB_Rouge_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Trails, new byte[] { Convert.ToByte(RGB_Shadow_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Trails + 1, new byte[] { Convert.ToByte(RGB_Shadow_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Trails + 2, new byte[] { Convert.ToByte(RGB_Shadow_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Trails, new byte[] { Convert.ToByte(RGB_Sonic_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Trails + 1, new byte[] { Convert.ToByte(RGB_Sonic_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Trails + 2, new byte[] { Convert.ToByte(RGB_Sonic_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Trails, new byte[] { Convert.ToByte(RGB_Tails_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Trails + 1, new byte[] { Convert.ToByte(RGB_Tails_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Trails + 2, new byte[] { Convert.ToByte(RGB_Tails_Trail_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Trails, new byte[] { Convert.ToByte(RGB_Vector_Trail_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Trails + 1, new byte[] { Convert.ToByte(RGB_Vector_Trail_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Trails + 2, new byte[] { Convert.ToByte(RGB_Vector_Trail_Colour[c].B * 255.0F) });
                        Thread.Sleep(Sleep_Delay);
                    }
                }

                // Restore Original Values to Game Memory
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Trails, new byte[] { Convert.ToByte(Amy_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Trails + 1, new byte[] { Convert.ToByte(Amy_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Trails + 2, new byte[] { Convert.ToByte(Amy_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Trails, new byte[] { Convert.ToByte(Big_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Trails + 1, new byte[] { Convert.ToByte(Big_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Big_Trails + 2, new byte[] { Convert.ToByte(Big_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Trails, new byte[] { Convert.ToByte(Charmy_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Trails + 1, new byte[] { Convert.ToByte(Charmy_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Charmy_Trails + 2, new byte[] { Convert.ToByte(Charmy_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Trails, new byte[] { Convert.ToByte(Cream_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Trails + 1, new byte[] { Convert.ToByte(Cream_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Cream_Trails + 2, new byte[] { Convert.ToByte(Cream_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Trails, new byte[] { Convert.ToByte(Espio_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Trails + 1, new byte[] { Convert.ToByte(Espio_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Trails + 2, new byte[] { Convert.ToByte(Espio_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Trails, new byte[] { Convert.ToByte(Knuckles_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Trails + 1, new byte[] { Convert.ToByte(Knuckles_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Knuckles_Trails + 2, new byte[] { Convert.ToByte(Knuckles_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Trails, new byte[] { Convert.ToByte(Omega_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Trails + 1, new byte[] { Convert.ToByte(Omega_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Omega_Trails + 2, new byte[] { Convert.ToByte(Omega_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Trails, new byte[] { Convert.ToByte(Rouge_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Trails + 1, new byte[] { Convert.ToByte(Rouge_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Rouge_Trails + 2, new byte[] { Convert.ToByte(Rouge_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Trails, new byte[] { Convert.ToByte(Shadow_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Trails + 1, new byte[] { Convert.ToByte(Shadow_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Shadow_Trails + 2, new byte[] { Convert.ToByte(Shadow_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Trails, new byte[] { Convert.ToByte(Sonic_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Trails + 1, new byte[] { Convert.ToByte(Sonic_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Trails + 2, new byte[] { Convert.ToByte(Sonic_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Trails, new byte[] { Convert.ToByte(Tails_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Trails + 1, new byte[] { Convert.ToByte(Tails_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Tails_Trails + 2, new byte[] { Convert.ToByte(Tails_Trail_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Trails, new byte[] { Convert.ToByte(Vector_Trail_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Trails + 1, new byte[] { Convert.ToByte(Vector_Trail_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Vector_Trails + 2, new byte[] { Convert.ToByte(Vector_Trail_Colour.B * 255.0F) });
            });

            // Run the Thread.
            HUE_Trail_Cycle_Thread.Start();
        }


        /// <summary>
        /// Cycles the HUE of Formation Barriers.
        /// </summary>
        private void Cycle_HUE_Character_Tornadoes()
        {
            // Setup the thread.
            Thread HUE_Tornado_Cycle_Thread = new Thread(x =>
            {
                // Retrieve Original Barrier Colours.
                Colorspace.ColorRGB Amy_Tornado_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Tornado, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Tornado + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Tornado + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Espio_Tornado_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Tornado, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Tornado + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Tornado + 2, 1) / 255.0F)
                );
                Colorspace.ColorRGB Sonic_Tornado_Colour = new ColorRGB
                (
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Tornado, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Tornado + 1, 1) / 255.0F),
                    Convert.ToDouble(Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Tornado + 2, 1) / 255.0F)
                );

                // Original Colours as HSL for Cycle Value Calculation.
                ColorHSV HSL_Amy_Tornado_Colour = new ColorHSV(Amy_Tornado_Colour);
                ColorHSV HSL_Espio_Tornado_Colour = new ColorHSV(Espio_Tornado_Colour);
                ColorHSV HSL_Sonic_Tornado_Colour = new ColorHSV(Sonic_Tornado_Colour);

                // Arrays of HUEs of colours.
                ColorRGB[] RGB_Amy_Tornado_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Espio_Tornado_Colour = new ColorRGB[255];
                ColorRGB[] RGB_Sonic_Tornado_Colour = new ColorRGB[255];

                // Precalculate cycle values (CPU Saving)
                for (int z = 0; z < 255; z++)
                {
                    // Obtain HUE Values
                    byte Amy_Ball_Hue = (byte)((HSL_Amy_Tornado_Colour.H * 255.0F) + z);
                    byte Espio_Ball_Hue = (byte)((HSL_Espio_Tornado_Colour.H * 255.0F) + z);
                    byte Sonic_Ball_Hue = (byte)((HSL_Sonic_Tornado_Colour.H * 255.0F) + z);

                    // Cycle Hue
                    ColorHSV Amy_Tornado_Colour_New = new ColorHSV(Amy_Ball_Hue / 255.0F, HSL_Amy_Tornado_Colour.S, HSL_Amy_Tornado_Colour.V);
                    ColorHSV Espio_Tornado_Colour_New = new ColorHSV(Espio_Ball_Hue / 255.0F, HSL_Espio_Tornado_Colour.S, HSL_Espio_Tornado_Colour.V);
                    ColorHSV Sonic_Tornado_Colour_New = new ColorHSV(Sonic_Ball_Hue / 255.0F, HSL_Sonic_Tornado_Colour.S, HSL_Sonic_Tornado_Colour.V);

                    // Convert back to RGB
                    RGB_Amy_Tornado_Colour[z] = new ColorRGB(Amy_Tornado_Colour_New);
                    RGB_Espio_Tornado_Colour[z] = new ColorRGB(Espio_Tornado_Colour_New);
                    RGB_Sonic_Tornado_Colour[z] = new ColorRGB(Sonic_Tornado_Colour_New);
                }

                while (Currently_Cycling_Tornado_Hue)
                {
                    for (int c = 0; c < 255; c++) // C++, lol
                    {
                        // Write to Game Memory
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Tornado, new byte[] { Convert.ToByte(RGB_Amy_Tornado_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Tornado + 1, new byte[] { Convert.ToByte(RGB_Amy_Tornado_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Tornado + 2, new byte[] { Convert.ToByte(RGB_Amy_Tornado_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Tornado, new byte[] { Convert.ToByte(RGB_Espio_Tornado_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Tornado + 1, new byte[] { Convert.ToByte(RGB_Espio_Tornado_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Tornado + 2, new byte[] { Convert.ToByte(RGB_Espio_Tornado_Colour[c].B * 255.0F) });

                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Tornado, new byte[] { Convert.ToByte(RGB_Sonic_Tornado_Colour[c].R * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Tornado + 1, new byte[] { Convert.ToByte(RGB_Sonic_Tornado_Colour[c].G * 255.0F) });
                        Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Tornado + 2, new byte[] { Convert.ToByte(RGB_Sonic_Tornado_Colour[c].B * 255.0F) });

                        Thread.Sleep(Sleep_Delay);
                    }
                }

                // Restore Original Values to Game Memory
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Tornado, new byte[] { Convert.ToByte(Amy_Tornado_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Tornado + 1, new byte[] { Convert.ToByte(Amy_Tornado_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Amy_Tornado + 2, new byte[] { Convert.ToByte(Amy_Tornado_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Tornado, new byte[] { Convert.ToByte(Espio_Tornado_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Tornado + 1, new byte[] { Convert.ToByte(Espio_Tornado_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Espio_Tornado + 2, new byte[] { Convert.ToByte(Espio_Tornado_Colour.B * 255.0F) });

                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Tornado, new byte[] { Convert.ToByte(Sonic_Tornado_Colour.R * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Tornado + 1, new byte[] { Convert.ToByte(Sonic_Tornado_Colour.G * 255.0F) });
                Program.Sonic_Heroes_Process.WriteMemory((IntPtr)SonicHeroesVariables.Game_Colour_CharacterColours.Sonic_Tornado + 2, new byte[] { Convert.ToByte(Sonic_Tornado_Colour.B * 255.0F) });

            });

            // Run the Thread.
            HUE_Tornado_Cycle_Thread.Start();
        }
    }
}
