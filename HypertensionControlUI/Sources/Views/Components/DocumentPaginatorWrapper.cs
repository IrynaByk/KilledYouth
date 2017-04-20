using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HypertensionControlUI.Views.Components
{
    public class DocumentPaginatorWrapper : DocumentPaginator

    {
        #region Fields

        private readonly Size m_PageSize;
        private readonly DocumentPaginator m_Paginator;
        private Size m_Margin;
        private Typeface m_Typeface;

        #endregion


        #region Properties

        public override bool IsPageCountValid

        {
            get { return m_Paginator.IsPageCountValid; }
        }

        public override int PageCount

        {
            get { return m_Paginator.PageCount; }
        }

        public override Size PageSize

        {
            get { return m_Paginator.PageSize; }

            set { m_Paginator.PageSize = value; }
        }

        public override IDocumentPaginatorSource Source

        {
            get { return m_Paginator.Source; }
        }

        #endregion


        #region Initialization

        public DocumentPaginatorWrapper( DocumentPaginator paginator, Size pageSize, Size margin )

        {
            m_PageSize = pageSize;

            m_Margin = margin;

            m_Paginator = paginator;

            m_Paginator.PageSize = new Size( m_PageSize.Width - margin.Width*2,
                                             m_PageSize.Height - margin.Height*2);
        }

        #endregion


        #region Public methods

        public override DocumentPage GetPage( int pageNumber )

        {
            var page = m_Paginator.GetPage( pageNumber );

            // Create a wrapper visual for transformation and add extras

            var newpage = new ContainerVisual();

            var title = new DrawingVisual();

            using ( var ctx = title.RenderOpen() )

            {
                if ( m_Typeface == null )

                {
                    m_Typeface = new Typeface("Times New Roman");
                }

                var text = new FormattedText("Page" + (pageNumber + 1),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                m_Typeface,
                14,
                Brushes.Black);

                ctx.DrawText( text, new Point( 0, -96/4 ) ); // 1/4 inch above page content
            }

            var smallerPage = new ContainerVisual();

            smallerPage.Children.Add( page.Visual );

            newpage.Children.Add( smallerPage );

            newpage.Children.Add( title );

            newpage.Transform = new TranslateTransform( m_Margin.Width, m_Margin.Height );

            return new DocumentPage( newpage, m_PageSize, Move( page.BleedBox ), Move( page.ContentBox ) );
        }

        #endregion


        #region Non-public methods

        private Rect Move( Rect rect )

        {
            if ( rect.IsEmpty )

                return rect;

            return new Rect( rect.Left + m_Margin.Width, rect.Top + m_Margin.Height,
                             rect.Width, rect.Height );
        }

        #endregion
    }
}
