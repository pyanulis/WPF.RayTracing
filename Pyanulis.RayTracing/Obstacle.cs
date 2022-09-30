using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing
{
    internal class Obstacle
    {
        public virtual bool Hit(Ray r, double tMin, double tMax, ref HitRecord hitRecord) 
        {
            return false;
        }
    }
}
