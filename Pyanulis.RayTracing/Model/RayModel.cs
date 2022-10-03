using Pyanulis.RayTracing.Model.Materials;
using Pyanulis.RayTracing.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing.Model
{
    internal class RayModel : IModel
    {
        #region Constants

        private const double c_aspectRatio = 3.0 / 2.0;

        #endregion

        #region Private fields

        private readonly ObstacleSet m_world = new();
        private readonly Camera m_camera;
        private CancellationTokenSource m_tokenSource = new();

        private int m_imgWidth;
        private int m_imgHeight = 200;
        private RayColor[,] m_rayColorsAsync;

        #endregion

        #region Public properties

        public IViewModel ViewModel { get; set; }

        public RayColor[,] ColorMap => m_rayColorsAsync;

        public int SamplesRate { get; set; } = 100;
        public int ColorDepth { get; set; } = 50;
        public int ImageHeight
        {
            get => m_imgHeight;
            set
            {
                m_imgHeight = value;
                InitMap();
            }
        }
        public int ThreadCount { get; set; } = 8;
        public bool IsLive { get; set; } = true;

        public TimeSpan LastDuration { get; private set; }

        public int ImageWidth => m_imgWidth;

        #endregion

        #region Constructor

        public RayModel()
        {
            InitMap();

            m_world = WorldContainer.SimpleThree;

            m_camera = new Camera(
                lookfrom: new Vec3(0, 4, 7),
                lookat: new Vec3(0, 1, 0),
                vup: new Vec3(0, 1, 0),
                vfov: 20,
                aspectRatio: c_aspectRatio,
                aperture: 0.1,
                focus_dist: 10.0);
        }

        #endregion

        #region Public methods

        public async void GenerateAsync()
        {
            int m_threadCount = ThreadCount;

            InitMap();

            int range = ImageHeight / m_threadCount;
            int jMin = 0;
            List<Task> tasks = new();

            m_tokenSource = new CancellationTokenSource();
            AutoResetEvent[] events = new AutoResetEvent[m_threadCount];

            Stopwatch stopwatch = new();
            stopwatch.Start();
            for (int i = 0; i < m_threadCount; ++i)
            {
                int jMax = i == m_threadCount - 1 ? ImageHeight - 1 : jMin + range - 1;
                int min = jMin;
                int key = i + 1;

                Progress<double> progress = new(p => ViewModel.SetProgress(key, p));
                AutoResetEvent autoEvent = new(false);
                events[i] = autoEvent;

                Task task = new(() => { Generate(min, jMax, progress, autoEvent); }, m_tokenSource.Token);

                tasks.Add(task);
                task.Start();
                jMin += range;
                ViewModel.AddThreadProgress(key);
            }

            CancellationTokenSource watcherSource = new();
            if (IsLive)
            {
                Task watcher = new(() =>
                {
                    while (true)
                    {
                        watcherSource.Token.ThrowIfCancellationRequested();

                        if (WaitHandle.WaitAny(events, 100) > -1)
                        {
                            ViewModel.StepCompleted();
                        }
                    }
                },
                watcherSource.Token);

                watcher.Start();
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                m_tokenSource.Dispose();
            }
            watcherSource.Cancel();

            stopwatch.Stop();
            LastDuration = stopwatch.Elapsed.Duration();
            ViewModel.GenerationCompleted();
        }

        public async void GenerateWithShuffleAsync()
        {
            int m_threadCount = ThreadCount;

            InitMap();

            //Shuffle to make image fade in
            IEnumerable<(int, int)> pixels = Shuffle(out int total);

            int page = total / m_threadCount;
            int jMin = 0;
            List<Task> tasks = new();

            m_tokenSource = new CancellationTokenSource();
            AutoResetEvent[] events = new AutoResetEvent[m_threadCount];

            Stopwatch stopwatch = new();
            stopwatch.Start();
            int pageStart = 0;
            for (int i = 0; i < m_threadCount; ++i)
            {
                IEnumerable<(int, int)> piece = new List<(int, int)>(pixels.Skip(pageStart).Take(page));
                pageStart += page;

                int key = i + 1;

                Progress<double> progress = new(p => ViewModel.SetProgress(key, p));
                AutoResetEvent autoEvent = new(false);
                events[i] = autoEvent;

                Task task = new(() => { GenerateWithShuffle(piece, progress, autoEvent); }, m_tokenSource.Token);

                tasks.Add(task);
                task.Start();

                ViewModel.AddThreadProgress(key);
            }

            CancellationTokenSource watcherSource = new();
            if (IsLive)
            {
                Task watcher = new(() =>
                {
                    while (true)
                    {
                        watcherSource.Token.ThrowIfCancellationRequested();

                        if (WaitHandle.WaitAny(events, 100) > -1)
                        {
                            ViewModel.StepCompleted();
                        }
                    }
                },
                watcherSource.Token);

                watcher.Start();
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                m_tokenSource.Dispose();
            }
            watcherSource.Cancel();

            stopwatch.Stop();
            LastDuration = stopwatch.Elapsed.Duration();
            ViewModel.GenerationCompleted();
        }

        public void Cancel()
        {
            m_tokenSource.Cancel();
        }

        #endregion

        #region Private methods

        private void Generate(int jMin, int jMax, IProgress<double> progress, AutoResetEvent autoResetEvent)
        {
            Random random = new();

            //Shuffle to make image fade in
            IEnumerable<(int, int)> pixels = Shuffle(jMin, jMax, out int total);
            double dTotal = total * 1.0;
            int step = 1;
            foreach ((int, int) pixel in pixels)
            {
                int j = pixel.Item1;
                int i = pixel.Item2;

                RayColor pixelColor = new(0, 0, 0, SamplesRate);
                for (int s = 0; s < SamplesRate; ++s)
                {
                    m_tokenSource.Token.ThrowIfCancellationRequested();

                    // x and y are normalized to be <=1 and become a coefficient of a basis vector
                    double x = ((i * 1.0) + random.NextDouble()) / (m_imgWidth - 1);
                    double y = ((j * 1.0) + random.NextDouble()) / (ImageHeight - 1);
                    Ray r = m_camera.GetRay(x, y);
                    pixelColor += GetRayColor(r, m_world, ColorDepth);
                }

                // need to invert j since image starts at the bottom
                int jInverted = ImageHeight - 1 - j;

                m_rayColorsAsync[jInverted, i] = pixelColor;

                if (step % m_imgWidth == 0)
                {
                    progress.Report(step / dTotal);

                    if (IsLive)
                    {
                        autoResetEvent.Set();
                    }
                }
                step++;
            }
        }

        private void GenerateWithShuffle(IEnumerable<(int, int)> page, IProgress<double> progress, AutoResetEvent autoResetEvent)
        {
            Random random = new();

            double dTotal = page.Count() * 1.0;
            int step = 1;
            foreach ((int, int) pixel in page)
            {
                int j = pixel.Item1;
                int i = pixel.Item2;

                RayColor pixelColor = new(0, 0, 0, SamplesRate);
                for (int s = 0; s < SamplesRate; ++s)
                {
                    m_tokenSource.Token.ThrowIfCancellationRequested();

                    // x and y are normalized to be <=1 and become a coefficient of a basis vector
                    double x = ((i * 1.0) + random.NextDouble()) / (m_imgWidth - 1);
                    double y = ((j * 1.0) + random.NextDouble()) / (ImageHeight - 1);
                    Ray r = m_camera.GetRay(x, y);
                    pixelColor += GetRayColor(r, m_world, ColorDepth);
                }

                // need to invert j since image starts at the bottom
                int jInverted = ImageHeight - 1 - j;

                m_rayColorsAsync[jInverted, i] = pixelColor;

                if (step % m_imgWidth == 0)
                {
                    progress.Report(step / dTotal);

                    if (IsLive)
                    {
                        autoResetEvent.Set();
                    }
                }
                step++;
            }
        }

        private RayColor GetRayColor(Ray ray, ObstacleSet world, int depth)
        {
            if (depth <= 0)
            {
                return new RayColor(0, 0, 0);
            }

            HitRecord record = null;
            if (world.Hit(ray, double.Epsilon, double.MaxValue, ref record))
            {
                Ray scattered = null;
                RayColor attenuation = null;
                if (record.Material.Scatter(ray, record, ref attenuation, ref scattered))
                {
                    return GetRayColor(scattered, world, depth - 1) * attenuation;
                }
                return new RayColor(0, 0, 0);

                //return new RayColor(0.5 * (record.Normal + new RayColor(1, 1, 1)));
            }

            Vec3 unitDirection = ray.Direction.Normalize();
            // blending color:
            // blendedValue = (1 − t) * startValue + t * endValue
            double t = 0.5 * (unitDirection.Y + 1.0);
            return new RayColor((1.0 - t) * new Vec3(1.0, 1.0, 1.0) + t * new Vec3(0.5, 0.7, 1.0));
        }

        private void InitMap()
        {
            m_imgWidth = (int)(ImageHeight * c_aspectRatio);
            m_rayColorsAsync = new RayColor[m_imgHeight, m_imgWidth];

            for (int j = 0; j < ImageHeight; j++)
                for (int i = 0; i < m_imgWidth; i++)
                {
                    m_rayColorsAsync[j, i] = new RayColor();
                }
        }

        private ObstacleSet CreateWorld()
        {
            ObstacleSet world = new();

            Material ground_material = new Lambertian(new RayColor(0.5, 0.5, 0.5));
            world.Add(new Sphere(new Vec3(0, -1000, 0), 1000, ground_material));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double choose_mat = Vec3.RandomDouble();
                    Vec3 center = new(a + 0.9 * Vec3.RandomDouble(), 0.2, b + 0.9 * Vec3.RandomDouble());

                    if ((center - new Vec3(4, 0.2, 0)).Length > 0.9)
                    {
                        Material sphere_material;

                        if (choose_mat < 0.8)
                        {
                            // diffuse
                            RayColor albedo = new(Vec3.Random() * Vec3.Random());
                            sphere_material = new Lambertian(albedo);
                            world.Add(new Sphere(center, 0.2, sphere_material));
                        }
                        else if (choose_mat < 0.95)
                        {
                            // metal
                            RayColor albedo = new(Vec3.Random(0.5, 1));
                            double fuzz = Vec3.RandomDouble(0, 0.5);
                            sphere_material = new Metal(albedo, fuzz);
                            world.Add(new Sphere(center, 0.2, sphere_material));
                        }
                        else
                        {
                            // glass
                            sphere_material = new Dielectric(1.5);
                            world.Add(new Sphere(center, 0.2, sphere_material));
                        }
                    }
                }
            }

            Material material1 = new Dielectric(1.5);
            world.Add(new Sphere(new Vec3(0, 1, 0), 1.0, material1));

            Material material2 = new Lambertian(new RayColor(0.4, 0.2, 0.1));
            world.Add(new Sphere(new Vec3(-4, 1, 0), 1.0, material2));

            Material material3 = new Metal(new RayColor(0.7, 0.6, 0.5), 0.0);
            world.Add(new Sphere(new Vec3(4, 1, 0), 1.0, material3));

            return world;
        }

        private IEnumerable<(int, int)> Shuffle(out int total)
        {
            return Shuffle(0, m_imgHeight - 1, out total);
        }

        private IEnumerable<(int, int)> Shuffle(int jMin, int jMax, out int total)
    {
            Random random = new();

            List<int> vert = new();
            List<int> hor = new();
            for (int j = jMin; j <= jMax; ++j)
            {
                vert.Add(j);
            }
            for (int i = 0; i < m_imgWidth; ++i)
            {
                hor.Add(i);
            }

            total = vert.Count * hor.Count;

            var coords = vert.SelectMany(j => hor.Select<int, (int, int)>(i => new ( j, i )));
            // OrderBy must execute only once to make distinct pieces
            List<(int, int)> list = new(coords.OrderBy(x => random.Next()));
            return list;
        }

        #endregion
    }
}
