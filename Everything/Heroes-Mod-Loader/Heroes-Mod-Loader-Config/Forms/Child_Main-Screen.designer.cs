namespace HeroesModLoaderConfig
{
    partial class MainScreen
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
            this.ListView_Mod_List = new HeroesModLoaderConfigurator.Classes.Visual.Controls.CustomListView();
            this.Column_Mod_Enabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Column_Mod_Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Column_Mod_Version = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Btn_Exit = new HeroesModLoaderConfig.CustomButton();
            this.Btn_SaveExit = new HeroesModLoaderConfig.CustomButton();
            this.Btn_Launch = new HeroesModLoaderConfig.CustomButton();
            this.Btn_OrderUp = new HeroesModLoaderConfig.CustomButton();
            this.Btn_OrderDown = new HeroesModLoaderConfig.CustomButton();
            this.BtnAlt_Options = new HeroesModLoaderConfig.CustomButton();
            this.TinyUI_TxtSpc2_Author = new System.Windows.Forms.Label();
            this.TinyUI_TxtSpc2_AuthorName = new System.Windows.Forms.Label();
            this.TinyUI_TxtSpc4_Description = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ListView_Mod_List
            // 
            this.ListView_Mod_List.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.ListView_Mod_List.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ListView_Mod_List.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ListView_Mod_List.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Column_Mod_Enabled,
            this.Column_Mod_Name,
            this.Column_Mod_Version});
            this.ListView_Mod_List.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListView_Mod_List.ForeColor = System.Drawing.Color.Gray;
            this.ListView_Mod_List.FullRowSelect = true;
            this.ListView_Mod_List.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.ListView_Mod_List.Location = new System.Drawing.Point(11, 12);
            this.ListView_Mod_List.MultiSelect = false;
            this.ListView_Mod_List.Name = "ListView_Mod_List";
            this.ListView_Mod_List.Size = new System.Drawing.Size(352, 227);
            this.ListView_Mod_List.TabIndex = 243;
            this.ListView_Mod_List.UseCompatibleStateImageBehavior = false;
            this.ListView_Mod_List.View = System.Windows.Forms.View.Details;
            this.ListView_Mod_List.SelectedIndexChanged += new System.EventHandler(this.ListView_Mod_List_SelectedIndexChanged);
            this.ListView_Mod_List.Click += new System.EventHandler(this.ListView_Mod_List_Click);
            this.ListView_Mod_List.DoubleClick += new System.EventHandler(this.ListView_Mod_List_DoubleClick);
            // 
            // Column_Mod_Enabled
            // 
            this.Column_Mod_Enabled.Text = "Enabled";
            this.Column_Mod_Enabled.Width = 20;
            // 
            // Column_Mod_Name
            // 
            this.Column_Mod_Name.DisplayIndex = 2;
            this.Column_Mod_Name.Text = "Mod Name";
            this.Column_Mod_Name.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Column_Mod_Name.Width = 282;
            // 
            // Column_Mod_Version
            // 
            this.Column_Mod_Version.DisplayIndex = 1;
            this.Column_Mod_Version.Text = "Version";
            this.Column_Mod_Version.Width = 50;
            // 
            // Btn_Exit
            // 
            this.Btn_Exit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            this.Btn_Exit.FlatAppearance.BorderSize = 0;
            this.Btn_Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Exit.ForeColor = System.Drawing.Color.White;
            this.Btn_Exit.Location = new System.Drawing.Point(285, 314);
            this.Btn_Exit.Name = "Btn_Exit";
            this.Btn_Exit.OwnerDrawText = "Exit";
            this.Btn_Exit.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.Btn_Exit.Size = new System.Drawing.Size(78, 26);
            this.Btn_Exit.TabIndex = 242;
            this.Btn_Exit.UseVisualStyleBackColor = false;
            this.Btn_Exit.Click += new System.EventHandler(this.Btn_Exit_Click);
            // 
            // Btn_SaveExit
            // 
            this.Btn_SaveExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            this.Btn_SaveExit.FlatAppearance.BorderSize = 0;
            this.Btn_SaveExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_SaveExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_SaveExit.ForeColor = System.Drawing.Color.White;
            this.Btn_SaveExit.Location = new System.Drawing.Point(156, 314);
            this.Btn_SaveExit.Name = "Btn_SaveExit";
            this.Btn_SaveExit.OwnerDrawText = "Save & Exit";
            this.Btn_SaveExit.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.Btn_SaveExit.Size = new System.Drawing.Size(123, 26);
            this.Btn_SaveExit.TabIndex = 241;
            this.Btn_SaveExit.UseVisualStyleBackColor = false;
            this.Btn_SaveExit.Click += new System.EventHandler(this.Btn_SaveExit_Click);
            // 
            // Btn_Launch
            // 
            this.Btn_Launch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            this.Btn_Launch.FlatAppearance.BorderSize = 0;
            this.Btn_Launch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Launch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Launch.ForeColor = System.Drawing.Color.White;
            this.Btn_Launch.Location = new System.Drawing.Point(11, 314);
            this.Btn_Launch.Name = "Btn_Launch";
            this.Btn_Launch.OwnerDrawText = "Launch & Exit";
            this.Btn_Launch.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.Btn_Launch.Size = new System.Drawing.Size(139, 26);
            this.Btn_Launch.TabIndex = 240;
            this.Btn_Launch.UseVisualStyleBackColor = false;
            this.Btn_Launch.Click += new System.EventHandler(this.Btn_Launch_Click);
            // 
            // Btn_OrderUp
            // 
            this.Btn_OrderUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            this.Btn_OrderUp.FlatAppearance.BorderSize = 0;
            this.Btn_OrderUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_OrderUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_OrderUp.ForeColor = System.Drawing.Color.White;
            this.Btn_OrderUp.Location = new System.Drawing.Point(11, 250);
            this.Btn_OrderUp.Name = "Btn_OrderUp";
            this.Btn_OrderUp.OwnerDrawText = "▲";
            this.Btn_OrderUp.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.Btn_OrderUp.Size = new System.Drawing.Size(30, 24);
            this.Btn_OrderUp.TabIndex = 244;
            this.Btn_OrderUp.UseVisualStyleBackColor = false;
            this.Btn_OrderUp.Click += new System.EventHandler(this.Btn_OrderUp_Click);
            // 
            // Btn_OrderDown
            // 
            this.Btn_OrderDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            this.Btn_OrderDown.FlatAppearance.BorderSize = 0;
            this.Btn_OrderDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_OrderDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_OrderDown.ForeColor = System.Drawing.Color.White;
            this.Btn_OrderDown.Location = new System.Drawing.Point(46, 250);
            this.Btn_OrderDown.Name = "Btn_OrderDown";
            this.Btn_OrderDown.OwnerDrawText = "▼";
            this.Btn_OrderDown.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.Btn_OrderDown.Size = new System.Drawing.Size(30, 24);
            this.Btn_OrderDown.TabIndex = 245;
            this.Btn_OrderDown.UseVisualStyleBackColor = false;
            this.Btn_OrderDown.Click += new System.EventHandler(this.Btn_OrderDown_Click);
            // 
            // BtnAlt_Options
            // 
            this.BtnAlt_Options.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(81)))), ((int)(((byte)(181)))));
            this.BtnAlt_Options.FlatAppearance.BorderSize = 0;
            this.BtnAlt_Options.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnAlt_Options.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnAlt_Options.ForeColor = System.Drawing.Color.White;
            this.BtnAlt_Options.Location = new System.Drawing.Point(81, 250);
            this.BtnAlt_Options.Name = "BtnAlt_Options";
            this.BtnAlt_Options.OwnerDrawText = "Options";
            this.BtnAlt_Options.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.BtnAlt_Options.Size = new System.Drawing.Size(67, 24);
            this.BtnAlt_Options.TabIndex = 247;
            this.BtnAlt_Options.UseVisualStyleBackColor = false;
            this.BtnAlt_Options.Click += new System.EventHandler(this.Btn_Options_Click);
            // 
            // TinyUI_TxtSpc2_Author
            // 
            this.TinyUI_TxtSpc2_Author.BackColor = System.Drawing.Color.Transparent;
            this.TinyUI_TxtSpc2_Author.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TinyUI_TxtSpc2_Author.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TinyUI_TxtSpc2_Author.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TinyUI_TxtSpc2_Author.Location = new System.Drawing.Point(153, 250);
            this.TinyUI_TxtSpc2_Author.Margin = new System.Windows.Forms.Padding(0);
            this.TinyUI_TxtSpc2_Author.Name = "TinyUI_TxtSpc2_Author";
            this.TinyUI_TxtSpc2_Author.Size = new System.Drawing.Size(118, 24);
            this.TinyUI_TxtSpc2_Author.TabIndex = 248;
            this.TinyUI_TxtSpc2_Author.Text = "Author:";
            this.TinyUI_TxtSpc2_Author.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TinyUI_TxtSpc2_AuthorName
            // 
            this.TinyUI_TxtSpc2_AuthorName.BackColor = System.Drawing.Color.Transparent;
            this.TinyUI_TxtSpc2_AuthorName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TinyUI_TxtSpc2_AuthorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TinyUI_TxtSpc2_AuthorName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TinyUI_TxtSpc2_AuthorName.Location = new System.Drawing.Point(219, 250);
            this.TinyUI_TxtSpc2_AuthorName.Margin = new System.Windows.Forms.Padding(0);
            this.TinyUI_TxtSpc2_AuthorName.Name = "TinyUI_TxtSpc2_AuthorName";
            this.TinyUI_TxtSpc2_AuthorName.Size = new System.Drawing.Size(144, 24);
            this.TinyUI_TxtSpc2_AuthorName.TabIndex = 249;
            this.TinyUI_TxtSpc2_AuthorName.Text = "N/A";
            this.TinyUI_TxtSpc2_AuthorName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TinyUI_TxtSpc4_Description
            // 
            this.TinyUI_TxtSpc4_Description.BackColor = System.Drawing.Color.Transparent;
            this.TinyUI_TxtSpc4_Description.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TinyUI_TxtSpc4_Description.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TinyUI_TxtSpc4_Description.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TinyUI_TxtSpc4_Description.Location = new System.Drawing.Point(0, 281);
            this.TinyUI_TxtSpc4_Description.Margin = new System.Windows.Forms.Padding(0);
            this.TinyUI_TxtSpc4_Description.Name = "TinyUI_TxtSpc4_Description";
            this.TinyUI_TxtSpc4_Description.Size = new System.Drawing.Size(378, 24);
            this.TinyUI_TxtSpc4_Description.TabIndex = 250;
            this.TinyUI_TxtSpc4_Description.Text = "No Description";
            this.TinyUI_TxtSpc4_Description.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(378, 358);
            this.Controls.Add(this.TinyUI_TxtSpc4_Description);
            this.Controls.Add(this.TinyUI_TxtSpc2_AuthorName);
            this.Controls.Add(this.TinyUI_TxtSpc2_Author);
            this.Controls.Add(this.BtnAlt_Options);
            this.Controls.Add(this.Btn_OrderDown);
            this.Controls.Add(this.Btn_OrderUp);
            this.Controls.Add(this.ListView_Mod_List);
            this.Controls.Add(this.Btn_Exit);
            this.Controls.Add(this.Btn_SaveExit);
            this.Controls.Add(this.Btn_Launch);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainScreen";
            this.Text = "SwappableScreen";
            this.Load += new System.EventHandler(this.MainScreen_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private CustomButton Btn_Launch;
        private CustomButton Btn_SaveExit;
        private CustomButton Btn_Exit;
        private System.Windows.Forms.ColumnHeader Column_Mod_Name;
        private System.Windows.Forms.ColumnHeader Column_Mod_Version;
        private System.Windows.Forms.ColumnHeader Column_Mod_Enabled;
        private CustomButton Btn_OrderUp;
        private CustomButton Btn_OrderDown;
        private CustomButton BtnAlt_Options;
        private System.Windows.Forms.Label TinyUI_TxtSpc2_Author;
        private System.Windows.Forms.Label TinyUI_TxtSpc2_AuthorName;
        private System.Windows.Forms.Label TinyUI_TxtSpc4_Description;
        private HeroesModLoaderConfigurator.Classes.Visual.Controls.CustomListView ListView_Mod_List;
    }
}