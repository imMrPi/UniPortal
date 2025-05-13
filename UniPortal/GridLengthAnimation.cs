using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace UniPortal
{
    public class GridLengthAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType => typeof(GridLength);

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        public GridLength From
        {
            get => (GridLength)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public GridLength To
        {
            get => (GridLength)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress == null)
                return GridLength.Auto;

            double fromVal = From.Value;
            double toVal = To.Value;

            return new GridLength(
                animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal,
                From.GridUnitType == GridUnitType.Auto || To.GridUnitType == GridUnitType.Auto ? GridUnitType.Pixel : From.GridUnitType);
        }

        protected override Freezable CreateInstanceCore() => new GridLengthAnimation();
    }
}
