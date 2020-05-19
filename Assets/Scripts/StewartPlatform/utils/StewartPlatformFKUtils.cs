using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.StewartPlatform.utils
{
    public static class StewartPlatformFKUtils
    {
        public static int NO_OF_LEGS = 6;

        public static float SLIDER_MIN_VALUE = 0.5f;
        public static float SLIDER_MAX_VALUE = 2.0f;

        public static Matrix<double> A_Matrix = Matrix<double>.Build.DenseOfArray(new double[,] { { 1.77, 1.77, 0 }, { 1.77, -1.77, 0 }, { 0.65, -2.42, 0 }, 
            { -2.42, -0.65, 0 }, { -2.42, 0.65, 0 }, { 0.65, 2.42, 0 } });
        public static Matrix<double> B_Matrix = Matrix<double>.Build.DenseOfArray(new double[,] { { 4.83, 1.29, 0 }, { 4.83, -1.3, 0 }, { -1.3, -4.83, 0 }, 
            { -3.54, -3.53, 0 }, { -3.53, 3.54, 0 }, { -1.29, 4.83, 0 } });
    }
}
