using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace UniPortal.Student.UserControlsMenu
{

    public partial class DashBoard : UserControl
    {
        public static string StoID = Login.username;
        public DashBoard()
        {
            InitializeComponent();
            DataContext = this;
            StudentIDLable.Text = StoID;
            FillDashbordInformation();

        }

        private void FillDashbordInformation()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DataBaseString"].ConnectionString;

            using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
            {

                try
                {
                    string query;
                    if (Login.UserType == "student")
                    {
                        query = "SELECT * FROM student WHERE Stno = @Username";
                    }
                    else if (Login.UserType == "teacher")
                    {
                        query = "SELECT * FROM teacher WHERE Tno = @Username";
                    }
                    else if (Login.UserType == "admin")
                    {
                        query = "SELECT * FROM admin WHERE Uname = @Username";
                    }
                    else
                    {
                        query = "";
                    }
                    databaseConnection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, databaseConnection))
                    {
                        command.Parameters.AddWithValue("@Username", StoID);


                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Move to the first row of the result set
                            {


                                string name =  reader.GetString(1); 
                                string family =  reader.GetString(2);
                                string fullname = name + family;
                                UserName.Text = fullname;
                                NightOrDay.Text = reader.GetString(4);
                                CreditStart.Text = reader.GetString(5);
                                CreditEnd.Text = reader.GetString(6);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


        }
    }


}
