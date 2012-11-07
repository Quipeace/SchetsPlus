using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchetsPlus
{
    public class SchetsControl : System.Windows.Forms.UserControl
    {
        private Bitmap overlayBitmap;   //Declareren overlayBitmap die later gebruikt wordt

        public ISchetsTool currentTool; //Declareren van de currenttool die gebruikt wordt
        public Action currentAction;    //Declareren van de currentAction die gebruikt wordt

        private Boolean muisVast;   //Declareren boolean muisVast die checkt of muis ingedrukt is
        public Schets schets;       //Declareren van schets
        public int lineThickness = 3;   //Declareren en zetten van de lijndikte op 3

        public SchetsControl(string imageName)
        {
            schets = new Schets(imageName, new Size(600, 500)); //Aanmaken van nieuw schetsobject

            currentTool = App.availableTools[0];    //Zeggen dat currentTool een pen is (standaard bij openen)
            currentAction = new PenAction();    //Ook gelijk de penAction hierbij aanroepen
            
            this.DoubleBuffered = true; //Voorkomen van flikkeren van scherm tijdens tekenen

            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;

            this.MouseDown += (object o, MouseEventArgs mea) =>
            {
                muisVast = true;    //Zeggen dat muisVast true is, zodra je muis indrukt

                overlayBitmap = new Bitmap(schets.imageSize.Width, schets.imageSize.Height);    //Zetten van overlayBitmap met de width en height
                setAction();    

                Point translatedPoint = translateMouseCoordinates(mea.Location);    //Pak de muis-coördinaten

                currentTool.MuisVast(this, translatedPoint);    //Gebruiken muiscoördinaten bij aanroepen methode muisVast
                currentAction.onMouseDown(this, translatedPoint.X, translatedPoint.Y, 3, schets.primaryColor);
            };
            this.MouseMove += (object o, MouseEventArgs mea) =>
            {
                if (muisVast)   //Doe dit alleen indien muis is ingedrukt
                {
                    Point translatedPoint = translateMouseCoordinates(mea.Location);    //Houdt telkens bij welke punten je over gaat

                    currentTool.MuisDrag(this, translatedPoint);    //Aanroepen methode muisdrag met deze muiscoördinaten
                    currentAction.onMouseMove(translatedPoint.X, translatedPoint.Y);
                }
            };
            this.MouseUp += (object o, MouseEventArgs mea) =>
            {
                muisVast = false;   //Boolean muisVast op false zetten aangezien muis nu los is

                Point translatedPoint = translateMouseCoordinates(mea.Location);    //Bekijken op welke coördinaten dit gebeurde

                currentTool.MuisLos(this, translatedPoint);     //Aanroepen methode muislos met deze coördinaten
                if (currentAction != null)  //Indien er een action was gebruikt voeg dit dan toe aan de action list en update de historyBox
                {
                    currentAction.onMouseUp(translatedPoint.X, translatedPoint.Y);
                    App.currentSchetsWindow.currentSchetsControl.schets.actions.Add(App.currentSchetsWindow.currentSchetsControl.currentAction);
                    App.historyWindow.updateHistoryList();
                }
            };
            this.KeyPress += (object o, KeyPressEventArgs kpea) =>
            {
                if (kpea.KeyChar > 32)
                {
                    currentTool.Letter(this, kpea.KeyChar);
                    if (currentAction is TextAction)
                    {
                        ((TextAction)currentAction).enteredText += kpea.KeyChar;    //Voeg de letter toe aan de enteredTExt indien het textAction was
                    }
                }
            };
        }

        private void teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics, this.Width, this.Height);                        // Plaatje laten tekenen
            if (muisVast)
            {
                pea.Graphics.DrawImage(overlayBitmap, 0, 0, this.Width, this.Height);   // Tekenen bitmap overlay (nieuwe actie terwijl muis muisVast)
            }
        }

        private void veranderAfmeting(object o, EventArgs ea)
        {
            this.Invalidate();
        }

        public Graphics MaakBitmapGraphics()    //Maak nieuwe bitmapgraphics aan
        {   
            Graphics g = schets.BitmapGraphics;     
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }
        public Graphics MaakOverlayBitmapGraphics() //Maak nieuwe overlaybitmap graphics aan
        {
            if (overlayBitmap == null)
            {
                overlayBitmap = new Bitmap(schets.imageSize.Width, schets.imageSize.Height);
            }
            Graphics g = Graphics.FromImage(overlayBitmap); 
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }

        public void Schoon()
        {   
            schets.Schoon();
            this.Invalidate();
        }
        public void Roteer(object o, EventArgs ea)
        {   
            schets.Roteer();
            this.veranderAfmeting(o, ea);
        }
        
        public Point translateMouseCoordinates(Point mouseLocation) //Indien het scherm verkleind is, verander dan de muiscoördinaten
        {
            double widthRatio = (double) schets.imageSize.Width / this.Width;
            double heightRatio = (double)schets.imageSize.Height / this.Height;

            int newMouseX = (int) (mouseLocation.X * widthRatio);
            int newMouseY = (int) (mouseLocation.Y * heightRatio);

            return new Point(newMouseX, newMouseY);
        }

        private void setAction()        //Het zetten van de acties bij de verschillende tools
        {
            if (currentTool is TekstTool)
            {
                currentAction = new TextAction();
            }
            else if (currentTool is GumTool)
            {
                currentAction = new EraserAction();
            }
            else if (currentTool is PenTool)
            {
                currentAction = new PenAction();
            }
            else if (currentTool is LijnTool)
            {
                currentAction = new LineAction();
            }
            else if (currentTool is VolRechthoekTool)
            {
                currentAction = new FillRectangleAction();
            }
            else if (currentTool is RechthoekTool)
            {
                currentAction = new RectangleAction();
            }
            else if (currentTool is FillEllipseTool)
            {
                currentAction = new FillEllipseAction();
            }
            else if (currentTool is EllipseTool)
            {
                currentAction = new EllipseAction();
            }
            else if (currentTool is FancyEraser)
            {
                currentAction = new FancyEraserAction();
            }

            currentAction.actionColor = schets.primaryColor;        //Zeggen welke kleur er gebruikt moet worden
        }
    }
}
