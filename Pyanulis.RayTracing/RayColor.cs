using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing
{
    internal class RayColor : Vec3
    {
        public RayColor(double e0, double e1, double e2) : base(e0, e1, e2)
        {
        }
        public RayColor(Vec3 vec3) : base(vec3.X, vec3.Y, vec3.Z)
        {
        }

        public byte R => Convert(X);
        public byte G => Convert(Y);
        public byte B => Convert(Z);

        public static RayColor operator *(RayColor vec3A, double t)
        {
            return new RayColor(vec3A.X * t, vec3A.Y * t, vec3A.Z * t);
        }

        private byte Convert(double factor)
        {
            return (byte)Math.Round(255 * factor, MidpointRounding.AwayFromZero); ;
        }
    }
}
