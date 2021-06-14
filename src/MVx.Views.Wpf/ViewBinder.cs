using System.Windows;

namespace MVx.Views
{
    public static class ViewBinder
    {
        public static void Bind(FrameworkElement view, object model)
        {
            view.DataContext = model;

            if (model is IViewAware viewAware)
            {
                viewAware.AttachView(view);
            }
        }
    }
}
