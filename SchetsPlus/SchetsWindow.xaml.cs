using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace SchetsPlus
{
    public partial class SchetsWindow : MetroWindow
    {
        public List<SchetsControl> schetsControls = new List<SchetsControl>();
        public List<WindowsFormsHost> hosts = new List<WindowsFormsHost>();                     // Hosts, opgeslagen voor schalen
        public ObservableCollection<TabItem> tabItems = new ObservableCollection<TabItem>();    // ObservableCollection zodat tabs automagisch toegevoegd worden
        public SchetsControl currentSchetsControl;
        public WindowsFormsHost currentHost;

        public TabItem item;

        public SchetsWindow()
        {
            InitializeComponent();
            tabControl.ItemsSource = tabItems;      // Tabs binden aan itemssource

            addNewSchets();                         // Eerste schets toevoegen
            App.schetsWindows.Add(this);            // Dit nieuwe window toevoegen aan lijst met windows
        }

        public SchetsWindow(SchetsControl control)  // Constructor aangeroepen wanneer een tab in een nieuw window geopend wordt.
        {
            InitializeComponent();
            tabControl.ItemsSource = tabItems;

            addExistingSchetsControl(control);

            App.schetsWindows.Add(this);
        }

        private void addNewSchets()                 // Nieuwe schets aanmaken en toevoegen
        {
            currentSchetsControl = new SchetsControl("new(" + schetsControls.Count + ").schep");
            addCurrentSchetsControl();
        }

        private void addExistingSchetsControl(SchetsControl schetsControl)  // bestaande schets toe laten voegen aan lijst
        {
            currentSchetsControl = schetsControl;
            addCurrentSchetsControl();
        }

        private void addCurrentSchetsControl()              // Huidige schetscontrol aan tabItems en lijst toevoegen
        {
            schetsControls.Add(currentSchetsControl);      
            MetroWindow_SizeChanged_1(null, null);          // control laten schalen indien het groter is dan het huidige venster
            
            WindowsFormsHost newHost = new WindowsFormsHost();  // WindowsFormsHosts om bestaande SchetsControl in te kunnen huisvesten
            newHost.Width = currentSchetsControl.Width;
            newHost.Height = currentSchetsControl.Height;
            newHost.Child = currentSchetsControl;
            hosts.Add(newHost);

            TabItem newItem = new TabItem();                    // Tabitem aanmaken met imageNaam, enige content is de host van hierboven
            newItem.Header = currentSchetsControl.schets.imageName;
            newItem.Content = newHost;

            MenuItem mnCloseTab = new MenuItem();               // Menuitem voor het sluiten van een tab
            mnCloseTab.Header = "Close";
            mnCloseTab.Click += mnCloseTab_Click;
            mnCloseTab.Tag = newItem;

            MenuItem mnTabInWindow = new MenuItem();            // Menuitem voor het openen in nieuw venster
            mnTabInWindow.Header = "Open in new window";
            mnTabInWindow.Click += mnTabInWindow_Click;
            mnTabInWindow.Tag = newItem;

            ContextMenu ctxMenu = new ContextMenu();            // Menuitems toevoegen aan contextmenu, voor een right-click action
            ctxMenu.Items.Add(mnCloseTab);
            ctxMenu.Items.Add(mnTabInWindow);
            newItem.ContextMenu = ctxMenu;

            tabItems.Add(newItem);                              // Tabitem aan lijst toevoegen, en als huidige tab zetten
            tabControl.SelectedIndex = tabItems.Count - 1;
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
                App.historyWindow.updateHistoryListSource();
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

        private void MetroWindow_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)  // Sluiten van dit venster
        {
            int unsavedControl = -1;
            for (int i = 0; i < schetsControls.Count; i++)      // Door lijst van controls van dit venster lopen
            {
                if (schetsControls[i].isDirty)                  // Als isDirty heeft de gebruiker iets aangepast sinds het openen
                {
                    unsavedControl = i;
                }
            }
            if (unsavedControl != -1)
            {
                MessageBoxResult result = MessageBox.Show(schetsControls[unsavedControl].schets.imageName + " contains unsaved changes, close anyway?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);
                e.Cancel = result == MessageBoxResult.No;       // De gebruiker kiest ervoor niet te sluiten
            }

            if (!e.Cancel)
            {
                App.schetsWindows.Remove(this);         // Remove this window from the list of open windows
                if (App.schetsWindows.Count == 0)       // If the list is empty, close helpers as well
                {
                    App.toolsWindow.Close();
                    App.colorPickerWindow.Close();
                    App.historyWindow.Close();
                }
            }
            else
            {
                tabControl.SelectedIndex = unsavedControl;  // De gebruiker wil het bestand eerst opslaan voor sluiten, actief maken
                mnSave_Click(null, null);                   // en opslaan simuleren
            }
        }

        public void MetroWindow_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            pinWindows();       // Size changed, move helper windows accordingly
                                // Als het venster kleiner is dan het plaatje...
            if (this.Width < currentSchetsControl.schets.imageSize.Width || (this.Height - 80) < currentSchetsControl.schets.imageSize.Height)
            {
                double widthDiff = this.Width / currentSchetsControl.schets.imageSize.Width;            // Verhoudingen berekenen
                double heightDiff = (this.Height - 80) / currentSchetsControl.schets.imageSize.Height;

                if (heightDiff < widthDiff)     // Als de hoogte meer afwijkt dan de breedte
                {
                    currentSchetsControl.Height = (int)this.Height - 80;        // De hoogte als uitgangspunt nemen
                    currentSchetsControl.Width = (int)(currentSchetsControl.Height * currentSchetsControl.schets.imageRatio);
                }
                else
                {
                    currentSchetsControl.Width = (int)this.Width;               // Zo niet, de breedte
                    currentSchetsControl.Height = (int)((this.Width) / currentSchetsControl.schets.imageRatio);
                }
            }
            else        // Het plaatje is niet groter dan het venster, dus de volledige size zetten
            {
                currentSchetsControl.Width = currentSchetsControl.schets.imageSize.Width;
                currentSchetsControl.Height = currentSchetsControl.schets.imageSize.Height;
            }

            try         // Try zodat een nullpointer wordt voorkomen bij het aanmaken van een schets (waar schaling van de host nog niet belangrijk is)
            {
                App.currentSchetsWindow.currentHost.Width = currentSchetsControl.Width; // Host evengroot als plaatje maken om clipping te voorkomen
                App.currentSchetsWindow.currentHost.Height = currentSchetsControl.Height;
            }
            catch (NullReferenceException) { }
        }

        public void pinWindows()        // Pin all possible helper windows
        {
            if (App.toolsWindow.isPinned)
            {
                pinToolsWindow();
            }
            if (App.colorPickerWindow.isPinned)
            {
                pinColorPickerWindow();
            }
            if (App.historyWindow.isPinned)
            {
                pinHistoryWindow();
            }
        }
        public void pinToolsWindow()    // If pinned, pin relative to the currently active (this) window.
        {
            App.toolsWindow.Top = this.Top;
            App.toolsWindow.Left = this.Left - App.toolsWindow.Width - 1;
        }
        public void pinColorPickerWindow()    // Idem
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
                currentHost = hosts[tabControl.SelectedIndex];
                MetroWindow_SizeChanged_1(null, null);
                App.colorPickerWindow.refreshColors();
                App.historyWindow.updateHistoryListSource();
            }
        }

        private void mnNew_Click(object sender, RoutedEventArgs e)
        {
            addNewSchets();
        }

        private void mnTools_Click(object sender, RoutedEventArgs e)        // Clicked on "tools" entry under view, show new tools menu
        {                                                                   // in case it's been closed
            try
            {
                App.toolsWindow.Close();
            }
            catch(Exception){}
            App.toolsWindow = new ToolsWindow();
            App.toolsWindow.isPinned = true;
            App.toolsWindow.Show();
            pinToolsWindow();
        }
        private void mnHistory_Click(object sender, RoutedEventArgs e)      // Idem for other optional windows
        {
            try
            {
                App.historyWindow.Close();
            }
            catch (Exception) { }
            App.historyWindow = new HistoryWindow();
            App.historyWindow.isPinned = true;
            App.historyWindow.Show();
            pinHistoryWindow();
        }
        private void mnColorpicker_Click(object sender, RoutedEventArgs e)  // That
        {
            try
            {
                App.colorPickerWindow.Close();
            }
            catch (Exception) { }
            App.colorPickerWindow = new ColorPickerWindow();
            App.colorPickerWindow.isPinned = true;
            App.colorPickerWindow.Show();
            pinColorPickerWindow();
        }

        private void mnCloseTab_Click(object sender, RoutedEventArgs e)     // Een tab wil gesloten worden
        {
            MenuItem sendingItem = (MenuItem)sender;
            TabItem connectedTab = (TabItem)sendingItem.Tag;
            WindowsFormsHost controlHost = (WindowsFormsHost) connectedTab.Content; // Host ophalen uit tabItem
            for(int i = 0; i < schetsControls.Count; i++)
            {
                if(schetsControls[i].Equals(controlHost.Child))                     // Nummer van schetsControl opzoeken, en uit lijst verwijderen
                {
                    schetsControls.Remove(schetsControls[i]);
                    break;
                }
            }
            tabItems.Remove((TabItem) sendingItem.Tag);                             // Uiteindelijk de tab zelf verwijderen
        }

        void mnTabInWindow_Click(object sender, RoutedEventArgs e)                  // Tab openen in nieuw venster
        {
            MenuItem sendingItem = (MenuItem)sender; 
            TabItem connectedTab = (TabItem)sendingItem.Tag;
            WindowsFormsHost controlHost = (WindowsFormsHost) connectedTab.Content; // Host weer opzoeken

            SchetsWindow newWindow = new SchetsWindow((SchetsControl) controlHost.Child);   // Nieuw venster openen met het bestaande schetscontrol
            newWindow.Show();
            
            mnCloseTab_Click(sender, null);
        }

        private void mnSave_Click(object sender, RoutedEventArgs e)
        {
            saveCurrentSchets(false);       // Opslaan zonder verplicht dialog
        }
        private void mnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            saveCurrentSchets(true);       // Opslaan met verplicht dialog
        }

        private void saveCurrentSchets(Boolean alwaysShowDialog)
        {
            bool save;
            if (currentSchetsControl.schets.imagePath == null || alwaysShowDialog)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();      // Nieuw save dialog    
                dlg.FileName = currentSchetsControl.schets.imageName; // Default file name
                dlg.DefaultExt = ".schep";
                dlg.Filter = "SchetsPlus Image (.schep)|*.schep|" +                             // COmpatible bestandsformaten
                             "BMP (*.bmp)|*.bmp|"+
                             "EMF (*.emf)|*.emf|"+
                             "GIF (*.gif)|*.gif|" +
                             "ICO (*.ico)|*.ico|" +
                             "JPEG (*.jpg)|*.jpg|" +
                             "PNG (*.png)|*.png|" +
                             "TIFF (*.tif)|*.tif|" +
                             "WMF (*.wmf)|*.wmf|" +
                             "All Files (*.*)|*.*";
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
                currentSchetsControl.isDirty = false;           // Opgeslagen, dus niet dirty

                for (int i = 0; i < schetsControls.Count; i++)  // Bestandsnaam updaten
                {
                    if (schetsControls[i].Equals(currentSchetsControl))
                    {
                        tabItems[i].Header = currentSchetsControl.schets.imageName;
                    }
                }

                try
                {
                    if(currentSchetsControl.schets.imagePath.EndsWith(".bmp"))  // bitmap opslaan naar extensie
                    {
                        currentSchetsControl.schets.bitmap.Save(currentSchetsControl.schets.imagePath, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    else if (currentSchetsControl.schets.imagePath.EndsWith(".emf"))
                    {
                        currentSchetsControl.schets.bitmap.Save(currentSchetsControl.schets.imagePath, System.Drawing.Imaging.ImageFormat.Emf);
                    }
                    else if (currentSchetsControl.schets.imagePath.EndsWith(".gif"))
                    {
                        currentSchetsControl.schets.bitmap.Save(currentSchetsControl.schets.imagePath, System.Drawing.Imaging.ImageFormat.Gif);
                    }
                    else if (currentSchetsControl.schets.imagePath.EndsWith(".ico"))
                    {
                        currentSchetsControl.schets.bitmap.Save(currentSchetsControl.schets.imagePath, System.Drawing.Imaging.ImageFormat.Icon);
                    }
                    else if (currentSchetsControl.schets.imagePath.EndsWith(".jpg"))
                    {
                        currentSchetsControl.schets.bitmap.Save(currentSchetsControl.schets.imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    else if (currentSchetsControl.schets.imagePath.EndsWith(".png"))
                    {
                        currentSchetsControl.schets.bitmap.Save(currentSchetsControl.schets.imagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else if (currentSchetsControl.schets.imagePath.EndsWith(".tif"))
                    {
                        currentSchetsControl.schets.bitmap.Save(currentSchetsControl.schets.imagePath, System.Drawing.Imaging.ImageFormat.Tiff);
                    }
                    else if (currentSchetsControl.schets.imagePath.EndsWith(".wmf"))
                    {
                        currentSchetsControl.schets.bitmap.Save(currentSchetsControl.schets.imagePath, System.Drawing.Imaging.ImageFormat.Wmf);
                    }
                    else  // Anders opslaan als .schep
                    {
                        Stream stream = File.Open(currentSchetsControl.schets.imagePath, FileMode.Create);  // Stream openen
                        BinaryFormatter formatter = new BinaryFormatter();                                  // Formatter aanmaken
                        formatter.Serialize(stream, currentSchetsControl.schets);                           // Serializen en schrijven
                        stream.Close();
                    }
                }
                catch (IOException)
                {
                    currentSchetsControl.isDirty = true;        // Een fout, dus niet opgeslagen, dus nog steeds dirty
                    MessageBoxResult result = MessageBox.Show("An error occured saving" + currentSchetsControl.schets.imageName + ". Retry?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        mnSave_Click(null, null);   // opnieuw proberen naar gebruikers inbreng
                    }
                }

            }
        }

        private void mnOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".schep";
               dlg.Filter = "SchetsPlus Image (.schep)|*.schep|" +  // Enkel BMP compatible bij laden (wellicht door transparantie?)
                             "BMP (*.bmp)|*.bmp|"+
                             "All Files (*.*)|*.*";

            // Show open file dialog box 
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                String newImagePath = dlg.FileName;
                if(currentSchetsControl.schets.actions.Count != 0)
                {
                    addNewSchets();         // Nieuw schetscontrol
                }

                if (newImagePath.EndsWith(".schep"))    // Schetsbestand, dus openen en deserializen
                {
                    Stream stream = File.OpenRead(newImagePath);
                    BinaryFormatter formatter = new BinaryFormatter();
                    currentSchetsControl.schets = (Schets)formatter.Deserialize(stream);
                    stream.Close();

                    currentSchetsControl.schets.TekenFromActions(currentSchetsControl);
                }
                else    // Het is een conventioneel plaatje, dus bitmap laten openen en aan de huidige control toevoegen
                {
                    Bitmap bmp = new Bitmap(newImagePath);
                    currentSchetsControl.schets.loadedBitmap = bmp;
                    currentSchetsControl.schets.bitmap = new Bitmap(bmp.Width, bmp.Height);
                    currentSchetsControl.schets.imageSize = currentSchetsControl.schets.bitmap.Size;
                    currentSchetsControl.schets.imageRatio = (double)currentSchetsControl.schets.imageSize.Width / (double)currentSchetsControl.schets.imageSize.Height;
                    App.currentSchetsWindow.MetroWindow_SizeChanged_1(null, null);

                    currentSchetsControl.Schoon();
                }

                currentSchetsControl.schets.imagePath = newImagePath;   // Pad updaten
                App.historyWindow.updateHistoryList();                  // Net zoals de history
            }
        }

        private void mnModifyCanvas_Click(object sender, RoutedEventArgs e)
        {
            CanvasSizeDialog dialog = new CanvasSizeDialog();           // canvasdialog openen
            dialog.Left = this.Left + this.Width - dialog.Width;
            dialog.Top = this.Top + 30;
            dialog.ShowDialog();
        }

        private void mnRotateCCW_Click(object sender, RoutedEventArgs e)    // Rotate couterclockwise-niet geimplementeerd
        {
            RotateAction newAction = new RotateAction(false);
            currentSchetsControl.schets.actions.Add(newAction);

            App.historyWindow.updateHistoryList();
        }

        private void mnRotateCw_Click(object sender, RoutedEventArgs e)     // clockwise, idem.
        {
            RotateAction newAction = new RotateAction(false);
            currentSchetsControl.schets.actions.Add(newAction);

            App.historyWindow.updateHistoryList();
        }
    }
}
