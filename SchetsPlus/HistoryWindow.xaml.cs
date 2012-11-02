using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    public partial class HistoryWindow : MetroWindow
    {
        public bool isPinned;

        public HistoryWindow()
        {
            InitializeComponent();

            this.isPinned = true;
        }

        private void btPin_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;
            if (isPinned)
            {
                App.currentSchetsWindow.pinTools();
            }
        }

        public void updateHistoryList()
        {
            int i = App.currentSchetsWindow.currentSchetsControl.schets.actionDrawLimit + 1; 
            while(i <= App.currentSchetsWindow.currentSchetsControl.schets.actions.Count() - 2)
            {
                App.currentSchetsWindow.currentSchetsControl.schets.actions.RemoveAt(App.currentSchetsWindow.currentSchetsControl.schets.actionDrawLimit + 1);
            }

            lvHistory.ItemsSource = App.currentSchetsWindow.currentSchetsControl.schets.actions;
            lvHistory.SelectedIndex = App.currentSchetsWindow.currentSchetsControl.schets.actions.Count() - 1;
            lvHistory.ScrollIntoView(App.currentSchetsWindow.currentSchetsControl.schets.actions[lvHistory.SelectedIndex]);
        }

        int previousSelection = 0;
        private void lvHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.currentSchetsWindow.currentSchetsControl.schets.actionDrawLimit = lvHistory.SelectedIndex;
            if (lvHistory.SelectedIndex != App.currentSchetsWindow.currentSchetsControl.schets.actions.Count() - 1 || lvHistory.SelectedIndex - previousSelection >= 1)
            {
                App.currentSchetsWindow.currentSchetsControl.schets.Schoon();
                App.currentSchetsWindow.currentSchetsControl.schets.TekenFromActions(App.currentSchetsWindow.currentSchetsControl);
            }
            else 
            {
                App.currentSchetsWindow.currentSchetsControl.schets.actions[lvHistory.SelectedIndex].draw(App.currentSchetsWindow.currentSchetsControl);
            }
            previousSelection = lvHistory.SelectedIndex;
        }
    }
}
