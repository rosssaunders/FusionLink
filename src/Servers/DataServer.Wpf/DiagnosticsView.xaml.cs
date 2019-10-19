using System.Windows.Controls;

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

        private void Help_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.RxdSolutionsFusionLinkHelpPage);
        }

        private void Logo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.RxdSolutionsHomepage);
        }
    }
}
