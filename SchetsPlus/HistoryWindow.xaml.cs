using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SchetsPlus
{
    public partial class HistoryWindow : MetroWindow
    {
        public bool isPinned;   //Het declareren van de boolean isPinned die bekijkt of er op pin gedrukt is

        public HistoryWindow()
        {
            InitializeComponent();

            this.isPinned = true;   //isPinned standaard true, zodat standaard gepint is
        }

        private void btPin_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;   //Verander isPinned indien erop geklikt is
            if (isPinned)
            {
                App.currentSchetsWindow.pinHistoryWindow(); //Indien ispinned is true, pin dan
            }
        }

        public void updateHistoryList()
        {
            int i = App.currentSchetsWindow.currentSchetsControl.schets.actionDrawLimit + 1;    //Zetten van variabele i, voor de while loop

            while (i <= App.currentSchetsWindow.currentSchetsControl.schets.actions.Count() - 2)
            {
                App.currentSchetsWindow.currentSchetsControl.schets.actions.RemoveAt(i);    //Verwijder alles onder de plek waar geklikt is
            }

            lvHistory.ItemsSource = App.currentSchetsWindow.currentSchetsControl.schets.actions;    //Zeggen dat de source de actions zijn
            lvHistory.SelectedIndex = App.currentSchetsWindow.currentSchetsControl.schets.actions.Count() - 1;  //De selectedindex is de laatste, aangezien je bij 0 begint
            if (lvHistory.SelectedIndex != -1)
            {
                lvHistory.ScrollIntoView(App.currentSchetsWindow.currentSchetsControl.schets.actions[lvHistory.SelectedIndex]); //Scrollen naar de laatste in de list
            }
        }

        private bool updatingSource = false;
        public void updateHistoryListSource()
        {
            updatingSource = true;  //Zetten van de boolean updatingSource dat er geupdate wordt
            try     // Catch NullPointerException that occurs when the history window isn't showing or currentSchetsControl hasn't been set yet.
            {
                lvHistory.ItemsSource = App.currentSchetsWindow.currentSchetsControl.schets.actions;    //De source is weer de actions uit de list
                lvHistory.SelectedIndex = App.currentSchetsWindow.currentSchetsControl.schets.actionDrawLimit;  //De geselecteerde is de laatste
                if (lvHistory.SelectedIndex != -1)
                {
                    lvHistory.ScrollIntoView(App.currentSchetsWindow.currentSchetsControl.schets.actions[lvHistory.SelectedIndex]); //Als het niet -1 is scroll dan naar de laatste
                }
            }
            catch (NullReferenceException) { }
            updatingSource = false;     //Updaten is weer false
        }

        private void lvHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!updatingSource)    //Indien er op een andere is geklikt uit de lijst en updatingSource is false, maak dan de actionDrawLimit naar de geselecteerde
            {
                App.currentSchetsWindow.currentSchetsControl.schets.actionDrawLimit = lvHistory.SelectedIndex;
                App.currentSchetsWindow.currentSchetsControl.schets.TekenFromActions(App.currentSchetsWindow.currentSchetsControl);
            }
        }

        private void btClean_Click(object sender, RoutedEventArgs e)
        {
            lvHistory.SelectedIndex = - 1;      //Indien er op clean geklikt is, dan is de geselecteerde -1
        }

        private void btUndo_Click(object sender, RoutedEventArgs e)
        {
            if (lvHistory.SelectedIndex >= 0)       //Indien er op undo geklikt kan worden en erop geklikt is ga er dan 1tje terug
            {
                lvHistory.SelectedIndex = lvHistory.SelectedIndex - 1;
            }
        }

        private void btRedo_Click(object sender, RoutedEventArgs e)     //Indien er op redo geklikt kan worden en erop geklikt is ga er dan 1 vooruit
        {
            if (lvHistory.SelectedIndex < App.currentSchetsWindow.currentSchetsControl.schets.actions.Count)
            {
                lvHistory.SelectedIndex++;
            }
        }
    }
}
