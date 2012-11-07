using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace SchetsPlus
{
    public partial class CanvasSizeDialog : MetroWindow
    {
        public CanvasSizeDialog()
        {
            InitializeComponent();

            tbHeightPx.Text = App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Height.ToString();
            tbWidthPx.Text = App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Width.ToString();
            tbWidthPer.Text = "100";
            tbHeightPer.Text = "100";
        }

        private void btSubmit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                modifyCanvasSize(int.Parse(tbWidthPx.Text), int.Parse(tbHeightPx.Text));
            }
            else
            {
                int newWidth = (int) ((double.Parse(tbWidthPer.Text)/100) * App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Width);
                int newHeight = (int) ((double.Parse(tbHeightPer.Text)/100) * App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Height);

                modifyCanvasSize(newWidth, newHeight);
            }
            this.Close();
        }

        private void modifyCanvasSize(int newWidth, int newHeight)
        {
            if ((newWidth > 4000 && newHeight > 4000) || newWidth < 1 || newHeight < 1)
            {
                return;
            }

            Schets existingSchets = App.currentSchetsWindow.currentSchetsControl.schets;
            Schets newSchets = new Schets(existingSchets.imageName, new System.Drawing.Size(newWidth, newHeight));
            newSchets.imagePath = existingSchets.imagePath;
            newSchets.actions = existingSchets.actions;
            newSchets.actionDrawLimit = existingSchets.actionDrawLimit;
            App.currentSchetsWindow.currentSchetsControl.schets = newSchets;

            App.currentSchetsWindow.currentSchetsControl.schets.TekenFromActions(App.currentSchetsWindow.currentSchetsControl);

            App.currentSchetsWindow.MetroWindow_SizeChanged_1(null, null);
        }
    }
}
