using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Pyanulis.RayTracing.View
{
    /// <summary>
    /// Interaction logic for SceneViewControl.xaml
    /// </summary>
    public partial class SceneViewControl : UserControl, ISceneView
    {
        public SceneViewControl()
        {
            InitializeComponent();
        }

        public Dispatcher ContextDispatcher => Dispatcher;

        public void ApplyImage(BitmapSource bitmap)
        {
            image.Height = bitmap.Height;
            image.Width = bitmap.Width;
            image.Source = bitmap;
        }

        public void GenerationStarted()
        {
            image.Source = null;
        }
    }
}
