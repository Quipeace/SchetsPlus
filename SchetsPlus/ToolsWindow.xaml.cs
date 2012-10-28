/*      Mahapps.Metro door MahApps, implementatie door
 * 
 * 
 * 
 * 
 */



using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace SchetsPlus
{

    public partial class ToolsWindow : MetroWindow
    {
        private MainWindow mainWindow;
        public bool isPinned;

        public ToolsWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
            this.isPinned = true;
        }

        private void btPin_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;
            if (isPinned)
            {
                mainWindow.pinTools();
            }
        }
    }
}
