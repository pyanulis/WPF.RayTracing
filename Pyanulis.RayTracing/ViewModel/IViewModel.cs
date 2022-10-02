namespace Pyanulis.RayTracing.ViewModel
{
    internal interface IViewModel
    {
        int SamplesRate { get; set; }
        int ColorDepth { get; set; }
        int ImageHeight { get; set; }

        void Generate();
        void Cancel();
        void GenerationCompleted();
        void AddThreadProgress(int key);
        void SetProgress(int key, double value);
    }
}
