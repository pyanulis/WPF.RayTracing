using Pyanulis.RayTracing.Model.Strategies;
using Pyanulis.RayTracing.ViewModel;
using System;

namespace Pyanulis.RayTracing.Model
{
    internal interface IModel
    {
        void GenerateAsync();
        void GenerateWithShuffleAsync();
        void Cancel();
        void SetStrategy(GenerateStrategyAbstract generateStrategy);

        RayColor[,] ColorMap { get; }
        IViewModel ViewModel { get; set; }
        TimeSpan LastDuration { get; }

        int SamplesRate { get; set; }
        int ColorDepth { get; set; }
        int ImageHeight { get; set; }
        int ImageWidth { get; }
        int ThreadCount { get; set; }
        bool IsLive { get; set; }

        ObstacleSet World { get; set; }
    }
}
