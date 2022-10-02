namespace Pyanulis.RayTracing.Model
{
    /// <summary>
    /// Ray is a function P(t)=O + t*d. 
    /// P is a 3D position along a line in 3D. 
    /// O is the ray origin.
    /// d is the ray direction. 
    /// t is a point along the ray. 
    /// </summary>
    internal class Ray
    {
        private Vec3 m_origin; // ray's origin point
        private Vec3 m_direction;

        public Ray(Vec3 origin, Vec3 direction)
        {
            m_origin = origin;
            m_direction = direction;
        }

        public Vec3 Origin => m_origin;
        public Vec3 Direction => m_direction;

        public Vec3 At(double length) => m_origin + (length * m_direction);

    }
}
