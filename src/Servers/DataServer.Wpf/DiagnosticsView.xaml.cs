using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RxdSolutions.FusionLink.Client
{
    /// <summary>
    /// Interaction logic for DiagnosticsView.xaml
    /// </summary>
    public partial class DiagnosticsView : UserControl
    {
        public DiagnosticsView(DiagnosticsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void Help_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.RxdSolutionsFusionLinkHelpPage);
        }

        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.RxdSolutionsHomepage);
        }

        private void CopyConnectionStringButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.ConnectionString.Text);
        }
    }
}
