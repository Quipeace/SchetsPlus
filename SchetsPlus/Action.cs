using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace SchetsPlus
{
    [Serializable]
    public abstract class Action
    {
        public bool drawAction;         //Het declareren van de variabele die bepaalt of er gegumt mag worden
        public int lineThickness = 3;   //Het declareren en zetten van de lijndikte
        public Color actionColor;       //Het declareren van de variabele die de kleur gaat aangeven

        public int[] startPoint = new int[2];   //Het declareren van de waarde die als punt wordt gebruikt bij indrukken muis 
        public int[] endPoint = new int[2];     //Het declareren van de waarde die als punt wordt gebruikt bij loslaten muis 

        public void onMouseDown(SchetsControl s, int x, int y, int lineT, Color color) //Functie die bij indrukken muis variabelen zet
        {
            startPoint[0] = x;
            startPoint[1] = y;

            lineThickness = lineT;
            actionColor = color;
        }

        public abstract void onMouseMove(int x, int y); //Abstracte functie die later gebruikt wordt als muis beweegt

        public void onMouseUp(int x, int y) //Functie die bij loslaten muis de coördinaten zet
        {
            App.currentSchetsWindow.currentSchetsControl.currentAction.endPoint[0] = x;
            App.currentSchetsWindow.currentSchetsControl.currentAction.endPoint[1] = y;
        }

        public abstract bool isInClick(int x, int y); //Abstracte functie die later wordt gebruikt om te bepalen of er op het object is geklikt

        public virtual void draw(SchetsControl s) //Functie die de kleur waarmee getekend wordt zet
        {
            s.schets.primaryColor = actionColor;
        }
    }

    [Serializable]
    public class PenAction : Action
    {
        public List<int[]> points = new List<int[]>();  //Aanmaken van de list waarin de punten staan waarover de lijn is gegaan

        public override void onMouseMove(int x, int y)
        {
            points.Add(new int[] { x, y });             //Punten toevoegen aan de list
        }
        public override string ToString()
        {
            return "Pen";                              //Zeggen dat het een pen is
        }
        public override void draw(SchetsControl s)     //Functie waarmee wordt bepaald waar er getekend moet worden
        {
            base.draw(s);
            App.availableTools[0].MuisVast(s, new Point(startPoint[0], startPoint[1]));     
            for(int i = 1; i < points.Count; i++)
            {
                App.availableTools[0].MuisDrag(s, new Point(points[i][0], points[i][1]));
            }
            App.availableTools[0].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)    //Functie waarmee wordt bepaald of je geklikt hebt op de pen of niet, indien je de fancyEraser gebruikt
        {
            for (int i = 0; i < points.Count; i++)      //Loop door alle points om te kijken of je erop of dichtbij geklikt hebt
            {
                if (((points[i][0] - 2 == x) || (points[i][0] - 1 == x) || (points[i][0] == x) || (points[i][0] + 1 == x) || (points[i][0] + 2 == x)) && ((points[i][1] - 2 == y) || (points[i][1] - 1 == y) || (points[i][1] == y) || (points[i][1] + 1 == y) || (points[i][1] + 2 == y)))
                {
                    return true; //Is dit is het geval, return true
                }
            }
            return false;       //Anders return false en gum niet
        }
    }

    [Serializable]
    public class EraserAction : PenAction
    {
        public override string ToString()
        {
            return "Eraser";        //Zeggen dat dit de normale gum is
        }
        public override void draw(SchetsControl s)  //Functie waarmee wordt bepaald waar er getekend moet worden
        {
            s.schets.primaryColor = Color.White;
            App.availableTools[0].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            for (int i = 1; i < points.Count; i++)
            {
                App.availableTools[0].MuisDrag(s, new Point(points[i][0], points[i][1]));
            }
            App.availableTools[0].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)
        {
            return false;   //De gum kan niet verwijderd worden met de fancygum, dus altijd return false
        }
    }

    [Serializable]
    public class FancyEraserAction : Action
    {
        public Action erasedAction;

        public override string ToString()
        {
            return "Object Eraser";     //Zeggen dat dit de fancyEraser is
        }
        public override void onMouseMove(int x, int y)  //Deze kent geen mousemove alleen mouseklik
        {
        }

        public override void draw(SchetsControl s)   //Functie waarmee wordt bepaald waar er gegumt moet worden
        {
            App.availableTools[8].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[8].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }

        public override bool isInClick(int x, int y)
        {
            return false;   //Je kan de fancyEraser niet met zichzelf verwijderen, dus return false
        }
    }

    [Serializable]
    public class TextAction : Action
    {
        public string enteredText = ""; //Het declareren en op leeg zetten van de ingevoerde text
        public override string ToString()
        {
            return "Text";  //Zeggen dat dit een textbox is
        }
        public override void onMouseMove(int x, int y)  //Een textbox kent geen onMouseMove 
        {
        }

        public override void draw(SchetsControl s) //Functie waarmee wordt bepaald waar er getekend moet worden
        {
            App.availableTools[6].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            for (int i = 0; i < enteredText.Length; i++ )
            {
                App.availableTools[6].Letter(s, enteredText[i]);
            }
        }

         public override bool isInClick(int x, int y) //Functie waarmee bepaald wordt of er in het vierkant rond de text geklikt is
        {
            if (x <= Math.Max(endPoint[0], startPoint[0]) && x >= Math.Min(endPoint[0], startPoint[0]) && y <= Math.Max(startPoint[0], endPoint[1]) && y >= Math.Min(startPoint[1], endPoint[1]))
            {
                return true;    //Indien dit zo is, return true
            }
            return false;   //Anders return false
        }
    }

    [Serializable]
    public class LineAction : Action
    {
        public override void onMouseMove(int x, int y)  //Een lijn beweeg je niet, maar heeft alleen begin- en eindpunten
        {
        }
        public override string ToString()
        {
            return "Line";  //Zeggen dat het een lijn is
        }
        public override void draw(SchetsControl s)  //Functie waarmee wordt bepaald waar er getekend moet worden
        {
            base.draw(s);
            App.availableTools[1].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[1].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)    //Functie waarmee er bepaald wordt of er op de lijn geklikt is
        {
            double avRaise = (double)(Math.Max(startPoint[1], endPoint[1]) - Math.Min(startPoint[1], endPoint[1])) / (double)(Math.Max(startPoint[0], endPoint[0]) - Math.Min(startPoint[0], endPoint[0]));     //Eerst bereken de stijging van de lijn
            int xDiff = Math.Max(endPoint[0], startPoint[0]) - Math.Min(endPoint[0], startPoint[0]);    //Bereken het verschil in X richting
            int yDiff = Math.Max(endPoint[1], startPoint[1]) - Math.Min(endPoint[1], startPoint[1]);    //Bereken het verschil in Y richting
            double yTotal = startPoint[1];  //De variabele yTotal gebruiken om te bepalen hoever je op de y zit
            double xTotal = Math.Min(startPoint[0], endPoint[0]);   //De variabele xTotal gebruiken om te bepalen hoever je op de x zit

            if (startPoint[0] < endPoint[0] && startPoint[1] > endPoint[1]) //Er zijn 4 mogelijkheden om een lijn te tekenen, hiermee ga je ze 1 voor 1 langs
            {
                for (int i = 0; i <= xDiff; i++) //For loop om de yTotal en xTotal op te hogen en dan te kijken of je erop geklikt hebt
                {
                    yTotal = startPoint[1] - (avRaise * i);
                    xTotal = Math.Min(startPoint[0], endPoint[0]) + i;
                    if (((xTotal - 2 == x) || (xTotal - 1 == x) || (xTotal == x) || (xTotal + 1 == x) || (xTotal + 2 == x)) && (((int)(yTotal - 2) == y) || ((int)(yTotal - 1) == y) || ((int)(yTotal) == y) || ((int)(yTotal + 1) == y) || ((int)(yTotal + 2) == y)))
                    {
                        return true;
                    }
                }
            }
            if (startPoint[0] < endPoint[0] && startPoint[1] < endPoint[1]) //Er zijn 4 mogelijkheden om een lijn te tekenen, hiermee ga je ze 1 voor 1 langs
            {
                for (int i = 0; i <= xDiff; i++)    //For loop om de yTotal en xTotal op te hogen en dan te kijken of je erop geklikt hebt
                {
                    yTotal = startPoint[1] + (avRaise * i);
                    xTotal = Math.Min(startPoint[0], endPoint[0]) + i;
                    if (((xTotal - 2 == x) || (xTotal - 1 == x) || (xTotal == x) || (xTotal + 1 == x) || (xTotal + 2 == x)) && (((int)(yTotal - 2) == y) || ((int)(yTotal - 1) == y) || ((int)(yTotal) == y) || ((int)(yTotal + 1) == y) || ((int)(yTotal + 2) == y)))
                    {
                        return true;
                    }
                }
            }
            if (startPoint[0] > endPoint[0] && startPoint[1] > endPoint[1]) //Er zijn 4 mogelijkheden om een lijn te tekenen, hiermee ga je ze 1 voor 1 langs
            {
                for (int i = xDiff; i >= 0; i--)     //For loop om de yTotal en xTotal op te hogen en dan te kijken of je erop geklikt hebt
                {
                    yTotal = startPoint[1] - (avRaise * i);
                    xTotal = startPoint[0] - i;
                    if (((xTotal - 2 == x) || (xTotal - 1 == x) || (xTotal == x) || (xTotal + 1 == x) || (xTotal + 2 == x)) && (((int)(yTotal - 2) == y) || ((int)(yTotal - 1) == y) || ((int)(yTotal) == y) || ((int)(yTotal + 1) == y) || ((int)(yTotal + 2) == y)))
                    {
                        return true;
                    }
                }
            }
            if (startPoint[0] > endPoint[0] && startPoint[1] < endPoint[1]) //Er zijn 4 mogelijkheden om een lijn te tekenen, hiermee ga je ze 1 voor 1 langs
            {
                for (int i = xDiff; i >= 0; i--)    //For loop om de yTotal en xTotal op te hogen en dan te kijken of je erop geklikt hebt
                {
                    yTotal = startPoint[1] + (avRaise * i);
                    xTotal = startPoint[0] - i;
                    if (((xTotal - 2 == x) || (xTotal - 1 == x) || (xTotal == x) || (xTotal + 1 == x) || (xTotal + 2 == x)) && (((int)(yTotal - 2) == y) || ((int)(yTotal - 1) == y) || ((int)(yTotal) == y) || ((int)(yTotal + 1) == y) || ((int)(yTotal + 2) == y)))
                    {
                        return true;
                    }
                }
            }
            if (double.IsInfinity(avRaise)) //Indien de lijn verticaal is, is de stijging oneindig 
            {
                for (int i = yDiff; i >= 0; i--)    //For loop om de yTotal en xTotal op te hogen en dan te kijken of je erop geklikt hebt
                {
                    yTotal = Math.Min(startPoint[1], endPoint[1]) + i;
                    if (((xTotal - 2 == x) || (xTotal - 1 == x) || (xTotal == x) || (xTotal + 1 == x) || (xTotal + 2 == x)) && (((int)(yTotal - 2) == y) || ((int)(yTotal - 1) == y) || ((int)(yTotal) == y) || ((int)(yTotal + 1) == y) || ((int)(yTotal + 2) == y)))
                    {
                        return true;
                    }
                }
            }
            if (avRaise == 0)   //Indien de lijn horizontaal is, is de stijging 0
            {
                for (int i = 0; i < xDiff; i++) //For loop om de yTotal en xTotal op te hogen en dan te kijken of je erop geklikt hebt
                {
                    xTotal = Math.Min(startPoint[0], endPoint[0]) + i;
                    if (((xTotal - 2 == x) || (xTotal - 1 == x) || (xTotal == x) || (xTotal + 1 == x) || (xTotal + 2 == x)) && (((int)(yTotal - 2) == y) || ((int)(yTotal - 1) == y) || ((int)(yTotal) == y) || ((int)(yTotal + 1) == y) || ((int)(yTotal + 2) == y)))
                    {
                        return true;
                    }
                }
            }
            return false;   //Indien geen van alle waar is, return false
        }
    }

    [Serializable]
    public class RectangleAction : Action
    {
        public override void onMouseMove(int x, int y)  //Een rectangle kent geen mouseMove
        {
        }
        public override string ToString()
        {
            return "Rectangle"; //Zeggen dat het een rectangle is
        }
        public override void draw(SchetsControl s) //Functie waarmee wordt bepaald waar er getekend moet worden
        {
            base.draw(s);
            App.availableTools[2].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[2].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)    //Functie waarmee bepaald wordt of je op de randen van de rectangle geklikt hebt
        {
            if ((x <= endPoint[0] + 2 && x >= endPoint[0] - 2) || (x >= startPoint[0] - 2 && x <= startPoint[0] + 2))   //Er zijn 4 mogelijkheden om een rectangle te tekenen en hiermee loop je ze langs
            {
                if (y <= Math.Max(startPoint[1], endPoint[1]) && y >= Math.Min(startPoint[1], endPoint[1]))
                {
                    return true;
                }
            }
            if ((y >= startPoint[1] - 2 && y <= startPoint[1] + 2) || (y >= endPoint[1] - 2 && y <= endPoint[1] + 2))   //Er zijn 4 mogelijkheden om een rectangle te tekenen en hiermee loop je ze langs
            {
                if (x <= Math.Max(startPoint[0], endPoint[0]) && x >= Math.Min(startPoint[0], endPoint[0]))
                {
                    return true;
                }
            }
            return false;   //Is er niet op geklikt, return dan false
        }
    }

    [Serializable]
    public class FillRectangleAction : Action
    {
        public override void onMouseMove(int x, int y) //Een filled rectangle kent geen mouseMove
        {
        }
        public override string ToString()
        {
            return "Filled Rectangle";  //Zeggen dat het een filled rectangle is
        }
        public override void draw(SchetsControl s)  //Functie waarmee wordt bepaald waar er getekend moet worden
        {
            base.draw(s);
            App.availableTools[3].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[3].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)   //Functie waarmee bepaald wordt of je binnen de rectangle geklikt hebt
        {
            if (x <= Math.Max(endPoint[0], startPoint[0]) && x >= Math.Min(endPoint[0], startPoint[0]) && y <= Math.Max(startPoint[0], endPoint[1]) && y >= Math.Min(startPoint[1], endPoint[1]))
            {
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class EllipseAction : Action
    {
        public override void onMouseMove(int x, int y)  //Een ellipse kent geen mouseMove
        {
        }
        public override string ToString()
        {
            return "Ellipse";   //Zeggen dat het een ellipse is
        }
        public override void draw(SchetsControl s)  //Functie waarmee wordt bepaald waar er getekend moet worden
        {
            base.draw(s);
            App.availableTools[4].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[4].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)    //Functie waarmee bepaald wordt of je op de rand van de ellipse geklikt hebt
        {
            double xRadius = ((Math.Max(endPoint[0], startPoint[0])) - (Math.Min(endPoint[0], startPoint[0]))) / 2; //X straal
            double yRadius = ((Math.Max(endPoint[1], startPoint[1])) - (Math.Min(endPoint[1], startPoint[1]))) / 2; //Y straal
            double xMid = (endPoint[0] + startPoint[0]) / 2;    //X midden
            double yMid = (endPoint[1] + startPoint[1]) / 2;    //Y midden
            double xEllipse = (((x - xMid) * (x - xMid)) / (xRadius * xRadius));    //Gebruik de middelpuntsvergelijking
            double yEllipse = (((y - yMid) * (y - yMid)) / (yRadius * yRadius));
            double totalEllipse = xEllipse + yEllipse;
            if (totalEllipse > 0.9 && totalEllipse < 1.1)   //Bekijk of het binnen de waarden valt van de rand
            {
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class FillEllipseAction : Action
    {
        public override void onMouseMove(int x, int y)  //Een filled ellipse kent geen mouseMove
        {
        }
        public override string ToString()
        {
            return "Filled Ellipse";    //Zeggen dat het filled een ellipse is
        }
        public override void draw(SchetsControl s)  //Functie waarmee wordt bepaald waar er getekend moet worden
        {
            base.draw(s);
            App.availableTools[5].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[5].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)    //Functie waarmee je bekijkt of je binnen de ellipse geklikt hebt
        {
            double xRadius = ((Math.Max(endPoint[0], startPoint[0])) - (Math.Min(endPoint[0], startPoint[0]))) / 2; //X straal
            double yRadius = ((Math.Max(endPoint[1], startPoint[1])) - (Math.Min(endPoint[1], startPoint[1]))) / 2; //Y straal
            double xMid = (endPoint[0] + startPoint[0]) / 2;    //X midden
            double yMid = (endPoint[1] + startPoint[1]) / 2;    //Y midden
            double xEllipse = (((x - xMid) * (x - xMid)) / (xRadius * xRadius));    //Gebruik de middelpuntsvergelijking
            double yEllipse = (((y - yMid) * (y - yMid)) / (yRadius * yRadius));
            double totalEllipse = xEllipse + yEllipse;
            if (totalEllipse <= 1)  //Kijken of het kleiner dan 1 is, want dan heb je er binnen geklikt
            {
                return true;
            }
            return false;
        }
    }
}
