using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml;
using UniPortal.Admins;
using UniPortal.Student;
using UniPortal.Student.UserControlsMenu;
using UniPortal.Teachers;

namespace UniPortal
{

    public partial class Login : Window
    {
        public bool DevModeActive = false;
        private bool isAnimationRunning = false;
        public bool MenuClicked = false;
        public bool border1Anim = true;
        public bool border2Anim = false;
        public bool border3Anim = true;
        public bool changeMoving = true;
        public bool menuClickefNotifes = true;
        private bool Notifes = false;
        private bool NotifesfGeneral = false;
        private bool NotifesStudent = false;
        private bool NotifesTeacher = false;
        private bool NotifesUsers = false;
        private bool NotifesNewUsers = false;
        // private bool isRowExpanded = false;



        public Login()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
         
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            ButtonAnimation(border1, ref border1Anim, 5);
            ButtonAnimation(border2, ref border2Anim, 5);
            ButtonAnimation(border3, ref border3Anim, 5);



        }

        private void HambergerMenu_Click(object sender, RoutedEventArgs e)
        {
            if (menuClickefNotifes)
            {
                Topic.Visibility = Visibility.Visible;
                documnetsGridCol2.Height = new GridLength(60);
                documnetsGridCol3.Height = new GridLength(0);
                subNotifesGrid.Visibility = Visibility.Visible;
                menuClickefNotifes = false;

            }
            else
            {
                try
                {
                    documnetsGridCol2.Height = new GridLength(0);
                    documnetsGridCol3.Height = new GridLength(0);
                    subNotifesGrid.Visibility = Visibility.Collapsed;
                    menuClickefNotifes = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            AnimateGridColumns();


        }
        private void ButtonAnimation(Border border, ref bool anim, int to)
        {
            to = anim ? 5 * to : 2 * to;

            DoubleAnimation SizeAnimation = new DoubleAnimation();
            SizeAnimation.From = border.Width;
            SizeAnimation.To = to;
            SizeAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(SizeAnimation);

            Storyboard.SetTargetProperty(SizeAnimation, new PropertyPath("(FrameworkElement.Width)"));
            Storyboard.SetTarget(SizeAnimation, border);

            storyboard.Begin();
            anim = !anim;
        }
        private void AnimateGridColumns()
        {

            double currentCol1Width = MainGrid.ColumnDefinitions[0].Width.Value;
            double currentCol3Width = MainGrid.ColumnDefinitions[2].Width.Value;


            double col1To = MenuClicked ? 1 : 9;
            double col3To = MenuClicked ? 9 : 1;


            GridLengthAnimation column1Animation = new GridLengthAnimation
            {
                From = new GridLength(currentCol1Width, GridUnitType.Star),
                To = new GridLength(col1To, GridUnitType.Star),
                Duration = TimeSpan.FromSeconds(1)
            };


            GridLengthAnimation column3Animation = new GridLengthAnimation
            {
                From = new GridLength(currentCol3Width, GridUnitType.Star),
                To = new GridLength(col3To, GridUnitType.Star),
                Duration = TimeSpan.FromSeconds(1)
            };


            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(column1Animation);
            storyboard.Children.Add(column3Animation);

            Storyboard.SetTarget(column1Animation, MainGrid.ColumnDefinitions[0]);
            Storyboard.SetTargetProperty(column1Animation, new PropertyPath("Width"));

            Storyboard.SetTarget(column3Animation, MainGrid.ColumnDefinitions[2]);
            Storyboard.SetTargetProperty(column3Animation, new PropertyPath("Width"));

            storyboard.Begin();


            MenuClicked = !MenuClicked;
        }

        public static string username;
        public string password;
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {

            username = Email.Text;
            password = Passwords.Password;

     
            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Login successful!");

            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }

        //private void ForgotPass_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    LoginBorder.Visibility = Visibility.Collapsed;
        //    ForgotPassword.Visibility = Visibility.Visible;
        //}
        private void ForgotPass_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LoginBorder.Visibility = Visibility.Collapsed;
            ForgotPassword.Visibility = Visibility.Visible;
            DevModePanel.Visibility = Visibility.Collapsed;
        }
        public static string UserType;
        private bool AuthenticateUser(string username, string password)
        {
            bool isAuthenticated = false;

            string connectionString = ConfigurationManager.ConnectionStrings["DataBaseString"].ConnectionString;

            using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    string query = "SELECT * FROM Users WHERE Uname = @Username AND Upass = @Password";
                    databaseConnection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, databaseConnection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Move to the first row of the result set
                            {
                                isAuthenticated = true;
                                UserType = reader.GetString(2); // Assuming UType is in the third column

                                Window formToOpen = null;

                                if (UserType == "student")
                                {
                                    formToOpen = new StudentForm();
                                }
                                else if (UserType == "teacher")
                                {
                                    formToOpen = new TeacherForm();
                                }
                                else if (UserType == "admin")
                                {
                                    formToOpen = new AdminsForm();
                                }
                             

                                if (formToOpen != null)
                                {
                                    formToOpen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                    formToOpen.ShowDialog();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return isAuthenticated;
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = NewEmail.Text;
            string password = NewPassword.Password;

            Action<string, string> signUpAction = (user, pass) =>
            {
                if (SignUp(user, pass))
                {
                    MessageBox.Show("Sign Up successful!");
                }
                else
                {
                    MessageBox.Show("Try Again");
                }
            };

            signUpAction(username, password);
        }

        private bool SignUp(string username, string password)
        {
            bool isSignUp = false;

            using (MySqlConnection databaseConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["DataBaseString"].ConnectionString))
            {
                try
                {
                    string query = "INSERT INTO Users (UID, Password) VALUES (@username, @password)";
                    databaseConnection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, databaseConnection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            isSignUp = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return isSignUp;
        }



        private void SignUpInLoginPage_Click(object sender, RoutedEventArgs e)
        {
            LoginBorder.Visibility = Visibility.Collapsed;
            SignUpBorder.Visibility = Visibility.Visible;
            DevModePanel.Visibility = Visibility.Collapsed;
        }


        private void LoginInSignUpPage_Click(object sender, RoutedEventArgs e)
        {
            LoginBorder.Visibility = Visibility.Visible;
            SignUpBorder.Visibility = Visibility.Collapsed;
            DevModePanel.Visibility = Visibility.Collapsed;
        }

        private void NumberMethod_Click(object sender, RoutedEventArgs e)
        {
            NumberPanel.Visibility = Visibility.Visible;
            EmailPanel.Visibility = Visibility.Collapsed;
            DevModePanel.Visibility = Visibility.Collapsed;
            EmailMethod.IsChecked = false;
            NumberMethod.IsChecked = true;

        }

        private void EmailMethod_Click(object sender, RoutedEventArgs e)
        {
            EmailMethod.IsChecked = true;
            NumberMethod.IsChecked = false;
            NumberPanel.Visibility = Visibility.Collapsed;
            EmailPanel.Visibility = Visibility.Visible;
            DevModePanel.Visibility = Visibility.Collapsed;
        }



        private void LoginInForgotPage_Click(object sender, RoutedEventArgs e)
        {
            LoginBorder.Visibility = Visibility.Visible;
            ForgotPassword.Visibility = Visibility.Collapsed;
            DevModePanel.Visibility = Visibility.Collapsed;
        }


        private void Topic_Click(object sender, RoutedEventArgs e)
        {

            ExpandNotif(documnetsGrid, 2, 400, 1, ref Notifes);
        }

        private void NotifGeneral_Click(object sender, RoutedEventArgs e)
        {
            ExpandNotif(subNotifesGrid, 1, 150, 0.5, ref NotifesfGeneral);
        }
        private void NotifStudent_Click(object sender, RoutedEventArgs e)
        {

            ExpandNotif(subNotifesGrid, 3, 150, 0.5, ref NotifesStudent);
        }
        private void NotifTeacher_Click(object sender, RoutedEventArgs e)
        {
            ExpandNotif(subNotifesGrid, 5, 50, 0.5, ref NotifesTeacher);
        }
        private void NotifUsers_Click(object sender, RoutedEventArgs e)
        {
            ExpandNotif(subNotifesGrid, 7, 50, 0.5, ref NotifesUsers);
        }

        private void NotifNewUsers_Click(object sender, RoutedEventArgs e)
        {
            ExpandNotif(subNotifesGrid, 9, 50, 0.5, ref NotifesNewUsers);
        }
        private void ExpandNotif(Grid grid, int i, int addHeight, double second, ref bool isRowExpanded)
        {
            if (isAnimationRunning)
            {
                return;
            }

            isAnimationRunning = true;

            double currentRowHeight = grid.RowDefinitions[i].Height.Value;
            double newHeight;
            if (isRowExpanded)
            {
                newHeight = currentRowHeight - addHeight;
            }
            else
            {
                newHeight = currentRowHeight + addHeight;
            }

            GridLengthAnimation heightAnimation = new GridLengthAnimation
            {
                From = new GridLength(currentRowHeight, GridUnitType.Pixel),
                To = new GridLength(newHeight, GridUnitType.Pixel),
                Duration = TimeSpan.FromSeconds(second)
            };

            heightAnimation.Completed += (s, e) =>
            {
                isAnimationRunning = false;
            };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(heightAnimation);
            Storyboard.SetTarget(heightAnimation, grid.RowDefinitions[i]);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("Height"));
            storyboard.Begin();

            isRowExpanded = !isRowExpanded;
        }


        #region Check Login Information


        #endregion

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            DevModeActive = true;
            LoginBorder.Visibility = Visibility.Collapsed;
            ForgotPassword.Visibility = Visibility.Collapsed;
            DevModePanel.Visibility = Visibility.Visible;
            Window formToOpen = null;

            if (UserType == "student")
            {
                formToOpen = new StudentForm();
            }
            else if (UserType == "teacher")
            {
                formToOpen = new TeacherForm();
            }
            else if (UserType == "admin")
            {
                formToOpen = new AdminsForm();
            }


            if (formToOpen != null)
            {
                formToOpen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                formToOpen.ShowDialog();
            }
        }



        private void SkipAsStudent_Click(object sender, RoutedEventArgs e)
        {

            Window formToOpen = null;
            formToOpen = new StudentForm();

            if (formToOpen != null)
            {
                formToOpen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                formToOpen.ShowDialog();
            }

        }


        private void SkipAsTeacher_Click(object sender, RoutedEventArgs e)
        {
            Window formToOpen = null;
            formToOpen = new TeacherForm();

            if (formToOpen != null)
            {
                formToOpen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                formToOpen.ShowDialog();
            }

        }

        private void SkipAsAdmin_Click(object sender, RoutedEventArgs e)
        {
            Window formToOpen = null;
            formToOpen = new AdminsForm();

            if (formToOpen != null)
            {
                formToOpen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                formToOpen.ShowDialog();
            }
        }
    }
}


