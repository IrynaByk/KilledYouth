using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace HypertensionControlUI.Views.Components
{
    /// <summary>
    ///     Interaction logic for HyperlinkButton.xaml
    /// </summary>
    public partial class HyperlinkButton : Button
    {
        #region Constants

        public static readonly DependencyProperty NavigateUriProperty = DependencyProperty.Register(
            "NavigateUri", typeof (Uri), typeof (HyperlinkButton), new FrameworkPropertyMetadata( default(Uri) ) );

        #endregion


        #region Fields

        private readonly Hyperlink _hyperlink;

        #endregion


        #region Properties

        public Uri NavigateUri
        {
            get { return (Uri) GetValue( NavigateUriProperty ); }
            set { SetValue( NavigateUriProperty, value ); }
        }

        #endregion


        #region Events and invocation

        protected override void OnClick()
        {
            base.OnClick();
            _hyperlink.DoClick();
        }

        protected override void OnContentChanged( object oldContent, object newContent )
        {
            var grid = new Grid();
            grid.Children.Add( new TextBlock( _hyperlink ) );
            grid.Children.Add( new ContentPresenter() );

            base.OnContentChanged( oldContent, grid );
        }

        #endregion


        #region Initialization

        public HyperlinkButton()
        {
            _hyperlink = new Hyperlink();
            _hyperlink.SetBinding( Hyperlink.NavigateUriProperty,
                                   new Binding { Source = this, Path = new PropertyPath( "NavigateUri" ), Mode = BindingMode.OneWay } );
            InitializeComponent();
        }

        #endregion
    }
}
