using System.Runtime.InteropServices;

namespace Heroes_Sample_Mod
{
    public static class Invoke_External_Class
    {
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern int DwmEnableComposition(bool fEnable);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport(@"Tweakbox.dll")]
        public static extern void Play_Sound(int Sound_Effect_BANK_And_ID, int Unknown_Sound_Stream_Address, byte Unknown_AlwaysZero, int Unknown_AlwaysZero_II);

        [DllImport(@"Tweakbox.dll")]
        public static extern void Play_Song(int Song_Name_Pointer);

        [DllImport(@"Tweakbox.dll")]
        public static extern void Initialize_ASM_Widescreen_Injection();

        [DllImport(@"Tweakbox.dll")]
        public static extern void Load_Level_Function(int Level_ID);

        [DllImport(@"Tweakbox.dll")]
        public static extern int Load_Level_Function_II(int Game_State_ID);

        [DllImport(@"Tweakbox.dll")]
        public static extern int Load_Test_Level_Function(int Game_State_ID);

        [DllImport(@"Tweakbox.dll")]
        public static extern int Load_Debug_Function();

        [DllImport(@"Tweakbox.dll")]
        public static extern int Font_Load_Test_Function_Method();

        [DllImport(@"Tweakbox.dll")]
        public static extern int Load_Main_Menu_Function();

        [DllImport(@"Tweakbox.dll")]
        public static extern int Load_AFS_Language_File();

        [DllImport(@"Tweakbox.dll")]
        public static extern int Load_File_Into_Memory(int Pointer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PointerX">Pointer pointed at by 0x00A2F8B0</param>
        /// <returns></returns>
        [DllImport(@"Tweakbox.dll")]
        public static extern int Reload_Bank3(int PointerX);

        [DllImport(@"Tweakbox.dll")]
        public static extern int Change_Game_State(int Game_State_ID);

        [DllImport(@"Tweakbox.dll")]
        public static extern int Test_Method_X();


        [DllImport(@"Tweakbox.dll")]
        public static extern int Exit_Stage_X();

        [DllImport(@"Tweakbox.dll")]
        public static extern int Experiment_X();
    }
}
