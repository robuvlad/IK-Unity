using Assets.Scripts.DeltaRobot.utils;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DeltaRobot.logic
{
    public class SpheresCreation : ScriptableObject
    {
        private ServiceConverter serviceConverter;
        private Vector3[] centresSpheres;
        private double[,] a;
        private double[] b, c, coefficients;

        private double L, l;
        private double W_B, U_B, S_B, W_P, U_P, S_P;

        public Vector3 endEffectorPosition { get; set; }
        public SingularityAppearance singularityAppearance;

        
        public void Init(ServiceConverter serviceConverter, Vector3[] centresSpheres, double[,] a, double[] b, double[] c, double[] coefficients)
        {
            this.serviceConverter = serviceConverter;
            this.centresSpheres = centresSpheres;
            this.a = a;
            this.b = b;
            this.c = c;
            this.coefficients = coefficients;

            singularityAppearance = ScriptableObject.CreateInstance<SingularityAppearance>();
            singularityAppearance.Init(serviceConverter);
        }

        public void InitStructureRobot(double W_B, double U_B, double S_B, double W_P, double U_P, double S_P)
        {
            this.W_B = W_B;
            this.U_B = U_B;
            this.S_B = S_B;
            this.W_P = W_P;
            this.U_P = U_P;
            this.S_P = S_P;
            this.L = DeltaRobotFKUtils.L;
            this.l = DeltaRobotFKUtils.l;
        }

        public void UpdateInvisibleCircles(double[] thetas)
        {
            UpdateCentresCircles(thetas);
            UpdateFirstVariables();
            UpdateSecondVariables();
            UpdateCoefficientsEquation();

            FindRootsFromEquation(thetas);
        }

        private void UpdateCentresCircles(double[] thetas)
        {
            double radian1 = serviceConverter.ConvertDegreesToRadians(thetas[0]);
            double radian2 = serviceConverter.ConvertDegreesToRadians(thetas[1]);
            double radian3 = serviceConverter.ConvertDegreesToRadians(thetas[2]);

            SetCircleCenter1(radian1);
            SetCircleCenter2(radian2);
            SetCircleCenter3(radian3);
        }

        private void SetCircleCenter1(double radian1)
        {
            float x = 0.0f;
            float y = serviceConverter.ConvertDoubleToFloat(-W_B - L * Math.Cos(radian1) + U_P);
            float z = serviceConverter.ConvertDoubleToFloat(-L * Math.Sin(radian1));
            SetCentreOfSpecificCircle(0, x, y, z);
        }

        private void SetCircleCenter2(double radian2)
        {
            float x = serviceConverter.ConvertDoubleToFloat((Math.Sqrt(3) / 2.0) * (W_B + L * Math.Cos(radian2)) - S_P / 2.0);
            float y = serviceConverter.ConvertDoubleToFloat((1.0 / 2.0) * (W_B + L * Math.Cos(radian2)) - W_P);
            float z = serviceConverter.ConvertDoubleToFloat(-L * Math.Sin(radian2));
            SetCentreOfSpecificCircle(1, x, y, z);
        }

        private void SetCircleCenter3(double radian3)
        {
            float x = serviceConverter.ConvertDoubleToFloat(-(Math.Sqrt(3) / 2.0) * (W_B + L * Math.Cos(radian3)) + S_P / 2.0);
            float y = serviceConverter.ConvertDoubleToFloat((1.0 / 2.0) * (W_B + L * Math.Cos(radian3)) - W_P);
            float z = serviceConverter.ConvertDoubleToFloat(-L * Math.Sin(radian3));
            SetCentreOfSpecificCircle(2, x, y, z);
        }

        private void SetCentreOfSpecificCircle(int indexCentreOfCircle, float x, float y, float z)
        {
            centresSpheres[indexCentreOfCircle] = new Vector3(x, y, z);
        }

        private void UpdateFirstVariables()
        {
            a[0, 0] = 2 * (centresSpheres[2].x - centresSpheres[0].x);
            a[0, 1] = 2 * (centresSpheres[2].y - centresSpheres[0].y);
            a[0, 2] = 2 * (centresSpheres[2].z - centresSpheres[0].z);

            a[1, 0] = 2 * (centresSpheres[2].x - centresSpheres[1].x);
            a[1, 1] = 2 * (centresSpheres[2].y - centresSpheres[1].y);
            a[1, 2] = 2 * (centresSpheres[2].z - centresSpheres[1].z);

            b[0] = Math.Pow(l, 2) - Math.Pow(l, 2) - Math.Pow(centresSpheres[0].x, 2) - Math.Pow(centresSpheres[0].y, 2) - Math.Pow(centresSpheres[0].z, 2) +
                Math.Pow(centresSpheres[2].x, 2) + Math.Pow(centresSpheres[2].y, 2) + Math.Pow(centresSpheres[2].z, 2);
            b[1] = Math.Pow(l, 2) - Math.Pow(l, 2) - Math.Pow(centresSpheres[1].x, 2) - Math.Pow(centresSpheres[1].y, 2) - Math.Pow(centresSpheres[1].z, 2) +
                Math.Pow(centresSpheres[2].x, 2) + Math.Pow(centresSpheres[2].y, 2) + Math.Pow(centresSpheres[2].z, 2);
        }

        private void UpdateSecondVariables()
        {
            c[0] = (a[0, 0] / a[0, 2]) - (a[1, 0] / a[1, 2]);
            c[1] = (a[0, 1] / a[0, 2]) - (a[1, 1] / a[1, 2]);
            c[2] = (b[1] / a[1, 2]) - (b[0] / a[0, 2]);
            c[3] = -(c[1] / c[0]);
            c[4] = -(c[2] / c[0]);
            c[5] = (-a[1, 0] * c[3] - a[1, 1]) / a[1, 2];
            c[6] = (b[1] - a[1, 0] * c[4]) / a[1, 2];
        }

        private void UpdateCoefficientsEquation()
        {
            coefficients[0] = Math.Pow(c[3], 2) + 1 + Math.Pow(c[5], 2);
            coefficients[1] = 2 * c[3] * (c[4] - centresSpheres[0].x) - 2 * centresSpheres[0].y + 2 * c[5] * (c[6] - centresSpheres[0].z);
            coefficients[2] = c[4] * (c[4] - 2 * centresSpheres[0].x) + c[6] * (c[6] - 2 * centresSpheres[0].z) + Math.Pow(centresSpheres[0].x, 2) +
                Math.Pow(centresSpheres[0].y, 2) + Math.Pow(centresSpheres[0].z, 2) - Math.Pow(l, 2);
        }

        private void FindRootsFromEquation(double[] thetas)
        {
            double[] coefs = new double[3] { coefficients[2], coefficients[1], coefficients[0] };
            System.Numerics.Complex[] roots = FindRoots.Polynomial(coefs);
            if (float.IsNaN(serviceConverter.ConvertDoubleToFloat(roots[0].Real)) || float.IsNaN(serviceConverter.ConvertDoubleToFloat(roots[0].Imaginary)))
            {
                //SINGULARITY APPEARANCE
                singularityAppearance.ResolveSingularity(centresSpheres, thetas, endEffectorPosition);
                endEffectorPosition = singularityAppearance.endEffectorPosition;
                //singularityAppearance.ResolveSingularity2(centresSpheres, thetas);
            }
            else
                for (int i = 0; i < 2; i++)
                {
                    if (roots[i].Imaginary == 0)
                    {
                        double x = c[3] * roots[i].Real + c[4];
                        double z = c[5] * roots[i].Real + c[6];
                        if (z < 0)
                        {
                            double y = roots[i].Real;
                            endEffectorPosition = new Vector3(serviceConverter.ConvertDoubleToFloat(x), serviceConverter.ConvertDoubleToFloat(z),
                                serviceConverter.ConvertDoubleToFloat(y));
                        }
                    }
                }
        }
    }
}
