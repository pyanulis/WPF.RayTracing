using Pyanulis.RayTracing.Model.Materials;
using Pyanulis.RayTracing.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing.Model
{
    internal class RayModel : IModel
    {
        private const double c_aspectRatio = 3.0 / 2.0;
        private const int c_dpi = 96;

        private readonly ObstacleSet m_world = new ObstacleSet();
        private readonly Camera m_camera;

        private readonly CancellationTokenSource m_tokenSource = new CancellationTokenSource();

        private int m_imgWidth;
        private int m_imgHeight = 200;
        private RayColor[,] m_rayColorsAsync;

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
                m_imgWidth = (int)(m_imgHeight * c_aspectRatio);

                m_rayColorsAsync = new RayColor[m_imgHeight, m_imgWidth];
            }
        }

        public int ImageWidth => m_imgWidth;

        public RayModel()
        {
            m_imgWidth = (int)(ImageHeight * c_aspectRatio);
            m_rayColorsAsync = new RayColor[m_imgHeight, m_imgWidth];

            Material material_ground = new Lambertian(new RayColor(0.8, 0.8, 0.0));
            //Material material_center = new Lambertian(new RayColor(0.7, 0.3, 0.3));
            //Material material_left = new Metal(new RayColor(0.8, 0.8, 0.8));
            Material material_center = new Lambertian(new RayColor(0.1, 0.2, 0.5));
            Material material_left = new Dielectric(1.5);
            Material material_right = new Metal(new RayColor(0.8, 0.6, 0.2), 0.3);

            m_world.Add(new Sphere(new Vec3(0.0, -100.5, -1.0), 100.0, material_ground));
            m_world.Add(new Sphere(new Vec3(0.0, 0.0, -1.0), 0.5, material_center));
            m_world.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), 0.5, material_left));
            m_world.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), -0.4, material_left));
            m_world.Add(new Sphere(new Vec3(1.0, 0.0, -1.0), 0.5, material_right));

            double R = Math.Cos(Math.PI / 4);

            //Material material_left = new Lambertian(new RayColor(0, 0, 1));
            //Material material_right = new Lambertian(new RayColor(1, 0, 0));

            //m_world.Add(new Sphere(new Vec3(-R, 0, -1), R, material_left));
            //m_world.Add(new Sphere(new Vec3(R, 0, -1), R, material_right));

            //m_world = CreateWorld();

            m_camera = new Camera(
                lookfrom: new Vec3(0, 3, 7),
                lookat: new Vec3(0, 0, 0),
                vup: new Vec3(0, 1, 0),
                vfov: 20,
                aspectRatio: c_aspectRatio,
                aperture: 0.1,
                focus_dist: 10.0);

            for (int j = 0; j < ImageHeight; j++)
                for (int i = 0; i < m_imgWidth; i++)
                {
                    m_rayColorsAsync[j, i] = new RayColor();
                }
        }

        public async void GenerateAsync()
        {
            const int threadCount = 8;

            RayColor[,] rayColors = new RayColor[ImageHeight, m_imgWidth];
            int range = ImageHeight / threadCount;
            int jMin = 0;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < threadCount; ++i)
            {
                int jMax = i == threadCount - 1 ? ImageHeight - 1 : jMin + range - 1;
                int min = jMin;
                int key = i + 1;
                Progress<double> progress = new Progress<double>(p => ViewModel.SetProgress(key, p));
                Task task = new Task(() =>
                {
                    Generate(
                        min,
                        jMax,
                        rayColors,
                        progress);
                },
                m_tokenSource.Token);

                tasks.Add(task);
                jMin += range;
                ViewModel.AddThreadProgress(key);
            }

            foreach (var task in tasks)
            {
                task.Start();
            }

            await Task.WhenAll(tasks);

            ViewModel.GenerationCompleted();
        }

        private void Generate(int jMin, int jMax, RayColor[,] rayColors, IProgress<double> progress)
        {
            Random random = new Random();

            List<int> vert = new List<int>();
            List<int> hor = new List<int>();
            for (int j = jMin; j <= jMax; ++j)
            {
                vert.Add(j);
            }
            for (int i = 0; i < m_imgWidth; ++i)
            {
                hor.Add(i);
            }
            //double total = c_samplePerPixel > 10 ? vert.Count * hor.Count : vert.Count;
            double total = vert.Count;

            int step = 0;
            foreach (int j in vert.OrderBy(x => random.Next()).OrderBy(y => random.Next()).ToArray())
            {
                foreach (int i in hor.OrderBy(x => random.Next()).OrderBy(y => random.Next()).ToArray())
                {
                    RayColor pixelColor = new RayColor(0, 0, 0, SamplesRate);
                    for (int s = 0; s < SamplesRate; ++s)
                    {
                        if (m_tokenSource.IsCancellationRequested)
                        {
                            return;
                        }
                        // x and y are normalized to be <=1 and become a coefficient of a basis vector
                        double x = ((i * 1.0) + random.NextDouble()) / (m_imgWidth - 1);
                        double y = ((j * 1.0) + random.NextDouble()) / (ImageHeight - 1);
                        Ray r = m_camera.GetRay(x, y);
                        pixelColor += GetRayColor(r, m_world, ColorDepth);
                    }

                    // need to invert j since image starts at the bottom
                    int jInverted = ImageHeight - 1 - j;

                    //lock (m_sync)
                    {
                        m_rayColorsAsync[jInverted, i] = pixelColor;
                    }

                    //if (c_samplePerPixel > 10)
                    {
                        //progress.Report(step++ / total);
                    }
                }

                //if (c_samplePerPixel < 10)
                {
                    progress.Report(step / total);
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

        public void Cancel()
        {
            m_tokenSource.Cancel();
        }
    }
}
