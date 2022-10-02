using System;

namespace Pyanulis.RayTracing.Model
{
    internal class Camera
    {
        #region Private fields
        
        private const double c_focalLength = 1.0;

        // Camera viewport (screen)
        private readonly double m_viewportHeight = 2.0;
        private readonly double m_viewportWidth;

        private readonly double m_aspectRatio = 16.0 / 9.0;
        // Z-axis is directed to viewer - scene is -z
        // m_horizontal and m_vertical - are kind of basis, but with length set to the viewport's size
        private readonly Vec3 m_origin = new Vec3(0, 0, 0);
        private readonly Vec3 m_horizontal;
        private readonly Vec3 m_vertical;
        private readonly Vec3 m_lowerLeftCorner;
        private readonly Vec3 u, v, w;
        private readonly double m_lensRadius;

        #endregion

        #region Constructor

        public Camera(
            Vec3 lookfrom,
            Vec3 lookat,
            Vec3 vup,
            double vfov, // vertical field-of-view in degrees
            double aspectRatio,
            double aperture,
            double focus_dist)
        {
            m_aspectRatio = aspectRatio;
            double theta = DegreesToRadians(vfov);
            double h = Math.Tan(theta / 2);
            m_viewportHeight = 2.0 * h;
            m_viewportWidth = m_aspectRatio * m_viewportHeight;

            w = (lookfrom - lookat).Normalize();
            u = vup.Cross(w).Normalize();
            v = w.Cross(u);

            m_origin = lookfrom;

            m_horizontal = focus_dist * m_viewportWidth * u;
            m_vertical = focus_dist * m_viewportHeight * v;
            m_lowerLeftCorner = m_origin - m_horizontal / 2 - m_vertical / 2 - focus_dist * w;

            m_lensRadius = aperture / 2;
        }

        #endregion

        #region Public methods
        public Ray GetRay(double s, double t)
        {
            Vec3 rd = m_lensRadius * Vec3.RandomInUnitDisk();
            Vec3 offset = u * rd.X + v * rd.Y;

            return new Ray(
                m_origin + offset,
                m_lowerLeftCorner + s * m_horizontal + t * m_vertical - m_origin - offset
            );
        }
        #endregion

        #region Private methods
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        } 
        #endregion
    }
}
