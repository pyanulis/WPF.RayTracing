using System.Collections.Generic;

namespace Pyanulis.RayTracing.Model
{
    internal class ObstacleSet : Obstacle
    {
        private readonly List<Obstacle> m_obstacles = new List<Obstacle>();

        public void Clear()
        {
            m_obstacles.Clear();
        }
        public void Add(Obstacle obstacle)
        {
            m_obstacles.Add(obstacle);
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord hitRecord)
        {
            HitRecord tempRec = null;
            bool hitAny = false;
            double closest = tMax;

            foreach (Obstacle obstacle in m_obstacles)
            {
                if (obstacle.Hit(r, tMin, closest, ref tempRec))
                {
                    hitAny = true;
                    closest = tempRec.T;
                    hitRecord = tempRec;
                }
            }

            return hitAny;
        }
    }
}