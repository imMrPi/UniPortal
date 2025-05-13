//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Shapes;
//using System.Windows;

//namespace UniPortal.Admins.UserControlsMenu
//{
//    public class ConnectionLine
//    {
//        public Shape StartShape { get; }
//        public Shape EndShape { get; }
//        public Line Line { get; }

//        public ConnectionLine(Shape startShape, Shape endShape)
//        {
//            StartShape = startShape;
//            EndShape = endShape;
//            Line = new Line
//            {
//                Stroke = System.Windows.Media.Brushes.Black,
//                StrokeThickness = 2
//            };
//        }

//        public void UpdatePosition()
//        {
//            var start = StartShape.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
//            var end = EndShape.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));

//            Line.X1 = start.X + StartShape.Width / 2;
//            Line.Y1 = start.Y + StartShape.Height / 2;
//            Line.X2 = end.X + EndShape.Width / 2;
//            Line.Y2 = end.Y + EndShape.Height / 2;
//        }
//    }
//}
