using Assets.Scripts.StewartPlatform.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.Scripts.StewartPlatform.init
{
    public class ServiceInitializerIK : IInitialize
    {
        private Slider[] sliders;

        public ServiceInitializerIK(Slider[] sliders)
        {
            this.sliders = sliders;
        }

        public void InitializeSliders()
        {
            //heaven
            sliders[0].minValue = StewartPlatformIKUtils.SLIDER_Z_TRANSITION_MIN_VALUE;
            sliders[0].maxValue = StewartPlatformIKUtils.SLIDER_Z_TRANSITION_MAX_VALUE;
            sliders[0].value = StewartPlatformIKUtils.SLIDER_HEAVEN_VALUE;

            //surge
            sliders[1].minValue = StewartPlatformIKUtils.SLIDER_XOY_TRANSITION_MIN_VALUE;
            sliders[1].maxValue = StewartPlatformIKUtils.SLIDER_XOY_TRANSITION_MAX_VALUE;
            sliders[1].value = StewartPlatformIKUtils.SLIDER_SURGE_VALUE;

            //sway
            sliders[2].minValue = StewartPlatformIKUtils.SLIDER_XOY_TRANSITION_MIN_VALUE;
            sliders[2].maxValue = StewartPlatformIKUtils.SLIDER_XOY_TRANSITION_MAX_VALUE;
            sliders[2].value = StewartPlatformIKUtils.SLIDER_SWAY_VALUE;

            //roll
            sliders[3].minValue = StewartPlatformIKUtils.SLIDER_ROTATION_MIN_VALUE;
            sliders[3].maxValue = StewartPlatformIKUtils.SLIDER_ROTATION_MAX_VALUE;
            sliders[3].value = StewartPlatformIKUtils.SLIDER_ROLL_VALUE;

            //pitch
            sliders[4].minValue = StewartPlatformIKUtils.SLIDER_ROTATION_MIN_VALUE;
            sliders[4].maxValue = StewartPlatformIKUtils.SLIDER_ROTATION_MAX_VALUE;
            sliders[4].value = StewartPlatformIKUtils.SLIDER_PITCH_VALUE;

            //yaw
            sliders[5].minValue = StewartPlatformIKUtils.SLIDER_ROTATION_MIN_VALUE;
            sliders[5].maxValue = StewartPlatformIKUtils.SLIDER_ROTATION_MAX_VALUE;
            sliders[5].value = StewartPlatformIKUtils.SLIDER_YAW_VALUE;
        }
    }
}
