using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.StewartPlatform.utils
{
    public static class StewartPlatformIKUtils
    {
        public static int NUMBER_OF_LEGS = 6;

        public static float SLIDER_ROTATION_MIN_VALUE = -25.0f;
        public static float SLIDER_ROTATION_MAX_VALUE = 25.0f;

        public static float SLIDER_XOY_TRANSITION_MIN_VALUE = -1.5f;
        public static float SLIDER_XOY_TRANSITION_MAX_VALUE = 1.5f;

        public static float SLIDER_Z_TRANSITION_MIN_VALUE = 3.5f;
        public static float SLIDER_Z_TRANSITION_MAX_VALUE = 5.5f;

        public static float SLIDER_HEAVEN_VALUE = 5.16f;
        public static float SLIDER_SURGE_VALUE = 0.07f;
        public static float SLIDER_SWAY_VALUE = 0.29f;
        public static float SLIDER_ROLL_VALUE = 0.0f;
        public static float SLIDER_PITCH_VALUE = 5.0f;
        public static float SLIDER_YAW_VALUE = -10.0f;
    }
}
