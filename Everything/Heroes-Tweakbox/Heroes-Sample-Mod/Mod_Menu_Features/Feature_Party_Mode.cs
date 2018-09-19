using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colorspace;
using System.Threading;
using SharpDX.Direct2D1;
using SonicHeroes.Memory;
using SonicHeroes.Variables;
using SharpDX.Mathematics.Interop;
using System.IO;
using System.Windows.Forms;

namespace Heroes_Sample_Mod
{
    /// <summary>
    /// Summons a transparent overlay which shines all of the colours of the rainbow.
    /// </summary>
    public class Feature_Party_Mode
    {
        /// <summary>
        /// Is party mode enabled?
        /// </summary>
        bool PartyModeEnabled = false; 
        
        /// <summary>
        /// Current colour index for array iterations. Smooth HUE cycling relies on constant Integet Overflows.
        /// </summary>
        byte CurrentColourIndex = 0;

        /// <summary>
        /// Array storing the current colours with which the player can overlay the screen.
        /// </summary>
        ColorRGB[] BaseColour_Colours = new ColorRGB[256];

        /// <summary>
        /// The opacity of the "Party Mode" overlay.
        /// </summary>
        float Party_Mode_Opacity = 0.30F;

        /// <summary>
        /// Pointer to the Sonic R track, "Diamond in The Sky"
        /// </summary>
        int Pointer_DiamondInTheSky;

        /// <summary>
        /// Sets up everything necessary for "party mode". All of the colours.
        /// </summary>
        public Feature_Party_Mode()
        {
            // Initialize "Diamond in The Sky"
            string[] File_List = Directory.GetFiles(Environment.CurrentDirectory + @"\dvdroot\bgm\", "*.adx");

            for (int x = 0; x < File_List.Length; x++)
            {
                // If "Diamong in the Sky" is present within the strings.
                if (File_List[x].Contains("SR_Partymode.adx"))
                {
                    // Get the file name of the track (just in case).
                    string File_Name_Only = Path.GetFileName(File_List[x]); // Get File Name Only.

                    // Get the bytes to be written into game memory & allocate memory.
                    byte[] Memory_Bytes = Encoding.ASCII.GetBytes(File_Name_Only); // Get bytes to write file name in memory.
                    IntPtr Write_Address = Program.Sonic_Heroes_Process.AllocateMemory(Memory_Bytes.Length); // Memory address.

                    // Write the file onto game memory.
                    Program.Sonic_Heroes_Process.WriteMemory(Write_Address, Memory_Bytes); // Write the file name to memory.

                    // Store address
                    Pointer_DiamondInTheSky = (int)Write_Address;

                    // Break out from the loop.
                    break;
                }
            }

            // Defines the starting colour at which the colour cycling begins at;
            ColorRGB BaseColour = new ColorRGB(1F, 0F, 0F);
            ColorHSV BaseColourHSV = new ColorHSV(BaseColour); // Convert to HSV

            // Precalculate cycle values (CPU Saving)
            for (int z = 0; z < 256; z++)
            {
                // Obtain HUE Values
                byte BaseColour_Hue = (byte)((BaseColourHSV.H * 255.0F) + z); // Convert to max 255 and increment 1.

                // Cycle Hue                                       // Back to out of 1.
                ColorHSV Current_BaseColour_Hue = new ColorHSV(BaseColour_Hue / 255.0F, BaseColourHSV.S, BaseColourHSV.V);

                // Convert back to RGB
                BaseColour_Colours[z] = new ColorRGB(Current_BaseColour_Hue);
            }
        }

        /// <summary>
        /// Toggles party mode status, adds/removes an overlay covering the screen which will rotate all of the colours.
        /// </summary>
        public void Toggle_Party_Mode()
        {
            if (PartyModeEnabled)
            {
                // Remove the additional function pointer assigned to the delegate to render the box during the game menu editing.
                Program.Sonic_Heroes_Overlay.direct2DRenderMethod -= Draw_Overlay;
                PartyModeEnabled = false;
            }
            else
            {
                // Add a function pointer to the delegate delegate to render the box during the game menu editing.
                Program.Sonic_Heroes_Overlay.direct2DRenderMethod += Draw_Overlay;

                // Play the song "Diamond in The Sky"
                Invoke_External_Class.Play_Song(Pointer_DiamondInTheSky);

                // Disable The Character voices if they are enabled.
                if (Program.Feature_Toggle_Character_Chatter_X.Get_CharacterChatter()) { Program.Feature_Toggle_Character_Chatter_X.Toggle_CharacterChatter(); }
                if (Program.Feature_Toggle_Character_Chatter_X.Get_VoiceComments()) { Program.Feature_Toggle_Character_Chatter_X.Toggle_CharacterCommentChatter(); }

                // Go Go Go RGB
                Program.Feature_Cycle_RGB_Colours_X.Toggle_HUE_Cycle_All_Combined();
                PartyModeEnabled = true;
            }
        }

        /// <summary>
        /// Returns whether party mode is enabled or disabled.
        /// </summary>
        /// <returns></returns>
        public bool Get_ToggleState() { return PartyModeEnabled; }

        /// <summary>
        /// Draws the overlay if necessary, i.e. if character is in a level.
        /// </summary>
        /// <param name="DirectX_Graphics_Window"></param>
        private void Draw_Overlay(WindowRenderTarget DirectX_Graphics_Window)
        {
            // Get Information of whether the overlay should be shown.
            byte Is_Currently_In_Level = Program.Sonic_Heroes_Process.ReadMemory<byte>((IntPtr)SonicHeroesVariables.Game_CurrentState.CurrentlyInLevel, 1);

            // If the player is in a stage.
            if (Is_Currently_In_Level == 1) { Draw_Window(DirectX_Graphics_Window); Invoke_External_Class.Play_Song(Pointer_DiamondInTheSky); }
        }

        /// <summary>
        /// Draws the XYZ Overlay Window
        /// </summary>
        /// <param name="DirectX_Graphics_Window"></param>
        private void Draw_Window(WindowRenderTarget DirectX_Graphics_Window)
        {
            try
            {
                // Increment index.
                CurrentColourIndex += 3;

                // Fill BG Rectangle
                DirectX_Graphics_Window.FillRectangle
                (
                    new RawRectangleF(0, 0, Program.Sonic_Heroes_Overlay.overlayWinForm.Width, Program.Sonic_Heroes_Overlay.overlayWinForm.Height), 
                    new SolidColorBrush
                    (
                        DirectX_Graphics_Window, 
                        new RawColor4
                        (
                            (float)(BaseColour_Colours[CurrentColourIndex].R), 
                            (float)(BaseColour_Colours[CurrentColourIndex].G), 
                            (float)(BaseColour_Colours[CurrentColourIndex].B), 
                            Party_Mode_Opacity
                        )
                    )
                );
            }
            catch { }
        }
    }
}
