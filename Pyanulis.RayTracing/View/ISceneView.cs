using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Pyanulis.RayTracing.View
{
    internal interface ISceneView
    {
        void ApplyImage(BitmapSource bitmap);
        Dispatcher ContextDispatcher { get; }
        void GenerationStarted();
    }
}
