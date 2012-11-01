using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows.Forms.Integration;

namespace SchetsPlus
{
    public partial class SchetsWindow : MetroWindow
    {
        public List<SchetsControl> schetsControls = new List<SchetsControl>();
        public SchetsControl currentSchetsControl;

        public SchetsWindow()
        {
            InitializeComponent();

            addNewSchets();

            App.schetsWindows.Add(this);
        }

        public SchetsWindow(SchetsControl control)
        {
            InitializeComponent();

            addExistingSchetsControl(control);
        }

        private void addNewSchets()
        {
            currentSchetsControl = new SchetsControl("Untitled(" + schetsControls.Count + ").schep");
            MetroWindow_SizeChanged_1(null, null);

            WindowsFormsHost newHost = new WindowsFormsHost();
            newHost.Height = currentSchetsControl.Height;
            newHost.Width = currentSchetsControl.Width;
            newHost.Child = currentSchetsControl;

            schetsControls.Add(currentSchetsControl);
            currentSchetsControl.Height = 500;
            currentSchetsControl.Width = 700;
            TabItem newItem = new TabItem();
            newItem.Header = currentSchetsControl.schets.imageName;
            newItem.Content = newHost;
            tabControl.Items.Add(newItem);
        }
        private void addExistingSchetsControl(SchetsControl schetsControl)
        {
            currentSchetsControl = schetsControl;
            MetroWindow_SizeChanged_1(null, null);

            WindowsFormsHost newHost = new WindowsFormsHost();
            newHost.Height = currentSchetsControl.Height;
            newHost.Width = currentSchetsControl.Width;
            newHost.Child = currentSchetsControl;

            schetsControls.Add(currentSchetsControl);
            currentSchetsControl.Height = 500;
            currentSchetsControl.Width = 700;
            TabItem newItem = new TabItem();
            newItem.Header = currentSchetsControl.schets.imageName;
            newItem.Content = newHost;
            tabControl.Items.Add(newItem);
        }


        private void MetroWindow_LocationChanged_1(object sender, EventArgs e)
        {
            pinWindows();
        }

        private void MetroWindow_Activated_1(object sender, EventArgs e)
        {
            App.currentSchetsWindow = this;

            pinWindows();
            App.toolsWindow.Show();
            App.toolsWindow.Topmost = true;
            App.colorPickerWindow.Show();
            App.colorPickerWindow.Topmost = true;
            App.historyWindow.Show();
            App.historyWindow.Topmost = true;
        }

        private void MetroWindow_Deactivated_1(object sender, EventArgs e)
        {
            App.toolsWindow.Topmost = false;
            App.colorPickerWindow.Topmost = false;
            App.historyWindow.Topmost = false;
        }

        private void MetroWindow_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.schetsWindows.Remove(this);
            if (App.schetsWindows.Count == 0)
            {
                App.toolsWindow.Close();
                App.colorPickerWindow.Close();
                App.historyWindow.Close();
            }
        }

        private void MetroWindow_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            pinWindows();

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

        public void pinWindows()
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
        public void pinTools()
        {
            App.toolsWindow.Top = this.Top;
            App.toolsWindow.Left = this.Left - App.toolsWindow.Width - 1;
        }
        public void pinColorPicker()
        {
            App.colorPickerWindow.Top = this.Top + this.Height - App.colorPickerWindow.Height;
            App.colorPickerWindow.Left = this.Left + this.Width + 1;
        }
        public void pinHistoryWindow()
        {
            App.historyWindow.Top = this.Top;
            App.historyWindow.Left = this.Left + this.Width + 1;
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSchetsControl = schetsControls[tabControl.SelectedIndex];
            MetroWindow_SizeChanged_1(null, null);
        }

        private void button_new_Click(object sender, RoutedEventArgs e)
        {
            addNewSchets();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SchetsWindow newWindow = new SchetsWindow(currentSchetsControl);
            newWindow.Show();

            for (int i = 0; i < schetsControls.Count; i++)
            {
                if (schetsControls[i].Equals(schetsControls))
                {
                    tabControl.Items.RemoveAt(i);
                    schetsControls.RemoveAt(i);
                }
            }
        }
    }
}
