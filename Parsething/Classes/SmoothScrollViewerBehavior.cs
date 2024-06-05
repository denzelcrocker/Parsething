using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;

namespace Parsething.Classes
{
    public class SmoothScrollBehavior : Behavior<ListView>
    {
        private const double ScrollDuration = 100; // Duration in milliseconds

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                var scrollViewer = FindVisualChild<ScrollViewer>(AssociatedObject);
                if (scrollViewer != null)
                {
                    scrollViewer.PreviewMouseWheel -= OnPreviewMouseWheel;
                }
            }
            AssociatedObject.Loaded -= OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = FindVisualChild<ScrollViewer>(AssociatedObject);
            if (scrollViewer != null)
            {
                scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            }
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var scrollViewer = (ScrollViewer)sender;
            double newVerticalOffset = scrollViewer.VerticalOffset - e.Delta;
            AnimateScroll(scrollViewer, newVerticalOffset);
        }

        private void AnimateScroll(ScrollViewer scrollViewer, double toValue)
        {
            var animation = new DoubleAnimation
            {
                From = scrollViewer.VerticalOffset,
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(ScrollDuration),
                EasingFunction = new QuadraticEase()
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, scrollViewer);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ScrollViewerBehaviorHelper.VerticalOffsetProperty));
            storyboard.Begin();
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    return (T)child;
                }
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
    }

    public static class ScrollViewerBehaviorHelper
    {
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerBehaviorHelper),
                new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static double GetVerticalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(VerticalOffsetProperty);
        }

        public static void SetVerticalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(VerticalOffsetProperty, value);
        }

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }
    }
}