using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System;

namespace UniPortal.Teachers.UserControlsMenu
{
    public partial class SearchStudent : UserControl
    {
        private MySqlConnection connection;
        private MySqlDataAdapter dataAdapter;
        private DataSet dataSet;

        public SearchStudent()
        {
            InitializeComponent();
            DataShow();
        }

        public void DataShow()
        {
            string query = "SELECT * FROM student.grades WHERE " +
                           "StudentID LIKE @StudentID AND " +
                           "Name LIKE @Name AND " +
                           "Family LIKE @Family AND " +
                           "Course LIKE @Course";
            DataShowAll(query, new MySqlParameter("@StudentID", txtStudentNo.Text + "%"),
                              new MySqlParameter("@Name", txtFirstName.Text + "%"),
                              new MySqlParameter("@Family", txtLastName.Text + "%"),
                              new MySqlParameter("@Course", txtField.Text + "%"));
        }

        public void DataShowAll(string query, params MySqlParameter[] parameters)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DataBaseString"].ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddRange(parameters);
                    con.Open();
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    dataGridStudents.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + (ex.InnerException != null ? "\nInner Exception: " + ex.InnerException.Message : ""));
            }
        }

        private void StudentIDOnlySearchBtn()
        {
            string query = "SELECT * FROM student WHERE StudentID LIKE @StudentID";
            DataShowAll(query, new MySqlParameter("@StudentID", txtSearchNo.Text + "%"));
        }

        private void txtStudentNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataShow();
        }

        private void txtField_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataShow();
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataShow();
        }

        private void txtFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataShow();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            StudentIDOnlySearchBtn();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string query = "UPDATE student SET " +
                           "Name = @Name, " +
                           "Family = @Family, " +
                           "Course = @Course " +
                           "WHERE StudentID = @StudentID";
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DataBaseString"].ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("@StudentID", txtStudentNo.Text));
                    cmd.Parameters.Add(new MySqlParameter("@Name", txtFirstName.Text));
                    cmd.Parameters.Add(new MySqlParameter("@Family", txtLastName.Text));
                    cmd.Parameters.Add(new MySqlParameter("@Course", txtField.Text));
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                DataShow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + (ex.InnerException != null ? "\nInner Exception: " + ex.InnerException.Message : ""));
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string query = "DELETE FROM student WHERE StudentID = @StudentID";
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DataBaseString"].ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("@StudentID", txtStudentNo.Text));
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                DataShow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + (ex.InnerException != null ? "\nInner Exception: " + ex.InnerException.Message : ""));
            }
        }

        private void dataGridStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridStudents.SelectedItem is DataRowView row)
            {
                txtStudentNo.Text = row["StudentID"].ToString();
                txtFirstName.Text = row["Name"].ToString();
                txtLastName.Text = row["Family"].ToString();
                txtField.Text = row["Course"].ToString();
            }
        }

        private void EmptyBox_Click(object sender, RoutedEventArgs e)
        {
            txtStudentNo.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtField.Text = "";
        }
    }
}
