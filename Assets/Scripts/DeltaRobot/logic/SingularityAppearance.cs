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
    public class SingularityAppearance : ScriptableObject
    {
        private ServiceConverter serviceConverter;
        public Vector3 endEffectorPosition { get; set; }

        private double L, l;

        private double zn, A, B, C;
        private double x_, y_;
        private double a_, b_, c_, d_, e_, f_;

        public void Init(ServiceConverter serviceConverter)
        {
            this.serviceConverter = serviceConverter;
            this.L = DeltaRobotFKUtils.L;
            this.l = DeltaRobotFKUtils.l;
        }

        private void InitSingularityVariables(Vector3[] centresSpheres, double[] thetas)
        {
            zn = -L * Math.Sin(serviceConverter.ConvertDegreesToRadians(thetas[0]));
            A = 1;
            B = -2 * zn;
            PrepareLastVariable(centresSpheres);
            C = zn * zn - l * l + Math.Pow(x_ - centresSpheres[0].x, 2) + Math.Pow(y_ - centresSpheres[0].y, 2);
        }

        private void PrepareLastVariable(Vector3[] centresSpheres)
        {
            a_ = 2 * (centresSpheres[2].x - centresSpheres[0].x);
            b_ = 2 * (centresSpheres[2].y - centresSpheres[0].y);
            c_ = Math.Pow(l, 2) - Math.Pow(l, 2) - Math.Pow(centresSpheres[0].x, 2) - Math.Pow(centresSpheres[0].y, 2) + Math.Pow(centresSpheres[2].x, 2) +
                Math.Pow(centresSpheres[2].y, 2);
            d_ = 2 * (centresSpheres[2].x - centresSpheres[1].x);
            e_ = 2 * (centresSpheres[2].y - centresSpheres[1].y);
            f_ = Math.Pow(l, 2) - Math.Pow(l, 2) - Math.Pow(centresSpheres[1].x, 2) - Math.Pow(centresSpheres[1].y, 2) + Math.Pow(centresSpheres[2].x, 2) +
                Math.Pow(centresSpheres[2].y, 2);

            x_ = (c_ * e_ - b_ * f_) / (a_ * e_ - b_ * d_);
            y_ = (a_ * f_ - c_ * d_) / (a_ * e_ - b_ * d_);
        }

        public void ResolveSingularity(Vector3[] centresSpheres, double[] thetas)
        {
            InitSingularityVariables(centresSpheres, thetas);

            double[] coefs = new double[3] { C, B, A };
            System.Numerics.Complex[] roots = FindRoots.Polynomial(coefs);
            for (int i = 0; i < roots.Length; i++)
            {
                if (roots[i].Imaginary == 0 && roots[i].Real < 0)
                {
                    endEffectorPosition = new Vector3(serviceConverter.ConvertDoubleToFloat(x_), serviceConverter.ConvertDoubleToFloat(roots[i].Real),
                        serviceConverter.ConvertDoubleToFloat(y_));
                    //Debug.Log("singularity " + endEffectorPosition);
                }
            }
        }
    }
}
