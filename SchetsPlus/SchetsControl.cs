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
        ISchetsTool huidigeTool;
        private Boolean vast;
        public Schets schets;
        private Color penkleur;

        public Color PenKleur 
        {   get { return penkleur; } 
        }
        public SchetsControl()
        {
            this.DoubleBuffered = true;

            this.schets = new Schets(new Size(700, 500));

            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;

            ISchetsTool[] deTools = { new PenTool()         
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new TekstTool()
                                    , new GumTool()
                                    };
            String[] deKleuren = { "Black", "Red", "Green", "Blue"
                                 , "Yellow", "Magenta", "Cyan" 
                                 };
            huidigeTool = deTools[0];

            this.MouseDown += (object o, MouseEventArgs mea) =>
            {
                vast = true;
                huidigeTool.MuisVast(this, mea.Location);
            };
            this.MouseMove += (object o, MouseEventArgs mea) =>
            {
                if (vast)
                {
                    huidigeTool.MuisDrag(this, mea.Location);
                }
            };
            this.MouseUp += (object o, MouseEventArgs mea) =>
            {
                vast = false;
                huidigeTool.MuisLos(this, mea.Location);
            };
            this.KeyPress += (object o, KeyPressEventArgs kpea) =>
            {
                huidigeTool.Letter(this, kpea.KeyChar);
            };
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
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
        /*public void VeranderKleur(object obj, EventArgs ea)
        {   string kleurNaam = ((ComboBox)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }*/
        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {   
            string kleurNaam = ((ToolStripMenuItem)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }
    }
}
