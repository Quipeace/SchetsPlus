using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;

namespace SchetsPlus
{
    [Serializable]
    public class Schets
    {
        [NonSerialized]
        public Bitmap bitmap;

        public Size imageSize;
        public double imageRatio;
        public string imageName;
        public string imagePath;

        public int actionDrawLimit;

        public Color primaryColor;
        public Color secondaryColor;

        public ObservableCollection<Action> actions = new ObservableCollection<Action>();

        public Schets(string name, Size imageSize)
        {
            this.imageName = name;
            this.imageSize = imageSize;
            this.imageRatio = (double) imageSize.Width / (double) imageSize.Height;
            this.bitmap = new Bitmap(imageSize.Width, imageSize.Height);

            primaryColor = Color.Black;
            secondaryColor = Color.White;
        }
        public Graphics BitmapGraphics
        {
            get 
            {
                if (bitmap == null)
                {
                    this.bitmap = new Bitmap(imageSize.Width, imageSize.Height);
                }
                return Graphics.FromImage(bitmap); 
            }
        }

        public void VeranderAfmeting(Size sz)
        {
            if (bitmap is Bitmap)
            {

            }
            //TODO canvas resize
        }
        public void Teken(Graphics gr, int width, int height)
        {
            gr.Clear(Color.White);
            gr.DrawImage(bitmap, 0, 0, width, height);
        }

        public void TekenFromActions(SchetsControl s)
        {
            Color tempPrimary = primaryColor;

            for (int n = actionDrawLimit + 1; n < actions.Count; n++)
            {
                if (actions[n] is FancyEraserAction)
                {
                    ((FancyEraserAction)actions[n]).erasedAction.drawAction = true;
                }
            }

            for (int i = 0; i <= actionDrawLimit && i < actions.Count; i++)
            {
                s.currentAction = actions[i];

                if (actions[i].drawAction)
                {
                    actions[i].draw(s);
                }
            }
            primaryColor = tempPrimary;
        }

        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        }
        public void Roteer()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }
    }
}
