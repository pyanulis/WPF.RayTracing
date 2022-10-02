using System;

namespace Pyanulis.RayTracing.Model.Materials
{
    internal class Dielectric : Material
    {
        private readonly double m_index_of_refraction;// Index of Refraction

        public Dielectric(double index_of_refraction)
        {
            m_index_of_refraction = index_of_refraction;
        }

        public override bool Scatter(Ray r_in, HitRecord rec, ref RayColor attenuation, ref Ray scattered)
        {
            attenuation = new RayColor(1.0, 1.0, 1.0);
            double refraction_ratio = rec.IsFrontFace ? (1.0 / m_index_of_refraction) : m_index_of_refraction;

            Vec3 unit_direction = r_in.Direction.Normalize();
            double cos_theta = Math.Min((-unit_direction).Dot(rec.Normal), 1.0);
            double sin_theta = Math.Sqrt(1.0 - cos_theta * cos_theta);

            bool cannot_refract = refraction_ratio * sin_theta > 1.0;
            Vec3 direction;

            if (cannot_refract || Reflectance(cos_theta, refraction_ratio) > (new Random().NextDouble()))
                direction = unit_direction.Reflect(rec.Normal);
            else
                direction = unit_direction.Refract(rec.Normal, refraction_ratio);

            scattered = new Ray(rec.Point, direction);
            return true;
        }

        private static double Reflectance(double cosine, double ref_idx)
        {
            // Use Schlick's approximation for reflectance.
            double r0 = (1 - ref_idx) / (1 + ref_idx);
            r0 = r0 * r0;
            return r0 + (1 - r0) * Math.Pow((1 - cosine), 5);
        }
    }
}
