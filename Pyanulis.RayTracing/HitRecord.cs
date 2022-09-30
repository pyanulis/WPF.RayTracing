using Pyanulis.RayTracing.Model.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing
{
    internal record class HitRecord(Vec3 Point, Vec3 Normal, double T, bool IsFrontFace, Material Material);
}
