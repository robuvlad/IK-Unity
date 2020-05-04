using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DeltaRobot.drawing
{
    public class DrawingSimulation
    {
        private Slider[] sliders;
        private LineRenderer line;
        private Transform endEffector;

        private bool isRunning1, isRunning2, isRunning3, isRunning4;
        private float movingVariable = 10.0f;
        private float maxMovingFirstSlider = 34.0f;
        private float maxMovingSecondSlider = 33.0f;
        private float minMovingFirstSlider = -34.0f;
        private float minMovingSecondSlider = -33.0f;

        public DrawingSimulation(Slider[] sliders, LineRenderer line, Transform endEffector)
        {
            this.sliders = sliders;
            this.line = line;
            this.endEffector = endEffector;

            isRunning1 = true;
            isRunning2 = false;
            isRunning3 = false;
            isRunning4 = false;
        }

        public void DrawLines()
        {
            DrawFirstLine();
            DrawSecondLine();
            DrawThirdLine();
            DrawFourthLine();
        }

        private void DrawFirstLine()
        {
            if (sliders[1].value <= maxMovingFirstSlider && isRunning1 == true)
            {
                sliders[1].value += movingVariable * Time.deltaTime;
                SetPoint(1, 4);
            }
            else if (sliders[1].value >= maxMovingFirstSlider && isRunning1 == true)
            {
                isRunning2 = true;
                isRunning1 = false;
            }
        }

        private void DrawSecondLine()
        {
            if (sliders[2].value <= maxMovingSecondSlider && isRunning2 == true)
            {
                sliders[2].value += movingVariable * Time.deltaTime;
                SetPoint(2, 4);
            }
            else if (sliders[2].value >= maxMovingSecondSlider && isRunning2 == true)
            {
                isRunning3 = true;
                isRunning2 = false;
            }
        }

        private void DrawThirdLine()
        {
            if (sliders[1].value >= minMovingFirstSlider && isRunning3 == true)
            {
                sliders[1].value -= movingVariable * Time.deltaTime;
                SetPoint(3, 4);
            }
            else if (sliders[1].value <= minMovingFirstSlider && isRunning3 == true)
            {
                isRunning4 = true;
                isRunning3 = false;
            }
        }

        private void DrawFourthLine()
        {
            if (sliders[2].value >= minMovingSecondSlider && isRunning4 == true)
            {
                sliders[2].value -= movingVariable * Time.deltaTime;
                SetPoint(4, 4);
            }
            else
            {
                isRunning4 = false;
            }
        }

        private void SetPoint(int start, int end)
        {
            for (int index = start; index <= end; index++)
            {
                line.SetPosition(index, endEffector.position + new Vector3(0.0f, -2.0f, 0.0f));
            }
        }

    }
}
