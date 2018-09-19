using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SonicHeroes.Misc;
using SonicHeroes.Controller;

namespace HeroesModLoaderConfig
{
    public class Sonic_Heroes_Program
    {
        /// <summary>
        /// Store the Sonic Heroes Configuration File
        /// </summary>
        public SonicHeroes.Misc.SonicHeroes_Miscallenous.Sonic_Heroes_Configuration_File ConfigFile = SonicHeroes.Misc.SonicHeroes_Miscallenous.Load_Configuration_File(); // :pads the Sonic Heroes configuration file.
        public byte[] SonicHeroesExecutable = SonicHeroes.Misc.SonicHeroes_Miscallenous.Get_Executable_As_Array(Program.Executable_Path);
        public SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Handle = new SonicHeroes.Controller.DirectInput_Joystick_Manager();
    }
}
