using Pyanulis.RayTracing.ViewModel;

namespace Pyanulis.RayTracing.Model
{
    internal interface IModel
    {
        void GenerateAsync();
        void Cancel();
        RayColor[,] ColorMap { get; }
        IViewModel ViewModel { get; set; }

        int SamplesRate { get; set; }
        int ColorDepth { get; set; }
        int ImageHeight { get; set; }
        int ImageWidth { get; }
    }
}
