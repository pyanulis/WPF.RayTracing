using Pyanulis.RayTracing.Model;
using Pyanulis.RayTracing.Model.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/// <summary>
/// https://raytracing.github.io/books/RayTracingInOneWeekend.html
/// </summary>
namespace Pyanulis.RayTracing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Image
        private const double c_aspectRatio = 3.0 / 2.0;
        private const int c_imgWidth = 300;
        private const int c_imgHeight = (int)(c_imgWidth / c_aspectRatio);
        private const int c_dpi = 96;

        private const int c_samplePerPixel = 30;
        private const int c_rayColorDepth = 10;

        private readonly ObstacleSet m_world = new ObstacleSet();
        private readonly Camera m_camera;

        public MainWindow()
        {
            InitializeComponent();

            image.Height = c_imgHeight;
            image.Width = c_imgWidth;

            //Material material_ground = new Lambertian(new RayColor(0.8, 0.8, 0.0));
            ////Material material_center = new Lambertian(new RayColor(0.7, 0.3, 0.3));
            ////Material material_left = new Metal(new RayColor(0.8, 0.8, 0.8));
            //Material material_center = new Lambertian(new RayColor(0.1, 0.2, 0.5));
            //Material material_left = new Dielectric(1.5);
            //Material material_right = new Metal(new RayColor(0.8, 0.6, 0.2), 0.3);

            //m_world.Add(new Sphere(new Vec3(0.0, -100.5, -1.0), 100.0, material_ground));
            //m_world.Add(new Sphere(new Vec3(0.0, 0.0, -1.0), 0.5, material_center));
            //m_world.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), 0.5, material_left));
            //m_world.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), -0.4, material_left));
            //m_world.Add(new Sphere(new Vec3(1.0, 0.0, -1.0), 0.5, material_right));

            double R = Math.Cos(Math.PI / 4);

            //Material material_left = new Lambertian(new RayColor(0, 0, 1));
            //Material material_right = new Lambertian(new RayColor(1, 0, 0));

            //m_world.Add(new Sphere(new Vec3(-R, 0, -1), R, material_left));
            //m_world.Add(new Sphere(new Vec3(R, 0, -1), R, material_right));

            m_world = CreateWorld();

            m_camera = new Camera(
                lookfrom: new Vec3(13, 2, 3),
                lookat: new Vec3(0, 0, 0),
                vup: new Vec3(0, 1, 0),
                vfov: 20, 
                aspectRatio: c_aspectRatio, 
                aperture: 0.1,
                focus_dist: 10.0);

            RayColor[,] rayColors = GenerateAsync();
            image.Source = CreateImage(rayColors);
        }

        private ObstacleSet CreateWorld()
        {
            ObstacleSet world = new ObstacleSet();

            Material ground_material = new Lambertian(new RayColor(0.5, 0.5, 0.5));
            world.Add(new Sphere(new Vec3(0, -1000, 0), 1000, ground_material));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double choose_mat = Vec3.RandomDouble();
                    Vec3 center = new Vec3(a + 0.9 * Vec3.RandomDouble(), 0.2, b + 0.9 * Vec3.RandomDouble());

                    if ((center - new Vec3(4, 0.2, 0)).Length > 0.9)
                    {
                        Material sphere_material;

                        if (choose_mat < 0.8)
                        {
                            // diffuse
                            RayColor albedo = new RayColor(Vec3.Random() * Vec3.Random());
                            sphere_material = new Lambertian(albedo);
                            world.Add(new Sphere(center, 0.2, sphere_material));
                        }
                        else if (choose_mat < 0.95)
                        {
                            // metal
                            RayColor albedo = new RayColor(Vec3.Random(0.5, 1));
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

        // O,d,t - from ray definition
        // a,b,c - quadratic equation parameters
        // C - sphere's center, r - radius
        //
        // Intersection of a ray with a sphere is found by resolving this:
        // (d*d)t^2+(2d*(O−C))*t+(O−C)*(O−C)−r^2=0
        // a = b*b (ray vector scalar)
        // b = 2d*(O−C)
        // c = (O−C)*(O−C)−r^2
        //
        // Two roots - ray goes through the sphere
        // One root - ray touches it
        // Zero roots - no intersection
        private double HitSphere(Vec3 center, double radius, Ray ray)
        {
            Vec3 oc = ray.Origin - center;
            double a = ray.Direction.Dot(ray.Direction);
            double b = 2.0 * oc.Dot(ray.Direction);
            double c = oc.Dot(oc) - radius * radius;
            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                return -1.0;
            }
            else
            {
                return (-b - Math.Sqrt(discriminant)) / (2.0 * a);
            }
        }

        private RayColor RayColor(Ray ray, ObstacleSet world, int depth)
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
                    return RayColor(scattered, world, depth - 1) * attenuation;
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

        private RayColor[,] Generate()
        {
            RayColor[,] rayColors = new RayColor[c_imgHeight,c_imgWidth];

            Random random = new Random();
            for (int j = c_imgHeight - 1; j >= 0; --j)
            {
                for (int i = 0; i < c_imgWidth; ++i)
                {
                    RayColor pixelColor = new RayColor(0, 0, 0, c_samplePerPixel);
                    for (int s = 0; s < c_samplePerPixel; ++s)
                    {
                        // x and y are normalized to be <=1 and become a coefficient of a basis vector
                        double x = ((i * 1.0) + random.NextDouble()) / (c_imgWidth - 1);
                        double y = ((j * 1.0) + random.NextDouble()) / (c_imgHeight - 1);
                        Ray r = m_camera.GetRay(x, y);
                        pixelColor += RayColor(r, m_world, c_rayColorDepth);
                    }

                    // need to invert j since image starts at the bottom
                    int jInverted = c_imgHeight - 1 - j;
                    rayColors[jInverted, i] = pixelColor;
                }
            }

            return rayColors;
        }

        RayColor[,] rayColorsAsync = new RayColor[c_imgHeight, c_imgWidth];
        private object m_sync = new object();
        private RayColor[,] GenerateAsync()
        {
            const int threadCount = 8;
            RayColor[,] rayColors = new RayColor[c_imgHeight, c_imgWidth];
            int range = c_imgHeight / threadCount;
            int jMin = 0;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < threadCount; ++i)
            {
                int jMax = i == threadCount - 1 ? c_imgHeight - 1 : jMin + range - 1;
                int min = jMin;
                Task task = new Task(() => { Generate(min, jMax, rayColors); });
                Task t = task;
                tasks.Add(task);
                jMin += range;
            }

            foreach (var task in tasks)
            {
                task.Start();
            }
            Task.WaitAll(tasks.ToArray());

            return rayColorsAsync;
        }

        private void Generate(int jMin, int jMax, RayColor[,] rayColors)
        {
            Random random = new Random();
            for (int j = jMin; j <= jMax; ++j)
            {
                for (int i = 0; i < c_imgWidth; ++i)
                {
                    RayColor pixelColor = new RayColor(0, 0, 0, c_samplePerPixel);
                    for (int s = 0; s < c_samplePerPixel; ++s)
                    {
                        // x and y are normalized to be <=1 and become a coefficient of a basis vector
                        double x = ((i * 1.0) + random.NextDouble()) / (c_imgWidth - 1);
                        double y = ((j * 1.0) + random.NextDouble()) / (c_imgHeight - 1);
                        Ray r = m_camera.GetRay(x, y);
                        pixelColor += RayColor(r, m_world, c_rayColorDepth);
                    }

                    // need to invert j since image starts at the bottom
                    int jInverted = c_imgHeight - 1 - j;

                    //lock (m_sync)
                    {
                        rayColorsAsync[jInverted, i] = pixelColor;
                    }
                }
            }
        }

        private BitmapSource CreateImage(RayColor[,] colorList)
        {
            const int channels = 4;
            PixelFormat pixelFormat = PixelFormats.Bgr32;
            int width = c_imgWidth;
            int height = c_imgHeight;
            int rawStride = width * channels;
            byte[] rawImage = new byte[rawStride * height];

            for (int y = 0; y < c_imgHeight; ++y)
            {
                for (int x = 0; x < c_imgWidth; ++x)
                {
                    RayColor color = colorList[y, x];
                    var index = (y * rawStride) + (x * channels);

                    rawImage[index] = color.B;
                    rawImage[index + 1] = color.G;
                    rawImage[index + 2] = color.R;
                    rawImage[index + 3] = 255;
                }
            }


            BitmapSource bitmap = BitmapSource.Create(width, height,
                c_dpi, c_dpi, pixelFormat, null,
                rawImage, rawStride);

            return bitmap;
        }

        private static double RandomDouble(double min, double max)
        {
            return min + (max - min) * (new Random()).NextDouble();
        }

        private void BtnGenerateClick(object sender, RoutedEventArgs e)
        {
            RayColor[,] rayColors = Generate();
            image.Source = CreateImage(rayColors);
        }
    }
}
