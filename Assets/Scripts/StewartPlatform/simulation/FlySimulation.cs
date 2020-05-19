using Assets.Scripts.StewartPlatform.utils;
using MathNet.Numerics.LinearAlgebra;
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
        private Transform[] childrenFinal;
        private GameObject[] childrenCylinders;
        private Material[] legMaterials;

        private const float ERROR_LEVEL = 0.08f;
        private Matrix<double> A_Matrix;

        private bool[] areErrors = new bool[] { false, false, false, false, false, false };
        private float[] currentErrors = new float[] { 0, 0, 0, 0, 0, 0 };

        public FlySimulation(Slider[] sliders, Transform[] childrenFinal, GameObject[] childrenCylinders, Material[] legMaterials)
        {
            this.sliders = sliders;
            this.childrenFinal = childrenFinal;
            this.childrenCylinders = childrenCylinders;
            this.legMaterials = legMaterials;
            this.A_Matrix = StewartPlatformFKUtils.A_Matrix;
        }

        public void CheckError()
        {
            for(int i=0; i < StewartPlatformFKUtils.NO_OF_LEGS; i++)
            {
                double difference = Math.Abs(childrenFinal[i].position.x - A_Matrix.At(i, 0));
                if (difference > ERROR_LEVEL)
                {
                    SetSlidersDisabled(i);
                } else {
                    UpdateColorSlider(i);
                }
            }
        }

        private void SetSlidersDisabled(int index)
        {
            if (!areErrors[index])
            {
                currentErrors[index] = sliders[index].value;
                sliders[index].image.color = Color.red;
                UpdateLegColor(index, true);
                areErrors[index] = true;
            } else {
                sliders[index].value = currentErrors[index];
            }
        }

        private void UpdateColorSlider(int index)
        {
            UpdateLegColor(index, false);
            areErrors[index] = false;
            sliders[index].image.color = Color.green;
        }

        public void UpdateLegColor(int index, bool isMoving)
        {
            if (isMoving)
                childrenCylinders[index].GetComponent<Renderer>().material = legMaterials[0];
            else
                childrenCylinders[index].GetComponent<Renderer>().material = legMaterials[1];
        }

        /*private float moving = 0.1f;
        private bool isLevel1 = true;
        private bool isLevel2 = false;

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
        }*/
    }
}
