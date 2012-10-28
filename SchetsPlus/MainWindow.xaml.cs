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
    public partial class MainWindow : MetroWindow
    {
        private ToolsWindow toolsWindow;
        private ColorPickerWindow colorPickerWindow;

        private SchetsControl currentSchetsControl;
        private List<WindowsFormsHost> schetsControlHosts = new List<WindowsFormsHost>();
        private List<SchetsControl> schetsControls = new List<SchetsControl>();

        public MainWindow()
        {
            InitializeComponent();

            toolsWindow = new ToolsWindow(this);
            toolsWindow.Show();
            colorPickerWindow = new ColorPickerWindow(this);
            colorPickerWindow.Show();

            addNewSchets();
        }

        private void addNewSchets()
        {
            currentSchetsControl = new SchetsControl();
            MetroWindow_SizeChanged_1(null, null);

            WindowsFormsHost newHost = new WindowsFormsHost();
            newHost.Height = currentSchetsControl.Height;
            newHost.Width = currentSchetsControl.Width;
            newHost.Child = currentSchetsControl;

            schetsControls.Add(currentSchetsControl);
            currentSchetsControl.Height = 500;
            currentSchetsControl.Width = 700;
            TabItem newItem = new TabItem();
            newItem.Header = "Untitled(" + schetsControls.Count + ").schep";
            newItem.Content = newHost;
            tabControl.Items.Add(newItem);
            tabControl.SelectedIndex = schetsControls.Count - 1;
        }

        private void MetroWindow_LocationChanged_1(object sender, EventArgs e)
        {
            pinWindows();
        }

        private void MetroWindow_Activated_1(object sender, EventArgs e)
        {
            pinWindows();

            toolsWindow.Topmost = true;
            colorPickerWindow.Topmost = true;
        }

        private void MetroWindow_Deactivated_1(object sender, EventArgs e)
        {
            toolsWindow.Topmost = false;
            colorPickerWindow.Topmost = false;
        }

        private void MetroWindow_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            toolsWindow.Close();
            colorPickerWindow.Close();
        }

        private void MetroWindow_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            pinWindows();

            Debug.WriteLine("SIZECHANGE");
            if (this.Width < currentSchetsControl.schets.imageSize.Width || (this.Height - 80) < currentSchetsControl.schets.imageSize.Height)
            {
                double widthDiff = this.Width / currentSchetsControl.schets.imageSize.Width;
                double heightDiff = (this.Height - 80) / currentSchetsControl.schets.imageSize.Height;

                if (heightDiff < widthDiff)
                {
                    currentSchetsControl.Height = (int) this.Height - 80;
                    currentSchetsControl.Width = (int) (currentSchetsControl.Height * currentSchetsControl.schets.ratio);
                }
                else
                {
                    currentSchetsControl.Width = (int)this.Width;
                    currentSchetsControl.Height = (int)((this.Width) / currentSchetsControl.schets.ratio);
                }
            }
            else
            {

                currentSchetsControl.Width = currentSchetsControl.schets.imageSize.Width;
                currentSchetsControl.Height = currentSchetsControl.schets.imageSize.Height;

                Debug.WriteLine("SETWIDTH: " + currentSchetsControl.Height);
            }
        }

        public void pinWindows()
        {
            if (toolsWindow.isPinned)
            {
                pinTools();
            }
            if (colorPickerWindow.isPinned)
            {
                pinColorPicker();
            }
        }
        public void pinTools()
        {
            toolsWindow.Height = this.Height;
            toolsWindow.Top = this.Top;
            toolsWindow.Left = this.Left - toolsWindow.Width - 1;

        }
        public void pinColorPicker()
        {
            colorPickerWindow.Top = this.Top + this.Height - colorPickerWindow.Height;
            colorPickerWindow.Left = this.Left + this.Width + 1;
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSchetsControl = schetsControls[tabControl.SelectedIndex];
            MetroWindow_SizeChanged_1(null, null);
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            addNewSchets();
        }
    }
}
