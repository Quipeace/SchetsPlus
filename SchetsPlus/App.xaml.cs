using System.Collections.Generic;
using System.Windows;

namespace SchetsPlus
{
    public partial class App : Application
    {
        public static ToolsWindow toolsWindow = new ToolsWindow();      //De windows aanroepen bij het openen van de applicatie
        public static ColorPickerWindow colorPickerWindow = new ColorPickerWindow();
        public static HistoryWindow historyWindow = new HistoryWindow();

        public static List<SchetsWindow> schetsWindows = new List<SchetsWindow>();  //Het maken van de list 
        public static SchetsWindow currentSchetsWindow; //Het declareren van de huidige schets window

        public static ISchetsTool[] availableTools = { new PenTool()        //Het maken van alle beschikbare tools  
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
