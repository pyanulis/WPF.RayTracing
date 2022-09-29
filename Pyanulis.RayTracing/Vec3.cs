using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing
{
    internal class Vec3
    {
        #region Private fields

        private double[] m_e;

        #endregion

        #region Constructors

        public Vec3()
        {
            m_e = new[] { 0.0, 0.0, 0.0 };
        }

        public Vec3(double e0, double e1, double e2)
        {
            m_e = new[] { e0, e1, e2 };
        }

        #endregion

        #region Public properties

        public double X => m_e[0];
        public double Y => m_e[1];
        public double Z => m_e[2];
        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        #endregion

        #region Operators

        public double this[int index] => m_e[index];

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

        public static Vec3 Normalize(Vec3 v)
        {
            return v / v.Length;
        }

        public Vec3 Normalize()
        {
            return this / Length;
        }

        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }

        #endregion
    }
}
