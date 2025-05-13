using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using UniPortal.Student.UserControlsMenu;

namespace UniPortal.Admins.UserControlsMenu
{
    public partial class SetUnits : UserControl
    {
        private DispatcherTimer _timer;

        public SetUnits()
        {
            InitializeComponent();
            UserControls = new ObservableCollection<UserControl>();
            DataContext = this;
            drawingCanvas.MouseLeftButtonDown += DrawingCanvas_MouseLeftButtonDown;
            drawingCanvas.MouseMove += DrawingCanvas_MouseMove;
            drawingCanvas.MouseLeftButtonUp += DrawingCanvas_MouseLeftButtonUp;

            StartTimer();
        }
        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            RemoveUserControl(DrawingRectTangle.RemovedRectangle, DrawingRectTangle.RectangelRemoved);
        }

        #region Grid
        private void DrawingGridCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DrawGrid();
        }

        private void DrawingGridCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            GridCanvas.Children.Clear();
        }

        private void DrawGrid()
        {
            double width = GridCanvas.ActualWidth;
            double height = GridCanvas.ActualHeight;
            double cellSize = 20;

            GridCanvas.Children.Clear();

            for (double x = 0; x <= width; x += cellSize)
            {
                Line verticalLine = new Line
                {
                    Stroke = Brushes.LightGray,
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = height
                };
                GridCanvas.Children.Add(verticalLine);
            }

            for (double y = 0; y <= height; y += cellSize)
            {
                Line horizontalLine = new Line
                {
                    Stroke = Brushes.LightGray,
                    X1 = 0,
                    Y1 = y,
                    X2 = width,
                    Y2 = y
                };
                GridCanvas.Children.Add(horizontalLine);
            }
        }
        #endregion

        #region Generate Shapes
        public ObservableCollection<UserControl> UserControls { get; set; }
        public int CounterX = 0;
        public int CounterY = 0;

        private void CreateCircle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var customCircle = new DrawingCircle();
            AddUserControlToCanvas(customCircle);
        }
        private void CreatGeneralCourseCircle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var customCircle = new DrawingCircleForGeneralCourse();
            AddUserControlToCanvas(customCircle);
        }
        private void CreateTriangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var customPolygon = new DrawingTriangle();
            AddUserControlToCanvas(customPolygon);
        }

        private void CreateRectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var customPolygon = new DrawingRectTangle();
            AddUserControlToCanvas(customPolygon);
        }



        private void CreateTriangleBasicCourse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var customPolygon = new DrawingTriangleForBasic();
            AddUserControlToCanvas(customPolygon);
        }
        private void AddUserControlToCanvas(UserControl control)
        {
            CounterX += 15;

            Canvas.SetLeft(control, 100 + CounterX);
            Canvas.SetTop(control, 100 + CounterY);
            control.AllowDrop = true;
            drawingCanvas.Children.Add(control);
            UserControls.Add(control);

            if (CounterX > 150)
            {
                CounterX = 0;
                CounterY += 30;
            }

            // add TO undoStack
            AddUserControl(control, true);
        }

        #endregion

        #region Drag and Drop
        public UserControl FoundedUserControl;
        private UserControl draggedControl;
        private Point mouseOffset;

        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(drawingCanvas);
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(drawingCanvas, clickPoint);

            if (hitTestResult != null)
            {
                DependencyObject clickedObject = hitTestResult.VisualHit;
                while (clickedObject != null && !(clickedObject is UserControl))
                {
                    clickedObject = VisualTreeHelper.GetParent(clickedObject);
                }

                if (clickedObject is UserControl clickedUserControl)
                {
                    draggedControl = clickedUserControl;
                    mouseOffset = e.GetPosition(draggedControl);
                    mouseOffset.X -= draggedControl.ActualWidth / 2;
                    mouseOffset.Y -= draggedControl.ActualHeight / 2;
                    drawingCanvas.CaptureMouse();
                }
            }
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedControl != null)
            {
                Point newPosition = e.GetPosition(drawingCanvas);
                double newLeft = newPosition.X - mouseOffset.X;
                double newTop = newPosition.Y - mouseOffset.Y;

                if (newLeft < 0)
                {
                    newLeft = 0;
                }
                else if (newLeft + draggedControl.ActualWidth > drawingCanvas.ActualWidth)
                {
                    newLeft = drawingCanvas.ActualWidth - draggedControl.ActualWidth - 100;
                }

                if (newTop < 0)
                {
                    newTop = 0;
                }
                else if (newTop + draggedControl.ActualHeight > drawingCanvas.ActualHeight)
                {
                    newTop = drawingCanvas.ActualHeight - draggedControl.ActualHeight;
                }

                Canvas.SetLeft(draggedControl, newLeft);
                Canvas.SetTop(draggedControl, newTop);
            }
        }

        private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedControl != null)
            {
                drawingCanvas.ReleaseMouseCapture();
                draggedControl = null;
            }
        }
        #endregion

        #region ZoomIn & ZoomOut

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            
            e.Handled = true;

           
            if (e.Delta > 0)
            {
                ContentScaleTransform.ScaleX *= 1.1;
                ContentScaleTransform.ScaleY *= 1.1;
            }
            else
            {
                ContentScaleTransform.ScaleX /= 1.1;
                ContentScaleTransform.ScaleY /= 1.1;
            }
        }

        private const double ZoomFactor = 1.1;
        private const double MaxScale = 1;
        private const double MinScale = 0.5;

        private void drawingCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point position = e.GetPosition(drawingCanvas);
            if (e.Delta > 0)
            {
                ZoomIn(position);
            }
            else if (e.Delta < 0)
            {
                ZoomOut(position);
            }
        }

        private void ZoomIn(Point position)
        {
            if (ContentScaleTransform.ScaleX < MaxScale)
            {
                ApplyZoom(ZoomFactor, position);
            }
        }

        private void ZoomOut(Point position)
        {
            if (ContentScaleTransform.ScaleX > MinScale)
            {
                ApplyZoom(1 / ZoomFactor, position);
            }
        }

        private void ApplyZoom(double zoomFactor, Point center)
        {
            double absoluteX = center.X * ContentScaleTransform.ScaleX + ContentScaleTransform.CenterX;
            double absoluteY = center.Y * ContentScaleTransform.ScaleY + ContentScaleTransform.CenterY;

            ContentScaleTransform.ScaleX *= zoomFactor;
            ContentScaleTransform.ScaleY *= zoomFactor;

            ContentScaleTransform.CenterX = absoluteX - center.X * ContentScaleTransform.ScaleX;
            ContentScaleTransform.CenterY = absoluteY - center.Y * ContentScaleTransform.ScaleY;
        }
        #endregion

        #region Save and Load UserControls

        private void SaveUserControlsToFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*",
                DefaultExt = ".xaml",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                List<UserControlInfo> userControlInfos = new List<UserControlInfo>();
                foreach (UIElement element in drawingCanvas.Children)
                {
                    if (element is UserControl userControl)
                    {
                        // Set all TextBoxes to read-only
                        SetTextBoxesReadOnly(userControl, true);

                        string xaml = RemoveNameAttributes(XamlWriter.Save(userControl));
                        UserControlInfo info = new UserControlInfo
                        {
                            Xaml = xaml,
                            Left = Canvas.GetLeft(userControl),
                            Top = Canvas.GetTop(userControl)
                        };
                        userControlInfos.Add(info);

                        // Restore all TextBoxes to editable
                        SetTextBoxesReadOnly(userControl, false);
                    }
                }

                StringBuilder xamlBuilder = new StringBuilder();
                xamlBuilder.AppendLine("<UserControls>");
                foreach (var info in userControlInfos)
                {
                    xamlBuilder.AppendLine("<UserControlInfo>");
                    xamlBuilder.AppendLine($"<Xaml><![CDATA[{info.Xaml}]]></Xaml>");
                    xamlBuilder.AppendLine($"<Left>{info.Left}</Left>");
                    xamlBuilder.AppendLine($"<Top>{info.Top}</Top>");
                    xamlBuilder.AppendLine("</UserControlInfo>");
                }

                // Save connections as UserControl pairs
                xamlBuilder.AppendLine("<Connections>");
                foreach (var pair in controlPairs)
                {
                    int firstControlIndex = drawingCanvas.Children.OfType<UserControl>().ToList().IndexOf(pair[0]);
                    int secondControlIndex = drawingCanvas.Children.OfType<UserControl>().ToList().IndexOf(pair[1]);
                    xamlBuilder.AppendLine("<Connection>");
                    xamlBuilder.AppendLine($"<FirstControlIndex>{firstControlIndex}</FirstControlIndex>");
                    xamlBuilder.AppendLine($"<SecondControlIndex>{secondControlIndex}</SecondControlIndex>");
                    xamlBuilder.AppendLine("</Connection>");
                }
                xamlBuilder.AppendLine("</Connections>");

                xamlBuilder.AppendLine("</UserControls>");

                File.WriteAllText(filePath, xamlBuilder.ToString());
            }
        }
        private string RemoveNameAttributes(string xaml)
        {
            var doc = XDocument.Parse(xaml);
            foreach (var element in doc.Descendants())
            {
                element.Attributes().Where(a => a.Name.LocalName == "Name" || a.Name.LocalName == "x:Name").Remove();
            }
            return doc.ToString();
        }
        private void SetTextBoxesReadOnly(UserControl userControl, bool isReadOnly)
        {
            foreach (var textBox in FindChildren<TextBox>(userControl))
            {
                textBox.IsReadOnly = isReadOnly;
            }
        }
        private List<T> FindChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            List<T> result = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    result.Add((T)child);
                }
                result.AddRange(FindChildren<T>(child));
            }
            return result;
        }

        private void LoadUserControlsFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*",
                DefaultExt = ".xaml"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

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
                        List<UserControl> userControls = new List<UserControl>();
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

                                // Validate indices
                                if (firstControlIndex >= 0 && firstControlIndex < userControls.Count &&
                                    secondControlIndex >= 0 && secondControlIndex < userControls.Count)
                                {
                                    var firstControl = userControls[firstControlIndex];
                                    var secondControl = userControls[secondControlIndex];

                                    AddConnection(firstControl, secondControl);
                                }
                                else
                                {
                                    // Handle the case where indices are out of range
                                    MessageBox.Show($"Invalid indices: {firstControlIndex}, {secondControlIndex}");
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
                    }
                }
            }
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


        private bool isConnectionMode = false;
        private UserControl firstControl = null;
        private ObservableCollection<ConnectionInfo> connections = new ObservableCollection<ConnectionInfo>();
        private List<UserControl> userControls = new List<UserControl>();
        private List<List<UserControl>> controlPairs = new List<List<UserControl>>();
        private Stack<UndoRedoAction> undoStack = new Stack<UndoRedoAction>();
        private Stack<UndoRedoAction> redoStack = new Stack<UndoRedoAction>();

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

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isConnectionMode)
            {
                var clickPoint = e.GetPosition(drawingCanvas);
                var hitTestResult = VisualTreeHelper.HitTest(drawingCanvas, clickPoint);

                if (hitTestResult != null)
                {
                    var clickedObject = hitTestResult.VisualHit as DependencyObject;

                    while (clickedObject != null && !(clickedObject is UserControl))
                    {
                        clickedObject = VisualTreeHelper.GetParent(clickedObject);
                    }

                    if (clickedObject is UserControl clickedControl)
                    {
                        UserControl_MouseLeftButtonDown(clickedControl, e);
                    }
                }
                e.Handled = true;
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isConnectionMode) return;

            var clickedControl = sender as UserControl;

            if (firstControl == null)
            {
                firstControl = clickedControl;
            }
            else
            {
                var secondControl = clickedControl;
                AddConnection(firstControl, secondControl);
                firstControl = null;  // Reset to allow further connections
                isConnectionMode = false; // Optional: reset connection mode after one connection
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

            if (isNewAction)
            {
                // Save the current state to the undo stack
                var action = new UndoRedoAction
                {
                    ActionType = ActionType.AddConnection,
                    Connections = new List<ConnectionInfo> { connection },
                    UserControls = new List<UserControl>()
                };
                undoStack.Push(action);
                redoStack.Clear(); // Clear the redo stack only if it's a new action

                // Debug message
                Debug.WriteLine("Added connection and pushed to undo stack.");
            }
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
        private void UpdateLinePosition(Line line, UserControl first, UserControl second)
        {
            try
            {
                var firstCenter = GetControlCenter(first);
                var secondCenter = GetControlCenter(second);
                line.X1 = firstCenter.X + 50;
                line.Y1 = firstCenter.Y + 50;
                line.X2 = secondCenter.X + 50;
                line.Y2 = secondCenter.Y + 50;
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"An error occurred: {ex.Message}");
               
                MessageBox.Show($"An error occurred while updating the line position: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveUserControlsToFile();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControlsFromFile();
        }

    
        public class UndoRedoAction
        {
            public ActionType ActionType { get; set; }
            public List<UserControl> UserControls { get; set; }
            public List<ConnectionInfo> Connections { get; set; }
        }

        public enum ActionType
        {
            AddConnection,
            RemoveConnection,
            AddUserControl,
            RemoveUserControl
        }

        private void ConnectionsButton_Click(object sender, RoutedEventArgs e)
        {
            isConnectionMode = true;
            firstControl = null;
        }

        #endregion

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            // Unsubscribe event handlers for each connection
            foreach (var connection in connections)
            {
                connection.FirstControl.SizeChanged -= Control_SizeChanged;
                connection.SecondControl.SizeChanged -= Control_SizeChanged;
                connection.FirstControl.LayoutUpdated -= Control_LayoutUpdated;
                connection.SecondControl.LayoutUpdated -= Control_LayoutUpdated;
            }

            // Clear the canvas
            drawingCanvas.Children.Clear();

            // Clear the connections list
            connections.Clear();

            // Clear the control pairs list
            controlPairs.Clear();

            // Clear the user controls list
            userControls.Clear();

            // Reset connection mode
            isConnectionMode = false;
            firstControl = null;

            // Clear undo and redo stacks
            undoStack.Clear();
            redoStack.Clear();

            // Debug message
            Debug.WriteLine("Reset clicked, cleared all states.");
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (undoStack.Count > 0)
            {
                // Pop the last action from the undo stack
                var lastAction = undoStack.Pop();

                // Perform the opposite action
                switch (lastAction.ActionType)
                {
                    case ActionType.AddConnection:
                        foreach (var connection in lastAction.Connections)
                        {
                            RemoveConnection(connection, false);
                        }
                        break;
                    case ActionType.RemoveConnection:
                        foreach (var connection in lastAction.Connections)
                        {
                            AddConnection(connection.FirstControl, connection.SecondControl);
                        }
                        break;
                    case ActionType.AddUserControl:
                        foreach (var control in lastAction.UserControls)
                        {
                            RemoveUserControl(control, false);
                        }
                        break;
                    case ActionType.RemoveUserControl:
                        foreach (var control in lastAction.UserControls)
                        {
                            AddUserControl(control, false);
                        }
                        break;
                }

                // Push the undone action to the redo stack
                redoStack.Push(lastAction);
                UpdateConnections();

                // Debug message
                Debug.WriteLine("Undo action performed.");
            }
        }


        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (redoStack.Count > 0)
            {
                // Pop the last action from the redo stack
                var lastAction = redoStack.Pop();

                // Reapply the action
                switch (lastAction.ActionType)
                {
                    case ActionType.AddConnection:
                        foreach (var connection in lastAction.Connections)
                        {
                            AddConnection(connection.FirstControl, connection.SecondControl, false);
                        }
                        break;
                    case ActionType.RemoveConnection:
                        foreach (var connection in lastAction.Connections)
                        {
                            RemoveConnection(connection, false);
                        }
                        break;
                    case ActionType.AddUserControl:
                        foreach (var control in lastAction.UserControls)
                        {
                            AddUserControl(control, false);
                        }
                        break;
                    case ActionType.RemoveUserControl:
                        foreach (var control in lastAction.UserControls)
                        {
                            RemoveUserControl(control, false);
                        }
                        break;
                }

                // Push the redone action to the undo stack
                undoStack.Push(lastAction);
                UpdateConnections();
                // Debug message
                Debug.WriteLine("Redo action performed.");
            }
        }


        private void RemoveConnection(ConnectionInfo connection, bool addToUndoStack = true)
        {
            // Remove event handlers
            connection.FirstControl.SizeChanged -= Control_SizeChanged;
            connection.SecondControl.SizeChanged -= Control_SizeChanged;
            connection.FirstControl.LayoutUpdated -= Control_LayoutUpdated;
            connection.SecondControl.LayoutUpdated -= Control_LayoutUpdated;

            // Remove the line from the canvas
            drawingCanvas.Children.Remove(connection.Line);

            // Remove the connection from the list
            connections.Remove(connection);

            // Optionally, add the removal to the undo stack
            if (addToUndoStack)
            {
                var action = new UndoRedoAction
                {
                    ActionType = ActionType.RemoveConnection,
                    Connections = new List<ConnectionInfo> { connection },
                    UserControls = new List<UserControl>()
                };
                undoStack.Push(action);
                redoStack.Clear(); // Clear the redo stack

              
                Debug.WriteLine("Removed connection and pushed to undo stack.");
            }
        }
        private void AddUserControl(UserControl control, bool isNewAction)
        {
            // Check if the control is already in the canvas and remove it if it is
            if (drawingCanvas.Children.Contains(control))
            {
                drawingCanvas.Children.Remove(control);
            }

            drawingCanvas.Children.Add(control);
            userControls.Add(control);

            // Save the current state to the undo stack
            var action = new UndoRedoAction
            {
                ActionType = ActionType.AddUserControl,
                UserControls = new List<UserControl> { control },
                Connections = new List<ConnectionInfo>()
            };
            undoStack.Push(action);

            if (isNewAction)
            {
                redoStack.Clear(); // Clear the redo stack only if it's a new action
            }

            // Debug message
            Debug.WriteLine("Added UserControl and pushed to undo stack.");
        }





        public void RemoveUserControl(UserControl control, bool isNewAction = true)
        {
            drawingCanvas.Children.Remove(control);
            userControls.Remove(control);

            // Optionally, add the removal to the undo stack
            if (isNewAction)
            {
                var action = new UndoRedoAction
                {
                    ActionType = ActionType.RemoveUserControl,
                    UserControls = new List<UserControl> { control },
                    Connections = new List<ConnectionInfo>()
                };
                undoStack.Push(action);

                redoStack.Clear(); 

            
                Debug.WriteLine("Removed UserControl and pushed to undo stack.");
            }
        }





    }
}
