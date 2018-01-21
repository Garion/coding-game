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
    public class Util
    {
        public static float Clamp(float a, float min, float max)
        {
            if (a < min) return min;
            if (a > max) return max;
            return a;
        }
    }

    public class IO
    {
        public IO()
        {
            lastPosition = new Point();
            position = new Point();
            speed = new Point();
            nextCheckpoint = new Point();
            opponent = new Point();
        }

        public void Read()
        {
            if (lastPosition != null)
            {
                lastPosition.x = position.x;
                lastPosition.y = position.y;
            }
            string[] io = Console.ReadLine().Split(' ');
            position.x = int.Parse(io[0]);
            position.y = int.Parse(io[1]);
            nextCheckpoint.x = int.Parse(io[2]); // x position of the next check point
            nextCheckpoint.y = int.Parse(io[3]); // y position of the next check point
            nextCheckpointDist = int.Parse(io[4]); // distance to the next checkpoint
            nextCheckpointAngle = int.Parse(io[5]); // angle between your pod orientation and the direction of the next checkpoint

            io = Console.ReadLine().Split(' ');
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
        public float absAngle;
    }

    public class Point
    {
        public Point() { x = 0; y = 0; }
        public Point(int x, int y) { this.x = x; this.y = y; }
        public Point(float x, float y) { this.x = x; this.y = y; }
        public Point(Point p) { this.x = p.x; this.y = p.y; }

        public void Add(Point p) { x += p.x; y += p.y; }

        public static Point operator +(Point p1, Point p2) { return new Point(p1.x + p2.x, p1.y + p2.y); }
        public static Point operator -(Point p1, Point p2) { return new Point(p1.x - p2.x, p1.y - p2.y); }
        public override string ToString() { return (int)x + " " + (int)y; }

        public float x;
        public float y;
    }

    public class Solver
    {
        public Point EvalTarget(Point src, Point srcSpeed, Point dest)
        {
            return dest;
        }
    }

    public class Player
    {
        static void Main(string[] args)
        {
            //testAcceleration();
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

                Point target = solver.EvalTarget(io.position, io.speed, io.nextCheckpoint);
                io.Power(target, io.absAngle < 90 ? 100 : 0);
            }
        }
    }
}