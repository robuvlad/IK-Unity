using Assets.Scripts.DeltaRobot.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DeltaRobot.init
{
    public class ServiceInitializer : ScriptableObject, IInitialize
    {
        private Transform[] topVerticesPoints = null;
        private Transform[] PPoints = null;
        private Slider[] sliders = null;
        private ServiceConverter serviceConverter;

        public double S_B { get; set; }
        public double U_B { get; set; }
        public double W_B { get; set; }
        public double S_P { get; set; }
        public double U_P { get; set; }
        public double W_P { get; set; }

        public void Init(ServiceConverter serviceConverter, Transform[] topVerticesPoints, Transform[] PPoints, Slider[] sliders)
        {
            this.serviceConverter = serviceConverter;
            this.topVerticesPoints = topVerticesPoints;
            this.PPoints = PPoints;
            this.sliders = sliders;
        }

        public void InitTriangles()
        {
            this.S_B = serviceConverter.ConvertToTwoDecimalPlaces((topVerticesPoints[0].position - topVerticesPoints[1].position).magnitude);
            this.U_B = serviceConverter.ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 3.0) * S_B);
            this.W_B = serviceConverter.ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 6.0) * S_B);

            this.S_P = serviceConverter.ConvertToTwoDecimalPlaces((PPoints[0].position - PPoints[1].position).magnitude);
            this.U_P = serviceConverter.ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 3.0) * S_P);
            this.W_P = serviceConverter.ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 6.0) * S_P);
        }

        public void InitSliderValuesIK()
        {
            sliders[0].minValue = DeltaRobotIKUtils.X_MIN_VALUE;
            sliders[0].maxValue = DeltaRobotIKUtils.X_MAX_VALUE;
            sliders[0].value = DeltaRobotIKUtils.X_INITIAL_VALUE;

            sliders[1].minValue = DeltaRobotIKUtils.Y_MIN_VALUE;
            sliders[1].maxValue = DeltaRobotIKUtils.Y_MAX_VALUE;
            sliders[1].value = DeltaRobotIKUtils.Y_INITIAL_VALUE;

            sliders[2].minValue = DeltaRobotIKUtils.Z_MIN_VALUE;
            sliders[2].maxValue = DeltaRobotIKUtils.Z_MAX_VALUE;
            sliders[2].value = DeltaRobotIKUtils.Z_INITIAL_VALUE;
        }

        public void InitSliderValuesFK()
        {
            for (int i = 0; i < DeltaRobotFKUtils.NO_OF_LEGS; i++)
            {
                sliders[i].minValue = DeltaRobotFKUtils.SLIDER_MIN_VALUE;
                sliders[i].maxValue = DeltaRobotFKUtils.SLIDER_MAX_VALUE;
            }
            sliders[0].value = serviceConverter.ConvertDoubleToFloat(DeltaRobotFKUtils.VALUE_THETA_1);
            sliders[1].value = serviceConverter.ConvertDoubleToFloat(DeltaRobotFKUtils.VALUE_THETA_2);
            sliders[2].value = serviceConverter.ConvertDoubleToFloat(DeltaRobotFKUtils.VALUE_THETA_3);
            /*sliders[0].value = serviceConverter.ConvertDoubleToFloat(thetas[0]);
            sliders[1].value = serviceConverter.ConvertDoubleToFloat(thetas[1]);
            sliders[2].value = serviceConverter.ConvertDoubleToFloat(thetas[2]);*/
        }
    }
}
