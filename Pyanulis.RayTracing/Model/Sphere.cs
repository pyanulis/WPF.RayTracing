using Pyanulis.RayTracing.Model.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing.Model
{
    internal class Sphere : Obstacle
    {
        private readonly Vec3 m_center;
        private readonly Material m_material;
        private readonly double m_radius;

        public Sphere(Vec3 center, double radius, Material material)
        {
            m_center = center;
            m_radius = radius;
            m_material = material;
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord hitRecord)
        {
            Vec3 oc = r.Origin - m_center;
            double a = r.Direction.LengthSquared;
            double half_b = oc.Dot(r.Direction);
            double c = oc.LengthSquared - m_radius * m_radius;

            double discriminant = half_b * half_b - a * c;
            if (discriminant < 0) return false;
            double sqrtd = Math.Sqrt(discriminant);

            // Find the nearest root that lies in the acceptable range.
            double root = (-half_b - sqrtd) / a;
            if (root < tMin || tMax < root)
            {
                root = (-half_b + sqrtd) / a;
                if (root < tMin || tMax < root)
                    return false;
            }

            Vec3 point = r.At(root);
            Vec3 outwardNormal = (point - m_center) / m_radius;
            bool frontFace = r.Direction.Dot(outwardNormal) < 0;
            outwardNormal = frontFace ? outwardNormal : -outwardNormal;

            hitRecord = new(point, outwardNormal, root, frontFace, m_material);

            return true;
        }
    }
}
