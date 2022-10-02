using System.Windows.Media.Imaging;

namespace Pyanulis.RayTracing.View
{
    internal interface ISceneView
    {
        void ApplyImage(BitmapSource bitmap);
    }
}
