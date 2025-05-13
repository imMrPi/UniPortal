using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace UniPortal.Student
{
    public class DrawCoursesDigrams
    {
        private Canvas diagramCanvas;
        private ListBox listBox;

        public DrawCoursesDigrams(Canvas canvas, ListBox listBox)
        {
            this.diagramCanvas = canvas;
            this.listBox = listBox;
        }

        public void DrawShapes()
        {
            AddRectangle("ریاضی عمومی 1", 50, 50);

            AddTriangle("فیزیک 1", 200, 200);
        }

        private void AddRectangle(string courseName, double left, double top)
        {
            var rect = new Rectangle
            {
                Width = 100,
                Height = 50,
                Fill = Brushes.LightGreen,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Tag = courseName
            };

            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);

            diagramCanvas.Children.Add(rect);


            var textBlock = new TextBlock
            {
                Text = courseName,
                Foreground = Brushes.Black,
                Background = Brushes.Transparent,
                TextAlignment = TextAlignment.Center,
                Width = rect.Width,
                Height = rect.Height
            };

            Canvas.SetLeft(textBlock, left);
            Canvas.SetTop(textBlock, top + 15);

            diagramCanvas.Children.Add(textBlock);


            rect.MouseLeftButtonUp += Shape_MouseLeftButtonUp;
        }

        private void AddTriangle(string courseName, double left, double top)
        {
            var triangle = new Polygon
            {
                Points = new PointCollection(new Point[] { new Point(0, 50), new Point(50, 0), new Point(100, 50) }),
                Fill = Brushes.LightPink,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Tag = courseName
            };

            Canvas.SetLeft(triangle, left);
            Canvas.SetTop(triangle, top);

            diagramCanvas.Children.Add(triangle);


            var textBlock = new TextBlock
            {
                Text = courseName,
                Foreground = Brushes.Black,
                Background = Brushes.Transparent,
                TextAlignment = TextAlignment.Center,
                Width = 100,
                Height = 50
            };

            Canvas.SetLeft(textBlock, left);
            Canvas.SetTop(textBlock, top + 15);

            diagramCanvas.Children.Add(textBlock);


            triangle.MouseLeftButtonUp += Shape_MouseLeftButtonUp;
        }

        private void Shape_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Shape shape && shape.Tag is string courseName)
            {
                listBox.Items.Add(courseName);
            }
        }
    }
}
