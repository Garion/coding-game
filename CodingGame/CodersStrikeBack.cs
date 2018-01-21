using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// Write an action using Console.WriteLine()
// To debug: Console.Error.WriteLine("Debug messages...");

// You have to output the target position
// followed by the power (0 <= thrust <= 100)
// i.e.: "x y thrust"

namespace CodersStrikeBack
{
    public class FakeInput
    {
        static public string NextLine() { return lines[curLine++]; }

        static int curLine = 0;
        //                        x y checkpoint.x checkpoint.y checkpoint.dist checkpoint.angle
        //                        oponent.x oponent.y
        static string[] lines = { "7519 2562 3627 5271 4742 0 ", "-1 -1", "7619 2562 3627 5271 4824 145", "-1 -1" , "7804 2562 3627 5271 4979 147", "-1 -1" };
    }

    public class Util
    {
        public static double Clamp(double a, double min, double max)
        {
            if (a < min) return min;
            if (a > max) return max;
            return a;
        }
    }

    public class IO
    {
        // CONFIG
        static bool usefakeInput = false;

        public IO()
        {
            position = new Point();
            speed = new Point();
            nextCheckpoint = new Point();
            opponent = new Point();
        }

        string[] ReadLine()
        {
            if (usefakeInput)
                return FakeInput.NextLine().Split(' ');
            else
                return Console.ReadLine().Split(' ');
        }

        public void Read()
        {
            if (lastPosition != null)
            {
                lastPosition.x = position.x;
                lastPosition.y = position.y;
            }
            string[] io = ReadLine();
            position.x = int.Parse(io[0]);
            position.y = int.Parse(io[1]);
            nextCheckpoint.x = int.Parse(io[2]); // x position of the next check point
            nextCheckpoint.y = int.Parse(io[3]); // y position of the next check point
            nextCheckpointDist = int.Parse(io[4]); // distance to the next checkpoint
            nextCheckpointAngle = int.Parse(io[5]); // angle between your pod orientation and the direction of the next checkpoint

            io = ReadLine();
            opponent.x = int.Parse(io[0]);
            opponent.y = int.Parse(io[1]);

            absAngle = nextCheckpointAngle > 0 ? nextCheckpointAngle : -nextCheckpointAngle;

            if (lastPosition == null)
                lastPosition = new Point(position);

            // Eval speed
            speed = position - lastPosition;
        }

        // Output
        public void Boost(Point target) { Console.WriteLine(target.ToString() + " BOOST"); }
        public void Power(Point target, int power) { Console.WriteLine(target.ToString() + " " + power); }

        public Point position;
        public Point speed;
        public Point nextCheckpoint;
        public int nextCheckpointDist; // distance to the next checkpoint
        public int nextCheckpointAngle; // angle between your pod orientation and the direction of the next checkpoint
        public Point opponent;

        // custom
        public Point lastPosition;
        public double absAngle;
    }

    public class Point
    {
        public Point() { x = 0; y = 0; }
        public Point(int x, int y) { this.x = x; this.y = y; }
        public Point(double x, double y) { this.x = x; this.y = y; }
        public Point(Point p) { this.x = p.x; this.y = p.y; }

        public void Add(Point p) { x += p.x; y += p.y; }
        static public double Dot(Point p1, Point p2) { return p1.x * p2.x + p1.y + p2.y; }
        public double Length() { return Math.Sqrt(x*x+y*y); }

        public static Point operator +(Point p1, Point p2) { return new Point(p1.x + p2.x, p1.y + p2.y); }
        public static Point operator -(Point p1, Point p2) { return new Point(p1.x - p2.x, p1.y - p2.y); }
        public static Point operator *(double k, Point p) { return new Point(k * p.x, k * p.y); }
        public override string ToString() { return (int)x + " " + (int)y; }

        public double x;
        public double y;
    }

    public class Sample
    {
        public Sample() {}
        public Sample(Point pos, Point speed) { this.pos = pos; this.speed = speed; }
        public Sample(Sample sample) { this.pos = new Point(sample.pos); this.speed = new Point(sample.speed); }
        public Point pos;
        public Point speed;
    }

    public class Solver
    {
        public Point EvalBestAcceleration(Sample sample, Point dest)
        {
            int bestAngle = 0;
            Point acceleration = AngleToAcceleration(bestAngle);
            double bestDistance = ClosestDistance(sample, dest, acceleration);
            for (int angle = 10; angle < 360; angle += 10)
            {
                acceleration = AngleToAcceleration(angle);
                double distance = ClosestDistance(sample, dest, acceleration);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestAngle = angle;
                }
            }

            return AngleToAcceleration(bestAngle);
        }

        public Point AngleToAcceleration(int angle) { return new Point(100.0 * Math.Cos(angle * Math.PI / 180.0), 100.0 * Math.Sin(angle * Math.PI / 180.0)); }

        public double ClosestDistance(Sample sample, Point dest, Point acceleration)
        {
            Point distance = dest - sample.pos;
            double closestDistance = distance.Length();

            //if (Point.Dot(distance, acceleration) > 0)
            {
                Sample curSample = new Sample(sample);
                for (int i = 0; i < 50; i++)
                {
                    curSample = EvalFrame(curSample, acceleration);
                    double currentDistance = (dest - curSample.pos).Length();
                    if (currentDistance < closestDistance)
                        closestDistance = currentDistance;
                }
            }

            return closestDistance;
        }

        public Sample EvalFrame(Sample sample, Point acceleration)
        {
            // v = 0.85 * v + a
            // p += v
            Sample nextFrame = new Sample();
            nextFrame.speed = 0.85 * sample.speed + acceleration;
            nextFrame.pos = sample.pos + nextFrame.speed;
            return nextFrame;
        }
    }

    public class Player
    {
        static void Main(string[] args)
        {
            advancedPod();
        }

        static void advancedPod()
        {
            IO io = new IO();
            Solver solver = new Solver();
            //bool hasBoost = true;

            while (true)
            {
                io.Read();

                Point acceleration = solver.EvalBestAcceleration(new Sample(io.position, io.speed), io.nextCheckpoint);
                io.Power(io.position + acceleration, 100);
                //x y checkpoint.x checkpoint.y checkpoint.dist checkpoint.angle
                Console.Error.WriteLine("(pos, chkpt, dist, angle)='" + io.position + " " + io.nextCheckpoint + " " + io.nextCheckpointDist + " " + io.nextCheckpointAngle + "' accel=" + acceleration.ToString());
            }
        }
    }
}