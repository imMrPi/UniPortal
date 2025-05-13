using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UniPortal.Common
{
    public static class UserControlUtils
    {
        public static void HandlePassedCourse(bool passcourse, UserControl usercontrol)
        {
            if (passcourse)
            {
                List<Border> borders = FindChildren<Border>(usercontrol);
                if (borders != null && borders.Count >= 2)
                {
                    Border unselectedBorder = borders[1];
                    Border selectedBorder = borders[2];

                    unselectedBorder.Visibility = Visibility.Hidden;
                    selectedBorder.Visibility = Visibility.Visible;
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
    }
}
