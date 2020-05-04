using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DeltaRobot
{
    public static class DeltaRobotIKUtils
    {
        public static int NO_OF_LEGS = 3;

        public static double L = 2.0f;
        public static double l = 4.0f;

        public static float X_MIN_VALUE = -2.5f;
        public static float X_MAX_VALUE = 2.5f;
        public static float X_INITIAL_VALUE = 0.0f;

        public static float Y_MIN_VALUE = -2.5f;
        public static float Y_MAX_VALUE = 2.5f;
        public static float Y_INITIAL_VALUE = 0.0f;

        public static float Z_MIN_VALUE = -5.5f;
        public static float Z_MAX_VALUE = -2.5f;
        public static float Z_INITIAL_VALUE = -3.0f;


        public static int NO_OF_TOYS = 7;

        public static float TIME_BETWEEN_BOXES = 3.5f;
        public static float TIME_BETWEEN_MOTION = 0.005f;

        public static int MOVE_DOWN = 20;
        public static int MOVE_UP = 20;
        public static int MOVE_RIGHT = 20;
        public static int MOVE_LEFT = 40;
        public static int MOVE_MACHINE = 15;

        public static float SLIDER_INCREMENT = 0.1f;
    }
}
