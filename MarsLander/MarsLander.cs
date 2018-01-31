using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsLander
{
    class MarsLander
    {
        class IO
        {
            static bool usefakeInput = false;

            public void Read()
            {
                string[] inputs = Console.ReadLine().Split(' ');
                pos = new Vector2(int.Parse(inputs[0]), int.Parse(inputs[1]));
                speed = new Vector2(int.Parse(inputs[2]), int.Parse(inputs[3]));
                fuel = int.Parse(inputs[4]); // the quantity of remaining fuel in liters.
                angleDeg = int.Parse(inputs[5]); // the rotation angle in degrees (-90 to 90).
                power = int.Parse(inputs[6]); // the thrust power (0 to 4).
            }

            public Vector2 pos;
            public Vector2 speed;
            public int fuel;
            public int angleDeg;
            public int power;
        }

        public class Vector2
        {
            public Vector2() { x = 0; y = 0; }
            public Vector2(int x, int y) { this.x = x; this.y = y; }
            public Vector2(double x, double y) { this.x = x; this.y = y; }
            public Vector2(Vector2 p) { this.x = p.x; this.y = p.y; }

            public void Add(Vector2 p) { x += p.x; y += p.y; }
            static public double Dot(Vector2 p1, Vector2 p2) { return p1.x * p2.x + p1.y + p2.y; }
            public double Length() { return Math.Sqrt(x * x + y * y); }
            public Vector2 Normalize() { double length = Length(); x /= length; y /= length; return this; }

            public static Vector2 operator +(Vector2 p1, Vector2 p2) { return new Vector2(p1.x + p2.x, p1.y + p2.y); }
            public static Vector2 operator -(Vector2 p1, Vector2 p2) { return new Vector2(p1.x - p2.x, p1.y - p2.y); }
            public static Vector2 operator *(double k, Vector2 p) { return new Vector2(k * p.x, k * p.y); }
            public override string ToString() { return x + " " + y; }

            public double x;
            public double y;
        }

        static void Main(string[] args)
        {
            Vector2 landingSite = new Vector2(-1, -1);

            string[] inputs;
            int lastX = -1;
            int lastY = -1;
            int surfaceN = int.Parse(Console.ReadLine()); // the number of points used to draw the surface of Mars.
            for (int i = 0; i < surfaceN; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int landX = int.Parse(inputs[0]); // X coordinate of a surface point. (0 to 6999)
                int landY = int.Parse(inputs[1]); // Y coordinate of a surface point. By linking all the points together in a sequential fashion, you form the surface of Mars.

                if (lastY == landY)
                    // Horizontal surface found
                    landingSite = new Vector2((lastX + landX) / 2, lastY);

                lastX = landX;
                lastY = landY;
            }

            IO input = new IO();
            input.Read();

            List<Vector2> navPoints = new List<Vector2>();
            navPoints.Add(new Vector2(landingSite.x, input.pos.y));
            navPoints.Add(landingSite);
            int navPointIndex = 0;

            Vector2 g = new Vector2(0, -3.711);

            bool lastCheckpoint = false;

            while (true)
            {
                Vector2 target = navPoints[navPointIndex];
                double targetPrecision = 1000; // 100 m is enough

                double targetXDist = Math.Abs(target.x - input.pos.x);
                double targetYDist = Math.Abs(target.y - input.pos.y);
                if (targetXDist < targetPrecision && navPointIndex + 1 < navPoints.Count())
                {
                    navPointIndex++;
                    lastCheckpoint = navPointIndex == navPoints.Count() - 1;
                }

                double maxSpeed = lastCheckpoint ? 19 : 30;

                Vector2 targetPos = navPoints[navPointIndex];
                Vector2 targetSpeed = maxSpeed * (targetPos - input.pos).Normalize();
                Vector2 targetAcceleration = targetSpeed - input.speed;

                if (lastCheckpoint && targetYDist < 30)
                    targetAcceleration.x = 0;

                // targetAcceleration = g + command
                Vector2 targetThrust = targetAcceleration - g;

                if (targetThrust.y > 4)
                    targetThrust.y = 3.6;
                else if (targetThrust.y < 0)
                    targetThrust.y = 0;

                if (lastCheckpoint)
                {
                    if (targetThrust.Length() > 4)
                        targetThrust = 4 * targetThrust.Normalize();
                }
                else
                {
                    Vector2 old = new Vector2(targetThrust);

                    double maxXThrust = 4 - Math.Sqrt(targetThrust.y * targetThrust.y);
                    if (targetThrust.Length() > 4)
                        targetThrust.x = Math.Max(Math.Min(targetThrust.x, maxXThrust), -maxXThrust);

                    Console.Error.WriteLine("targetThrust=" + old + " -> " + targetThrust + " maxXThrust=" + maxXThrust);
                }

                const double rad2Deg = 57.295779513;
                double targetAngle = Math.Atan2(targetThrust.y, targetThrust.x) * rad2Deg - 90;

                /*Console.Error.WriteLine("targetXDist=" + targetXDist + " targetYDist=" + targetYDist + " targetSpeed=" + targetSpeed +
                    " targetAcceleration=" + targetAcceleration + " targetThrust=" + targetThrust + " targetAngle=" + targetAngle);*/

                Console.WriteLine(((int)targetAngle).ToString() + " " + Convert.ToInt32(targetThrust.Length()).ToString());

                input.Read();
            }
        }
    }
}