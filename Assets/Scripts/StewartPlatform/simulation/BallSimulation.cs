using Assets.Scripts.StewartPlatform.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StewartPlatform.simulation
{
    public class BallSimulation
    {
        private Slider[] sliders;

        private const float MIN_ROTATION = -10.0f;
        private const float MAX_ROTATION = 12.0f;

        private const float MIN_XOY = -1.0f;
        private const float MAX_XOY = 1.2f;

        private const float MIN_Z = 4.3f;
        private const float MAX_Z = 5.3f;

        public BallSimulation(Slider[] sliders)
        {
            this.sliders = sliders;
        }

        public void CheckValues()
        {
            for (int i = 0; i < StewartPlatformIKUtils.NUMBER_OF_LEGS; i++)
            {
                if (i == 0 && (sliders[i].value <= MIN_Z || sliders[i].value >= MAX_Z))
                {
                    SetSlidersDisabled(i);
                    return;
                }
                else if ((i == 1 || i == 2) && (sliders[i].value <= MIN_XOY || sliders[i].value >= MAX_XOY))
                {
                    SetSlidersDisabled(i);
                    return;
                }
                else if (sliders[i].value <= MIN_ROTATION || sliders[i].value >= MAX_ROTATION)
                {
                    SetSlidersDisabled(i);
                    return;
                }
                else
                {
                    SetSlidersEnabled();
                }
            }
        }

        private void SetSlidersDisabled(int currentSlider)
        {
            for (int i = 0; i < StewartPlatformIKUtils.NUMBER_OF_LEGS; i++)
            {
                if (i != currentSlider)
                {
                    sliders[i].enabled = false;
                    sliders[i].image.color = Color.red;
                }
            }
        }

        private void SetSlidersEnabled()
        {
            for (int i = 0; i < StewartPlatformIKUtils.NUMBER_OF_LEGS; i++)
            {
                sliders[i].enabled = true;
                sliders[i].image.color = Color.white;
            }
        }
    }
}
