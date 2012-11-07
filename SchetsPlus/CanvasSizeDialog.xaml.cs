using MahApps.Metro.Controls;
using System.Drawing;

namespace SchetsPlus
{
    public partial class CanvasSizeDialog : MetroWindow
    {
        public CanvasSizeDialog()
        {
            InitializeComponent();

            tbHeightPx.Text = App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Height.ToString();
            tbWidthPx.Text = App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Width.ToString();
        }

        private void btSubmit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int heightPx = int.Parse(tbHeightPx.Text);
            int widthPx = int.Parse(tbWidthPx.Text);

            if (heightPx != App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Height || widthPx != App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Width)
            {
                App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Height = heightPx;
                App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Width = widthPx;

                App.currentSchetsWindow.currentSchetsControl.schets.bitmap = new Bitmap(heightPx, widthPx);;

                App.currentSchetsWindow.MetroWindow_SizeChanged_1(null, null);

                App.currentSchetsWindow.currentSchetsControl.schets.Schoon();
                App.currentSchetsWindow.currentSchetsControl.schets.TekenFromActions(App.currentSchetsWindow.currentSchetsControl);

            }
            else
            {

            }
            this.Close();
        }
    }
}
