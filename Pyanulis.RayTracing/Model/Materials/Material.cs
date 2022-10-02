namespace Pyanulis.RayTracing.Model.Materials
{
    internal class Material
    {
        public virtual bool Scatter(Ray r_in, HitRecord rec, ref RayColor attenuation, ref Ray scattered)
        {
            return false;
        }
    };
}
