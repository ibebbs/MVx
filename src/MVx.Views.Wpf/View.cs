using System;
using System.Windows;
using System.Windows.Controls;

namespace MVx.Views
{
    public static class View
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached("Model", typeof(object), typeof(View), new PropertyMetadata(null, ModelPropertyChanged));

        private static void ModelPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var view = ViewLocator.Instance.LocateForModel(e.NewValue, target);

                view.DataContext = e.NewValue;

                SetContentProperty(target, view);
            }
            else
            {
                SetContentProperty(target, e.NewValue);
            }
        }


        private static bool SetContentProperty(ContentControl contentControl, object newValue)
        {
            contentControl.Content = newValue;

            return true;
        }

        private static bool SetContentProperty(Control control, object newValue)
        {
            var property = control.GetType().GetProperty("Content");

            if (property != null)
            {
                property.SetValue(control, newValue);

                return true;
            }
            else
            {
                throw new ArgumentException($"Could not find content property on type: '{control.GetType().Name}'", nameof(control));
            }
        }

        private static bool SetContentProperty(DependencyObject target, object newValue)
        {
            return target switch
            {
                ContentControl cc => SetContentProperty(cc, newValue),
                Control c => SetContentProperty(c, newValue),
                _ => throw new ArgumentException($"Unknown target type: '{target.GetType().Name}'", nameof(target))
            };
        }

        public static object GetModel(DependencyObject obj)
        {
            return (object)obj.GetValue(ModelProperty);
        }

        public static void SetModel(DependencyObject obj, object value)
        {
            obj.SetValue(ModelProperty, value);
        }
    }
}
