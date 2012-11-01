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
        public double imageRatio;
        public string imageName;

        public Schets(string name, Size imageSize)
        {
            this.imageName = name;
            this.imageSize = imageSize;
            this.imageRatio = (double) imageSize.Width / (double) imageSize.Height;
            this.bitmap = new Bitmap(imageSize.Width, imageSize.Height);
        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
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
