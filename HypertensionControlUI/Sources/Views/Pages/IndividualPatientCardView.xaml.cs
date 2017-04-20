using System;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
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

        public int SaveAsXps( string fileName )

        {

            using ( var container = Package.Open( fileName + ".xps",
                                                  FileMode.Create ) )

            using ( var xpsDoc = new XpsDocument( container, CompressionOption.Maximum ) )

            {
                var rsm = new XpsSerializationManager( new XpsPackagingPolicy( xpsDoc ), false );

                var documentClone = CloneXamlControl( IndividualCard );

                var paginator = ((IDocumentPaginatorSource)documentClone).DocumentPaginator;

                // 8 inch x 6 inch, with half inch margin

                paginator = new DocumentPaginatorWrapper( paginator, new Size( 768, 676 ), new Size( 48, 48 ) );

                rsm.SaveAsXaml( paginator );
            }
            return 0;
        }

        private T CloneXamlControl<T>( T source ) where  T : DependencyObject
        {
            var gridXaml = XamlWriter.Save( source );
            using ( var stringReader = new StringReader( gridXaml ) )
            using ( var xmlReader = XmlReader.Create( stringReader ) )
                return (T) XamlReader.Load( xmlReader );
        }

        #endregion


        private void Print_Click( object sender, RoutedEventArgs e )
        {
            SaveAsXps( "printCard" );
        }
    }
}
