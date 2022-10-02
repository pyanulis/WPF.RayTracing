using System;

namespace Pyanulis.RayTracing.Model
{
    internal class Vec3
    {
        #region Private fields

        private double m_x = 0.0;
        private double m_y = 0.0;
        private double m_z = 0.0;

        #endregion

        #region Constructors

        public Vec3(double x, double y, double z)
        {
            m_x = x;
            m_y = y;
            m_z = z;
        }

        #endregion

        #region Public properties

        public double X => m_x;
        public double Y => m_y;
        public double Z => m_z;
        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);
        public double LengthSquared => Length * Length;

        #endregion

        #region Public static methods

        public static Vec3 Normalize(Vec3 v)
        {
            return v / v.Length;
        }

        public static Vec3 Random()
        {
            Random r = new Random();
            return new Vec3(r.NextDouble(), r.NextDouble(), r.NextDouble());
        }

        public static Vec3 Random(double min, double max)
        {
            return new Vec3(RandomDouble(min, max), RandomDouble(min, max), RandomDouble(min, max));
        }

        public static Vec3 RandomInUnitSphere()
        {
            while (true)
            {
                Vec3 p = Vec3.Random(-1, 1);
                if (p.LengthSquared >= 1) continue;
                return p;
            }
        }

        public static Vec3 RandomUnitVector()
        {
            return Normalize(RandomInUnitSphere());
        }

        public static Vec3 RandomInUnitDisk()
        {
            while (true)
            {
                Vec3 p = new Vec3(RandomDouble(-1, 1), RandomDouble(-1, 1), 0);
                if (p.LengthSquared >= 1) continue;
                return p;
            }
        }

        public static double RandomDouble()
        {
            return new Random().NextDouble();
        }

        public static double RandomDouble(double min, double max)
        {
            return min + (max - min) * RandomDouble();
        }

        #endregion

        #region Operators

        //public double this[int index] => m_e[index];

        public static Vec3 operator -(Vec3 vec3) => new Vec3(-vec3.X, -vec3.Y, -vec3.Z);

        public static Vec3 operator +(Vec3 vec3A, Vec3 vec3B)
        {
            return new Vec3(vec3A.X + vec3B.X, vec3A.Y + vec3B.Y, vec3A.Z + vec3B.Z);
        }

        public static Vec3 operator +(Vec3 vec3A, int a)
        {
            return new Vec3(vec3A.X + a, vec3A.Y + a, vec3A.Z + a);
        }

        public static Vec3 operator -(Vec3 vec3A, Vec3 vec3B)
        {
            return new Vec3(vec3A.X - vec3B.X, vec3A.Y - vec3B.Y, vec3A.Z - vec3B.Z);
        }

        public static Vec3 operator *(Vec3 vec3A, Vec3 vec3B)
        {
            return new Vec3(vec3A.X * vec3B.X, vec3A.Y * vec3B.Y, vec3A.Z * vec3B.Z);
        }

        public static Vec3 operator *(Vec3 vec3A, double t)
        {
            return new Vec3(vec3A.X * t, vec3A.Y * t, vec3A.Z * t);
        }

        public static Vec3 operator /(Vec3 vec3A, double t)
        {
            return vec3A * (1 / t);
        }

        public static Vec3 operator *(double t, Vec3 vec3A)
        {
            return vec3A * t;
        }

        #endregion

        #region Public methods

        public double Dot(Vec3 u, Vec3 v)
        {
            return u.X * v.X
                 + u.Y * v.Y
                 + u.Z * v.Z;
        }

        public Vec3 Cross(Vec3 u, Vec3 v)
        {
            return new Vec3(u.Y * v.Z - u.Z * v.Y,
                        u.Z * v.X - u.X * v.Z,
                        u.X * v.Y - u.Y * v.X);
        }

        public double Dot(Vec3 v)
        {
            return X * v.X
                 + Y * v.Y
                 + Z * v.Z;
        }

        public Vec3 Cross(Vec3 v)
        {
            return new Vec3(Y * v.Z - Z * v.Y,
                        Z * v.X - X * v.Z,
                        X * v.Y - Y * v.X);
        }

        public Vec3 Normalize()
        {
            return this / Length;
        }

        public bool IsNearZero()
        {
            return (Math.Abs(m_x) < double.Epsilon) && (Math.Abs(m_y) < double.Epsilon) && (Math.Abs(m_z) < double.Epsilon);
        }

        public Vec3 Reflect(Vec3 n)
        {
            return this - ((2 * Dot(n)) * n);
        }

        public Vec3 Refract(Vec3 n, double etai_over_etat)
        {
            double cos_theta = Math.Min((-this).Dot(n), 1.0);
            Vec3 r_out_perp = etai_over_etat * (this + cos_theta * n);
            Vec3 r_out_parallel = -Math.Sqrt(Math.Abs(1.0 - r_out_perp.LengthSquared)) * n;
            return r_out_perp + r_out_parallel;
        }

        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }

        #endregion
    }
}
