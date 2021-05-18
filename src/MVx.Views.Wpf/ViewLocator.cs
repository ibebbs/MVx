using System;
using System.Windows;
using System.Windows.Controls;

namespace MVx.Views.Wpf
{
    /// <summary>
    /// Represents a component that can locate the view for a given model
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Get the view for the specified <paramref name="model"/>, optionally
        /// determined by where it is being displayed provided by <paramref name="target"/>
        /// </summary>
        /// <param name="model">The model to get the view for</param>
        /// <param name="target">The location the view will be displayed</param>
        /// <returns></returns>
        FrameworkElement LocateForModel(object model, DependencyObject target);
    }

    /// <summary>
    /// Provides a default implementation of the <see cref="IViewLocator"/> interface
    /// </summary>
    /// <remarks>
    /// To use different conventions, replace the value of the <see cref="Instance"/> property with
    /// a new implementation of <see cref="IViewLocator"/>
    /// </remarks>
    public class ViewLocator : IViewLocator
    {
        public static IViewLocator Instance { get; set; } = new ViewLocator();

        /// <summary>
        /// Get the view for the specified <paramref name="model"/>, optionally
        /// determined by where it is being displayed provided by <paramref name="target"/>
        /// </summary>
        /// <param name="model">The model to get the view for</param>
        /// <param name="target">The location the view will be displayed</param>
        public FrameworkElement LocateForModel(object model, DependencyObject target)
        {
            var viewTypeName = model.GetType().FullName.Replace("Model", string.Empty);

            var viewType = Type.GetType(viewTypeName, false);

            if (viewType != null)
            {
                return Activator.CreateInstance(viewType) as FrameworkElement;
            }
            else
            {
                return new ContentControl();
            }
        }
    }
}
