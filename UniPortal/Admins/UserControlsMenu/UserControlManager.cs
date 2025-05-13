
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace UniPortal.Admins.UserControlsMenu
{
    public class UserControlManager
    {
        public ObservableCollection<UserControl> UserControls { get; set; }

        public UserControlManager()
        {
            UserControls = new ObservableCollection<UserControl>();
        }

        public void AddUserControl(UserControl userControl)
        {
            UserControls.Add(userControl);
        }

        public void RemoveUserControl(UserControl userControl)
        {
            UserControls.Remove(userControl);
        }
    }

}
