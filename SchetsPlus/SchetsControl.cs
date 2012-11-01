using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Controls;
using System.Windows.Forms;

namespace SchetsPlus
{
    public class SchetsControl : System.Windows.Forms.UserControl
    {
        public ISchetsTool currentTool;

        private Boolean vast;
        public Schets schets;
        public Color penkleur;

        public Color PenKleur 
        {   
            get 
            {
                return penkleur;
            } 
        }

        public SchetsControl(string imageName)
        {
            currentTool = App.availableTools[0];
            penkleur = Color.Black;
            
            this.DoubleBuffered = true;
            this.schets = new Schets(imageName, new Size(700, 500));

            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;

            this.MouseDown += (object o, MouseEventArgs mea) =>
            {
                vast = true;
                currentTool.MuisVast(this, mea.Location);
            };
            this.MouseMove += (object o, MouseEventArgs mea) =>
            {
                if (vast)
                {
                    currentTool.MuisDrag(this, mea.Location);
                }
            };
            this.MouseUp += (object o, MouseEventArgs mea) =>
            {
                vast = false;
                currentTool.MuisLos(this, mea.Location);
            };
            this.KeyPress += (object o, KeyPressEventArgs kpea) =>
            {
                currentTool.Letter(this, kpea.KeyChar);
            };
        }

        private void teken(object o, PaintEventArgs pea)
        {   
            schets.Teken(pea.Graphics, this.Size.Width, this.Size.Height);
        }
        private void veranderAfmeting(object o, EventArgs ea)
        {
            this.Invalidate();
        }
        public Graphics MaakBitmapGraphics()
        {   
            Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }
        public Graphics MaakBitmapGraphics_Aliased()
        {
            Graphics g = schets.BitmapGraphics;
            return g;
        }
        public void Schoon(object o, EventArgs ea)
        {   
            schets.Schoon();
            this.Invalidate();
        }
        public void Roteer(object o, EventArgs ea)
        {   
            schets.Roteer();
            this.veranderAfmeting(o, ea);
        }
    }
}
