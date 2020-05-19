using Assets.Scripts.StewartPlatform.utils;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StewartPlatform.init
{
    public class ServiceInitializerFK : IInitialize
    {
        private Slider[] sliders;

        public ServiceInitializerFK(Slider[] sliders)
        {
            this.sliders = sliders;
        }

        public void InitializeSliders()
        {
            for (int i = 0; i < StewartPlatformFKUtils.NO_OF_LEGS; i++)
            {
                sliders[i].minValue = StewartPlatformFKUtils.SLIDER_MIN_VALUE;
                sliders[i].maxValue = StewartPlatformFKUtils.SLIDER_MAX_VALUE;
                //sliders[i].enabled = false;
            }
        }
    }
}
