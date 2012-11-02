using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SchetsPlus
{
    public partial class App : Application
    {
        public static ToolsWindow toolsWindow = new ToolsWindow();
        public static ColorPickerWindow colorPickerWindow = new ColorPickerWindow();
        public static HistoryWindow historyWindow = new HistoryWindow();

        public static List<SchetsWindow> schetsWindows = new List<SchetsWindow>();
        public static SchetsWindow currentSchetsWindow;

        public static ISchetsTool[] availableTools = { new PenTool()         
                                       , new LijnTool()
                                       , new RechthoekTool()
                                       , new VolRechthoekTool()
                                       , new EllipseTool()
                                       , new FillEllipseTool()
                                       , new TekstTool()
                                       , new GumTool()
                                       , new FancyEraser()
                                       };
    }
}
