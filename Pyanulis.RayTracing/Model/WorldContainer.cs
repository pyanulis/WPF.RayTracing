using Pyanulis.RayTracing.Model.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyanulis.RayTracing.Model
{
    internal static class WorldContainer
    {
        private static object s_syncObj = new object();

        private static ObstacleSet m_glassWithMoons;
        private static ObstacleSet m_sphereInSphere;
        private static ObstacleSet m_simpleThree;
        private static ObstacleSet m_simpleSix;
        private static ObstacleSet m_heavyRich;

        public static ObstacleSet HeavyRich
        {
            get
            {
                lock (s_syncObj)
                {
                    m_heavyRich ??= FillHeavyRich();
                }
                return m_heavyRich;
            }
        }
        public static ObstacleSet SimpleSix
        {
            get
            {
                lock (s_syncObj)
                {
                    m_simpleSix ??= FillSimpleSix();
                }
                return m_simpleSix;
            }
        }
        public static ObstacleSet SimpleThree
        {
            get
            {
                lock (s_syncObj)
                {
                    m_simpleThree ??= FillSimpleThree();
                }
                return m_simpleThree;
            }
        }
        public static ObstacleSet SphereInSphere
        {
            get
            {
                lock (s_syncObj)
                {
                    m_sphereInSphere ??= FillSphereInSphere();
                }
                return m_sphereInSphere;
            }
        }
        public static ObstacleSet GlassWithMoons 
        {
            get
            {
                lock (s_syncObj)
                {
                    m_glassWithMoons ??= FillGlassWithMoons();
                }
                return m_glassWithMoons;
            }
        }

        static WorldContainer()
        {

        }

        private static ObstacleSet FillGlassWithMoons()
        {
            ObstacleSet world = new() { Name = nameof(GlassWithMoons) };

            Material materialGlass = new Dielectric(10);

            Material materialMoon1 = new Lambertian(new RayColor(0.7, 0.8, 0.5));
            Material materialMoon2 = new Lambertian(new RayColor(77, 77, 77));
            Material materialMoon3 = new Lambertian(new RayColor(100, 150, 200));
            Material materialMoon4 = new Lambertian(new RayColor(255, 50, 50));

            world.Add(new Sphere(new Vec3(0.0, 0.7, -1.0), 0.5, materialGlass));

            world.Add(new Sphere(new Vec3(0.0, 1.55, -1.0), 0.3, materialMoon1));
            world.Add(new Sphere(new Vec3(-1.0, 0.7, -1.0), 0.3, materialMoon2));
            world.Add(new Sphere(new Vec3(1.0, 0.7, -1.0), 0.3, materialMoon3));
            world.Add(new Sphere(new Vec3(0.0, -0.05, -1.0), 0.3, materialMoon4));

            return world;
        }

        private static ObstacleSet FillSphereInSphere()
        {
            ObstacleSet world = new() { Name = nameof(SphereInSphere) };

            Material materialOutter = new Dielectric(10);
            Material materialInner = new Metal(new RayColor(255, 255, 153), 0.7);

            world.Add(new Sphere(new Vec3(0.0, 1.0, -0.5), 1.0, materialOutter));
            world.Add(new Sphere(new Vec3(0.0, 1.0, -0.5), 0.5, materialInner));

            return world;
        }

        private static ObstacleSet FillSimpleThree()
        {
            ObstacleSet world = new() { Name = nameof(SimpleThree) };

            Material material_center = new Lambertian(new RayColor(0.1, 0.2, 0.5));
            Material material_left = new Dielectric(1.5);
            Material material_right = new Metal(new RayColor(0.8, 0.6, 0.2), 0.3);

            world.Add(new Sphere(new Vec3(0.0, 0.0, -1.0), 0.5, material_center));
            world.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), 0.5, material_left));
            world.Add(new Sphere(new Vec3(1.0, 0.0, -1.0), 0.5, material_right));

            return world;
        }

        private static ObstacleSet FillSimpleSix()
        {
            ObstacleSet world = new() { Name = nameof(SimpleSix) };

            Material material_center = new Lambertian(new RayColor(0.1, 0.2, 0.5));
            Material material_left = new Dielectric(1.5);
            Material material_right = new Metal(new RayColor(0.8, 0.6, 0.2), 0.3);
            Material material_metal = new Metal(new RayColor(0.8, 0.9, 0.4), 0.6);
            Material material_lambert = new Lambertian(new RayColor(0.7, 0.8, 0.5));
            Material material_diel = new Dielectric(2.5);

            world.Add(new Sphere(new Vec3(0.0, 0.0, -1.0), 0.5, material_center));
            world.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), 0.5, material_left));
            world.Add(new Sphere(new Vec3(1.0, 0.0, -1.0), 0.5, material_right));

            world.Add(new Sphere(new Vec3(-1.0, 1.0, -1.0), 0.5, material_metal));
            world.Add(new Sphere(new Vec3(1.0, 1.0, -1.0), 0.5, material_lambert));
            world.Add(new Sphere(new Vec3(0.0, 0.7, -0.5), 0.5, material_diel));

            return world;
        }

        private static ObstacleSet FillHeavyRich()
        {
            ObstacleSet world = new() { Name = nameof(HeavyRich) };

            Material ground_material = new Lambertian(new RayColor(0.5, 0.5, 0.5));
            world.Add(new Sphere(new Vec3(0, -1000, 0), 1000, ground_material));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double choose_mat = Vec3.RandomDouble();
                    Vec3 center = new(a + 0.9 * Vec3.RandomDouble(), 0.2, b + 0.9 * Vec3.RandomDouble());

                    if ((center - new Vec3(4, 0.2, 0)).Length > 0.9)
                    {
                        Material sphere_material;

                        if (choose_mat < 0.8)
                        {
                            // diffuse
                            RayColor albedo = new(Vec3.Random() * Vec3.Random());
                            sphere_material = new Lambertian(albedo);
                            world.Add(new Sphere(center, 0.2, sphere_material));
                        }
                        else if (choose_mat < 0.95)
                        {
                            // metal
                            RayColor albedo = new(Vec3.Random(0.5, 1));
                            double fuzz = Vec3.RandomDouble(0, 0.5);
                            sphere_material = new Metal(albedo, fuzz);
                            world.Add(new Sphere(center, 0.2, sphere_material));
                        }
                        else
                        {
                            // glass
                            sphere_material = new Dielectric(1.5);
                            world.Add(new Sphere(center, 0.2, sphere_material));
                        }
                    }
                }
            }

            Material material1 = new Dielectric(1.5);
            world.Add(new Sphere(new Vec3(0, 1, 0), 1.0, material1));

            Material material2 = new Lambertian(new RayColor(0.4, 0.2, 0.1));
            world.Add(new Sphere(new Vec3(-4, 1, 0), 1.0, material2));

            Material material3 = new Metal(new RayColor(0.7, 0.6, 0.5), 0.0);
            world.Add(new Sphere(new Vec3(4, 1, 0), 1.0, material3));

            return world;
        }
    }
}
