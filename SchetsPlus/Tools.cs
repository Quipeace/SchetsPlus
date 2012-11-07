using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace SchetsPlus
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);    //Zeggen dat deze methodes gebruikt zullen gaan worden
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool   
    {
        protected Point startpunt;  //Aanmaken van het startpunt en van de kleur waarmee getekent wordt
        protected Brush kwast = Brushes.Black;

        public virtual void MuisVast(SchetsControl s, Point p)
        {
            kwast = new SolidBrush(s.schets.primaryColor);  //Zeggen welke kleur de kwast is
            startpunt = p;  //Zeggen wat het startpunt is, dus het punt waarop muis ingedrukt wordt
        }
        public virtual void MuisLos(SchetsControl s, Point p){}     //Virtuele en abstracte methodes die later gebruikt worden
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
    }

    public class TekstTool : StartpuntTool
    {
        string enteredText; //Declareren van de ingevoerde tekst

        public override void MuisVast(SchetsControl s, Point p)
        {
            base.MuisVast(s, p);    //Aanroepen van de muisvastmethode van startpunttool
            enteredText = "";   //Zetten dat ingevoerde tekst standaard niks is
        }
        public override void MuisDrag(SchetsControl s, Point p) //Bij tekst is er geen muisdrag
        {
        }

        public override void Letter(SchetsControl s, char c)
        {
            enteredText += c;   //Zeggen dat de ingevoerde tekst een char erbij krijgt

            Graphics g = s.MaakBitmapGraphics();    //Aanmaken graphics voor het tekenen

            Font font = new Font("Tahoma", 40);     //Zeggen welk lettertyp en grootte
            string tekst = c.ToString();            
            SizeF sz = g.MeasureString(enteredText, font, this.startpunt, StringFormat.GenericTypographic); //De grootte berekenen
            g.DrawString(enteredText, font, kwast, this.startpunt, StringFormat.GenericTypographic);    //De string tekenen op het scherm

            s.currentAction.endPoint[0] = (int) (s.currentAction.startPoint[0] + sz.Width);     //De endpoint.X variabel maken aan de hoeveelheid ingevulde tekst
            s.currentAction.endPoint[1] = (int)(s.currentAction.startPoint[1] + sz.Height);     //De endpoint.Y variabel maken aan de hoeveelheid ingevulde tekst
            s.Invalidate(); //Teken het scherm opnieuw 
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle maakRechthoek(Point p1, Point p2)   //Methode die ervoor zorgt dat de rectangle gemaakt wordt
        {   
            return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y)), new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y)));
        }
        public static Pen MaakPen(Brush b, int dikte)
        {
            Pen pen = new Pen(b, dikte);    //Aanmaken van de pen

            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }

        public override void MuisVast(SchetsControl s, Point p)
        {  
            base.MuisVast(s, p);    //Gebruik de methode muisvast van startpunttool
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);     //Gebruik de methode muislos van startpunttool

            s.MaakOverlayBitmapGraphics().Clear(Color.Transparent);     
            this.Bezig(s.MaakOverlayBitmapGraphics(), this.startpunt, p);   //Roep de methode bezig aan
            s.Invalidate();     //Teken het scherm opnieuw
        }
        public override void MuisLos(SchetsControl s, Point p)
        {   
            base.MuisLos(s, p); //Gebruik de methode muislos van startpunttool

            s.MaakOverlayBitmapGraphics().Clear(Color.Transparent); 
            this.Bezig(s.MaakBitmapGraphics(), startpunt, p);   //Roep de methode bezig aan
            s.Invalidate(); //Teken het scherm opnieuw
        }
        public override void Letter(SchetsControl s, char c)    //Moet overriden omdat letter abstract is
        {
        }

        public abstract void Bezig(Graphics g, Point p1, Point p2);     //Aanmaken abstracte methode bezig
    }

    public class RechthoekTool : TweepuntTool
    {
        public override void Bezig(Graphics g, Point p1, Point p2)  
        {   
            g.DrawRectangle(MaakPen(kwast, 3), TweepuntTool.maakRechthoek(p1, p2)); //Overriden methode bezig en teken een rectangle met behulp van tweepunttool.maakrechthoek
        }
    }
    public class VolRechthoekTool : RechthoekTool
    {
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillRectangle(kwast, TweepuntTool.maakRechthoek(p1, p2)); //Overriden methode bezig en teken een gevulde rectangle met behulp van tweepunttool.maaktrechthoek
        }
    }

    public class EllipseTool : TweepuntTool
    {
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.maakRechthoek(p1, p2));   //Overriden methode bezig en teken een ellipse met behulp van tweepunttool.maakrechthoek
        }
    }
    public class FillEllipseTool : TweepuntTool
    {
        public override void MuisVast(SchetsControl s, Point p)
        {
            base.MuisVast(s, p);    //Gebruik de methode muisvast uit tweepunttool
        }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillEllipse(kwast, TweepuntTool.maakRechthoek(p1, p2));   //Overriden methode bezig en teken een gevulde ellipse met behulp van tweepunttool.maakrechthoek
        }
    }


    public class LijnTool : TweepuntTool
    {
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawLine(MaakPen( kwast,3), p1.X, p1.Y, p2.X, p2.Y);  //Overriden methode bezig en teken een lijn tussen de begin- en eindpunten
        }
    }

    public class PenTool : LijnTool
    {
        public override void MuisDrag(SchetsControl s, Point p) //Teken kleine lijntjes tussen de losse punten van de pen
        {
            this.MuisLos(s, p); //Gebruik de methode muislos van lijntool -> dus van tweepunttool
            this.MuisVast(s, p);    //Gebruik de methode muisvast van lijntool -> dus van tweepunttool
        }
    }
    
    public class GumTool : PenTool
    {
        public override void Bezig(Graphics g, Point p1, Point p2)
        {   
            g.DrawLine(MaakPen(Brushes.White, 7), p1.X, p1.Y, p2.X, p2.Y); //Overriden methode bezig en teken een witte lijn tussen de begin- en eindpunten van de lijn
        }
    }

    public class FancyEraser : StartpuntTool
    {
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p); //Gebruik de methode muislos van startpunttool
            Boolean hasErased = false;  //Zet de bool hasErased op false zodat er nog niet gegumt is
            int i;  //Declareer int i voor een latere for-loop
            if(s.schets.actionEraseLimit == -1)
            {
                i = s.schets.actions.Count - 1; //Zetten van int i
            }
            else
            {
                i = s.schets.actionEraseLimit;  //Zetten van int i
            }
            for (; i >= 0; i--)
            {
                if (!(s.schets.actions[i] is FancyEraserAction) && s.schets.actions[i].isInClick(p.X, p.Y)) //Zeggen wat er moet gebeuren als je ergens op klikt en dit geen fancyEraser object is
                {
                    if (!s.schets.actions[i].drawAction)    //Kijken of het een tekenactie is of niet, indien niet dan continue
                    {
                        continue;
                    }
                    s.schets.actions[i].drawAction = false; //Zet drawaction op false
                    ((FancyEraserAction)s.currentAction).erasedAction = s.schets.actions[i];    //Gum deze action weg
                    hasErased = true;   //Zet hasErased op true, dus er is gegumt
                    break;  //Break uit de loop
                }
            }
            if (!hasErased) //Als hasErased nog valt is, dan zet currentAction = null
            {
                s.currentAction = null;
            }

        }

        public override void MuisDrag(SchetsControl s, Point p) { } //Overriden methodes muisdrag en letter aangezien ze abstract zijn, maar zet niks in body
        public override void Letter(SchetsControl s, char c)  { }
    }
}
