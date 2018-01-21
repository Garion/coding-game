using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodersStrikeBack;

class Old
{    
    static void testAcceleration()
    {
        IO io = new IO();

        // v = 0.85 * a + 100
        // p += v

        int frame = 0;

        while (true)
        {
            io.Read();

            int power = frame < 20 ? 100 : 0;
            Point target = io.position + new Point(frame == 0 ? 1 : -1, 0);
            Console.Error.WriteLine(frame + ", " + target.ToString());
            io.Power(target, power);
            frame++;
        }
    }

    static void simplePod()
    {
        IO io = new IO();

        int lastCheckpointAngle = 0;
        bool firstFrame = true;
        bool hasBoost = true;

        while (true)
        {
            io.Read();

            double power = 100.0f;

            // powerfactor 1
            double fullSpeedAngle = 70.0f;
            double noSpeedAngle = 90.0f;
            double powerFactor = (double)((io.absAngle - noSpeedAngle)) / (double)((fullSpeedAngle - noSpeedAngle));
            powerFactor = Util.Clamp(powerFactor, 0.0f, 1.0f);

            power *= powerFactor;

            // powerfactor 2
            if (io.nextCheckpointAngle > 0 && lastCheckpointAngle > 0 && io.nextCheckpointAngle > lastCheckpointAngle
                || io.nextCheckpointAngle < 0 && lastCheckpointAngle < 0 && io.nextCheckpointAngle < lastCheckpointAngle)
                power *= 0.5f;

            if (hasBoost && power == 100 && io.nextCheckpointAngle < 10 && io.nextCheckpointAngle > -10
                && (io.nextCheckpointDist > 8000 || io.nextCheckpointDist > 2000 && firstFrame))
            {
                io.Boost(io.nextCheckpoint);
                Console.Error.WriteLine("dist=" + io.nextCheckpointDist + " power=BOOST");
                hasBoost = false;
            }
            else
            {
                io.Power(io.nextCheckpoint, (int)power);
                Console.Error.WriteLine("dist=" + io.nextCheckpointDist + " power=" + (int)power);
            }

            lastCheckpointAngle = io.nextCheckpointAngle;
            firstFrame = false;
        }
    }
}
