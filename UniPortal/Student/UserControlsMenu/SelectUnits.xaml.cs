using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using UniPortal.Admins.UserControlsMenu;
using System.Windows.Threading;
using Mysqlx.Crud;

namespace UniPortal.Student.UserControlsMenu
{
    public partial class SelectUnits : UserControl
    {
        private bool isConnectionMode = false;
        private UserControl firstControl = null;
        private ObservableCollection<ConnectionInfo> connections = new ObservableCollection<ConnectionInfo>();
        private List<UserControl> userControls = new List<UserControl>();
        private List<List<UserControl>> controlPairs = new List<List<UserControl>>();

        private DispatcherTimer timer;
        public static List<string> SelectedCourseOnCode = new List<string>();
        public static int CourseSelectedCounter = 0;
        public SelectUnits()
        {
            InitializeComponent();
            LoadUserControlsFromFile();


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            AddSelectedCourseInList();
        }
        public void AddSelectedCourseInList()
        {
            SelectedCourseList.Items.Clear();
            foreach(string course in SelectedCourseOnCode)
            {
               
                SelectedCourseList.Items.Add(new ListBoxItem { Content = course});
            }
           if(CourseSelectedCounter >= 7)
            {
                MessageBox.Show("You have reached the maximum unit selection limit");
            }
        }

     
        public static List<T> FindChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            List<T> children = new List<T>();
            if (parent == null) return children;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child != null && child is T tChild)
                {
                    children.Add(tChild);
                }

                children.AddRange(FindChildren<T>(child));
            }

            return children;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControlsFromFile();
        }
        public class UserControlInfo
        {
            public string Xaml { get; set; }
            public double Left { get; set; }
            public double Top { get; set; }
        }

        public class ConnectionInfo
        {
            public UserControl FirstControl { get; set; }
            public UserControl SecondControl { get; set; }
            public Line Line { get; set; }
        }
        private void Control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateConnections();
        }

        private void Control_LayoutUpdated(object sender, EventArgs e)
        {
            UpdateConnections();
        }
        private void UpdateConnections()
        {
            foreach (var connection in connections)
            {
                UpdateLinePosition(connection.Line, connection.FirstControl, connection.SecondControl);
            }
        }
        private void AddConnection(UserControl first, UserControl second, bool isNewAction = true)
        {
            var connection = new ConnectionInfo
            {
                FirstControl = first,
                SecondControl = second,
                Line = CreateConnectionLine(first, second)
            };
            connections.Add(connection);

            // Save pairs in the list
            controlPairs.Add(new List<UserControl> { first, second });

            first.SizeChanged += Control_SizeChanged;
            second.SizeChanged += Control_SizeChanged;
            first.LayoutUpdated += Control_LayoutUpdated;
            second.LayoutUpdated += Control_LayoutUpdated;


        }

        private Line CreateConnectionLine(UserControl first, UserControl second)
        {
            var line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            UpdateLinePosition(line, first, second);
            // Adding the line at the first layer of the Canvas
            drawingCanvas.Children.Insert(0, line);
            return line;
        }
        private void UpdateLinePosition(Line line, UserControl first, UserControl second)
        {
            var firstCenter = GetControlCenter(first);
            var secondCenter = GetControlCenter(second);
            line.X1 = firstCenter.X + 50;
            line.Y1 = firstCenter.Y + 50;
            line.X2 = secondCenter.X + 50;
            line.Y2 = secondCenter.Y + 50;
        }
        private Point GetControlCenter(UserControl control)
        {
            double left = Canvas.GetLeft(control);
            double top = Canvas.GetTop(control);
            double width = control.ActualWidth;
            double height = control.ActualHeight;

            // Wait for layout to update if ActualWidth or ActualHeight are 0
            if (width == 0 || height == 0)
            {
                control.UpdateLayout();
                width = control.ActualWidth;
                height = control.ActualHeight;
            }

            return new Point(left + width / 2, top + height / 2);
        }

        private void LoadUserControlsFromFile()
        {
            string filePath = "C:/Users/Asus/Desktop/All Charts/Computer.xaml";

            if (File.Exists(filePath))
            {
                string xaml = File.ReadAllText(filePath);
                using (StringReader stringReader = new StringReader(xaml))
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    var document = XDocument.Load(xmlReader);
                    var userControlInfos = document.Root.Elements("UserControlInfo")
                        .Select(element => new UserControlInfo
                        {
                            Xaml = element.Element("Xaml").Value,
                            Left = double.Parse(element.Element("Left").Value),
                            Top = double.Parse(element.Element("Top").Value)
                        }).ToList();

                    drawingCanvas.Children.Clear();
                    connections.Clear();
                    controlPairs.Clear();

                    // Load user controls
                    userControls = new List<UserControl>();
                    foreach (var info in userControlInfos)
                    {
                        using (StringReader controlReader = new StringReader(info.Xaml))
                        using (XmlReader controlXmlReader = XmlReader.Create(controlReader))
                        {
                            UserControl control = (UserControl)XamlReader.Load(controlXmlReader);
                            Canvas.SetLeft(control, info.Left);
                            Canvas.SetTop(control, info.Top);
                            drawingCanvas.Children.Add(control);
                            userControls.Add(control);
                        }
                    }

                    // Load connections as UserControl pairs
                    var connectionElements = document.Root.Element("Connections")?.Elements("Connection");
                    if (connectionElements != null)
                    {
                        foreach (var element in connectionElements)
                        {
                            int firstControlIndex = int.Parse(element.Element("FirstControlIndex").Value);
                            int secondControlIndex = int.Parse(element.Element("SecondControlIndex").Value);

                            
                            if (firstControlIndex >= 0 && secondControlIndex >= 0)
                            {
                             var firstControl = userControls[firstControlIndex];
                                var secondControl = userControls[secondControlIndex];
                                AddConnection(firstControl, secondControl);
                            }
                    

                         
                        }
                    }

                    // Attach event handlers for updating lines
                    foreach (var userControl in drawingCanvas.Children.OfType<UserControl>())
                    {
                        userControl.SizeChanged += Control_SizeChanged;
                        userControl.LayoutUpdated += Control_LayoutUpdated;
                    }

                    // Update connections to ensure they are correctly positioned
                    UpdateConnections();

                    // Perform passed course handling in the Loaded event
                    drawingCanvas.Loaded += DrawingCanvas_Loaded;
                }
            }
        }

        private void DrawingCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> passedCourses = GetPassedCourses();

          
            foreach (var userControl in drawingCanvas.Children.OfType<UserControl>())
            {
                if (userControl.Tag != null)
                {
                    string userControlTag = userControl.Tag.ToString();
                    bool passedCourse = passedCourses.Contains(userControlTag);
                    if (userControl is DrawingCircle drawingCircle)
                    {
                        drawingCircle.PassedCourse = passedCourse;
                        drawingCircle.ToggleBorderVisibilityForPassedCourse();
                    }
                    if (userControl is DrawingCircleForGeneralCourse drawingCircleGenral)
                    {
                        drawingCircleGenral.PassedCourse = passedCourse;
                        drawingCircleGenral.ToggleBorderVisibilityForPassedCourse();
                    }
                    if (userControl is DrawingRectTangle drawingRectAngle)
                    {
                        drawingRectAngle.PassedCourse = passedCourse;
                        drawingRectAngle.ToggleBorderVisibilityForPassedCourse();
                    }
                    if (userControl is DrawingTriangle drawingTriangle)
                    {
                        drawingTriangle.PassedCourse = passedCourse;
                        drawingTriangle.ToggleBorderVisibilityForPassedCourse();
                    }
                    if (userControl is DrawingTriangleForBasic drawingTriangleForBasic)
                    {
                        drawingTriangleForBasic.PassedCourse = passedCourse;
                        drawingTriangleForBasic.ToggleBorderVisibilityForPassedCourse();
                    }
                }
            }
        }
        private List<string> GetPassedCourses()
        {
            List<string> passedCourseNames = new List<string>();
            string connectionString = ConfigurationManager.ConnectionStrings["DataBaseString"].ConnectionString;

            using (MySqlConnection databaseConnection = new MySqlConnection(connectionString))
            {
                string query = "SELECT CourseName FROM passedcourse WHERE StudentID = @StudentID";
                databaseConnection.Open();

                using (MySqlCommand command = new MySqlCommand(query, databaseConnection))
                {
                    command.Parameters.AddWithValue("@StudentID", Login.username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            passedCourseNames.Add(reader["CourseName"].ToString());
                        }
                    }
                }
            }

            return passedCourseNames;
        }


        #region ZoomIn & ZoomOut

        private void ZoomIn_Click(object sender, MouseButtonEventArgs e)
        {
            Zoom(1.1);
        }

        private void ZoomOut_Click(object sender, MouseButtonEventArgs e)
        {
            Zoom(0.9);
        }

        private void drawingCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Zoom(1.1);
            }
            else
            {
                Zoom(0.9);
            }
        }

        private void Zoom(double factor)
        {
            var transform = ContentScaleTransform;

            // Get the current scale values
            double scaleX = transform.ScaleX * factor;
            double scaleY = transform.ScaleY * factor;

            // Set the new scale values
            transform.ScaleX = scaleX;
            transform.ScaleY = scaleY;
        }
        #endregion

        #region Drag and Drop
        private bool isDragging = false;
        private Point clickPosition;

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(draggableCanvas);
            draggableCanvas.CaptureMouse();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(parentCanvas);

                double newLeft = currentPosition.X - clickPosition.X;
                double newTop = currentPosition.Y - clickPosition.Y;

                if (newLeft < 0)
                {
                    newLeft = 0;
                }
                else if (newLeft + draggableCanvas.ActualWidth > parentCanvas.ActualWidth)
                {
                    newLeft = parentCanvas.ActualWidth - draggableCanvas.ActualWidth;
                }

                if (newTop < 0)
                {
                    newTop = 0;
                }
                else if (newTop + draggableCanvas.ActualHeight > parentCanvas.ActualHeight)
                {
                    newTop = parentCanvas.ActualHeight - draggableCanvas.ActualHeight;
                }

                Canvas.SetLeft(draggableCanvas, newLeft);
                Canvas.SetTop(draggableCanvas, newTop);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            draggableCanvas.ReleaseMouseCapture();
        }
        #endregion


        #region
        public static List<string> storedCourseNames = new List<string>();
        #endregion
    }
}

