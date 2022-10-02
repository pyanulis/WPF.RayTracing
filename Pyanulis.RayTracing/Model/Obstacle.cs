namespace Pyanulis.RayTracing.Model
{
    internal class Obstacle
    {
        public virtual bool Hit(Ray r, double tMin, double tMax, ref HitRecord hitRecord) 
        {
            return false;
        }
    }
}
