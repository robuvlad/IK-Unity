using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StewartPlatform.simulation
{
    public class FlySimulation
    {
        private Slider[] sliders;

        private float moving = 0.1f;
        private bool isLevel1 = true;
        private bool isLevel2 = false;

        public FlySimulation(Slider[] sliders)
        {
            this.sliders = sliders;
        }

        public void Fly()
        {
            Level1();
            Level2();
        }

        private void Level1()
        {
            if (isLevel1)
            {
                if (sliders[0].value <= 1.4f)
                {
                    sliders[0].value += moving * Time.deltaTime;
                }

                if (sliders[1].value >= 0.9f)
                {
                    sliders[1].value -= moving * Time.deltaTime;
                }

                if (sliders[2].value >= 0.9f)
                {
                    sliders[2].value -= moving * Time.deltaTime;
                }

                if (sliders[3].value <= 0.8f)
                {
                    sliders[3].value += moving * Time.deltaTime;
                }

                if (sliders[4].value <= 1.2f)
                {
                    sliders[4].value += moving * Time.deltaTime;
                }

                if (sliders[5].value <= 1.4f)
                {
                    sliders[5].value += moving * Time.deltaTime;
                }
                else
                {
                    isLevel1 = false;
                    isLevel2 = true;
                }
            }
        }

        private void Level2()
        {
            if (isLevel2)
            {
                if (sliders[0].value >= 0.9f)
                {
                    sliders[0].value -= moving * Time.deltaTime;
                }

                if (sliders[1].value <= 1.3f)
                {
                    sliders[1].value += moving * Time.deltaTime;
                }

                if (sliders[2].value <= 1.2f)
                {
                    sliders[2].value += moving * Time.deltaTime;
                }

                if (sliders[3].value <= 1.1f)
                {
                    sliders[3].value += moving * Time.deltaTime;
                }

                if (sliders[4].value >= 0.9f)
                {
                    sliders[4].value -= moving * Time.deltaTime;
                }

                if (sliders[5].value >= 0.8f)
                {
                    sliders[5].value -= moving * Time.deltaTime;
                }
                else
                {
                    isLevel2 = false;
                    isLevel1 = true;
                }
            }
        }
    }
}
