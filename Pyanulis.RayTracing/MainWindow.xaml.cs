using Pyanulis.RayTracing.Model;
using Pyanulis.RayTracing.ViewModel;
using System.Windows;

/// <summary>
/// https://raytracing.github.io/books/RayTracingInOneWeekend.html
/// </summary>
namespace Pyanulis.RayTracing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IModel m_model;
        private IViewModel m_viewModel;

        public MainWindow()
        {
            InitializeComponent();

            m_model = new RayModel();
            m_viewModel = new RayViewModel(m_model, m_view, toolbar);
            toolbar.ViewModel = m_viewModel;
            DataContext = m_viewModel;
        }
    }
}
