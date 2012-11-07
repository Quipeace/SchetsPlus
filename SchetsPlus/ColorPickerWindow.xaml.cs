using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SchetsPlus
{
    public partial class ColorPickerWindow : MetroWindow
    {
        public bool isPinned;

        public ColorPickerWindow()
        {
            InitializeComponent();
           
            this.isPinned = true;   //Zet standaard op pinned
        }

        private void btPin_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;   //Verander pinned naar tegenovergestelde
            if (isPinned)
            {
                App.currentSchetsWindow.pinColorPickerWindow(); //Indien pinned = true, pin dan
            }
        }

        private void sliderRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbRed.Text = ((int)sliderRed.Value).ToString(); //Neem de waarde van de slider en maak hier een string van
            setColorFromBoxes();    //Gebruik deze color als primary color
            updateHexColor();   //Update de heximale kleur
        }
        private void sliderGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbGreen.Text = ((int)sliderGreen.Value).ToString(); //Neem de waarde van de slider en maak hier een string van
            setColorFromBoxes();    //Gebruik deze color als primary color
            updateHexColor();   //Update de heximale kleur
        }
        private void sliderBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbBlue.Text = ((int)sliderBlue.Value).ToString();   //Neem de waarde van de slider en maak hier een string van
            setColorFromBoxes();    //Gebruik deze color als primary color
            updateHexColor();   //Update de heximale kleur
        }
        private void tbHexColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            string colorcode = tbHexColor.Text;
            try         //Probeer de primarykleur te zetten
            {
                int colorAsInt = Int32.Parse(colorcode.Replace("#", ""), NumberStyles.HexNumber);
                byte b = (byte)(colorAsInt & 255);  //Het zetten van de verschillende kleuren
                byte g = (byte)((colorAsInt >> 8) & 255);
                byte r = (byte)((colorAsInt >> 16) & 255);
                if (cvPrimaryColor != null)
                {
                    cvPrimaryColor.Background = new SolidColorBrush(Color.FromArgb(255, r, g, b));  
                    App.currentSchetsWindow.currentSchetsControl.schets.primaryColor = System.Drawing.Color.FromArgb(255, r, g, b); //Zeg dat de primaryColor met deze kleur wordt
                }
            }
            catch (FormatException)
            { }
        }

        private void setColorFromBoxes()
        {
            App.currentSchetsWindow.currentSchetsControl.schets.primaryColor = System.Drawing.Color.FromArgb(255, byte.Parse(tbRed.Text), byte.Parse(tbGreen.Text), byte.Parse(tbBlue.Text));   //Zet de primary kleur met deze waardes
            cvPrimaryColor.Background = new SolidColorBrush(Color.FromArgb(255, byte.Parse(tbRed.Text), byte.Parse(tbGreen.Text), byte.Parse(tbBlue.Text)));    //Ook de achtergrondkleur van de lijn doen
        }
        private void updateHexColor()
        {
            Color color = ((SolidColorBrush)cvPrimaryColor.Background).Color;   //Het zetten van de kleur
            tbHexColor.Text = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");   //Maak de tekst van de heximale kleur
        }

        private void cvSecondaryColor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush sBrush = (SolidColorBrush)cvSecondaryColor.Background;  //Het zetten van de primary en secundaire kleur achtergrond
            SolidColorBrush pBrush = (SolidColorBrush)cvPrimaryColor.Background;
            cvSecondaryColor.Background = cvPrimaryColor.Background;

            App.currentSchetsWindow.currentSchetsControl.schets.primaryColor = System.Drawing.Color.FromArgb(sBrush.Color.A, sBrush.Color.R, sBrush.Color.G, sBrush.Color.B);   //Zetten van de primaire en secundaire kleur 
            App.currentSchetsWindow.currentSchetsControl.schets.secondaryColor = System.Drawing.Color.FromArgb(pBrush.Color.A, pBrush.Color.R, pBrush.Color.G, pBrush.Color.B);
           
            sliderRed.Value = sBrush.Color.R;       //Pas de sliders aan aan de waarde
            sliderGreen.Value = sBrush.Color.G;
            sliderBlue.Value = sBrush.Color.B;
        }

        public void refreshColors()
        {
            try // To catch nullpointer that occurs when the color picker hasn't loaded (yet)
            {
                System.Drawing.Color primary = App.currentSchetsWindow.currentSchetsControl.schets.primaryColor;
                sliderRed.Value = primary.R;        //Het veranderen van de waardes van de sliders
                sliderGreen.Value = primary.G;
                sliderBlue.Value = primary.B;
                App.currentSchetsWindow.currentSchetsControl.schets.primaryColor = primary;
                updateHexColor();   //Heximale waarde updaten aan de kleur

                System.Drawing.Color secondary = App.currentSchetsWindow.currentSchetsControl.schets.secondaryColor;
                cvSecondaryColor.Background = new SolidColorBrush(Color.FromArgb(255, secondary.R, secondary.G, secondary.B));
            }
            catch (NullReferenceException)
            {
            }
        }

        private void GetPixelColor(Point point) //TODO fix
        {

            

            try
            {
                BitmapSource source = (BitmapSource)imColorPicker.Source;
                // Make sure that the point is within the dimensions of the image.

                    // Create a cropped image at the supplied point coordinates.
                    var croppedBitmap = new CroppedBitmap(source,
                                                          new Int32Rect((int)point.X, (int)point.Y, 1, 1));

                    // Copy the sampled pixel to a byte array.
                    var pixels = new byte[4];
                    croppedBitmap.CopyPixels(pixels, 4, 0);

                    //cvPrimaryColor.Background = new SolidColorBrush(Color.FromRgb(pixels[2], pixels[1], pixels[0]));
                    sliderRed.Value = pixels[2];
                    sliderGreen.Value = pixels[1];
                    sliderBlue.Value = pixels[0];

            }
            catch (Exception)
            {
            }
        }

        private void imColorPicker_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePoint = e.GetPosition(imColorPicker);    //Bekijken mouseposition in de kleurbol

            double dist = Math.Sqrt((mousePoint.X - 100) * (mousePoint.X - 100) + (mousePoint.Y - 100) * (mousePoint.Y - 100)); //Berekenen afstand

            if(dist < 100)
            {
                GetPixelColor(mousePoint);  //Neem de kleur waarop je geklikt hebt
            }
        }     
    }
}
