using System.Windows;

namespace HypertensionControlUI.Views.Components
{
    public static class TextBoxHelpers
    {
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.RegisterAttached(
            "PlaceholderText", typeof(string), typeof(TextBoxHelpers), new PropertyMetadata(default(string)));

        public static void SetPlaceholderText(DependencyObject element, string value)
        {
            element.SetValue(PlaceholderTextProperty, value);
        }

        public static string GetPlaceholderText(DependencyObject element)
        {
            return (string)element.GetValue(PlaceholderTextProperty);
        }
    }
}
