using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsPlus
{
    [Serializable]
    public class Schets
    {
        public Bitmap bitmap;
        public Bitmap loadedBitmap;

        public Size imageSize;
        public double imageRatio;
        public string imageName;
        public string imagePath;

        public int rotation;

        public int actionDrawLimit;
        public int actionEraseLimit = -1;

        public Color primaryColor;
        public Color secondaryColor;

        public ObservableCollection<Action> actions = new ObservableCollection<Action>();   // ObservableCollection to automatically update historyWindow

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

        public Schets(string name, Size imageSize)
        {
            this.rotation = 0;
            this.imageName = name;
            this.imageSize = imageSize;
            this.imageRatio = (double) imageSize.Width / (double) imageSize.Height;
            this.bitmap = new Bitmap(imageSize.Width, imageSize.Height);

            primaryColor = Color.Black;
            secondaryColor = Color.White;
        }

        public void VeranderAfmeting(Size sz)
        {
            if (bitmap is Bitmap)
            {

            }
        }
        public void Teken(Graphics gr, int width, int height)
        {
            gr.Clear(Color.White);
            gr.DrawImage(bitmap, 0, 0, width, height);
        }

        public void TekenFromActions(SchetsControl s)
        {
            Color tempPrimary = primaryColor;
            App.currentSchetsWindow.currentSchetsControl.Schoon();

            switch (rotation)
            {
                case 90:
                    Roteer(false);
                    break;
                case 180:
                    Roteer(false); Roteer(false);
                    break;
                case 270:
                    Roteer(true);
                    break;
            }

            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].drawAction = true;
            }

            for (int i = 0; i <= actionDrawLimit && i < actions.Count; i++)
            {
                if (actions[i] is FancyEraserAction)
                {
                    s.currentAction = actions[i];

                    s.schets.actionEraseLimit = i;
                    ((FancyEraserAction)actions[i]).draw(s);
                    s.schets.actionEraseLimit = -1;
                }
            }
            for (int i = actionDrawLimit + 1; i < actions.Count; i++)
            {
                if (actions[i] is FancyEraserAction)
                {
                    ((FancyEraserAction)actions[i]).erasedAction.drawAction = true;
                }
            }
            for (int i = 0; i <= actionDrawLimit && i < actions.Count; i++)
            {
                s.currentAction = actions[i];
                if (actions[i].drawAction && !(actions[i] is FancyEraserAction))
                {
                    actions[i].draw(s);
                }
            }

            switch (rotation)
            {
                case 90:
                    Roteer(true);
                    break;
                case 180:
                    Roteer(false); Roteer(false);
                    break;
                case 270:
                    Roteer(true);
                    break;
            }
        }

        public void Schoon()
        {
            Debug.WriteLine("SCHOON");
            bitmap = loadedBitmap;
        }
        public void Roteer(bool cw)
        {
            /*if (cw)
            {
                bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                rotation += 90;
                if(rotation > 270) 
                    rotation = 0;
            }
            else
            {
                bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                rotation -= 90;
                if (rotation < 0)
                    rotation = 270;
            }

            imageSize = bitmap.Size;*/
        }
    }
}
