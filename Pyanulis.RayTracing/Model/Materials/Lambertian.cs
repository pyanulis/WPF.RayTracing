using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing.Model.Materials
{
    internal class Lambertian : Material
    {
        // add probability of scattering, attenuation would be albedo/p
        //private readonly double m_probability = 0.5;

        private readonly RayColor m_albedo;
        public Lambertian(RayColor albedo)
        {
            m_albedo = albedo;
        }

        public override bool Scatter(Ray r_in, HitRecord rec, ref RayColor attenuation, ref Ray scattered)
        {
            Vec3 scatterDirection = rec.Normal + Vec3.RandomUnitVector();
            // In case Vec3.RandomUnitVector() is opposite of rec.Normal
            if (scatterDirection.IsNearZero())
            {
                scatterDirection = rec.Normal;
            }
            scattered = new Ray(rec.Point, scatterDirection);
            attenuation = m_albedo;

            return true;
        }
    }
}
