using System.Windows;

namespace MVx.Views
{
    public interface IViewAware
    {
        void AttachView(FrameworkElement view);
    }
}
