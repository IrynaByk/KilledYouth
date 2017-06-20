using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace HypertensionControlUI.Views.Components
{
    public class DocumentPaginatorWrapper : DocumentPaginator

    {
        #region Fields

        private readonly Size _pageSize;
        private readonly DocumentPaginator _paginator;
        private Size _margin;
        private Typeface _typeface;

        #endregion


        #region Properties

        public override bool IsPageCountValid => _paginator.IsPageCountValid;

        public override int PageCount => _paginator.PageCount;

        public override Size PageSize
        {
            get => _paginator.PageSize;
            set => _paginator.PageSize = value;
        }

        public override IDocumentPaginatorSource Source => _paginator.Source;

        #endregion


        #region Initialization

        public DocumentPaginatorWrapper( DocumentPaginator paginator, Size pageSize, Size margin )
        {
            _pageSize = pageSize;
            _margin = margin;
            _paginator = paginator;
            _paginator.PageSize = new Size( _pageSize.Width - margin.Width * 2, _pageSize.Height - margin.Height * 2 );
        }

        #endregion


        #region Public methods

        public override DocumentPage GetPage( int pageNumber )
        {
            var page = _paginator.GetPage( pageNumber );

            // Create a wrapper visual for transformation and add extras

            var newpage = new ContainerVisual();
            var title = new DrawingVisual();

            using ( var ctx = title.RenderOpen() )
            {
                if ( _typeface == null )
                    _typeface = new Typeface( "Times New Roman" );

                var text = new FormattedText( "Page" + (pageNumber + 1),
                                              CultureInfo.CurrentCulture,
                                              FlowDirection.LeftToRight,
                                              _typeface,
                                              14,
                                              Brushes.Black );

                ctx.DrawText( text, new Point( 0, -96 / 4 ) ); // 1/4 inch above page content
            }

            var smallerPage = new ContainerVisual();
            smallerPage.Children.Add( page.Visual );

            newpage.Children.Add( smallerPage );
            newpage.Children.Add( title );
            newpage.Transform = new TranslateTransform( _margin.Width, _margin.Height );

            return new DocumentPage( newpage, _pageSize, Move( page.BleedBox ), Move( page.ContentBox ) );
        }

        #endregion


        #region Non-public methods

        private Rect Move( Rect rect )
        {
            if ( rect.IsEmpty )
                return rect;

            return new Rect( rect.Left + _margin.Width, rect.Top + _margin.Height, rect.Width, rect.Height );
        }

        #endregion
    }
}
