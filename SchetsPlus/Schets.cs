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
        public Bitmap bitmap;           // Bitmap waarop getekend wordt.
        public Bitmap loadedBitmap;     // Bitmap die de gebruiker eventueel heeft geladen (achtergrond)

        public Size imageSize;          // Grootte van het plaatje
        public double imageRatio;       // Breedte/hoogteverhouding
        public string imageName;        // Naam + extentie
        public string imagePath;        // Volledige pad naar bestand

        public int rotation;            // Aantal graden rotatie

        public int actionDrawLimit;         // Limiet tot waar acties getekend moeten worden (bij selectie uit historywindow)
        public int actionEraseLimit = -1;   // Limiet tot waar acties gewist moeten worden (bij selectie uit historywindow)

        public Color primaryColor;      // Primaire kleur uit colourpicker
        public Color secondaryColor;    // Secondaire kleur uit colourpicker

        public ObservableCollection<Action> actions = new ObservableCollection<Action>();   // ObservableCollection to automatically update historyWindow

        public Graphics BitmapGraphics  // Bitmapgraphics, voor gebruik bij tekenen
        {
            get
            {
                if (bitmap == null)     // Indien null, zoals na laden dmv serialization, nieuwe bitmap on-demand aanmaken
                {
                    this.bitmap = new Bitmap(imageSize.Width, imageSize.Height);
                }
                return Graphics.FromImage(bitmap);
            }
        }

        public Schets(string name, Size imageSize)
        {
            this.rotation = 0;          // Init vars
            this.imageName = name;
            this.imageSize = imageSize;
            this.imageRatio = (double) imageSize.Width / (double) imageSize.Height;
            this.bitmap = new Bitmap(imageSize.Width, imageSize.Height);

            primaryColor = Color.Black;
            secondaryColor = Color.White;
        }

        public void Teken(Graphics gr, int width, int height)
        {
            gr.Clear(Color.White);                          // Witte achtergrond
            gr.DrawImage(bitmap, 0, 0, width, height);      // Daaroverheen het getekende kunstwerk
        }

        public void TekenFromActions(SchetsControl s)       // Tekenen via acties
        {
            Color tempPrimary = primaryColor;               // Tijdelijke primaire kleur vastleggen, zodat deze naderhand weer teruggezet kan worden
            App.currentSchetsWindow.currentSchetsControl.Schoon();

            for (int i = 0; i < actions.Count; i++)                             // Starten met alles tekenen
            {
                actions[i].drawAction = true;
            }
            for (int i = 0; i <= actionDrawLimit && i < actions.Count; i++)     // Weggegumde objecten binnen bereik niet tekenen
            {
                if (actions[i] is FancyEraserAction)
                {
                    s.currentAction = actions[i];

                    s.schets.actionEraseLimit = i;
                    ((FancyEraserAction)actions[i]).draw(s);
                    s.schets.actionEraseLimit = -1;
                }
            }
            for (int i = 0; i <= actionDrawLimit && i < actions.Count; i++)     // Overgebleven objecten wèl tekenen
            {
                s.currentAction = actions[i];
                if (actions[i].drawAction && !(actions[i] is FancyEraserAction))
                {
                    actions[i].draw(s);
                }
            }
        }

        public void Schoon()
        {
            BitmapGraphics.FillRectangle(Brushes.White, 0, 0, imageSize.Width, imageSize.Height);
            if (loadedBitmap != null)
            {
                BitmapGraphics.DrawImageUnscaledAndClipped(loadedBitmap, new Rectangle(0, 0, imageSize.Width, imageSize.Height));
            }
        }
        public void Roteer(bool cw)         // Rotatie, niet geïmplementeerd.
        {
            if (cw)
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

            imageSize = bitmap.Size;
            App.currentSchetsWindow.MetroWindow_SizeChanged_1(null, null);
        }
    }
}
