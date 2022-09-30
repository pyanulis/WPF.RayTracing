using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing.Model.Materials
{
    internal class Metal : Material
    {
        private readonly RayColor m_albedo;
        private readonly double m_fuzz;

        public Metal(RayColor albedo, double fuzz)
        {
            m_albedo = albedo;
            m_fuzz = fuzz < 1.0 ? fuzz : 1.0;
        }

        public override bool Scatter(Ray r_in, HitRecord rec, ref RayColor attenuation, ref Ray scattered)
        {
            Vec3 reflected = r_in.Direction.Normalize().Reflect(rec.Normal);
            scattered = new Ray(rec.Point, reflected + (m_fuzz * Vec3.RandomInUnitSphere()));
            attenuation = m_albedo;
            return scattered.Direction.Dot(rec.Normal) > 0;
        }
    }
}
