using Assets.Scripts.StewartPlatform.init;
using Assets.Scripts.StewartPlatform.utils;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StewartPlatform.logic
{
    public class ForwardKinematics : ScriptableObject
    {
        private static int NO_OF_LEGS = StewartPlatformFKUtils.NO_OF_LEGS;

        private Transform[] children, childrenFinal, bottomLegs;
        private Transform endEffector;
        private Slider[] sliders;

        private double[] variables = null;
        private Matrix<double> finalPosition = null;
        private Matrix<double> A_Matrix = null;
        private Matrix<double> B_Matrix = null;

        private StewartPlatformFKConverter serviceConverter;

        public void Init(StewartPlatformFKConverter serviceConverter, Transform[] children, Transform[] childrenFinal, Transform[] bottomLegs, Transform endEffector, 
            Slider[] sliders, Matrix<double> A_Matrix, Matrix<double> B_Matrix)
        {
            this.serviceConverter = serviceConverter;
            this.children = children;
            this.childrenFinal = childrenFinal;
            this.bottomLegs = bottomLegs;
            this.endEffector = endEffector;
            this.sliders = sliders;
            this.A_Matrix = A_Matrix;
            this.B_Matrix = B_Matrix;
        }
       
        public void DoForwardKinematics()
        {
            MoveLegsSliders();
            CalculateNextAprox(5);
            SetupEndEffectorPosition();
        }

        private void MoveLegsSliders()
        {
            for (int i = 0; i < NO_OF_LEGS; i++)
            {
                children[i].localPosition = new Vector3(0.0f, sliders[i].value, 0.0f);
            }
        }

        private void CalculateNextAprox(int noOfIterations)
        {
            SetupVariables();
            for (int i = 0; i < noOfIterations; i++)
            {
                Matrix<double> functionValue = CalculateFunction();
                Matrix<double> jacobian = CalculateJacobi();
                Matrix<double> nextAprox = finalPosition - jacobian.Inverse() * functionValue;
                for (int k = 0; k < NO_OF_LEGS; k++)
                {
                    nextAprox[k, 0] = serviceConverter.ConvertToFourDecimalPlaces(nextAprox[k, 0]);
                }
                UpdateImportantVariables(nextAprox);
            }
        }

        private Matrix<double> CalculateFunction()
        {
            Matrix<double> functions = Matrix<double>.Build.Dense(6, 1);
            var fun = SetupFunction();
            for (int i = 0; i < NO_OF_LEGS; i++)
            {
                UpdateVariables(i);
                double result = fun(variables);
                result = serviceConverter.ConvertToFourDecimalPlaces(result);
                functions[i, 0] = result;
            }
            return functions;
        }

        private Func<double[], double> SetupFunction()
        {
            Func<double[], double> fun = (f) => Math.Pow(f[0], 2) + Math.Pow(f[1], 2) + Math.Pow(f[2], 2) +
                Math.Pow(f[6], 2) + Math.Pow(f[7], 2) + Math.Pow(f[8], 2) +
                Math.Pow(f[9], 2) + Math.Pow(f[10], 2) + Math.Pow(f[11], 2) +
                2 * (f[6] * Math.Cos(f[3]) * Math.Cos(f[4]) + f[7] * (Math.Cos(f[3]) * Math.Sin(f[4]) * Math.Sin(f[5]) - Math.Sin(f[3]) * Math.Cos(f[5]))) * (f[0] - f[9]) +
                2 * (f[6] * Math.Sin(f[3]) * Math.Cos(f[4]) + f[7] * (Math.Sin(f[3]) * Math.Sin(f[4]) * Math.Sin(f[5]) + Math.Cos(f[3]) * Math.Cos(f[5]))) * (f[1] - f[10]) +
                2 * (f[6] * (-Math.Sin(f[4])) + f[7] * Math.Cos(f[4]) * Math.Sin(f[5])) * f[2] -
                2 * (f[0] * f[9] + f[1] * f[10]) - Math.Pow(f[12], 2) - Math.Pow(f[13], 2) - Math.Pow(f[14], 2);
            return fun;
        }

        private Matrix<double> CalculateJacobi()
        {
            var fun = SetupFunction();
            var jacobian = Matrix<double>.Build.Dense(6, 6);

            for (int i = 0; i < NO_OF_LEGS; i++)
            {
                UpdateVariables(i);
                for (int j = 0; j < NO_OF_LEGS; j++)
                {
                    double valueDerivative = Differentiate.FirstPartialDerivative(fun, variables, j);
                    valueDerivative = serviceConverter.ConvertToFourDecimalPlaces(valueDerivative);
                    jacobian[i, j] = valueDerivative;
                }
            }
            return jacobian;
        }

        private void UpdateImportantVariables(Matrix<double> matrix)
        {
            for (int i = 0; i < NO_OF_LEGS; i++)
            {
                variables[i] = matrix[i, 0];
                finalPosition[i, 0] = matrix[i, 0];
            }
        }

        private void SetupEndEffectorPosition()
        {
            float x = serviceConverter.ConvertDoubleToFloat(variables[0]);
            float y = serviceConverter.ConvertDoubleToFloat(variables[1]);
            float z = serviceConverter.ConvertDoubleToFloat(variables[2]);
            float alpha = serviceConverter.ConvertDoubleToFloat(serviceConverter.ConvertRadiansToDegrees(variables[3]));
            float beta = serviceConverter.ConvertDoubleToFloat(serviceConverter.ConvertRadiansToDegrees(variables[4]));
            float gama = serviceConverter.ConvertDoubleToFloat(serviceConverter.ConvertRadiansToDegrees(variables[5]));

            endEffector.position = new Vector3(x, z, y);
            endEffector.rotation = Quaternion.Euler(-gama, -beta, -alpha);
        }

        
        public void SetupVariables()
        {
            variables = new double[15];
            for (int i = 0; i < 15; i++)
            {
                variables[i] = 0;
            }
            UpdateVariables(0);
            SetupFirstApproximation();
        }

        private void UpdateVariables(int index)
        {
            variables[6] = A_Matrix[index, 0];
            variables[7] = A_Matrix[index, 1];

            variables[9] = B_Matrix[index, 0];
            variables[10] = B_Matrix[index, 1];

            Vector3 leg = GetLegVector(index);
            variables[12] = serviceConverter.ConvertFloatToDouble(leg.x);
            variables[13] = serviceConverter.ConvertFloatToDouble(leg.z);
            variables[14] = serviceConverter.ConvertFloatToDouble(leg.y);
        }

        private void SetupFirstApproximation()
        {
            variables[2] = 4;
            finalPosition = Matrix<double>.Build.Dense(6, 1);
            for (int i = 0; i < StewartPlatformFKUtils.NO_OF_LEGS; i++)
            {
                finalPosition[i, 0] = variables[i];
            }
        }

        private Vector3 GetLegVector(int index)
        {
            return childrenFinal[index].position - bottomLegs[index].position;
        }
    }
}
