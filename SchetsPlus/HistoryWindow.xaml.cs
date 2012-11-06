using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
                App.currentSchetsWindow.pinToolsWindow();
            }
        }

        public void updateHistoryList()
        {
            int i = App.currentSchetsWindow.currentSchetsControl.schets.actionDrawLimit + 1;

            while (i <= App.currentSchetsWindow.currentSchetsControl.schets.actions.Count() - 2)
            {
                App.currentSchetsWindow.currentSchetsControl.schets.actions.RemoveAt(i);
            }

            lvHistory.ItemsSource = App.currentSchetsWindow.currentSchetsControl.schets.actions;
            lvHistory.SelectedIndex = App.currentSchetsWindow.currentSchetsControl.schets.actions.Count() - 1;
            if (lvHistory.SelectedIndex != -1)
            {
                lvHistory.ScrollIntoView(App.currentSchetsWindow.currentSchetsControl.schets.actions[lvHistory.SelectedIndex]);
            }
        }

        private bool updatingSource = false;
        public void updateHistoryListSource()
        {
            updatingSource = true;
            try     // Catch NullPointerException that occurs when the history window isn't showing or currentSchetsControl hasn't been set yet.
            {
                lvHistory.ItemsSource = App.currentSchetsWindow.currentSchetsControl.schets.actions;
                lvHistory.SelectedIndex = App.currentSchetsWindow.currentSchetsControl.schets.actionDrawLimit;
                if (lvHistory.SelectedIndex != -1)
                {
                    lvHistory.ScrollIntoView(App.currentSchetsWindow.currentSchetsControl.schets.actions[lvHistory.SelectedIndex]);
                }
            }
            catch (NullReferenceException) { }
            updatingSource = false;
        }

        private void lvHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!updatingSource)
            {
                App.currentSchetsWindow.currentSchetsControl.schets.actionDrawLimit = lvHistory.SelectedIndex;
                App.currentSchetsWindow.currentSchetsControl.schets.TekenFromActions(App.currentSchetsWindow.currentSchetsControl);
            }
        }
    }
}
