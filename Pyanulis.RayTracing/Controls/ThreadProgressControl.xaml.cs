using System.Windows.Controls;

namespace Pyanulis.RayTracing.Controls
{
    /// <summary>
    /// Interaction logic for ThreadProgressControl.xaml
    /// </summary>
    public partial class ThreadProgressControl : UserControl
    {
        public ThreadProgressControl()
        {
            InitializeComponent();
        }

        public double Value 
        { 
            get => bar.Value; 
            set => bar.Value = (int)(value * 100);
        }
    }
}
