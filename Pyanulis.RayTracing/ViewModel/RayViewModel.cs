using Pyanulis.RayTracing.Model;
using Pyanulis.RayTracing.View;
using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pyanulis.RayTracing.ViewModel
{
    internal class RayViewModel : IViewModel, INotifyPropertyChanged
    {
        #region Constants and private fields
        
        private const int c_dpi = 96;

        private BitmapSource m_bitmap;
        private string m_lastDuration;

        #endregion

        #region Public properties

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
        public int ThreadCount
        {
            get => Model.ThreadCount;
            set => Model.ThreadCount = value;
        }
        public bool IsLive
        {
            get => Model.IsLive;
            set => Model.IsLive = value;
        }
        public string LastDuration
        {
            get => m_lastDuration;
            set
            {
                m_lastDuration = value;
                OnPropertyChanged(nameof(LastDuration));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor
        
        public RayViewModel(IModel model, ISceneView scene, IToolbarView toolbar)
        {
            Model = model;
            Scene = scene;
            Toolbar = toolbar;

            Model.ViewModel = this;
        }

        #endregion

        #region Public methods

        public void Cancel()
        {
            Model.Cancel();
        }

        public void Generate()
        {
            Model.GenerateAsync();
            Toolbar.GenerationStarted();
            Scene.GenerationStarted();
        }

        public void GenerationCompleted()
        {
            LastDuration = $"Last duration: {Model.LastDuration}";

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

        public void StepCompleted()
        {
            Scene.ContextDispatcher.Invoke(() =>
            {
                CreateImage(Model.ColorMap);
                Scene.ApplyImage(CreateImage(Model.ColorMap));
            });
        }

        #endregion

        #region Private methods

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

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        } 

        #endregion
    }
}
