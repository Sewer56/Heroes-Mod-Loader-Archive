using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace HeroesModLoaderConfig
{
    class Fonts
    {

        /// A collection of fonts to be used throughout the application. 
        public Font Hex_TitleBar_TinyUI;
        public Font Hex_Special_Text_TinyUI;
        public Font Hex_Special_Text_II_TinyUI;
        public Font Hex_ComboBox_TinyUI;
        public Font Hex_Textbox_Small_TinyUI;
        public Font Hex_Textbox_Smaller_TinyUI;
        public Font Hex_NumericUpDown_TinyUI;
        public Font Hex_Special_Text_III_TinyUI;
        public Font Hex_Special_Text_IV_TinyUI;

        /// <summary>
        /// Generate a private font collection in which the Roboto font files will be loaded into, the individual font styles will be assignes based on the members of this 
        /// </summary>
        PrivateFontCollection Roboto = new PrivateFontCollection();                                                 

                                                                                                                    
        public void SetupFonts()                                                                                    
        {                                                                                                           
            try
            {
                // Try adding the individual font files.
                Roboto.AddFontFile(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Fonts\RobotoCondensed-Regular.ttf");   
                Roboto.AddFontFile(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Fonts\RobotoCondensed-Italic.ttf");    
                Roboto.AddFontFile(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Fonts\RobotoCondensed-Bold.ttf");      
                Roboto.AddFontFile(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Fonts\RobotoCondensed-BoldItalic.ttf");

                Roboto.AddFontFile(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Fonts\Roboto-Regular.ttf");            
                Roboto.AddFontFile(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Fonts\Roboto-Bold.ttf");               
                Roboto.AddFontFile(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Fonts\Roboto-Light.ttf");              
                Roboto.AddFontFile(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Fonts\Roboto-Thin.ttf");               

                // Assign the new fonts based off of the individual font files.
                // See Hex-UI-Theming.cs : 75
                Hex_TitleBar_TinyUI = new Font(Roboto.Families[2], 20.25F);
                Hex_Special_Text_TinyUI = new Font(Roboto.Families[2], 18F);
                Hex_ComboBox_TinyUI = new Font(Roboto.Families[2], 9.75F);
                Hex_Textbox_Small_TinyUI = new Font(Roboto.Families[2], 14.25F);
                Hex_Textbox_Smaller_TinyUI = new Font(Roboto.Families[2], 12F);
                Hex_Special_Text_II_TinyUI = new Font(Roboto.Families[2], 14.25F);
                Hex_Special_Text_IV_TinyUI = new Font(Roboto.Families[1], 14.25F);
                Hex_NumericUpDown_TinyUI = new Font(Roboto.Families[1], 9.75F);
                Hex_Special_Text_III_TinyUI = new Font(Roboto.Families[2], 12F);
            }
            catch (Exception) { MessageBox.Show("The font files could not be found or loaded. You are a monster..."); }
        }
    }
}
