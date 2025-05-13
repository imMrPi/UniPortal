using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UniPortal.Student.UserControlsMenu
{
    /// <summary>
    /// Interaction logic for CourseToolTip.xaml
    /// </summary>
    public partial class CourseToolTip : UserControl
    {
        public static bool ToolTipButtonClicked = false;
        public static Border ClickedBorderForToolTip = new Border();
        public CourseToolTip()
        {
            InitializeComponent();
        }

        private void ClassChoosed_Click(object sender, RoutedEventArgs e)
        {
            ToolTipButtonClicked = true;
            SelectUnits selectUnits = new SelectUnits();
           // selectUnits.HideToolTip(ClickedBorderForToolTip);
  
        }
    }
}
