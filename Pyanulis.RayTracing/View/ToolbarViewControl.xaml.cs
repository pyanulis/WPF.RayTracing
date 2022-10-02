using Pyanulis.RayTracing.Controls;
using Pyanulis.RayTracing.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Pyanulis.RayTracing.View
{
    /// <summary>
    /// Interaction logic for ToolbarViewControl.xaml
    /// </summary>
    public partial class ToolbarViewControl : UserControl, IToolbarView
    {
        private Dictionary<int, ThreadProgressControl> m_progressBars = new Dictionary<int, ThreadProgressControl>();
        private IViewModel m_viewModel;

        public ToolbarViewControl()
        {
            InitializeComponent();
        }

        internal IViewModel ViewModel
        {
            get => m_viewModel;
            set
            {
                m_viewModel = value;
                DataContext = m_viewModel;
            }
        }

        public void AddThreadProgress(int key)
        {
            ThreadProgressControl ctlProgress = new ThreadProgressControl();
            ctlProgress.Margin = new Thickness(4, 4, 4, 0);
            ctlProgress.HorizontalAlignment = HorizontalAlignment.Stretch;
            ctlProgress.Height = 20;
            pnlStack.Children.Add(ctlProgress);
            m_progressBars.Add(key, ctlProgress);
        }

        public void SetProgress(int key, double value)
        {
            if (m_progressBars.ContainsKey(key))
            {
                m_progressBars[key].Value = value;
            }
        }

        private void Clear()
        {
            m_progressBars.Clear();
            pnlStack.Children.Clear();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Cancel();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Generate();
        }

        public void GenerationStarted()
        {
            gridSettings.IsEnabled = false;
            btnGenerate.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Visible;
        }

        public void GenerationCompleted()
        {
            gridSettings.IsEnabled = true;
            btnCancel.Visibility = Visibility.Collapsed;
            btnGenerate.Visibility = Visibility.Visible;
            Clear();
        }

        private void TxtPreviewText(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            e.Handled = !int.TryParse(textBox.Text + e.Text, out int num) || num <= 0;
        }
    }
}
