﻿using Pyanulis.RayTracing.Model;
using Pyanulis.RayTracing.Model.Materials;
using Pyanulis.RayTracing.ViewModel;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        private const int c_imgWidth = 600;
        private const int c_imgHeight = (int)(c_imgWidth / c_aspectRatio);
        private const int c_dpi = 96;

        private const int c_samplePerPixel = 100;
        private const int c_rayColorDepth = 50;

        private readonly ObstacleSet m_world = new ObstacleSet();
        private readonly Camera m_camera;

        CancellationTokenSource tokenSource = new CancellationTokenSource();

        private IModel m_model;
        private IViewModel m_viewModel;

        public MainWindow()
        {
            InitializeComponent();

            image.Height = c_imgHeight;
            image.Width = c_imgWidth;

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

            m_model = new RayModel();
            m_viewModel = new RayViewModel(m_model, m_view, toolbar);
            toolbar.ViewModel = m_viewModel;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            //GenerateAsync();
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
    }
}
