namespace Pyanulis.RayTracing.View
{
    public interface IToolbarView
    {
        void GenerationStarted();
        void GenerationCompleted();
        void AddThreadProgress(int key);
        void SetProgress(int key, double value);
    }
}
