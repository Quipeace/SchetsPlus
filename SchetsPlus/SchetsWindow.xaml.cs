﻿using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace SchetsPlus
{
    public partial class SchetsWindow : MetroWindow
    {
        public List<SchetsControl> schetsControls = new List<SchetsControl>();
        public ObservableCollection<TabItem> tabItems = new ObservableCollection<TabItem>();    // ObservableCollection to automatically update tabControl
        public SchetsControl currentSchetsControl;

        public TabItem item;

        public SchetsWindow()
        {
            InitializeComponent();
            tabControl.ItemsSource = tabItems;

            addNewSchets();

            App.schetsWindows.Add(this);
        }

        public SchetsWindow(SchetsControl control)
        {
            InitializeComponent();
            tabControl.ItemsSource = tabItems;

            addExistingSchetsControl(control);

            App.schetsWindows.Add(this);
        }

        private void addNewSchets()
        {
            currentSchetsControl = new SchetsControl("Untitled(" + schetsControls.Count + ").schep");
            addCurrentSchetsControl();
        }

        private void addExistingSchetsControl(SchetsControl schetsControl)
        {
            currentSchetsControl = schetsControl;
            addCurrentSchetsControl();
        }

        private void addCurrentSchetsControl()
        {
            schetsControls.Add(currentSchetsControl);
            MetroWindow_SizeChanged_1(null, null);

            WindowsFormsHost newHost = new WindowsFormsHost();
            newHost.Height = currentSchetsControl.Height;
            newHost.Width = currentSchetsControl.Width;
            newHost.Child = currentSchetsControl;

            TabItem newItem = new TabItem();
            newItem.Header = currentSchetsControl.schets.imageName;
            newItem.Content = newHost;

            MenuItem mnCloseTab = new MenuItem();
            mnCloseTab.Header = "Close";
            mnCloseTab.Click += mnCloseTab_Click;
            mnCloseTab.Tag = newItem;

            MenuItem mnTabInWindow = new MenuItem();
            mnTabInWindow.Header = "Open in new window";
            mnTabInWindow.Click += mnTabInWindow_Click;
            mnTabInWindow.Tag = newItem;

            ContextMenu ctxMenu = new ContextMenu();
            ctxMenu.Items.Add(mnCloseTab);
            ctxMenu.Items.Add(mnTabInWindow);
            newItem.ContextMenu = ctxMenu;

            tabItems.Add(newItem);
        }

        private void MetroWindow_LocationChanged_1(object sender, EventArgs e)
        {
            pinWindows();                           // Location changed, so move windows accordingly
        }

        private void MetroWindow_Activated_1(object sender, EventArgs e)
        {
            App.currentSchetsWindow = this;         // Since only one window can be active at a time, this'll be the active window

            pinWindows();
            try                                     // Showing windows is done in a try/catch, an exception will occur if a window
            {                                       // was manually closed before
                App.toolsWindow.Show();
                App.toolsWindow.Topmost = true;
            }
            catch (Exception)
            {
            }
            try
            {
                App.colorPickerWindow.Show();
                App.colorPickerWindow.Topmost = true;
                App.colorPickerWindow.refreshColors();
            }
            catch (Exception)
            {
            }
            try
            {
                App.historyWindow.Show();
                App.historyWindow.Topmost = true;
            }
            catch (Exception)
            {
            }
        }

        private void MetroWindow_Deactivated_1(object sender, EventArgs e)
        {
            App.toolsWindow.Topmost = false;        // Window was deactivated, so helper windows don't have to be on top anymore
            App.colorPickerWindow.Topmost = false;
            App.historyWindow.Topmost = false;
        }

        private void MetroWindow_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.schetsWindows.Remove(this);         // Remove this window from the list of open windows
            if (App.schetsWindows.Count == 0)       // If the list is empty, close helpers as well
            {
                App.toolsWindow.Close();
                App.colorPickerWindow.Close();
                App.historyWindow.Close();
            }
        }

        private void MetroWindow_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            pinWindows();       // Size changed, move helper windows accordingly

            if (this.Width < currentSchetsControl.schets.imageSize.Width || (this.Height - 80) < currentSchetsControl.schets.imageSize.Height)
            {
                double widthDiff = this.Width / currentSchetsControl.schets.imageSize.Width;
                double heightDiff = (this.Height - 80) / currentSchetsControl.schets.imageSize.Height;

                if (heightDiff < widthDiff)
                {
                    currentSchetsControl.Height = (int) this.Height - 80;
                    currentSchetsControl.Width = (int) (currentSchetsControl.Height * currentSchetsControl.schets.imageRatio);
                }
                else
                {
                    currentSchetsControl.Width = (int)this.Width;
                    currentSchetsControl.Height = (int)((this.Width) / currentSchetsControl.schets.imageRatio);
                }
            }
            else
            {
                currentSchetsControl.Width = currentSchetsControl.schets.imageSize.Width;
                currentSchetsControl.Height = currentSchetsControl.schets.imageSize.Height;
            }
        }

        public void pinWindows()        // Pin all possible helper windows
        {
            if (App.toolsWindow.isPinned)
            {
                pinTools();
            }
            if (App.colorPickerWindow.isPinned)
            {
                pinColorPicker();
            }
            if (App.historyWindow.isPinned)
            {
                pinHistoryWindow();
            }
        }
        public void pinTools()          // If pinned, pin relative to the currently active (this) window.
        {
            App.toolsWindow.Top = this.Top;
            App.toolsWindow.Left = this.Left - App.toolsWindow.Width - 1;
        }
        public void pinColorPicker()    // Idem
        {
            App.colorPickerWindow.Top = this.Top + this.Height - App.colorPickerWindow.Height;
            App.colorPickerWindow.Left = this.Left + this.Width + 1;
        }
        public void pinHistoryWindow()  // What the comment above said
        {
            App.historyWindow.Top = this.Top;
            App.historyWindow.Left = this.Left + this.Width + 1;
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)    // A user changed tabs, set current control
        {                                                                                       // and resize it, just in case.
            if(tabControl.SelectedIndex != -1)                                                  // -1 == no tabs
            {
                currentSchetsControl = schetsControls[tabControl.SelectedIndex];
                MetroWindow_SizeChanged_1(null, null);
                App.colorPickerWindow.refreshColors();
            }
        }


        private void mnNew_Click(object sender, RoutedEventArgs e)
        {
            addNewSchets();
        }


        private void mnTools_Click(object sender, RoutedEventArgs e)        // Clicked on "tools" entry under view, show new tools menu
        {                                                                   // in case it's been closed
            App.toolsWindow = new ToolsWindow();
            App.toolsWindow.isPinned = true;
            App.toolsWindow.Show();
            pinTools();
        }
        private void mnHistory_Click(object sender, RoutedEventArgs e)      // Idem for other optional windows
        {
            App.historyWindow = new HistoryWindow();
            App.historyWindow.isPinned = true;
            App.historyWindow.Show();
            pinTools();
        }
        private void mnColorpicker_Click(object sender, RoutedEventArgs e)  // That
        {
            App.colorPickerWindow = new ColorPickerWindow();
            App.colorPickerWindow.isPinned = true;
            App.colorPickerWindow.Show();
            pinTools();
        }

        private void mnCloseTab_Click(object sender, RoutedEventArgs e)
        {
            MenuItem sendingItem = (MenuItem)sender;
            TabItem connectedTab = (TabItem)sendingItem.Tag;
            WindowsFormsHost controlHost = (WindowsFormsHost) connectedTab.Content;
            for(int i = 0; i < schetsControls.Count; i++)
            {
                if(schetsControls[i].Equals(controlHost.Child))
                {
                    schetsControls.Remove(schetsControls[i]);
                    break;
                }
            }
            tabItems.Remove((TabItem) sendingItem.Tag);
        }
        void mnTabInWindow_Click(object sender, RoutedEventArgs e)
        {
            MenuItem sendingItem = (MenuItem)sender;
            TabItem connectedTab = (TabItem)sendingItem.Tag;
            WindowsFormsHost controlHost = (WindowsFormsHost) connectedTab.Content;

            SchetsWindow newWindow = new SchetsWindow((SchetsControl) controlHost.Child);
            newWindow.Show();


            mnCloseTab_Click(sender, null);
        }

        private void mnSave_Click(object sender, RoutedEventArgs e)
        {
            saveCurrentSchets(false);
        }
        private void mnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            saveCurrentSchets(true);
        }

        private void saveCurrentSchets(Boolean alwaysShowDialog)
        {
            bool save;
            if (currentSchetsControl.schets.imagePath == null || alwaysShowDialog)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = currentSchetsControl.schets.imageName; // Default file name
                dlg.DefaultExt = ".schep";
                dlg.Filter = "SchetsPlus Image (.schep)|*.schep";
                save = (bool)dlg.ShowDialog();
                if (save)
                {
                    currentSchetsControl.schets.imagePath = dlg.FileName;
                    currentSchetsControl.schets.imageName = Path.GetFileName(dlg.FileName);
                }
            }
            else
            {
                save = true;
            }

            if (save)
            {
                for (int i = 0; i < schetsControls.Count; i++)
                {
                    if (schetsControls[i].Equals(currentSchetsControl))
                    {
                        tabItems[i].Header = currentSchetsControl.schets.imageName;
                    }
                }

                try
                {
                    if(currentSchetsControl.schets.imagePath.EndsWith(".bmp"))
                    {
                        Stream stream = File.Open(currentSchetsControl.schets.imagePath, FileMode.Create);
                        BinaryFormatter bFormatter = new BinaryFormatter();
                        bFormatter.Serialize(stream, currentSchetsControl.schets.bitmap);
                        stream.Close();
                    }
                    else
                    {
                        Stream stream = File.Open(currentSchetsControl.schets.imagePath, FileMode.Create);
                        BinaryFormatter bFormatter = new BinaryFormatter();
                        bFormatter.Serialize(stream, currentSchetsControl.schets);
                        stream.Close();
                    }
                }
                catch (IOException)
                {
                    // Show error
                }
            }
        }

        private void mnOpen_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}