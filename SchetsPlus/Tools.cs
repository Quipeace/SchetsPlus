using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace SchetsPlus
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected Brush kwast;

        public virtual void MuisVast(SchetsControl s, Point p)
        {
            kwast = new SolidBrush(s.schets.primaryColor);
            startpunt = p;
        }
        public virtual void MuisLos(SchetsControl s, Point p){}
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
    }

    public class TekstTool : StartpuntTool
    {
        public override void MuisDrag(SchetsControl s, Point p)
        {
        }
        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = 
                gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
                gr.DrawString   (tekst, font, kwast, 
                                              this.startpunt, StringFormat.GenericTypographic);
                // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
                startpunt.X += (int)sz.Width;
                s.Invalidate();
            }
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        private Graphics overlayGraphics;

        public static Rectangle maakRechthoek(Point p1, Point p2)
        {   
            return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y)), new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y)));
        }
        public static Pen MaakPen(Brush b, int dikte)
        {   
            Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }

        public override void MuisVast(SchetsControl s, Point p)
        {  
            base.MuisVast(s, p);
            overlayGraphics = s.MaakOverlayBitmapGraphics();
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);

            overlayGraphics.Clear(Color.Transparent);
            this.Bezig(overlayGraphics, this.startpunt, p);
            s.Invalidate();
        }
        public override void MuisLos(SchetsControl s, Point p)
        {   
            base.MuisLos(s, p);

            overlayGraphics.Clear(Color.Transparent);
            this.Bezig(s.MaakBitmapGraphics(), startpunt, p);
            s.Invalidate();
        }
        public override void Letter(SchetsControl s, char c)
        {
        }

        public abstract void Bezig(Graphics g, Point p1, Point p2);
    }

    public class RechthoekTool : TweepuntTool
    {
        public override void Bezig(Graphics g, Point p1, Point p2)
        {   
            g.DrawRectangle(MaakPen(kwast, 3), TweepuntTool.maakRechthoek(p1, p2));
        }
    }
    public class VolRechthoekTool : RechthoekTool
    {
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillRectangle(kwast, TweepuntTool.maakRechthoek(p1, p2));
        }
    }

    public class EllipseTool : TweepuntTool
    {
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.maakRechthoek(p1, p2));
        }
    }
    public class FillEllipseTool : TweepuntTool
    {
        public override void MuisVast(SchetsControl s, Point p)
        {
            base.MuisVast(s, p);
        }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillEllipse(kwast, TweepuntTool.maakRechthoek(p1, p2));
        }
    }


    public class LijnTool : TweepuntTool
    {
        public override void Bezig(Graphics g, Point p1, Point p2)
        {   g.DrawLine(MaakPen( kwast,3), p1.X, p1.Y, p2.X, p2.Y);
        }
    }

    public class PenTool : LijnTool
    {
        public override void MuisDrag(SchetsControl s, Point p)
        {   this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
    }
    
    public class GumTool : PenTool
    {
        public override void Bezig(Graphics g, Point p1, Point p2)
        {   
            g.DrawLine(MaakPen(Brushes.White, 3), p1.X, p1.Y, p2.X, p2.Y);
        }
    }

    public class FancyEraser : StartpuntTool
    {
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            for (int i = s.schets.actions.Count - 1; i >= 0; i--)
            {
                if (s.schets.actions[i].isInClick(p.X, p.Y))
                {
                    s.schets.actions.RemoveAt(i);
                    break;
                }
            }
        }

        public override void MuisDrag(SchetsControl s, Point p) { }
        public override void Letter(SchetsControl s, char c)  { }
    }
}
