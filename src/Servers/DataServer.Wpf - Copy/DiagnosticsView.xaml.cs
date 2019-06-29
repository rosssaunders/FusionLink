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
    }
}
