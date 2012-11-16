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

            tbHeightPx.Text = App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Height.ToString(); //Zeggen welke width en height getallen er moeten staan
            tbWidthPx.Text = App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Width.ToString();
            tbWidthPer.Text = "100";    //Zeggen welke percentages er standaard moeten staan
            tbHeightPer.Text = "100";
        }

        private void btSubmit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                modifyCanvasSize(int.Parse(tbWidthPx.Text), int.Parse(tbHeightPx.Text));    //Indien pixels gekozen zijn, bereken dan de Size met dit
            }
            else
            {
                int newWidth = (int) ((double.Parse(tbWidthPer.Text)/100) * App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Width);   //Bereken eerst de pixels mbv percentages
                int newHeight = (int) ((double.Parse(tbHeightPer.Text)/100) * App.currentSchetsWindow.currentSchetsControl.schets.imageSize.Height);

                modifyCanvasSize(newWidth, newHeight);  //Roep de methode aan met de waardes
            }
            this.Close();   //Sluit de dialog
        }

        private void modifyCanvasSize(int newWidth, int newHeight)
        {
            if ((newWidth > 4000 && newHeight > 4000) || newWidth < 1 || newHeight < 1)
            {
                return; //De width en height mogen niet te groot of te klein worden
            }

            Schets existingSchets = App.currentSchetsWindow.currentSchetsControl.schets;    //Neem de bestaande schets
            Schets newSchets = new Schets(existingSchets.imageName, new System.Drawing.Size(newWidth, newHeight));  //Maak een nieuwe schets van deze, maar dan met andere width en height
            newSchets.imagePath = existingSchets.imagePath; //Neem de actions etc over van oude schets
            newSchets.actions = existingSchets.actions;
            newSchets.actionDrawLimit = existingSchets.actionDrawLimit;
            App.currentSchetsWindow.currentSchetsControl.schets = newSchets;    //Zeg dat de schets = nieuwe schets

            App.currentSchetsWindow.currentSchetsControl.schets.TekenFromActions(App.currentSchetsWindow.currentSchetsControl); //Tekenen van de actions

            App.currentSchetsWindow.MetroWindow_SizeChanged_1(null, null);
        }
    }
}
