using System;
using System.Threading;
using System.Windows.Forms;

namespace HeroesModLoaderConfig
{
    public partial class ControllerScreenTwo : Form
    {
        /// <summary>
        /// By turning on compositing for the form, we can kill the flickering of dark control overdraws as we draw our own controls in an interactive fashion..
        /// </summary>
        protected override CreateParams CreateParams { get { CreateParams cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; } }

        public ControllerScreenTwo()
        {
            InitializeComponent();
            CenterToScreen();
            this.BringToFront();
        }

        /// <summary>
        /// Updates the "Timeout" Counter when the user presses the button. Basically updates text for 9 seconds, 10th is taken by actual result. Simultaneously waits for user to press any button for mapping purposes.
        /// </summary>
        private void Button_GetControllerButtonInput(Button Change_Message_Button, int Timeout)
        {
            // Text object which will be set by the thread, if it is not empty, we can stop counting down.
            string ButtonText = "";

            // This thread will determing which button on the controller has been pressed, and set the button text accordingly.
            Thread GetControllerInputThread = new Thread
                (
                    () => ButtonText = "Button " + Program.Controller_Handle.Button_Mapping_GUI_Set_Button_With_Timeout_Any_Controller(Timeout)
                );
            GetControllerInputThread.Start();

            // A countdown is ran here from 10 to 0 seconds, the button will be updated with the timeout text as long as the ButtonText is not set.
            int CentiSeconds = Timeout * 10; // Multiply by 10 because the refresh occurs 10 timer per second.
            while (CentiSeconds > 0)
            {
                /// Unless you force refresh (Invalidate + Update), the program wouldn't be willing to upade the control manually.
                if (ButtonText != "") { Change_Message_Button.Text = ButtonText; Change_Message_Button.Refresh(); return; }
                Change_Message_Button.Text = "Timeout | " + CentiSeconds; Change_Message_Button.Refresh();
                CentiSeconds -= 1;
                Thread.Sleep(100);
            }
            Change_Message_Button.Text = "Button 255";

        }

        private void Btn_StartPause_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_StartPause, 10); }
        private void Btn_Jump_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Jump, 10); }
        private void Btn_Action_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Action, 10); }
        private void Btn_TeamBlast_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_TeamBlast, 10); }
        private void Btn_FormationL_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_FormationL, 10); }
        private void Btn_FormationR_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_FormationR, 10); }
        private void Btn_CameraL_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_CameraL, 10); }
        private void Btn_CameraR_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_CameraR, 10); }

        /// <summary>
        /// Save all of the set buttons to the controller button array when the user leaves the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControllerScreen_Leave(object sender, EventArgs e)
        {
            // Try-catch is used in the case user goes to another tab without setting a button on one of the controls.
            try { Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[0] = Convert.ToByte(Btn_StartPause.OwnerDrawText.Substring(Btn_StartPause.OwnerDrawText.IndexOf(" "))); } catch { }
            try { Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[1] = Convert.ToByte(Btn_Jump.OwnerDrawText.Substring(Btn_Jump.OwnerDrawText.IndexOf(" "))); } catch { }
            try { Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[2] = Convert.ToByte(Btn_Action.OwnerDrawText.Substring(Btn_Action.OwnerDrawText.IndexOf(" "))); } catch { }
            try { Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[3] = Convert.ToByte(Btn_FormationR.OwnerDrawText.Substring(Btn_FormationR.OwnerDrawText.IndexOf(" ")));} catch { }
            try { Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[4] = Convert.ToByte(Btn_FormationL.OwnerDrawText.Substring(Btn_FormationL.OwnerDrawText.IndexOf(" ")));} catch { }
            try { Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[5] = Convert.ToByte(Btn_TeamBlast.OwnerDrawText.Substring(Btn_TeamBlast.OwnerDrawText.IndexOf(" ")));} catch { }
            try { Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[6] = Convert.ToByte(Btn_CameraR.OwnerDrawText.Substring(Btn_CameraR.OwnerDrawText.IndexOf(" ")));} catch { }
            try { Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[7] = Convert.ToByte(Btn_CameraL.OwnerDrawText.Substring(Btn_CameraL.OwnerDrawText.IndexOf(" ")));} catch { }
        }

        /// <summary>
        /// When the user enters the screen, load all of the set buttons to the appropriate values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControllerScreen_Enter(object sender, EventArgs e)
        { 
            Btn_StartPause.Text = "Button " + Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[0];
            Btn_Jump.Text = "Button " + Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[1];
            Btn_Action.Text = "Button " + Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[2];
            Btn_FormationR.Text = "Button " + Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[3];
            Btn_FormationL.Text = "Button " + Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[4];
            Btn_TeamBlast.Text = "Button " + Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[5];
            Btn_CameraR.Text = "Button " + Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[6];
            Btn_CameraL.Text = "Button " + Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ControllerTwo[7];
        }
    }
}
