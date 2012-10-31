using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SchetsPlus.Action
{
    public class Action
    {
        public ISchetsTool selectedTool;
        public Action currentAction;
        public List<Action> actionList = new List<Action>();

        int[] startPoint = new int[2];
        int[] endPoint = new int[2];

        public Action()
        {
        }

        public Action(int x, int y)
        {
            startPoint[0] = x;
            startPoint[1] = y;
        }

        private void onClick()
        {
            if (selectedTool is PenTool) // pen
            {
                currentAction = new PenAction();
            }
            if (selectedTool is LijnTool) // lijn
            {
                currentAction = new LineAction();
            }
            if (selectedTool is RechthoekTool) // vierkant
            {
                currentAction = new RectangleAction();
            }
            if (selectedTool is VolRechthoekTool) // vierkant vol
            {
                currentAction = new RectangleFilledAction();
            }
        }

        private void onMouseMove(int x , int y)
        {
            if (currentAction is PenAction) // pen
            {
                (currentAction as PenAction).points.Add(new int[]{x, y});
            }
            else 
            {
                (currentAction).endPoint[0] = x;
                (currentAction).endPoint[1] = y;
            }
           
        }

        private void onMouseUp()
        {
            if (currentAction != null)
            {
                actionList.Add(currentAction);
            }
            currentAction = null;
        }

        public virtual void isInClick(int x, int y)
        { }
            
    }

    public class LineAction : Action
    {
        public override void isInClick(int x, int y)
        {
        }
    }

    public class PenAction : Action
    {
        public List<int[]> points = new List<int[]>();
        public override void isInClick(int x, int y)
        {
        }
    }

    public class RectangleAction : Action
    {
        public override void isInClick(int x, int y)
        {
        }
    }

    class RectangleFilledAction : Action
    {
        public override void isInClick(int x, int y)
        {
        }
    }

    public class CircleAction : Action
    {
        public override void isInClick(int x, int y)
        {
        }
    }

    public class CircleFilledAction : Action
    {
        public override void isInClick(int x, int y)
        {
        }
    }
}
