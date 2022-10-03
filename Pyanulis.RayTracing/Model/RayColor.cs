using System;

namespace Pyanulis.RayTracing.Model
{
    internal class RayColor : Vec3
    {
        private int m_samples = 1;

        public RayColor() : base(0, 0, 0)
        {
        }

        public RayColor(double x, double y, double z) : base(x, y, z)
        {
        }

        public RayColor(int r, int g, int b) : base(cb(r), cb(g), cb(b))
        {
        }

        public RayColor(double x, double y, double z, int samples) : base(x, y, z)
        {
            m_samples = samples;
        }

        public RayColor(Vec3 vec3) : base(vec3.X, vec3.Y, vec3.Z)
        {
        }

        public RayColor(Vec3 vec3, int samples) : base(vec3.X, vec3.Y, vec3.Z)
        {
            m_samples = samples;
        }

        public byte R => Convert(X);
        public byte G => Convert(Y);
        public byte B => Convert(Z);
        public int Samples => m_samples;

        public static RayColor operator *(RayColor vec3A, double t)
        {
            return new RayColor(vec3A.X * t, vec3A.Y * t, vec3A.Z * t);
        }

        public static RayColor operator +(RayColor vec3A, RayColor vec3B)
        {
            return new RayColor((Vec3)vec3A + vec3B, vec3A.m_samples);
        }

        public static RayColor operator *(RayColor vec3A, RayColor vec3B)
        {
            return new RayColor((Vec3)vec3A * vec3B, vec3A.m_samples);
        }

        private byte Convert(double factor)
        {
            double scale = 1.0 / m_samples;
            return (byte)Math.Round(255 * Math.Clamp(Math.Sqrt(factor * scale), 0.0, 0.999), MidpointRounding.AwayFromZero);
        }

        private static double ConvertBack(int rgb) => rgb * 1.0 / 255;

        private static double cb(int rgb) => ConvertBack(rgb);
    }
}
