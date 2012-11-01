﻿using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public partial class ColorPickerWindow : MetroWindow
    {
        public bool isPinned;

        public ColorPickerWindow()
        {
            InitializeComponent();
           
            this.isPinned = true;
        }

        private void btPin_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;
            if (isPinned)
            {
                App.currentSchetsWindow.pinColorPicker();
            }
        }

        private void sliderRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbRed.Text = ((int)sliderRed.Value).ToString();
            setColorFromBoxes();
            updateHexColor();
        }
        private void sliderGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbGreen.Text = ((int)sliderGreen.Value).ToString();
            setColorFromBoxes();
            updateHexColor();
        }
        private void sliderBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbBlue.Text = ((int)sliderBlue.Value).ToString();
            setColorFromBoxes();
            updateHexColor();
        }
        private void tbHexColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            string colorcode = tbHexColor.Text;
            try
            {
                int colorAsInt = Int32.Parse(colorcode.Replace("#", ""), NumberStyles.HexNumber);
                byte b = (byte)(colorAsInt & 255);
                byte g = (byte)((colorAsInt >> 8) & 255);
                byte r = (byte)((colorAsInt >> 16) & 255);
                if (cvPrimaryColor != null)
                {
                    cvPrimaryColor.Background = new SolidColorBrush(Color.FromArgb(255, r, g, b));
                    App.currentSchetsWindow.currentSchetsControl.penkleur = System.Drawing.Color.FromArgb(255, r, g, b);
                }
            }
            catch (FormatException)
            { }
        }

        private void setColorFromBoxes()
        {
            App.currentSchetsWindow.currentSchetsControl.penkleur = System.Drawing.Color.FromArgb(255, byte.Parse(tbRed.Text), byte.Parse(tbGreen.Text), byte.Parse(tbBlue.Text));
            cvPrimaryColor.Background = new SolidColorBrush(Color.FromArgb(255, byte.Parse(tbRed.Text), byte.Parse(tbGreen.Text), byte.Parse(tbBlue.Text)));
        }
        private void updateHexColor()
        {
            Color color = ((SolidColorBrush)cvPrimaryColor.Background).Color;
            tbHexColor.Text = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        private void cvSecondaryColor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush pBrush = (SolidColorBrush)cvSecondaryColor.Background;
            cvSecondaryColor.Background = cvPrimaryColor.Background;

            App.currentSchetsWindow.currentSchetsControl.penkleur = System.Drawing.Color.FromArgb(pBrush.Color.A, pBrush.Color.R, pBrush.Color.G, pBrush.Color.B);

            sliderRed.Value = pBrush.Color.R;
            sliderGreen.Value = pBrush.Color.G;
            sliderBlue.Value = pBrush.Color.B;
        }
    }
}
