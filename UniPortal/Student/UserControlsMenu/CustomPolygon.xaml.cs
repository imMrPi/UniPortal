using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UniPortal.Student.UserControlsMenu
{
    public partial class CustomPolygon : UserControl
    {
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(CustomPolygon), new PropertyMetadata(new PointCollection(), OnPointsChanged));

        public static readonly DependencyProperty GradientBrushProperty =
            DependencyProperty.Register("GradientBrush", typeof(Brush), typeof(CustomPolygon), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnGradientBrushChanged));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomPolygon), new PropertyMetadata(string.Empty, OnTextChanged));

        public static new readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(CustomPolygon), new PropertyMetadata(12.0, OnFontSizeChanged));

        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register("TextWidth", typeof(double), typeof(CustomPolygon), new PropertyMetadata(double.NaN, OnTextWidthChanged));

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(CustomPolygon), new PropertyMetadata(new Thickness(0), OnTextMarginChanged));

        public static new readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register("Foreground", typeof(Brush), typeof(CustomPolygon), new PropertyMetadata(new SolidColorBrush(Colors.White), OnForegroundChanged));

        public CustomPolygon()
        {
            InitializeComponent();
            ToolTip.Content = new CourseToolTip();
        
        }
        
        public PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        public Brush GradientBrush
        {
            get => (Brush)GetValue(GradientBrushProperty);
            set => SetValue(GradientBrushProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public double TextWidth
        {
            get => (double)GetValue(TextWidthProperty);
            set => SetValue(TextWidthProperty, value);
        }

        public Thickness TextMargin
        {
            get => (Thickness)GetValue(TextMarginProperty);
            set => SetValue(TextMarginProperty, value);
        }
        public new Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomPolygon)d;
            control.polygon.Points = (PointCollection)e.NewValue;
        }

        private static void OnGradientBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomPolygon)d;
            control.polygon.Fill = (Brush)e.NewValue;
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomPolygon)d;
            control.textBlock.Text = (string)e.NewValue;
        }

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomPolygon)d;
            control.textBlock.FontSize = (double)e.NewValue;
        }

        private static void OnTextWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomPolygon)d;
            control.textBlock.Width = (double)e.NewValue;
        }

        private static void OnTextMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomPolygon)d;
            control.textBlock.Margin = (Thickness)e.NewValue;
        }
        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomPolygon)d;
            control.textBlock.Foreground = (Brush)e.NewValue;
        }

   
    }
}
