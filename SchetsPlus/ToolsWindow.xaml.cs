using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace SchetsPlus
{
    // VOOR BUTTONS
    /*
     *                 <Image.Style>
                    <Style TargetType="{x:Type Image}">
                        <Style.Triggers>
                            <DataTrigger Binding="{Binding Value}" Value="False">
                                <Setter Property="Source" Value="Resources/colorwheel.png"/>
                            </DataTrigger>
                            <DataTrigger Binding="{Binding Value}" Value="True">
                                <Setter Property="Source" Value="Resources/image2.png"/>
                            </DataTrigger>
                        </Style.Triggers>
                    </Style>
                </Image.Style>*/


    public partial class ToolsWindow : MetroWindow
    {
        public bool isPinned;   //Declareren van de boolean die bijhoudt of er op pinned geklikt is

        public ToolsWindow()
        {
            InitializeComponent();

            this.isPinned = true;   //Zet pinned standaard op true
        }

        private void btPin_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;       //Indien er op pin geklikt is, maak pinned dan tegenovergestelde
            if (isPinned)
            {
                App.currentSchetsWindow.pinToolsWindow();   //Indien pinned is true, pin de toolswindow dan
            }
        }

        private void toolButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender; //Zeggen dat clickedbutton = het object waarop geklikt is, wat sowieso een button is

            if (clickedButton.Name == "btPen")  //Het zetten van alle buttons en ook zeggen welke tools daar dan mee te maken hebben
            {
                App.currentSchetsWindow.currentSchetsControl.currentTool = App.availableTools[0];
            }
            else if (clickedButton.Name == "btLine")
            {
                App.currentSchetsWindow.currentSchetsControl.currentTool = App.availableTools[1];
            }
            else if (clickedButton.Name == "btRect")
            {
                App.currentSchetsWindow.currentSchetsControl.currentTool = App.availableTools[2];
            }
            else if (clickedButton.Name == "btFillRect")
            {
                App.currentSchetsWindow.currentSchetsControl.currentTool = App.availableTools[3];
            }
            else if (clickedButton.Name == "btEllipse")
            {
                App.currentSchetsWindow.currentSchetsControl.currentTool = App.availableTools[4];
            }
            else if (clickedButton.Name == "btFillEllipse")
            {
                App.currentSchetsWindow.currentSchetsControl.currentTool = App.availableTools[5];
            }
            else if (clickedButton.Name == "btText")
            {
                App.currentSchetsWindow.currentSchetsControl.currentTool = App.availableTools[6];
            }
            else if (clickedButton.Name == "btEraser")
            {
                App.currentSchetsWindow.currentSchetsControl.currentTool = App.availableTools[7];
            }
            else if (clickedButton.Name == "btFancyEraser")
            {
                App.currentSchetsWindow.currentSchetsControl.currentTool = App.availableTools[8];
            }
        }
    }
}
