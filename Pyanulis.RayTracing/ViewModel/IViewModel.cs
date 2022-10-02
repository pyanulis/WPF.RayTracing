using System;

namespace Pyanulis.RayTracing.ViewModel
{
    internal interface IViewModel
    {
        int SamplesRate { get; set; }
        int ColorDepth { get; set; }
        int ImageHeight { get; set; }
        int ThreadCount { get; set; }
        bool IsLive { get; set; }
        string LastDuration { get; set; }

        void Generate();
        void Cancel();
        void GenerationCompleted();
        void StepCompleted();
        void AddThreadProgress(int key);
        void SetProgress(int key, double value);
    }
}
