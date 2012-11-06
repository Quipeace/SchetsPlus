using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace SchetsPlus
{
    [Serializable]
    public abstract class Action
    {
        public bool drawAction;
        public int lineThickness = 3;
        public Color actionColor;

        public int[] startPoint = new int[2];
        public int[] endPoint = new int[2];

        public void onMouseDown(SchetsControl s, int x, int y, int lineT, Color color)
        {
            startPoint[0] = x;
            startPoint[1] = y;

            lineThickness = lineT;
            actionColor = color;
        }

        public abstract void onMouseMove(int x, int y);

        public void onMouseUp(int x, int y)
        {
            App.currentSchetsWindow.currentSchetsControl.currentAction.endPoint[0] = x;
            App.currentSchetsWindow.currentSchetsControl.currentAction.endPoint[1] = y;
        }

        public abstract bool isInClick(int x, int y);

        public virtual void draw(SchetsControl s)
        {
            s.schets.primaryColor = actionColor;
        }
    }

    [Serializable]
    public class PenAction : Action
    {
        public List<int[]> points = new List<int[]>();

        public override void onMouseMove(int x, int y)
        {
            points.Add(new int[] { x, y });
        }
        public override string ToString()
        {
            return "Pen";
        }
        public override void draw(SchetsControl s)
        {
            base.draw(s);
            App.availableTools[0].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            for(int i = 1; i < points.Count; i++)
            {
                App.availableTools[0].MuisDrag(s, new Point(points[i][0], points[i][1]));
            }
            App.availableTools[0].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (((points[i][0] - 2 == x) || (points[i][0] - 1 == x) || (points[i][0] == x) || (points[i][0] + 1 == x) || (points[i][0] + 2 == x)) && ((points[i][1] - 2 == y) || (points[i][1] - 1 == y) || (points[i][1] == y) || (points[i][1] + 1 == y) || (points[i][1] + 2 == y)))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class EraserAction : PenAction
    {
        public override string ToString()
        {
            return "Eraser";
        }
        public override void draw(SchetsControl s)
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
            return false;
        }
    }

    [Serializable]
    public class FancyEraserAction : Action
    {
        public Action erasedAction;

        public override string ToString()
        {
            return "Object Eraser";
        }
        public override void onMouseMove(int x, int y)
        {
        }

        public override void draw(SchetsControl s)
        {
            App.availableTools[8].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[8].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }

        public override bool isInClick(int x, int y)
        {
            return false;
        }
    }

    [Serializable]
    public class TextAction : Action
    {
        public string enteredText = "";
        public override string ToString()
        {
            return "Text";
        }
        public override void onMouseMove(int x, int y)
        {
        }

        public override void draw(SchetsControl s)
        {
            App.availableTools[6].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[6].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            for (int i = 0; i < enteredText.Length; i++ )
            {
                App.availableTools[6].Letter(s, enteredText[i]);
            }
        }

         public override bool isInClick(int x, int y)
        {
            if (startPoint[0] < endPoint[0] && startPoint[1] > endPoint[1])
            {
                if (x <= endPoint[0] && x >= startPoint[0] && y <= startPoint[1] && y >= endPoint[1])
                {
                    return true;
                }
            }
            if (startPoint[0] < endPoint[0] && startPoint[1] < endPoint[1])
            { 
                if (x <= endPoint[0] && x >= startPoint[0] && y <= endPoint[1] && y >= startPoint[1])
                {
                    return true;
                }
            }
            if (startPoint[0] > endPoint[0] && startPoint[1] > endPoint[1])
            {
                if (x <= startPoint[0] && x >= endPoint[0] && y <= startPoint[1] && y >= endPoint[1])
                {
                    return true;
                }
            }
            if (startPoint[0] > endPoint[0] && startPoint[1] < endPoint[1])
            {
                if (x <= startPoint[0] && x >= endPoint[0] && y <= endPoint[1] && y >= startPoint[1])
                {
                    return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class LineAction : Action
    {
        public override void onMouseMove(int x, int y)
        {
        }
        public override string ToString()
        {
            return "Line";
        }
        public override void draw(SchetsControl s)
        {
            base.draw(s);
            App.availableTools[1].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[1].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)
        {
            double avRaise = (double)(Math.Max(startPoint[1], endPoint[1]) - Math.Min(startPoint[1], endPoint[1])) / (double)(Math.Max(startPoint[0], endPoint[0]) - Math.Min(startPoint[0], endPoint[0]));
            int xDiff = Math.Max(endPoint[0], startPoint[0]) - Math.Min(endPoint[0], startPoint[0]);
            double yTotal = startPoint[1];
            double xTotal = Math.Min(startPoint[0], endPoint[0]);

            if (startPoint[0] < endPoint[0] && startPoint[1] > endPoint[1])
            {
                for (int i = 0; i <= xDiff; i++)
                {
                    yTotal = startPoint[1] - (avRaise * i);
                    xTotal = Math.Min(startPoint[0], endPoint[0]) + i;
                    if (((xTotal - 2 == x) || (xTotal - 1 == x) || (xTotal == x) || (xTotal + 1 == x) || (xTotal + 2 == x)) && (((int)(yTotal - 2) == y) || ((int)(yTotal - 1) == y) || ((int)(yTotal) == y) || ((int)(yTotal + 1) == y) || ((int)(yTotal + 2) == y)))
                    {
                        return true;
                    }
                }
            }
            if (startPoint[0] < endPoint[0] && startPoint[1] < endPoint[1])
            {
                for (int i = 0; i <= xDiff; i++)
                {
                    yTotal = startPoint[1] + (avRaise * i);
                    xTotal = Math.Min(startPoint[0], endPoint[0]) + i;
                    if (((xTotal - 2 == x) || (xTotal - 1 == x) || (xTotal == x) || (xTotal + 1 == x) || (xTotal + 2 == x)) && (((int)(yTotal - 2) == y) || ((int)(yTotal - 1) == y) || ((int)(yTotal) == y) || ((int)(yTotal + 1) == y) || ((int)(yTotal + 2) == y)))
                    {
                        return true;
                    }
                }
            }
            if (startPoint[0] > endPoint[0] && startPoint[1] > endPoint[1])
            {
                for (int i = xDiff; i >= 0; i--)
                {
                    yTotal = startPoint[1] - (avRaise * i);
                    xTotal = startPoint[0] - i;
                    if (((xTotal - 2 == x) || (xTotal - 1 == x) || (xTotal == x) || (xTotal + 1 == x) || (xTotal + 2 == x)) && (((int)(yTotal - 2) == y) || ((int)(yTotal - 1) == y) || ((int)(yTotal) == y) || ((int)(yTotal + 1) == y) || ((int)(yTotal + 2) == y)))
                    {
                        return true;
                    }
                }
            }
            if (startPoint[0] > endPoint[0] && startPoint[1] < endPoint[1])
            {
                for (int i = xDiff; i >= 0; i--)
                {
                    yTotal = startPoint[1] + (avRaise * i);
                    xTotal = startPoint[0] - i;
                    if (((xTotal - 2 == x) || (xTotal - 1 == x) || (xTotal == x) || (xTotal + 1 == x) || (xTotal + 2 == x)) && (((int)(yTotal - 2) == y) || ((int)(yTotal - 1) == y) || ((int)(yTotal) == y) || ((int)(yTotal + 1) == y) || ((int)(yTotal + 2) == y)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    [Serializable]
    public class RectangleAction : Action
    {
        public override void onMouseMove(int x, int y)
        {
        }
        public override string ToString()
        {
            return "Rectangle";
        }
        public override void draw(SchetsControl s)
        {
            base.draw(s);
            App.availableTools[2].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[2].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)
        {
            if (startPoint[0] < endPoint[0] && startPoint[1] > endPoint[1])
            {
                if (((x <= endPoint[0] + 2 && x >= endPoint[0] - 2) && (y <= startPoint[1] && y >= endPoint[1])) || ((x >= startPoint[0] - 2 && x <= startPoint[0] + 2) && (y <= startPoint[1] && y >= endPoint[1])) || ((y >= startPoint[1] - 2 && y <= startPoint[1] + 2) && (x >= startPoint[0] && x <= endPoint[0])) || ((y >= endPoint[1] - 2 && y <= endPoint[1] + 2) && (x >= startPoint[0] && x <= endPoint[0])))
                {
                    return true;
                }
            }
            if (startPoint[0] < endPoint[0] && startPoint[1] < endPoint[1])
            {
                if (((x <= endPoint[0] + 2 && x >= endPoint[0] - 2) && (y >= startPoint[1] && y <= endPoint[1])) || ((x >= startPoint[0] - 2 && x <= startPoint[0] + 2) && (y >= startPoint[1] && y <= endPoint[1])) || ((y >= startPoint[1] - 2 && y <= startPoint[1] + 2) && (x >= startPoint[0] && x <= endPoint[0])) || ((y >= endPoint[1] - 2 && y <= endPoint[1] + 2) && (x >= startPoint[0] && x <= endPoint[0])))
                {
                    return true;
                }
            }
            if (startPoint[0] > endPoint[0] && startPoint[1] > endPoint[1])
            {
                if (((x <= endPoint[0] + 2 && x >= endPoint[0] - 2) && (y <= startPoint[1] && y >= endPoint[1])) || ((x >= startPoint[0] - 2 && x <= startPoint[0] + 2) && (y <= startPoint[1] && y >= endPoint[1])) || ((y >= startPoint[1] - 2 && y <= startPoint[1] + 2) && (x <= startPoint[0] && x >= endPoint[0])) || ((y >= endPoint[1] - 2 && y <= endPoint[1] + 2) && (x <= startPoint[0] && x >= endPoint[0])))
                {
                    return true;
                }
            }
            if (startPoint[0] > endPoint[0] && startPoint[1] < endPoint[1])
            {
                if (((x <= endPoint[0] + 2 && x >= endPoint[0] - 2) && (y >= startPoint[1] && y <= endPoint[1])) || ((x >= startPoint[0] - 2 && x <= startPoint[0] + 2) && (y >= startPoint[1] && y <= endPoint[1])) || ((y >= startPoint[1] - 2 && y <= startPoint[1] + 2) && (x <= startPoint[0] && x >= endPoint[0])) || ((y >= endPoint[1] - 2 && y <= endPoint[1] + 2) && (x <= startPoint[0] && x >= endPoint[0])))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class FillRectangleAction : Action
    {
        public override void onMouseMove(int x, int y)
        {
        }
        public override string ToString()
        {
            return "Filled Rectangle";
        }
        public override void draw(SchetsControl s)
        {
            base.draw(s);
            App.availableTools[3].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[3].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)
        {
            if (startPoint[0] < endPoint[0] && startPoint[1] > endPoint[1])
            {
                if (x <= endPoint[0] && x >= startPoint[0] && y <= startPoint[1] && y >= endPoint[1])
                {
                    return true;
                }
            }
            if (startPoint[0] < endPoint[0] && startPoint[1] < endPoint[1])
            { 
                if (x <= endPoint[0] && x >= startPoint[0] && y <= endPoint[1] && y >= startPoint[1])
                {
                    return true;
                }
            }
            if (startPoint[0] > endPoint[0] && startPoint[1] > endPoint[1])
            {
                if (x <= startPoint[0] && x >= endPoint[0] && y <= startPoint[1] && y >= endPoint[1])
                {
                    return true;
                }
            }
            if (startPoint[0] > endPoint[0] && startPoint[1] < endPoint[1])
            {
                if (x <= startPoint[0] && x >= endPoint[0] && y <= endPoint[1] && y >= startPoint[1])
                {
                    return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class EllipseAction : Action
    {
        public override void onMouseMove(int x, int y)
        {
        }
        public override string ToString()
        {
            return "Ellipse";
        }
        public override void draw(SchetsControl s)
        {
            base.draw(s);
            App.availableTools[4].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[4].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)
        {
            double xRadius = ((Math.Max(endPoint[0], startPoint[0])) - (Math.Min(endPoint[0], startPoint[0]))) / 2;
            double yRadius = ((Math.Max(endPoint[1], startPoint[1])) - (Math.Min(endPoint[1], startPoint[1]))) / 2;
            double xMid = (endPoint[0] + startPoint[0]) / 2;
            double yMid = (endPoint[1] + startPoint[1]) / 2;
            double xEllipse = (((x - xMid) * (x - xMid)) / (xRadius * xRadius));
            double yEllipse = (((y - yMid) * (y - yMid)) / (yRadius * yRadius));
            double totalEllipse = xEllipse + yEllipse;
            //De 0.9 en 1.1 staan voor de lijndikte weet niet hoe ik deze moet aanpassen :S
            if (totalEllipse > 0.9 && totalEllipse < 1.1)
            {
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class FillEllipseAction : Action
    {
        public override void onMouseMove(int x, int y)
        {
        }
        public override string ToString()
        {
            return "Filled Ellipse";
        }
        public override void draw(SchetsControl s)
        {
            base.draw(s);
            App.availableTools[5].MuisVast(s, new Point(startPoint[0], startPoint[1]));
            App.availableTools[5].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }
        public override bool isInClick(int x, int y)
        {
            double xRadius = ((Math.Max(endPoint[0], startPoint[0])) - (Math.Min(endPoint[0], startPoint[0]))) / 2;
            double yRadius = ((Math.Max(endPoint[1], startPoint[1])) - (Math.Min(endPoint[1], startPoint[1]))) / 2;
            double xMid = (endPoint[0] + startPoint[0]) / 2;
            double yMid = (endPoint[1] + startPoint[1]) / 2;
            double xEllipse = (((x - xMid) * (x - xMid)) / (xRadius * xRadius));
            double yEllipse = (((y - yMid) * (y - yMid)) / (yRadius * yRadius));
            double totalEllipse = xEllipse + yEllipse;
            if (totalEllipse <= 1)
            {
                return true;
            }
            return false;
        }
    }
}
