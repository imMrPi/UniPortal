using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace UniPortal.Student
{
    public partial class StudentForm : Window
    {

        public StudentForm()
        {
            InitializeComponent();
            MainContent.Content = new UserControlsMenu.DashBoard();
        }
        #region Booleans For control Menu
        private bool _SelectUint = true;
        private bool _DashBoard = true;
        private bool _Schedule = true;
        private bool _reports = true;
        private bool _requests = true;
        private bool _financialdepartment = true;
        #endregion

        #region Menu
        private void SelectUnitMenu_Click(object sender, RoutedEventArgs e)
        {
            DeterminingVisibility(ref _SelectUint, SelectUnitMenuPanel);
        }
        private void DashBoardMenu_Click(object sender, RoutedEventArgs e)
        {
            DeterminingVisibility(ref _DashBoard, DashBoardMenuPanel);
        }
        private void ScheduleMenu_Click(object sender, RoutedEventArgs e)
        {
            DeterminingVisibility(ref _Schedule, ScheduleMenuPanel);
        }

        private void reportsMenu_Click(object sender, RoutedEventArgs e)
        {
            DeterminingVisibility(ref _reports, reportsMenuPanel);
        }

        private void requestsMenu_Click(object sender, RoutedEventArgs e)
        {

            DeterminingVisibility(ref _requests, requestsMenuPanel);
        }

        private void financialdepartmentMenu_Click(object sender, RoutedEventArgs e)
        {

            DeterminingVisibility(ref _financialdepartment, financialdepartmentMenuPanel);
        }


        private void DeterminingVisibility(ref bool _VisibilityStatus, UIElement sender)
        {
            if (_VisibilityStatus)
            {
                sender.Visibility = Visibility.Visible;
                _VisibilityStatus = false;
            }
            else
            {
                sender.Visibility = Visibility.Collapsed;
                _VisibilityStatus = true;
            }
        }
        #endregion





        #region Panels Control To Visibilty
        private void DashBoardBTN_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserControlsMenu.DashBoard();
        }

        private void SelectUnitBTN_Click(object sender, RoutedEventArgs e)
        {

            MainContent.Content = new UserControlsMenu.SelectUnits();
        }

        #endregion

    }
}