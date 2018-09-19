using System;
using SonicHeroes.Variables;
using System.Windows.Forms;
using SharpDX.DirectInput;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;

namespace HeroesModLoaderConfig
{
    public partial class Controller_Screen_Loader : Form
    {
        /// <summary>
        /// This thread gets the controller inputs live and performs actions on the UI thread, e.g. draw, in real time.
        /// </summary>
        Thread RefreshControllerThread;

        /// <summary>
        /// By turning on compositing for the form, we can kill the flickering of dark control overdraws as we draw our own controls in an interactive fashion..
        /// </summary>
        protected override CreateParams CreateParams { get { CreateParams cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; } }

        /// <summary>
        /// ID of the current controller, used in the draw analogue stick operation.
        /// </summary>
        public static int Current_Controller_ID;

        /// <summary>
        /// Used in a cross thread invoke to draw a rectangle;
        /// </summary>
        public delegate void Fill_Rectangle_Delegate(Point Location_To_Move, Button ControlToDrawOn);

        // Store all controller GUIDs in a list.
        List<string> ControllerGUID = new List<string>(Program.Controller_Handle.PlayerControllers.Count);

        /// <summary>
        /// Constructor for the tweaks screen.
        /// </summary>
        public Controller_Screen_Loader()
        {
            InitializeComponent();
            CenterToScreen();
            this.BringToFront();
        }

        /// <summary>
        /// Reads analogue stick data on a separate thread and invokes UI thread to update the current coordinates.
        /// </summary>
        public void UpdateAnalogueSticks()
        {

            // Height and width are equal. // Both controls are the same size.
            int ControllerAnalogRange = this.FakeBtn_Test_Left_Stick.Size.Height / 2;
            // Pointer Size * 1.5
            const int Pointer_Size_Offset = (int)(8 * 1.5F);
            Point ControllerAnalogCenter_LeftStick = new Point(FakeBtn_Test_Left_Stick.Location.X + ControllerAnalogRange / 2, FakeBtn_Test_Left_Stick.Location.Y + ControllerAnalogRange / 2);
            Point ControllerAnalogCenter_RightStick = new Point(FakeBtn_Test_Right_Stick.Location.X + ControllerAnalogRange / 2, FakeBtn_Test_Right_Stick.Location.Y + ControllerAnalogRange / 2);

            while (true)
            {
                int Left_Stick_X = Program.Controller_Handle.PlayerControllers[Current_Controller_ID].Get_Axis_State(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic.X);
                int Left_Stick_Y = Program.Controller_Handle.PlayerControllers[Current_Controller_ID].Get_Axis_State(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic.Y);

                int Right_Stick_X = Program.Controller_Handle.PlayerControllers[Current_Controller_ID].Get_Axis_State(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic.Rotation_X);
                int Right_Stick_Y = Program.Controller_Handle.PlayerControllers[Current_Controller_ID].Get_Axis_State(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic.Rotation_Y);

                /// Calculate offsets.

                Left_Stick_X = Convert.ToInt16((Left_Stick_X / 1000.0F) * ControllerAnalogRange);
                Left_Stick_Y = Convert.ToInt16((Left_Stick_Y / 1000.0F) * ControllerAnalogRange);

                Right_Stick_X = Convert.ToInt16((Right_Stick_X / 1000.0F) * ControllerAnalogRange);
                Right_Stick_Y = Convert.ToInt16((Right_Stick_Y / 1000.0F) * ControllerAnalogRange);

                Point Controller_Left_Stick_Current_Position = new Point(ControllerAnalogCenter_LeftStick.X + Left_Stick_X + Pointer_Size_Offset, ControllerAnalogCenter_LeftStick.Y + Left_Stick_Y + Pointer_Size_Offset);
                Point Controller_Right_Stick_Current_Position = new Point(ControllerAnalogCenter_RightStick.X + Right_Stick_X + Pointer_Size_Offset, ControllerAnalogCenter_RightStick.Y + Right_Stick_Y + Pointer_Size_Offset);
                // Rectangle ControllerAnalogPosition_RightStick = new Rectangle(ControllerAnalogCenter_LeftStick, new Size(2, 2));
                Btn_Pointer_Left_Stick.Invoke(new Fill_Rectangle_Delegate(UIThread_MoveRectangle), new object[] { Controller_Left_Stick_Current_Position, Btn_Pointer_Left_Stick } );
                Btn_Pointer_Right_Stick.Invoke(new Fill_Rectangle_Delegate(UIThread_MoveRectangle), new object[] { Controller_Right_Stick_Current_Position, Btn_Pointer_Right_Stick } );

                Thread.Sleep(16);
            }

        }

        /// <summary>
        /// Method called by delegate in controller thread to move the analog stick pointer. Should override OnPaint() but I am well, lazy.
        /// </summary>
        /// <param name="Rectangle_To_Fill"></param>
        public void UIThread_MoveRectangle(Point Location_To_Move, Button Button_To_Move)
        {
            Button_To_Move.Location = Location_To_Move;
        }

        /// <summary>
        /// Updates the "Timeout" Counter when the user presses the button. Basically updates text for 9 seconds, 10th is taken by actual result. Simultaneously waits for user to press any button for mapping purposes.
        /// </summary>
        private void Button_GetControllerButtonInput(Button Change_Message_Button, int Timeout, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic Button_To_Assign)
        {
            // Text object which will be set by the thread, if it is not empty, we can stop counting down.
            string ButtonText = "";

            // This thread will determing which button on the controller has been pressed, and set the button text accordingly.
            int ControllerIndex = TinyUI_ComboBoxSmall_CurrentController.SelectedIndex;
            Thread GetControllerInputThread = new Thread
                (
                    () => ButtonText = "Btn " + Program.Controller_Handle.Button_Mapping_GUI_Set_Button_With_Timeout(Timeout, ControllerIndex, Button_To_Assign)
                );
            GetControllerInputThread.Start();

            // A countdown is ran here from 10 to 0 seconds, the button will be updated with the timeout text as long as the ButtonText is not set.
            int CentiSeconds = Timeout * 10; // Multiply by 10 because the refresh occurs 10 timer per second.
            while (CentiSeconds > 0)
            {
                /// Unless you force refresh (Invalidate + Update), the program wouldn't be willing to upade the control manually.
                if (ButtonText != "") { Change_Message_Button.Text = ButtonText; Change_Message_Button.Refresh(); return; }
                Change_Message_Button.Text = Convert.ToString(CentiSeconds); Change_Message_Button.Refresh();
                CentiSeconds -= 1;
                Thread.Sleep(100);
            }
            Change_Message_Button.Text = "Btn 255";
        }

        /// 
        /// Button Assignments
        ///        
        private void Btn_Face_A_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Face_A, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Button_A); }
        private void Btn_Face_B_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Face_B, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Button_B); }
        private void Btn_Face_X_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Face_X, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Button_X); }
        private void Btn_Face_Y_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Face_Y, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Button_Y);}
        private void Btn_Back_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Back, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Button_Back); }
        private void Btn_Guide_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Guide, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Optional_Button_Guide); }
        private void Btn_Start_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Start, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Button_Start); }
        private void Btn_Left_Stick_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Left_Stick, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Button_L3); }
        private void Btn_Right_Stick_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Right_Stick, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Button_R3); }
        private void Btn_Left_Button_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Left_Button, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Button_L1); }
        private void Btn_Right_Button_Click(object sender, EventArgs e) { Button_GetControllerButtonInput(Btn_Right_Button, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Buttons_Generic.Button_R1); }

        /// 
        /// Axis Assignments
        /// 

        /// <summary>
        /// Updates the "Timeout" Counter when the user presses the button. Basically updates text for 9 seconds, 10th is taken by actual result. Simultaneously waits for user to press any button for mapping purposes.
        /// </summary>
        private void Button_GetControllerAxisInput(Button Change_Message_Button, int Timeout, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic Axis_To_Assign)
        {
            // Text object which will be set by the thread, if it is not empty, we can stop counting down.
            string ButtonText = "";

            // This thread will determing which button on the controller has been pressed, and set the button text accordingly.
            int ControllerIndex = TinyUI_ComboBoxSmall_CurrentController.SelectedIndex;
            Thread GetControllerInputThread = new Thread
                (
                    () =>
                    {
                        var Result = Program.Controller_Handle.Button_Mapping_GUI_Set_Axis_With_Timeout(Timeout, ControllerIndex, Axis_To_Assign);
                        string PlusMinus = "";
                        if (Result.Item2) { PlusMinus = "+"; } else { PlusMinus = "-"; }
                        
                        ButtonText = Enum.GetName(typeof(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic), Result.Item1) + PlusMinus;
                    }
                );
            GetControllerInputThread.Start();

            // A countdown is ran here from 10 to 0 seconds, the button will be updated with the timeout text as long as the ButtonText is not set.
            int CentiSeconds = Timeout * 10; // Multiply by 10 because the refresh occurs 10 timer per second.
            while (CentiSeconds > 0)
            {
                /// Unless you force refresh (Invalidate + Update), the program wouldn't be willing to upade the control manually.
                if (ButtonText != "") { Change_Message_Button.Text = ButtonText; Change_Message_Button.Refresh(); return; }
                Change_Message_Button.Text = Convert.ToString(CentiSeconds); Change_Message_Button.Refresh();
                CentiSeconds -= 1;
                Thread.Sleep(100);
            }
            Change_Message_Button.Text = "Null";
        }

        /// 
        /// Assign Axis
        /// 
        private void Btn_Left_Trigger_Click(object sender, EventArgs e) { if (Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex].Information.Type != DeviceType.Keyboard) Button_GetControllerAxisInput(Btn_Left_Trigger, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic.Z); }
        private void Btn_Right_Trigger_Click(object sender, EventArgs e) { if (Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex].Information.Type != DeviceType.Keyboard) Button_GetControllerAxisInput(Btn_Right_Trigger, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic.Rotation_Z); }

        private void Btn_Left_Stick_Vertical_Click(object sender, EventArgs e) { if (Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex].Information.Type != DeviceType.Keyboard) Button_GetControllerAxisInput(Btn_Left_Stick_Vertical, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic.Y); }
        private void Btn_Left_Stick_Horizontal_Click(object sender, EventArgs e) { if (Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex].Information.Type != DeviceType.Keyboard) Button_GetControllerAxisInput(Btn_Left_Stick_Horizontal, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic.X); }
        private void Btn_Right_Stick_Vertical_Click(object sender, EventArgs e) { if (Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex].Information.Type != DeviceType.Keyboard) Button_GetControllerAxisInput(Btn_Right_Stick_Vertical, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic.Rotation_Y); }
        private void Btn_Right_Stick_Horizontal_Click(object sender, EventArgs e) { if (Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex].Information.Type != DeviceType.Keyboard) Button_GetControllerAxisInput(Btn_Right_Stick_Horizontal, 10, SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic.Rotation_X); }

        /// 
        /// Switch Current Controller To Read
        /// 
        private void TinyUI_ComboBoxSmall_CurrentController_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex].Load_Controller_Configuration(); } catch { } // Exception FirstTimeShownException

            // Save current controller.
            Program.Controller_Handle.PlayerControllers[Current_Controller_ID].Save_Controller_Configuration();

            // Swtich current controller.
            Current_Controller_ID = TinyUI_ComboBoxSmall_CurrentController.SelectedIndex;
            Load_Controller_Settings_Into_Control_Text();
        }

        /// <summary>
        /// Assigns controller values into appropriate text on buttons.
        /// </summary>
        private void Load_Controller_Settings_Into_Control_Text()
        {
            /// Ayy lmao
            SonicHeroes.Controller.Sonic_Heroes_Joystick CurrentGamepad = Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex];

            Btn_Face_A.Text = "Btn " + CurrentGamepad.Button_Mappings.Button_A;
            Btn_Face_B.Text = "Btn " + CurrentGamepad.Button_Mappings.Button_B;
            Btn_Face_X.Text = "Btn " + CurrentGamepad.Button_Mappings.Button_X;
            Btn_Face_Y.Text = "Btn " + CurrentGamepad.Button_Mappings.Button_Y;
            Btn_Back.Text = "Btn " + CurrentGamepad.Button_Mappings.Button_Back;
            Btn_Guide.Text = "Btn " + CurrentGamepad.Button_Mappings.Optional_Button_Guide;
            Btn_Start.Text = "Btn " + CurrentGamepad.Button_Mappings.Button_Start;
            Btn_Left_Stick.Text = "Btn " + CurrentGamepad.Button_Mappings.Button_L3;
            Btn_Right_Stick.Text = "Btn " + CurrentGamepad.Button_Mappings.Button_R3;
            Btn_Left_Button.Text = "Btn " + CurrentGamepad.Button_Mappings.Button_L1;
            Btn_Right_Button.Text = "Btn " + CurrentGamepad.Button_Mappings.Button_R1;

            if (CurrentGamepad.Information.Type == DeviceType.Keyboard)
            {
                Btn_Left_Stick_Horizontal.OwnerDrawText = "N/A";
                Btn_Left_Stick_Vertical.OwnerDrawText = "N/A";
                Btn_Right_Stick_Horizontal.OwnerDrawText = "N/A";
                Btn_Left_Trigger.OwnerDrawText = "N/A";
                Btn_Right_Trigger.OwnerDrawText = "N/A";
                Btn_Right_Stick_Vertical.OwnerDrawText = "N/A";
            }
            else
            {
                // For some reason directly assigning .Text does not work, need to export to a temporary string.
                string Temp = Enum.GetName(typeof(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic), CurrentGamepad.Axis_Mappings.LeftStick_X);
                Btn_Left_Stick_Horizontal.OwnerDrawText = Temp;

                Temp = Enum.GetName(typeof(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic), CurrentGamepad.Axis_Mappings.LeftStick_Y);
                Btn_Left_Stick_Vertical.OwnerDrawText = Temp;

                Temp = Enum.GetName(typeof(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic), CurrentGamepad.Axis_Mappings.RightStick_X);
                Btn_Right_Stick_Horizontal.OwnerDrawText = Temp;

                Temp = Enum.GetName(typeof(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic), CurrentGamepad.Axis_Mappings.RightStick_Y);
                Btn_Right_Stick_Vertical.OwnerDrawText = Temp;

                Temp = Enum.GetName(typeof(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic), CurrentGamepad.Axis_Mappings.LeftTrigger_Pressure);
                Btn_Left_Trigger.OwnerDrawText = Temp;

                Temp = Enum.GetName(typeof(SonicHeroes.Controller.Sonic_Heroes_Joystick.Controller_Axis_Generic), CurrentGamepad.Axis_Mappings.RightTrigger_Pressure);
                Btn_Right_Trigger.OwnerDrawText = Temp;

                if (CurrentGamepad.Axis_Mappings.LeftStick_X_IsReversed) { Btn_Left_Stick_Horizontal.OwnerDrawText += "+"; } else { Btn_Left_Stick_Horizontal.OwnerDrawText += "-"; }
                if (CurrentGamepad.Axis_Mappings.LeftStick_Y_IsReversed) { Btn_Left_Stick_Vertical.OwnerDrawText += "+"; } else { Btn_Left_Stick_Vertical.OwnerDrawText += "-"; }
                if (CurrentGamepad.Axis_Mappings.RightStick_X_IsReversed) { Btn_Right_Stick_Horizontal.OwnerDrawText += "+"; } else { Btn_Right_Stick_Horizontal.OwnerDrawText += "-"; }
                if (CurrentGamepad.Axis_Mappings.RightStick_Y_IsReversed) { Btn_Right_Stick_Vertical.OwnerDrawText += "+"; } else { Btn_Right_Stick_Vertical.OwnerDrawText += "-"; }
                if (CurrentGamepad.Axis_Mappings.LeftTrigger_Pressure_IsReversed) { Btn_Left_Trigger.OwnerDrawText += "+"; } else { Btn_Left_Trigger.OwnerDrawText += "-"; }
                if (CurrentGamepad.Axis_Mappings.RightTrigger_Pressure_IsReversed) { Btn_Right_Trigger.OwnerDrawText += "+"; } else { Btn_Right_Trigger.OwnerDrawText += "-"; }
            }

        }

        /// <summary>
        /// When we leave the region/area of the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Controller_Screen_Loader_Leave(object sender, EventArgs e)
        {
            RefreshControllerThread.Abort();
            // Save current controller.

            try { Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex].Save_Controller_Configuration(); } catch { }
        }

        /// <summary>
        /// When we enter the region/area of the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Controller_Screen_Loader_Enter(object sender, EventArgs e)
        {
            if (Program.Controller_Handle.PlayerControllers.Count == 0)
            {
                try { Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex].Load_Controller_Configuration(); Load_Controller_Settings_Into_Control_Text(); } catch { } // Exception FirstTimeShownException 
            }
            else
            {
                RefreshControllerThread = new Thread(() => UpdateAnalogueSticks());
                RefreshControllerThread.Start();
            }
        }


        /// <summary>
        /// Load stuff when user clicks on the form first time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Controller_Screen_Loader_Shown(object sender, EventArgs e)
        {
            /// Remember: Clear all fields on leaving page.

            /// Populate all controller choices.
            for (int x = 0; x < Program.Controller_Handle.PlayerControllers.Count; x++)
            {
                TinyUI_ComboBoxSmall_CurrentController.Items.Add(String.Format("[{0}] {1} ({2})", x, Program.Controller_Handle.PlayerControllers[x].Information.ProductName, Program.Controller_Handle.PlayerControllers[x].Information.ProductGuid));
                ControllerGUID.Add(Program.Controller_Handle.PlayerControllers[x].Information.ProductGuid.ToString());
            }

            /// Select first item if available.
            if (TinyUI_ComboBoxSmall_CurrentController.Items.Count > 0) { TinyUI_ComboBoxSmall_CurrentController.SelectedIndex = 0; Current_Controller_ID = TinyUI_ComboBoxSmall_CurrentController.SelectedIndex; try { Program.Controller_Handle.PlayerControllers[TinyUI_ComboBoxSmall_CurrentController.SelectedIndex].Load_Controller_Configuration(); Load_Controller_Settings_Into_Control_Text(); } catch { } }
        }
    }
}
