﻿namespace HeroesModLoaderConfig
{
    partial class MainFormSmall
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFormSmall));
            this.Toolstrip_Bottom = new System.Windows.Forms.ToolStrip();
            this.lbl_ActionBar_BottomRight = new System.Windows.Forms.ToolStripLabel();
            this.Panel_SideBar = new System.Windows.Forms.Panel();
            this.SideBtn_MainScreen = new System.Windows.Forms.Button();
            this.SideBtn_LoaderInputStack = new System.Windows.Forms.Button();
            this.SideBtn_LauncherSeparator = new System.Windows.Forms.Button();
            this.SideBtn_Tweaks = new System.Windows.Forms.Button();
            this.SideBtn_TweaksII = new System.Windows.Forms.Button();
            this.SideBtn_ControllerOne = new System.Windows.Forms.Button();
            this.SideBtn_ControllerTwo = new System.Windows.Forms.Button();
            this.SideBtn_LauncherSeparator_II = new System.Windows.Forms.Button();
            this.SideBtn_Themes = new System.Windows.Forms.Button();
            this.SideBtn_About = new System.Windows.Forms.Button();
            this.Panel_TitleBar = new System.Windows.Forms.Panel();
            this.TinyUI_TopLabel_PageTitle = new System.Windows.Forms.Label();
            this.Toolstrip_Bottom.SuspendLayout();
            this.Panel_SideBar.SuspendLayout();
            this.Panel_TitleBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // Toolstrip_Bottom
            // 
            this.Toolstrip_Bottom.AutoSize = false;
            this.Toolstrip_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            this.Toolstrip_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Toolstrip_Bottom.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Toolstrip_Bottom.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.Toolstrip_Bottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbl_ActionBar_BottomRight});
            this.Toolstrip_Bottom.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.Toolstrip_Bottom.Location = new System.Drawing.Point(0, 441);
            this.Toolstrip_Bottom.Name = "Toolstrip_Bottom";
            this.Toolstrip_Bottom.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Toolstrip_Bottom.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.Toolstrip_Bottom.Size = new System.Drawing.Size(374, 25);
            this.Toolstrip_Bottom.Stretch = true;
            this.Toolstrip_Bottom.TabIndex = 2;
            this.Toolstrip_Bottom.Text = "toolStrip2";
            this.Toolstrip_Bottom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Toolstrip_Bottom_MouseMove);
            // 
            // lbl_ActionBar_BottomRight
            // 
            this.lbl_ActionBar_BottomRight.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lbl_ActionBar_BottomRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbl_ActionBar_BottomRight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.lbl_ActionBar_BottomRight.Name = "lbl_ActionBar_BottomRight";
            this.lbl_ActionBar_BottomRight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl_ActionBar_BottomRight.Size = new System.Drawing.Size(150, 22);
            this.lbl_ActionBar_BottomRight.Text = "Executable Loaded!";
            // 
            // Panel_SideBar
            // 
            this.Panel_SideBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(41)))), ((int)(((byte)(46)))));
            this.Panel_SideBar.Controls.Add(this.SideBtn_MainScreen);
            this.Panel_SideBar.Controls.Add(this.SideBtn_LoaderInputStack);
            this.Panel_SideBar.Controls.Add(this.SideBtn_LauncherSeparator);
            this.Panel_SideBar.Controls.Add(this.SideBtn_Tweaks);
            this.Panel_SideBar.Controls.Add(this.SideBtn_TweaksII);
            this.Panel_SideBar.Controls.Add(this.SideBtn_ControllerOne);
            this.Panel_SideBar.Controls.Add(this.SideBtn_ControllerTwo);
            this.Panel_SideBar.Controls.Add(this.SideBtn_LauncherSeparator_II);
            this.Panel_SideBar.Controls.Add(this.SideBtn_Themes);
            this.Panel_SideBar.Controls.Add(this.SideBtn_About);
            this.Panel_SideBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_SideBar.ForeColor = System.Drawing.Color.White;
            this.Panel_SideBar.Location = new System.Drawing.Point(0, 44);
            this.Panel_SideBar.Margin = new System.Windows.Forms.Padding(0);
            this.Panel_SideBar.Name = "Panel_SideBar";
            this.Panel_SideBar.Size = new System.Drawing.Size(374, 42);
            this.Panel_SideBar.TabIndex = 138;
            this.Panel_SideBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel_SideBar_MouseMove);
            // 
            // SideBtn_MainScreen
            // 
            this.SideBtn_MainScreen.BackColor = this.Panel_SideBar.BackColor;
            this.SideBtn_MainScreen.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SideBtn_MainScreen.BackgroundImage")));
            this.SideBtn_MainScreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SideBtn_MainScreen.Dock = System.Windows.Forms.DockStyle.Right;
            this.SideBtn_MainScreen.FlatAppearance.BorderSize = 0;
            this.SideBtn_MainScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SideBtn_MainScreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideBtn_MainScreen.ForeColor = System.Drawing.Color.White;
            this.SideBtn_MainScreen.Location = new System.Drawing.Point(4, 0);
            this.SideBtn_MainScreen.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.SideBtn_MainScreen.Name = "SideBtn_MainScreen";
            this.SideBtn_MainScreen.Size = new System.Drawing.Size(42, 42);
            this.SideBtn_MainScreen.TabIndex = 148;
            this.SideBtn_MainScreen.UseVisualStyleBackColor = false;
            this.SideBtn_MainScreen.Click += new System.EventHandler(this.SideBtn_MainScreen_Click);
            // 
            // SideBtn_LoaderInputStack
            // 
            this.SideBtn_LoaderInputStack.BackColor = this.Panel_SideBar.BackColor;
            this.SideBtn_LoaderInputStack.BackgroundImage = global::HeroesModLoaderConfigurator.Properties.Resources.Mod_Loader_Icon_Controller_LoaderStack;
            this.SideBtn_LoaderInputStack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SideBtn_LoaderInputStack.Dock = System.Windows.Forms.DockStyle.Right;
            this.SideBtn_LoaderInputStack.FlatAppearance.BorderSize = 0;
            this.SideBtn_LoaderInputStack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SideBtn_LoaderInputStack.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideBtn_LoaderInputStack.ForeColor = System.Drawing.Color.White;
            this.SideBtn_LoaderInputStack.Location = new System.Drawing.Point(46, 0);
            this.SideBtn_LoaderInputStack.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.SideBtn_LoaderInputStack.Name = "SideBtn_LoaderInputStack";
            this.SideBtn_LoaderInputStack.Size = new System.Drawing.Size(42, 42);
            this.SideBtn_LoaderInputStack.TabIndex = 155;
            this.SideBtn_LoaderInputStack.UseVisualStyleBackColor = false;
            this.SideBtn_LoaderInputStack.Click += new System.EventHandler(this.SideBtn_LoaderInputStack_Click);
            // 
            // SideBtn_LauncherSeparator
            // 
            this.SideBtn_LauncherSeparator.BackColor = this.Panel_SideBar.BackColor;
            this.SideBtn_LauncherSeparator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SideBtn_LauncherSeparator.Dock = System.Windows.Forms.DockStyle.Right;
            this.SideBtn_LauncherSeparator.FlatAppearance.BorderSize = 0;
            this.SideBtn_LauncherSeparator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SideBtn_LauncherSeparator.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideBtn_LauncherSeparator.ForeColor = System.Drawing.Color.White;
            this.SideBtn_LauncherSeparator.Location = new System.Drawing.Point(88, 0);
            this.SideBtn_LauncherSeparator.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.SideBtn_LauncherSeparator.Name = "SideBtn_LauncherSeparator";
            this.SideBtn_LauncherSeparator.Size = new System.Drawing.Size(21, 42);
            this.SideBtn_LauncherSeparator.TabIndex = 153;
            this.SideBtn_LauncherSeparator.Text = "|";
            this.SideBtn_LauncherSeparator.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.SideBtn_LauncherSeparator.UseVisualStyleBackColor = false;
            // 
            // SideBtn_Tweaks
            // 
            this.SideBtn_Tweaks.BackColor = this.Panel_SideBar.BackColor;
            this.SideBtn_Tweaks.BackgroundImage = global::HeroesModLoaderConfigurator.Properties.Resources.Mod_Loader_Icon_Tweaks;
            this.SideBtn_Tweaks.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SideBtn_Tweaks.Dock = System.Windows.Forms.DockStyle.Right;
            this.SideBtn_Tweaks.FlatAppearance.BorderSize = 0;
            this.SideBtn_Tweaks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SideBtn_Tweaks.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideBtn_Tweaks.ForeColor = System.Drawing.Color.White;
            this.SideBtn_Tweaks.Location = new System.Drawing.Point(109, 0);
            this.SideBtn_Tweaks.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.SideBtn_Tweaks.Name = "SideBtn_Tweaks";
            this.SideBtn_Tweaks.Size = new System.Drawing.Size(40, 42);
            this.SideBtn_Tweaks.TabIndex = 151;
            this.SideBtn_Tweaks.UseVisualStyleBackColor = false;
            this.SideBtn_Tweaks.Click += new System.EventHandler(this.SideBtn_Tweaks_Click);
            // 
            // SideBtn_TweaksII
            // 
            this.SideBtn_TweaksII.BackColor = this.Panel_SideBar.BackColor;
            this.SideBtn_TweaksII.BackgroundImage = global::HeroesModLoaderConfigurator.Properties.Resources.Mod_Loader_Icon_Tweaks_2;
            this.SideBtn_TweaksII.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SideBtn_TweaksII.Dock = System.Windows.Forms.DockStyle.Right;
            this.SideBtn_TweaksII.FlatAppearance.BorderSize = 0;
            this.SideBtn_TweaksII.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SideBtn_TweaksII.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideBtn_TweaksII.ForeColor = System.Drawing.Color.White;
            this.SideBtn_TweaksII.Location = new System.Drawing.Point(149, 0);
            this.SideBtn_TweaksII.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.SideBtn_TweaksII.Name = "SideBtn_TweaksII";
            this.SideBtn_TweaksII.Size = new System.Drawing.Size(40, 42);
            this.SideBtn_TweaksII.TabIndex = 152;
            this.SideBtn_TweaksII.UseVisualStyleBackColor = false;
            this.SideBtn_TweaksII.Click += new System.EventHandler(this.SideBtn_TweaksII_Click);
            // 
            // SideBtn_ControllerOne
            // 
            this.SideBtn_ControllerOne.BackColor = this.Panel_SideBar.BackColor;
            this.SideBtn_ControllerOne.BackgroundImage = global::HeroesModLoaderConfigurator.Properties.Resources.Mod_Loader_Icon_Controller_Port_1;
            this.SideBtn_ControllerOne.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SideBtn_ControllerOne.Dock = System.Windows.Forms.DockStyle.Right;
            this.SideBtn_ControllerOne.FlatAppearance.BorderSize = 0;
            this.SideBtn_ControllerOne.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SideBtn_ControllerOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideBtn_ControllerOne.ForeColor = System.Drawing.Color.White;
            this.SideBtn_ControllerOne.Location = new System.Drawing.Point(189, 0);
            this.SideBtn_ControllerOne.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.SideBtn_ControllerOne.Name = "SideBtn_ControllerOne";
            this.SideBtn_ControllerOne.Size = new System.Drawing.Size(40, 42);
            this.SideBtn_ControllerOne.TabIndex = 149;
            this.SideBtn_ControllerOne.UseVisualStyleBackColor = false;
            this.SideBtn_ControllerOne.Click += new System.EventHandler(this.SideBtn_ControllerOne_Click);
            // 
            // SideBtn_ControllerTwo
            // 
            this.SideBtn_ControllerTwo.BackColor = this.Panel_SideBar.BackColor;
            this.SideBtn_ControllerTwo.BackgroundImage = global::HeroesModLoaderConfigurator.Properties.Resources.Mod_Loader_Icon_Controller_Port_2;
            this.SideBtn_ControllerTwo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SideBtn_ControllerTwo.Dock = System.Windows.Forms.DockStyle.Right;
            this.SideBtn_ControllerTwo.FlatAppearance.BorderSize = 0;
            this.SideBtn_ControllerTwo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SideBtn_ControllerTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideBtn_ControllerTwo.ForeColor = System.Drawing.Color.White;
            this.SideBtn_ControllerTwo.Location = new System.Drawing.Point(229, 0);
            this.SideBtn_ControllerTwo.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.SideBtn_ControllerTwo.Name = "SideBtn_ControllerTwo";
            this.SideBtn_ControllerTwo.Size = new System.Drawing.Size(40, 42);
            this.SideBtn_ControllerTwo.TabIndex = 150;
            this.SideBtn_ControllerTwo.UseVisualStyleBackColor = false;
            this.SideBtn_ControllerTwo.Click += new System.EventHandler(this.SideBtn_ControllerTwo_Click);
            // 
            // SideBtn_LauncherSeparator_II
            // 
            this.SideBtn_LauncherSeparator_II.BackColor = this.Panel_SideBar.BackColor;
            this.SideBtn_LauncherSeparator_II.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SideBtn_LauncherSeparator_II.Dock = System.Windows.Forms.DockStyle.Right;
            this.SideBtn_LauncherSeparator_II.FlatAppearance.BorderSize = 0;
            this.SideBtn_LauncherSeparator_II.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SideBtn_LauncherSeparator_II.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideBtn_LauncherSeparator_II.ForeColor = System.Drawing.Color.White;
            this.SideBtn_LauncherSeparator_II.Location = new System.Drawing.Point(269, 0);
            this.SideBtn_LauncherSeparator_II.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.SideBtn_LauncherSeparator_II.Name = "SideBtn_LauncherSeparator_II";
            this.SideBtn_LauncherSeparator_II.Size = new System.Drawing.Size(21, 42);
            this.SideBtn_LauncherSeparator_II.TabIndex = 154;
            this.SideBtn_LauncherSeparator_II.Text = "|";
            this.SideBtn_LauncherSeparator_II.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.SideBtn_LauncherSeparator_II.UseVisualStyleBackColor = false;
            // 
            // SideBtn_Themes
            // 
            this.SideBtn_Themes.BackColor = this.Panel_SideBar.BackColor;
            this.SideBtn_Themes.BackgroundImage = global::HeroesModLoaderConfigurator.Properties.Resources.Mod_Loader_Icon_Paintbrush;
            this.SideBtn_Themes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SideBtn_Themes.Dock = System.Windows.Forms.DockStyle.Right;
            this.SideBtn_Themes.FlatAppearance.BorderSize = 0;
            this.SideBtn_Themes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SideBtn_Themes.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideBtn_Themes.ForeColor = System.Drawing.Color.White;
            this.SideBtn_Themes.Location = new System.Drawing.Point(290, 0);
            this.SideBtn_Themes.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.SideBtn_Themes.Name = "SideBtn_Themes";
            this.SideBtn_Themes.Size = new System.Drawing.Size(42, 42);
            this.SideBtn_Themes.TabIndex = 146;
            this.SideBtn_Themes.UseVisualStyleBackColor = false;
            this.SideBtn_Themes.Click += new System.EventHandler(this.SideBtn_Themes_Click);
            // 
            // SideBtn_About
            // 
            this.SideBtn_About.BackColor = this.Panel_SideBar.BackColor;
            this.SideBtn_About.BackgroundImage = global::HeroesModLoaderConfigurator.Properties.Resources.Mod_Loader_Icon_About;
            this.SideBtn_About.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SideBtn_About.Dock = System.Windows.Forms.DockStyle.Right;
            this.SideBtn_About.FlatAppearance.BorderSize = 0;
            this.SideBtn_About.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SideBtn_About.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideBtn_About.ForeColor = System.Drawing.Color.White;
            this.SideBtn_About.Location = new System.Drawing.Point(332, 0);
            this.SideBtn_About.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.SideBtn_About.Name = "SideBtn_About";
            this.SideBtn_About.Size = new System.Drawing.Size(42, 42);
            this.SideBtn_About.TabIndex = 147;
            this.SideBtn_About.UseVisualStyleBackColor = false;
            this.SideBtn_About.Click += new System.EventHandler(this.SideBtn_About_Click);
            // 
            // Panel_TitleBar
            // 
            this.Panel_TitleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(53)))), ((int)(((byte)(64)))));
            this.Panel_TitleBar.Controls.Add(this.TinyUI_TopLabel_PageTitle);
            this.Panel_TitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_TitleBar.Location = new System.Drawing.Point(0, 0);
            this.Panel_TitleBar.Margin = new System.Windows.Forms.Padding(0);
            this.Panel_TitleBar.Name = "Panel_TitleBar";
            this.Panel_TitleBar.Size = new System.Drawing.Size(374, 44);
            this.Panel_TitleBar.TabIndex = 139;
            this.Panel_TitleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel_TitleBar_MouseMove);
            // 
            // TinyUI_TopLabel_PageTitle
            // 
            this.TinyUI_TopLabel_PageTitle.BackColor = System.Drawing.Color.Transparent;
            this.TinyUI_TopLabel_PageTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TinyUI_TopLabel_PageTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TinyUI_TopLabel_PageTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TinyUI_TopLabel_PageTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TinyUI_TopLabel_PageTitle.Location = new System.Drawing.Point(0, 0);
            this.TinyUI_TopLabel_PageTitle.Margin = new System.Windows.Forms.Padding(0);
            this.TinyUI_TopLabel_PageTitle.Name = "TinyUI_TopLabel_PageTitle";
            this.TinyUI_TopLabel_PageTitle.Size = new System.Drawing.Size(374, 44);
            this.TinyUI_TopLabel_PageTitle.TabIndex = 149;
            this.TinyUI_TopLabel_PageTitle.Text = "Main Menu";
            this.TinyUI_TopLabel_PageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.TinyUI_TopLabel_PageTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TinyUI_TopLabel_PageTitle_MouseMove);
            // 
            // MainFormSmall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(374, 466);
            this.Controls.Add(this.Panel_SideBar);
            this.Controls.Add(this.Toolstrip_Bottom);
            this.Controls.Add(this.Panel_TitleBar);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainFormSmall";
            this.Text = "ExoSkeletal UI Base by Sewer56lol";
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainFormSmall_MouseMove);
            this.Toolstrip_Bottom.ResumeLayout(false);
            this.Toolstrip_Bottom.PerformLayout();
            this.Panel_SideBar.ResumeLayout(false);
            this.Panel_TitleBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip Toolstrip_Bottom;
        public System.Windows.Forms.ToolStripLabel lbl_ActionBar_BottomRight;
        private System.Windows.Forms.Panel Panel_SideBar;
        private System.Windows.Forms.Panel Panel_TitleBar;
        private System.Windows.Forms.Label TinyUI_TopLabel_PageTitle;
        private System.Windows.Forms.Button SideBtn_Themes;
        private System.Windows.Forms.Button SideBtn_About;
        private System.Windows.Forms.Button SideBtn_MainScreen;
        private System.Windows.Forms.Button SideBtn_ControllerOne;
        private System.Windows.Forms.Button SideBtn_ControllerTwo;
        private System.Windows.Forms.Button SideBtn_Tweaks;
        private System.Windows.Forms.Button SideBtn_TweaksII;
        private System.Windows.Forms.Button SideBtn_LauncherSeparator;
        private System.Windows.Forms.Button SideBtn_LauncherSeparator_II;
        private System.Windows.Forms.Button SideBtn_LoaderInputStack;
    }
}