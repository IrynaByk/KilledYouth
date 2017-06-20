using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using HypertensionControlUI.ViewModels;
using HypertensionControlUI.Views.Components;

namespace HypertensionControlUI.Views.Pages
{
    /// <summary>
    ///     Interaction logic for IndividualPatientCard.xaml
    /// </summary>
    public partial class IndividualPatientCardView : PageViewBase<IndividualPatientCardViewModel>
    {
        #region Initialization

        public IndividualPatientCardView()
        {
            InitializeComponent();
        }

        #endregion


        #region Public methods

        public void SendToPrinter()
        {
            //  Clone the patient card
            var flowDocumentScrollViewer = IndividualPatientCardContainer.FindChild<FlowDocumentScrollViewer>();
            var flowDocument = flowDocumentScrollViewer.Document;
            var documentClone = CloneXamlControl( flowDocument );

            //  Get the document paginator
            //  Display the print dialog and print if confirmed
            var dialog = new PrintDialog();
            if ( dialog.ShowDialog() == true )
            {
                documentClone.PageHeight = dialog.PrintableAreaHeight;
                documentClone.PageWidth = dialog.PrintableAreaWidth;
                documentClone.PagePadding = new Thickness( 0 );

                documentClone.ColumnGap = 0;
                documentClone.ColumnWidth = dialog.PrintableAreaWidth;

                var paginator = ((IDocumentPaginatorSource) documentClone).DocumentPaginator;

                // Wrap with fixed page size paginator: 8 inch x 6 inch, with half inch margin
                paginator = new DocumentPaginatorWrapper( paginator, new Size( dialog.PrintableAreaWidth, dialog.PrintableAreaHeight ), new Size( 48, 48 ) );

                dialog.PrintDocument( paginator, "Patient card" );
            }
        }

        #endregion


        #region Event handlers

        private void Print_Click( object sender, RoutedEventArgs e )
        {
            SendToPrinter();
        }

        #endregion


        #region Non-public methods

        private T CloneXamlControl<T>( T source ) where T : DependencyObject
        {
            var gridXaml = XamlWriter.Save( source );
            using ( var stringReader = new StringReader( gridXaml ) )
            using ( var xmlReader = XmlReader.Create( stringReader ) )
                return (T) XamlReader.Load( xmlReader );
        }

        #endregion
    }
}
