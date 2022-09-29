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
        private const double c_aspectRatio = 16.0 / 9.0;
        private const int c_imgWidth = 800;
        private const int c_imgHeight = (int)(c_imgWidth / c_aspectRatio);
        private const int c_dpi = 96;

        // Camera viewport (screen)
        private const double c_viewportHeight = 2.0;
        private const double c_viewportWidth = c_aspectRatio * c_viewportHeight;
        private const double c_focalLength = 1.0;

        // Z-axis is directed to viewer - scene is -z
        // m_horizontal and m_vertical - are kind of basis, but with length set to the viewport's size
        private readonly Vec3 m_origin = new Vec3(0, 0, 0);
        private readonly Vec3 m_horizontal = new Vec3(c_viewportWidth, 0, 0);
        private readonly Vec3 m_vertical = new Vec3(0, c_viewportHeight, 0);
        private readonly Vec3 m_lowerLeftCorner;

        public MainWindow()
        {
            InitializeComponent();

            // Z-axis is directed to viewer - scene is -z
            m_lowerLeftCorner = m_origin - m_horizontal / 2 - m_vertical / 2 - new Vec3(0, 0, c_focalLength);

            image.Height = c_imgHeight;
            image.Width = c_imgWidth;

            RayColor[,] rayColors = Generate();
            image.Source = CreateImage(rayColors);
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

        private RayColor RayColor(Ray ray)
        {
            double t = HitSphere(new Vec3(0, 0, -1), 0.5, ray);
            if (t > 0.0)
            {
                Vec3 N = Vec3.Normalize(ray.At(t) - new Vec3(0, 0, -1));
                return new RayColor(N + 1) * 0.5;
            }

            Vec3 unitDirection = ray.Direction.Normalize();
            // blending color:
            // blendedValue = (1 − t) * startValue + t * endValue
            t = 0.5 * (unitDirection.Y + 1.0);
            return new RayColor((1.0 - t) * new Vec3(1.0, 1.0, 1.0) + t * new Vec3(0.5, 0.7, 1.0));
        }

        private RayColor[,] Generate()
        {
            RayColor[,] rayColors = new RayColor[c_imgHeight,c_imgWidth];

            for (int j = c_imgHeight-1; j >=0; --j)
            {
                for (int i = 0; i < c_imgWidth; ++i)
                {
                    // x and y are normalized to be <=1 and become a coefficient of a basis vector
                    double x = (i * 1.0) / (c_imgWidth - 1);
                    double y = (j * 1.0) / (c_imgHeight - 1);

                    Ray r = new Ray(m_origin, m_lowerLeftCorner + x * m_horizontal + y * m_vertical - m_origin);
                    RayColor pixelColor = RayColor(r);
                    rayColors[j,i] = pixelColor;
                }
            }

            return rayColors;
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
                96, 96, pixelFormat, null,
                rawImage, rawStride);

            return bitmap;
        }

        private void BtnGenerateClick(object sender, RoutedEventArgs e)
        {
            RayColor[,] rayColors = Generate();
            image.Source = CreateImage(rayColors);
        }
    }
}
