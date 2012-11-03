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
        private Bitmap overlayBitmap;

        public ISchetsTool currentTool;
        public Action currentAction;

        private Boolean muisVast;
        public Schets schets;
        public int lineThickness = 3;

        public Graphics overlayBitmapGraphics
        {
            get 
            {
                if (overlayBitmap == null)
                {
                    overlayBitmap = new Bitmap(schets.imageSize.Width, schets.imageSize.Height);
                }
                return Graphics.FromImage(overlayBitmap); 
            
            }
        }

        public SchetsControl(string imageName)
        {
            schets = new Schets(imageName, new Size(700, 500));

            currentTool = App.availableTools[0];
            currentAction = new PenAction();
            
            this.DoubleBuffered = true;


            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;

            this.MouseDown += (object o, MouseEventArgs mea) =>
            {
                muisVast = true;

                Point translatedPoint = translateMouseCoordinates(mea.Location);

                overlayBitmap = new Bitmap(schets.imageSize.Width, schets.imageSize.Height);
                setAction();
                currentAction.onMouseDown(this, translatedPoint.X, translatedPoint.Y, 3, schets.primaryColor);
                currentAction.actionColor = schets.primaryColor;
                currentTool.MuisVast(this, translatedPoint);
            };
            this.MouseMove += (object o, MouseEventArgs mea) =>
            {
                if (muisVast)
                {
                    Point translatedPoint = translateMouseCoordinates(mea.Location);

                    currentAction.onMouseMove(translatedPoint.X, translatedPoint.Y);
                    currentTool.MuisDrag(this, translatedPoint);
                }
            };
            this.MouseUp += (object o, MouseEventArgs mea) =>
            {
                muisVast = false;

                Point translatedPoint = translateMouseCoordinates(mea.Location);

                currentTool.MuisLos(this, translatedPoint);
                if (currentAction != null)
                {
                    currentAction.onMouseUp(translatedPoint.X, translatedPoint.Y);
                }
                App.historyWindow.updateHistoryList();
            };
            this.KeyPress += (object o, KeyPressEventArgs kpea) =>
            {
                currentTool.Letter(this, kpea.KeyChar);
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

        public Graphics MaakBitmapGraphics()
        {   
            Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }
        public Graphics MaakOverlayBitmapGraphics()
        {
            Graphics g = this.overlayBitmapGraphics;
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
        
        public Point translateMouseCoordinates(Point mouseLocation)
        {
            double widthRatio = (double) schets.imageSize.Width / this.Width;
            double heightRatio = (double)schets.imageSize.Height / this.Height;

            int newMouseX = (int) (mouseLocation.X * widthRatio);
            int newMouseY = (int) (mouseLocation.Y * heightRatio);

            return new Point(newMouseX, newMouseY);
        }

        private void setAction()
        {
            if (currentTool is GumTool)
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
        }
    }
}
