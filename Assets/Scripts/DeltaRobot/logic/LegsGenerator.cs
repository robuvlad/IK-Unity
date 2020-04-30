using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DeltaRobot.ik
{
    public class LegsGenerator : ScriptableObject
    {
        private double L, l;
        private double a, b, c;

        public double[] Ei { get; set; }
        public double[] Fi { get; set; }
        public double[] Gi { get; set; }

        public void Init(double L, double l, double a, double b, double c)
        {
            this.L = L;
            this.l = l;
            this.a = a;
            this.b = b;
            this.c = c;

            Ei = new double[3];
            Fi = new double[3];
            Gi = new double[3];
        }

        public void GenerateEi(Vector3 vec)
        {
            Ei[0] = 2 * L * (vec.y + a);
            Ei[1] = -L * (Math.Sqrt(3) * (vec.x + b) + vec.y + c);
            Ei[2] = L * (Math.Sqrt(3) * (vec.x - b) - vec.y - c);
        }

        public void GenerateFi(Vector3 vec)
        {
            for (int i = 0; i < DeltaRobotIKUtils.NO_OF_LEGS; i++)
            {
                Fi[i] = 2 * vec.z * L;
            }
        }

        public void GenerateGi(Vector3 vec)
        {
            double x = vec.x;
            double y = vec.y;
            double z = vec.z;

            Gi[0] = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(a, 2) + Math.Pow(L, 2) + 2 * y * a - Math.Pow(l, 2);
            Gi[1] = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(b, 2) + Math.Pow(c, 2) + Math.Pow(L, 2) +
                2 * (x * b + y * c) - Math.Pow(l, 2);
            Gi[2] = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(b, 2) + Math.Pow(c, 2) + Math.Pow(L, 2) +
                2 * (-x * b + y * c) - Math.Pow(l, 2);
        }

        public double GetRootsTi(double currentFi, double currentEi, double currentGi)
        {
            double ti1 = (-currentFi + Math.Sqrt(Math.Pow(currentEi, 2) + Math.Pow(currentFi, 2) - Math.Pow(currentGi, 2))) / (currentGi - currentEi);
            double ti2 = (-currentFi - Math.Sqrt(Math.Pow(currentEi, 2) + Math.Pow(currentFi, 2) - Math.Pow(currentGi, 2))) / (currentGi - currentEi);
            if (ti1 <= 1.5 && ti1 >= -1.5)
                return ti1;
            return ti2;
        }
    }
}
