using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using SonicHeroes.Misc;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace HeroesModLoaderConfig
{
    public partial class MainScreen : Form
    {
        /// <summary>
        /// By turning on compositing for the form, we can kill the flickering of dark control overdraws as we draw our own controls in an interactive fashion..
        /// </summary>
        protected override CreateParams CreateParams { get { CreateParams cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; } }

        public MainScreen()
        {
            InitializeComponent();
            CenterToScreen();
            this.BringToFront();
            ListView_Mod_List.MouseWheel += Listview_Custom_Scroll_Handler;
        }

        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Btn_SaveExit_Click(object sender, EventArgs e)
        {
            Save_Mod_Loader_Mod_List();
            if (Program.Game_Is_Sonic_Heroes)
            {
                File.WriteAllBytes(Program.Executable_Path, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable);
                SonicHeroes_Miscallenous.Save_Configuration_File(Program.Sonic_Heroes_Specific_Stuff.ConfigFile);
            }
            Application.Exit();
        }

        private void Btn_Launch_Click(object sender, EventArgs e)
        {
            Save_Mod_Loader_Mod_List();
            if (Program.Game_Is_Sonic_Heroes)
            {
                File.WriteAllBytes(Program.Executable_Path, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable);
                SonicHeroes_Miscallenous.Save_Configuration_File(Program.Sonic_Heroes_Specific_Stuff.ConfigFile);
            }
            Process.Start("HeroesModLoader.exe");
            Application.Exit();
        }

        /// <summary>
        /// Saves a mod list (listing of directories to be loaded) to a text file.
        /// </summary>
        private void Save_Mod_Loader_Mod_List()
        {
            // List of strings to store folder names of mods to load.
            List<string> Enabled_Mods_List = new List<string>();

            // Find the mod we are looking for
            for (int z = 0; z < ListView_Mod_List.Items.Count; z++)
            {
                for (int x = 0; x < Mod_Loader_Entries.Count; x++)
                {
                    if ( (Mod_Loader_Entries[x].Mod_Name == ListView_Mod_List.Items[z].SubItems[1].Text) && (Mod_Loader_Entries[x].Mod_Version == ListView_Mod_List.Items[z].SubItems[2].Text) )
                    {
                        if (Mod_Loader_Entries[x].Mod_Enabled == true)
                        {
                            Enabled_Mods_List.Add(Mod_Loader_Entries[x].Mod_Directory.Substring(Mod_Loader_Entries[x].Mod_Directory.LastIndexOf("\\") + 1));
                        }
                    }
                }
            }

            // Write out all enabled mods :)
            File.WriteAllLines(Environment.CurrentDirectory + "\\Mod-Loader-Config\\Enabled_Mods.txt", Enabled_Mods_List);
        }

        /// <summary>
        /// List of directories of every installed mod for Sonic Heroes.
        /// </summary>
        public List<string> Mod_Directories;

        /// <summary>
        /// List of individual mods for the mod loader.
        /// </summary>
        public List<Mod_Loader_Mod_INI_Entry> Mod_Loader_Entries = new List<Mod_Loader_Mod_INI_Entry>();

        /// <summary>
        /// When the form is loaded for the first time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainScreen_Load(object sender, EventArgs e)
        {
            ListView_Mod_List.Items.Clear();

            // Dumps a list of enabled mods.
            string[] Enabled_Mods_List = File.ReadAllLines(Environment.CurrentDirectory + "\\Mod-Loader-Config\\Enabled_Mods.txt");
            string Mod_Loader_Mods_Location = AppDomain.CurrentDomain.BaseDirectory + "Mod-Loader-Mods"; // Enabled mod list.

            // Add the enabled mods first.
            for (int x = 0; x < Enabled_Mods_List.Length; x++)
            {
                // Load our mod loader entry :)
                Mod_Loader_Mod_INI_Entry Mod_Loader_Entry = Parse_Mod_Loader_INI_Entry(Mod_Loader_Mods_Location + "\\" + Enabled_Mods_List[x] + @"\Mod.ini");

                // Generate ListViewItem for mod.
                ListViewItem Heroes_Mod_Item = new ListViewItem();
                Heroes_Mod_Item.Font = Program.xFonts.Hex_ComboBox_TinyUI;

                // Select colour based on enabled/disabled. ☑ ☐
                Heroes_Mod_Item.Text = "☑"; Mod_Loader_Entry.Mod_Enabled = true; Heroes_Mod_Item.ForeColor = System.Drawing.Color.LightGray;

                // Add subitems
                Heroes_Mod_Item.UseItemStyleForSubItems = true;
                Heroes_Mod_Item.SubItems.Add(new ListViewItem.ListViewSubItem(Heroes_Mod_Item, Mod_Loader_Entry.Mod_Name));
                Heroes_Mod_Item.SubItems.Add(new ListViewItem.ListViewSubItem(Heroes_Mod_Item, Mod_Loader_Entry.Mod_Version));

                // Assign to the mod loader entry
                Mod_Loader_Entry.Heroes_Mod_Listview_Item = Heroes_Mod_Item;

                // Add the item :3
                Mod_Loader_Entries.Add(Mod_Loader_Entry);
                ListView_Mod_List.Items.Add(Heroes_Mod_Item);
            }

            // Get all mod directories.
            Mod_Directories = Directory.GetDirectories(Mod_Loader_Mods_Location).ToList();

            // Remove all directories of already existing loaded mods from the directory list such that they do not become loaded once again.
            for (int x = 0; x < Mod_Directories.Count; x++)
            {
                string Mod_Directory_Name = Mod_Directories[x].Substring(Mod_Directories[x].LastIndexOf("\\") + 1);
                for (int z = 0; z < Enabled_Mods_List.Length; z++)
                {
                    if (Mod_Directory_Name == Enabled_Mods_List[z])
                    {
                        Mod_Directories.RemoveAt(x);
                        x--; // Decrement the index by 1 because the current index was removed.
                    }
                }
            }

            // Strip the path to the mod loader
            // Add each mod onto the selection.
            for (int x = 0; x < Mod_Directories.Count; x++)
            {
                // Load our mod loader entry :)
                Mod_Loader_Mod_INI_Entry Mod_Loader_Entry = Parse_Mod_Loader_INI_Entry(Mod_Directories[x] + @"\Mod.ini");

                // Generate ListViewItem for mod.
                ListViewItem Heroes_Mod_Item = new ListViewItem();
                Heroes_Mod_Item.Font = Program.xFonts.Hex_ComboBox_TinyUI;
                
                // Select colour based on enabled/disabled. ☑ ☐
                Heroes_Mod_Item.Text = "☐"; Mod_Loader_Entry.Mod_Enabled = false; Heroes_Mod_Item.ForeColor = System.Drawing.Color.Gray;

                // Add subitems
                Heroes_Mod_Item.UseItemStyleForSubItems = true;
                Heroes_Mod_Item.SubItems.Add(new ListViewItem.ListViewSubItem(Heroes_Mod_Item, Mod_Loader_Entry.Mod_Name));
                Heroes_Mod_Item.SubItems.Add(new ListViewItem.ListViewSubItem(Heroes_Mod_Item, Mod_Loader_Entry.Mod_Version));

                // Assign to the mod loader entry
                Mod_Loader_Entry.Heroes_Mod_Listview_Item = Heroes_Mod_Item;

                // Add the item :3
                Mod_Loader_Entries.Add(Mod_Loader_Entry);
                ListView_Mod_List.Items.Add(Heroes_Mod_Item);
            }

        }

        /// <summary>
        /// Parses and reads the Mod.ini file for an individual mod loader mod.
        /// </summary>
        /// <param name="Mod_Loader_INI_Path"></param>
        /// <returns></returns>
        private Mod_Loader_Mod_INI_Entry Parse_Mod_Loader_INI_Entry(string Mod_Loader_INI_Path)
        {
            Mod_Loader_Mod_INI_Entry Mod_Loader_Entry = new Mod_Loader_Mod_INI_Entry();
            Mod_Loader_Entry.Mod_Directory = Path.GetDirectoryName(Mod_Loader_INI_Path);

            // Read the file line by line.
            string Current_Line;
            System.IO.StreamReader Mod_Loader_INI_File = new System.IO.StreamReader(Mod_Loader_INI_Path);
            while ((Current_Line = Mod_Loader_INI_File.ReadLine()) != null)
            {
                if (Current_Line.StartsWith("Name")) { Mod_Loader_Entry.Mod_Name = Current_Line.Substring(Current_Line.IndexOf("=") + 1); }
                else if (Current_Line.StartsWith("Version")) { Mod_Loader_Entry.Mod_Version = Current_Line.Substring(Current_Line.IndexOf("=") + 1); }
                else if (Current_Line.StartsWith("Description")) { Mod_Loader_Entry.Mod_Description = Current_Line.Substring(Current_Line.IndexOf("=") + 1); }
                else if (Current_Line.StartsWith("Author")) { Mod_Loader_Entry.Mod_Author = Current_Line.Substring(Current_Line.IndexOf("=") + 1); }
                else if (Current_Line.StartsWith("Options")) { Mod_Loader_Entry.Options_ExecutableName = Current_Line.Substring(Current_Line.IndexOf("=") + 1); }
            }
            Mod_Loader_INI_File.Close();

            return Mod_Loader_Entry;
        }

        /// <summary>
        /// Entry for a mod loader mod.
        /// </summary>
        public struct Mod_Loader_Mod_INI_Entry
        {
            public string Mod_Directory;
            public string Mod_Name;
            public string Mod_Version;
            public string Mod_Description;
            public string Mod_Author;
            public string Options_ExecutableName;
            public bool Mod_Enabled;
            public ListViewItem Heroes_Mod_Listview_Item;
        }

        /// <summary>
        /// Get the current selected row on doubleclick and enable/disable mod.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_Mod_List_DoubleClick(object sender, EventArgs e)
        { ListView_Toggle_Mod(); }

        /// <summary>
        /// Get the current selected row on click and enable/disable mod if the click occurs in the region of the checkbox column..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_Mod_List_Click(object sender, EventArgs e)
        {
            // Obtain mouse position in terms of relative client coordinates.
            Point Mouse_Position = ListView_Mod_List.PointToClient(Control.MousePosition);
            // Obtain the information on the area/object the mouse is pointing to.
            ListViewHitTestInfo Hit_Information = ListView_Mod_List.HitTest(Mouse_Position);
            // Retrieve the column index which the mouse is pointing to.
            int Column_Index = Hit_Information.Item.SubItems.IndexOf(Hit_Information.SubItem);
            // If it is pointing towards the checkbox, enable or disable the mod.
            if (Column_Index == 0) { ListView_Toggle_Mod(); }
        }

        /// <summary>
        /// Disables a mod loader entry by manually modifying the listview item and properties of the entry.
        /// </summary>
        /// <param name="Mod_Loader_Entry"></param>
        private Mod_Loader_Mod_INI_Entry Toggle_Mod(Mod_Loader_Mod_INI_Entry Mod_Loader_Entry)
        {
            if (Mod_Loader_Entry.Mod_Enabled) { Mod_Loader_Entry.Mod_Enabled = false; ListView_Mod_List.SelectedItems[0].SubItems[0].Text = "☐"; Mod_Loader_Entry.Heroes_Mod_Listview_Item.ForeColor = Color.Gray; }
            else { Mod_Loader_Entry.Mod_Enabled = true; ListView_Mod_List.SelectedItems[0].SubItems[0].Text = "☑"; Mod_Loader_Entry.Heroes_Mod_Listview_Item.ForeColor = Color.LightGray; }
            return Mod_Loader_Entry;
        }
        
        /// <summary>
        /// Disables a mod obtained from the currently selected listview item.
        /// </summary>
        private void ListView_Toggle_Mod()
        {
            // Find the mod we are looking for
            for (int x = 0; x < Mod_Loader_Entries.Count; x++)
            {
                // Get current entry for the mod loader.
                Mod_Loader_Mod_INI_Entry Mod_Loader_Entry = Mod_Loader_Entries[x];
                // If the mod name and version matches a known mod, enable or disable the specific mod.
                if ((Mod_Loader_Entry.Mod_Name == ListView_Mod_List.SelectedItems[0].SubItems[1].Text) && (Mod_Loader_Entry.Mod_Version == ListView_Mod_List.SelectedItems[0].SubItems[2].Text))
                {
                    Mod_Loader_Entry = Toggle_Mod(Mod_Loader_Entry);
                }
                // Replace old mod loader entry.
                Mod_Loader_Entries[x] = Mod_Loader_Entry;
            }
        }

        /// <summary>
        /// Moves a mod up in the priority queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_OrderUp_Click(object sender, EventArgs e)
        {
            /* If one item is selected */
            if (ListView_Mod_List.SelectedItems.Count == 1) { ListView_MoveUp(ListView_Mod_List); }
        }

        /// <summary>
        /// Moves a mod down in the priority queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_OrderDown_Click(object sender, EventArgs e)
        {
            /* If one item is selected */
            if (ListView_Mod_List.SelectedItems.Count == 1) { ListView_MoveDown(ListView_Mod_List); }
        }

        /// <summary>
        /// Moves a listview item up by one space.
        /// </summary>
        private void ListView_MoveUp(ListView ListView_Mod_List)
        {
            int Current_Index = ListView_Mod_List.SelectedIndices[0]; // Current Index
            if (Current_Index > 0)
            {
                ListViewItem Current_Listview_Item = ListView_Mod_List.Items[Current_Index]; // Item we are using for swapping the listview.
                ListView_Mod_List.Items.RemoveAt(Current_Index); // Remove item.
                ListView_Mod_List.Items.Insert(Current_Index - 1, Current_Listview_Item); // Insert it one slot before.
            }
        }

        /// <summary>
        /// Moves a listview item up by one space.
        /// </summary>
        private void ListView_MoveDown(ListView ListView_Mod_List)
        {
            int Current_Index = ListView_Mod_List.SelectedIndices[0]; // Current Index
            if (Current_Index < ListView_Mod_List.Items.Count - 1)
            {
                ListViewItem Current_Listview_Item = ListView_Mod_List.Items[Current_Index]; // Item we are using for swapping the listview.
                ListView_Mod_List.Items.RemoveAt(Current_Index); // Remove item.
                ListView_Mod_List.Items.Insert(Current_Index + 1, Current_Listview_Item); // Insert it one slot before.
            }
        }

        /// <summary>
        /// If the mod loader
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Options_Click(object sender, EventArgs e)
        {
            bool Error_Shown = false;
            // Find the mod we are looking for
            for (int x = 0; x < Mod_Loader_Entries.Count; x++)
            {
                try
                {
                    // If mod name and version maitches, open the settings executable destined for that specific mod.
                    if ((Mod_Loader_Entries[x].Mod_Name == ListView_Mod_List.SelectedItems[0].SubItems[1].Text) && (Mod_Loader_Entries[x].Mod_Version == ListView_Mod_List.SelectedItems[0].SubItems[2].Text))
                    {
                        System.Diagnostics.Process.Start(Mod_Loader_Entries[x].Mod_Directory + "\\" + Mod_Loader_Entries[x].Options_ExecutableName);
                    }
                }
                catch
                {
                    try
                    {
                        System.Diagnostics.Process.Start("notepad.exe", Mod_Loader_Entries[x].Mod_Directory + "\\" + Mod_Loader_Entries[x].Options_ExecutableName);
                    }
                    catch
                    {
                        if (!Error_Shown) { MessageBox.Show("Select an actual mod, dummy."); Error_Shown = true; }
                    }
                }
            }
        }

        /// <summary>
        /// Matches the currently selected item and sets the author name and description accordingly.
        /// </summary>
        private void Set_Author_Name_Description()
        {
            // Find the mod we are looking for
            for (int x = 0; x < Mod_Loader_Entries.Count; x++)
            {
                // If mod name and version matches, open the settings executable destined for that specific mod.
                if ((Mod_Loader_Entries[x].Mod_Name == ListView_Mod_List.SelectedItems[0].SubItems[1].Text) && (Mod_Loader_Entries[x].Mod_Version == ListView_Mod_List.SelectedItems[0].SubItems[2].Text))
                {
                    TinyUI_TxtSpc2_AuthorName.Text = Mod_Loader_Entries[x].Mod_Author;
                    TinyUI_TxtSpc4_Description.Text = Mod_Loader_Entries[x].Mod_Description;
                    if (String.IsNullOrEmpty(Mod_Loader_Entries[x].Options_ExecutableName))
                    {
                        BtnAlt_Options.BackColor = HSL_Invert(Program.BottomToolstrip.BackColor);
                        BtnAlt_Options.Text = "Options";
                    }
                    else
                    {
                        BtnAlt_Options.BackColor = Program.BottomToolstrip.BackColor;
                        BtnAlt_Options.Text = "Config";
                    }
                }
            }
        }

        /// <summary>
        /// Returns a HSL inversted colour.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="darkenAmount"></param>
        /// <returns></returns>
        public Color HSL_Invert(Color Colour)
        {
            HSLColor HSL_Colour = new HSLColor(Colour);
            HSL_Colour.Hue = (HSL_Colour.Hue + 180) % 360;
            return HSL_Colour;
        }

        /// <summary>
        /// Load the details of the newly selected mod.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_Mod_List_SelectedIndexChanged(object sender, EventArgs e) { if (ListView_Mod_List.SelectedItems.Count > 0) { Set_Author_Name_Description(); } }


        /// <summary>
        /// Own implementation of mouse wheel scrolling for the listview control. We have removed the scrollbar, thus need to pay the sacrifice.
        /// </summary>
        private void Listview_Custom_Scroll_Handler(object sender, MouseEventArgs Event_Handler)
        {
            Mouse_Scroll_Current_Delta = Mouse_Scroll_Current_Delta + Event_Handler.Delta;
            int Lines_Scroll = (SystemInformation.MouseWheelScrollLines * Mouse_Scroll_Current_Delta / 120);

            try
            {
                if (Lines_Scroll >= 1) // Scroll up
                {
                    Mouse_Scroll_Current_Delta = 0;
                    if (ListView_Mod_List.SelectedIndices[0] - 1 >= 0) // Normal Scroll event
                    {
                        ListView_Mod_List.Items[ListView_Mod_List.SelectedIndices[0] - 1].Selected = true;
                    }
                    else { ListView_Mod_List.Items[ListView_Mod_List.Items.Count - 1].Selected = true; } // If it's the first item, go to last item.
                }
                else if (Lines_Scroll <= -1) // Scroll down with mouse
                {
                    Mouse_Scroll_Current_Delta = 0;
                    if (ListView_Mod_List.SelectedIndices[0] + 1 < ListView_Mod_List.Items.Count) // Normal scroll event.
                    {
                        ListView_Mod_List.Items[ListView_Mod_List.SelectedIndices[0] + 1].Selected = true;
                    }
                    else { ListView_Mod_List.Items[0].Selected = true; } // If it's the last item, select first from top again - loop scrolling.
                }

                ListView_Mod_List.EnsureVisible(ListView_Mod_List.SelectedIndices[0]);
            }
            catch { ListView_Mod_List.Items[0].Selected = true; } // If an item was not selected, select it :3
        }


        // Change in mouse wheel movement since last measurement.
        private static int Mouse_Scroll_Current_Delta; 
    }
}
