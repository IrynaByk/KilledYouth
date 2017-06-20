using System.Windows;
using System.Windows.Media;

namespace HypertensionControlUI.Views
{
    public static class VisualTreeExtensions
    {
        #region Public methods

        public static TChild FindChild<TChild>( this FrameworkElement parent, string name = null ) where TChild : FrameworkElement
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount( parent );
            for ( var childIndex = 0; childIndex < childrenCount; childIndex++ )
            {
                var child = VisualTreeHelper.GetChild( parent, childIndex ) as FrameworkElement;
                if ( child != null )
                {
                    if ( child is TChild typedChild && (name == null || child.Name == name) )
                        return typedChild;

                    typedChild = child.FindChild<TChild>( name );
                    if ( typedChild != null )
                        return typedChild;
                }
            }

            return null;
        }

        #endregion
    }
}
