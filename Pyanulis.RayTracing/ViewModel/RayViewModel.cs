using Pyanulis.RayTracing.Model;
using Pyanulis.RayTracing.View;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pyanulis.RayTracing.ViewModel
{
    internal class RayViewModel : IViewModel
    {
        private const int c_dpi = 96;
        private BitmapSource m_bitmap;

        internal IModel Model { get; }
        internal ISceneView Scene { get; }
        internal IToolbarView Toolbar { get; }

        public int SamplesRate
        {
            get => Model.SamplesRate;
            set => Model.SamplesRate = value;
        }
        public int ColorDepth
        {
            get => Model.ColorDepth;
            set => Model.ColorDepth = value;
        }
        public int ImageHeight
        {
            get => Model.ImageHeight;
            set => Model.ImageHeight = value;
        }

        public RayViewModel(IModel model, ISceneView scene, IToolbarView toolbar)
        {
            Model = model;
            Scene = scene;
            Toolbar = toolbar;

            Model.ViewModel = this;
        }

        public void Cancel()
        {
            Model.Cancel();
        }

        public void Generate()
        {
            Model.GenerateAsync();
            Toolbar.GenerationStarted();
        }

        public void GenerationCompleted()
        {
            Toolbar.GenerationCompleted();
            Scene.ApplyImage(CreateImage(Model.ColorMap));
        }

        public void AddThreadProgress(int key)
        {
            Toolbar.AddThreadProgress(key);
        }

        public void SetProgress(int key, double value)
        {
            Toolbar.SetProgress(key, value);
        }

        private BitmapSource CreateImage(RayColor[,] colorList)
        {
            const int channels = 4;
            PixelFormat pixelFormat = PixelFormats.Bgr32;
            int width = Model.ImageWidth;
            int height = Model.ImageHeight;

            int rawStride = width * channels;
            byte[] rawImage = new byte[rawStride * height];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    RayColor color = colorList[y, x];
                    var index = (y * rawStride) + (x * channels);

                    rawImage[index] = color.B;
                    rawImage[index + 1] = color.G;
                    rawImage[index + 2] = color.R;
                    rawImage[index + 3] = 255;
                }
            }


            m_bitmap = BitmapSource.Create(width, height,
                c_dpi, c_dpi, pixelFormat, null,
                rawImage, rawStride);

            return m_bitmap;
        }
    }
}
