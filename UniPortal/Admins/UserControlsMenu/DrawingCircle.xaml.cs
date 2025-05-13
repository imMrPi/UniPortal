using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UniPortal.Student.UserControlsMenu;
namespace UniPortal.Admins.UserControlsMenu
{
    public partial class DrawingCircle : UserControl
    {
        public bool SelectedCourse  { get; set; }

        public DrawingCircle()
        {
            InitializeComponent();
            this.Loaded += DrawingCircle_Loaded;

        }


        private void Shape_RightClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null && element.ContextMenu != null)
            {
                element.ContextMenu.IsOpen = true;
            }
        }


   

        #region

        public bool FirstBoard;


        private void DrawingCircle_Loaded(object sender, RoutedEventArgs e)
        {
            
            ToggleBorderVisibilityForPassedCourse();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!PassedCourse)
            {
                ToggleBorderVisibility();            
            }
        }

        public void ToggleBorderVisibility()
        {
            Window parentWindow = Window.GetWindow(this);

            if (parentWindow != null && parentWindow.Name == "StudentWindows")
            {
                List<Border> borders = FindChildren<Border>(this);
                if (borders != null && borders.Count >= 2)
                {
                    Border unselectedBorder = borders[1];
                    Border selectedBorder = borders[2];

                    if (!SelectedCourse && SelectUnits.CourseSelectedCounter <= 7)
                    {
                        if (!SelectUnits.SelectedCourseOnCode.Contains(this.Tag.ToString()))
                        {

                            unselectedBorder.Visibility = Visibility.Visible;
                            selectedBorder.Visibility = Visibility.Hidden;
                            SelectUnits.SelectedCourseOnCode.Add(this.Tag.ToString());
                                SelectUnits.CourseSelectedCounter++;
                            SelectedCourse = true;
                        }
                      
                    }
                    else
                    {
                       
                        if (SelectUnits.SelectedCourseOnCode.Contains(this.Tag.ToString()))
                        {
                            unselectedBorder.Visibility = Visibility.Hidden;
                            selectedBorder.Visibility = Visibility.Visible;
                          
                            SelectUnits.SelectedCourseOnCode.Remove(this.Tag.ToString());
                            SelectUnits.CourseSelectedCounter--;
                            SelectedCourse = false;
                        }
                       
                    }
                }
            }
        }


        public  bool PassedCourse = false;
        public void ToggleBorderVisibilityForPassedCourse()
        {
            Window parentWindow = Window.GetWindow(this);

            if (parentWindow != null && parentWindow.Name == "StudentWindows")
            {
                List<Border> borders = FindChildren<Border>(this);
                if (borders != null && borders.Count >= 3)
                {
                    Border border0 = borders[0];
                    Border border1 = borders[1];
                    Border border2 = borders[2];
                    Border border3 = borders[3];
                    Border border4 = borders[4];
                    if (PassedCourse)
                    {
                        border0.Visibility = Visibility.Visible;
                        border1.Visibility = Visibility.Hidden;//Selected Border
                        border2.Visibility = Visibility.Visible;//GreenBorder
                        border3.Visibility = Visibility.Visible;//Text
                        border4.Visibility = Visibility.Visible;//TickImage
             

                    }
                    else
                    {
                        border0.Visibility = Visibility.Visible;
                        border1.Visibility = Visibility.Hidden;//Selected Border
                        border2.Visibility = Visibility.Visible;//GreenBorder
                        border3.Visibility = Visibility.Visible;//Text
                        border4.Visibility = Visibility.Hidden;//TickImage
                    }
                }
            }
        }
        public static List<T> FindChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return new List<T>();

            List<T> result = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T)
                {
                    result.Add((T)child);
                }
                result.AddRange(FindChildren<T>(child));
            }
            return result;
        }
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child != null && child is T tChild)
                {
                    return tChild;
                }

                var result = FindChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    



        #endregion
        #region Delete Course
        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            var parentCanvas = VisualTreeHelper.GetParent(this) as Canvas;
            if (parentCanvas != null)
            {
                parentCanvas.Children.Remove(this);
            }
        }
        #endregion


        #region UserControl Apply Tag   ----> Used For DataBase Connection

        private void CourseName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newText = CourseName.Text;

            this.Tag = newText;
        }
        #endregion

  


    
    }
}
