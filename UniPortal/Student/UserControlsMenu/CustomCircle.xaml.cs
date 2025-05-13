using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UniPortal.Student.UserControlsMenu
{
    public partial class CustomCircle : UserControl
    {
        public static readonly DependencyProperty GradientBrushProperty =
            DependencyProperty.Register("GradientBrushCircle", typeof(Brush), typeof(CustomCircle), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnGradientBrushChanged));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("TextCircle", typeof(string), typeof(CustomCircle), new PropertyMetadata(string.Empty, OnTextChanged));

        public static new readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSizeCircle", typeof(double), typeof(CustomCircle), new PropertyMetadata(12.0, OnFontSizeChanged));

        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register("TextWidthCircle", typeof(double), typeof(CustomCircle), new PropertyMetadata(double.NaN, OnTextWidthChanged));

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMarginCircle", typeof(Thickness), typeof(CustomCircle), new PropertyMetadata(new Thickness(0), OnTextMarginChanged));

        public CustomCircle()
        {
            InitializeComponent();
            ToolTip.Content = new CourseToolTip();
        }

        public Brush GradientBrushCircle
        {
            get => (Brush)GetValue(GradientBrushProperty);
            set => SetValue(GradientBrushProperty, value);
        }

        public string TextCircle
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public double FontSizeCircle
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public double TextWidthCircle
        {
            get => (double)GetValue(TextWidthProperty);
            set => SetValue(TextWidthProperty, value);
        }

        public Thickness TextMarginCircle
        {
            get => (Thickness)GetValue(TextMarginProperty);
            set => SetValue(TextMarginProperty, value);
        }

        private static void OnGradientBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomCircle)d;
            control.circle.Background = (Brush)e.NewValue;
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomCircle)d;
            control.textBlock.Text = (string)e.NewValue;
        }

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomCircle)d;
            control.textBlock.FontSize = (double)e.NewValue;
        }

        private static void OnTextWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomCircle)d;
            control.textBlock.Width = (double)e.NewValue;
        }

        private static void OnTextMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomCircle)d;
            control.textBlock.Margin = (Thickness)e.NewValue;
        }
    }
}
