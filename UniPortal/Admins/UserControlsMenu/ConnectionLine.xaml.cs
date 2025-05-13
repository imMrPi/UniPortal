using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System;

public class ConnectionLine : UserControl
{
    public Shape StartShape { get; private set; }
    public Shape EndShape { get; private set; }
    public Line LineElement { get; private set; }

    public ConnectionLine()
    {

        LineElement = new Line
        {
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };
        this.Content = LineElement;
    }

    public void SetPosition(Point start, Point end)
    {
        if (double.IsNaN(start.X) || double.IsNaN(start.Y) ||
            double.IsNaN(end.X) || double.IsNaN(end.Y))
        {
            throw new ArgumentException("One of the points contains NaN values");
        }

        LineElement.X1 = start.X;
        LineElement.Y1 = start.Y;
        LineElement.X2 = end.X;
        LineElement.Y2 = end.Y;
    }

    public void UpdatePosition(Shape startShape, Shape endShape)
    {
        StartShape = startShape;
        EndShape = endShape;

        var start = GetShapeCenter(StartShape);
        var end = GetShapeCenter(EndShape);

        LineElement.X1 = start.X;
        LineElement.Y1 = start.Y;
        LineElement.X2 = end.X;
        LineElement.Y2 = end.Y;
    }

    private Point GetShapeCenter(Shape shape)
    {
        double x = Canvas.GetLeft(shape) + shape.Width / 2;
        double y = Canvas.GetTop(shape) + shape.Height / 2;
        return new Point(x, y);
    }

}