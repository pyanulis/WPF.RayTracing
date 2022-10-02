using Pyanulis.RayTracing.Model.Materials;

namespace Pyanulis.RayTracing.Model
{
    internal record class HitRecord(Vec3 Point, Vec3 Normal, double T, bool IsFrontFace, Material Material);
}
