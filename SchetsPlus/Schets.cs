using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace SchetsPlus
{
    public class Schets
    {
        private Bitmap bitmap;
        public Size imageSize;
        public double ratio;
        
        public Schets(Size imageSize)
        {
            this.imageSize = imageSize;
            this.ratio = (double) imageSize.Width / (double) imageSize.Height;
            bitmap = new Bitmap(imageSize.Width, imageSize.Height);
        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }

        public void VeranderAfmeting(Size sz)
        {
            //TODO canvas resize
        }
        public void Teken(Graphics gr, int width, int height)
        {
            gr.Clear(Color.White);
            gr.DrawImage(bitmap, 0, 0, width, height);
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
