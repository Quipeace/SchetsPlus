using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace SchetsPlus
{
    [Serializable]
    public abstract class Action
    {
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
            if (App.currentSchetsWindow.currentSchetsControl.currentAction != null)
            {
                App.currentSchetsWindow.currentSchetsControl.currentAction.endPoint[0] = x;
                App.currentSchetsWindow.currentSchetsControl.currentAction.endPoint[1] = y;
                App.currentSchetsWindow.currentSchetsControl.schets.actions.Add(App.currentSchetsWindow.currentSchetsControl.currentAction);
            }
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
        public override void onMouseMove(int x, int y)
        {
        }

        public override void draw(SchetsControl s)
        {
            App.availableTools[8].MuisLos(s, new Point(endPoint[0], endPoint[1]));
        }

        public override bool isInClick(int x, int y)
        {
            return false; // Hier niets doen, je kan een fancyeraser niet op deze manier ongedaan maken
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
            return false;
        }
    }
}
