using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StewartPlatformFK : MonoBehaviour
{
    [SerializeField] Transform endEffector = null;
    [SerializeField] Transform[] bottomLegs = null;
    [SerializeField] Transform[] topLegs = null;
    [SerializeField] Transform[] children = null;
    [SerializeField] Transform[] finalLegs = null;
    [SerializeField] Transform[] childrenFinal = null;
    [SerializeField] Slider[] sliders = null;
    [SerializeField] GameObject menu = null;

    double[] variables = null;
    Matrix<double> finalPosition = null;
    Matrix<double> A_Matrix = Matrix<double>.Build.DenseOfArray(new double[,] { { 1.77, 1.77, 0 }, { 1.77, -1.77, 0 }, { 0.65, -2.42, 0 }, { -2.42, -0.65, 0 }, { -2.42, 0.65, 0 }, { 0.65, 2.42, 0 } });
    Matrix<double> B_Matrix = Matrix<double>.Build.DenseOfArray(new double[,] { { 4.83, 1.29, 0 }, { 4.83, -1.3, 0 }, { -1.3, -4.83, 0 }, { -3.54, -3.53, 0 }, { -3.53, 3.54, 0 }, { -1.29, 4.83, 0 } });

    private const int NO_OF_LEGS = 6;

    void Start()
    {
        SetupMenu();
        RotateLegs();
        SetupVariables();
        SetupSliders();
    }

    void Update()
    {
        DoForwardKinematics();
    }

    private void SetupMenu()
    {
        menu.SetActive(true);
    }

    private void RotateLegs()
    {
        for(int i=0; i < NO_OF_LEGS; i++)
        {
            Vector3 direction = topLegs[i].position - bottomLegs[i].position;
            bottomLegs[i].rotation = Quaternion.LookRotation(direction);

            float smallLegDistance = (finalLegs[i].position - bottomLegs[i].position).magnitude;
            float bigLegDistance = (topLegs[i].position - bottomLegs[i].position).magnitude;
            children[i].localPosition = new Vector3(0.0f, (bigLegDistance - smallLegDistance) / 2.0f, 0.0f);

            sliders[i].value = (bigLegDistance - smallLegDistance) / 2.0f;
        }
    }

    private void SetupVariables()
    {
        variables = new double[15];
        for (int i = 0; i < 15; i++)
        {
            variables[i] = 0;
        }
        UpdateVariables(0);
        SetupFirstAproximation();
    }

    private void SetupFirstAproximation()
    {
        variables[2] = 4;
        finalPosition = Matrix<double>.Build.Dense(6, 1);
        for(int i = 0; i < NO_OF_LEGS; i++)
        {
            finalPosition[i, 0] = variables[i];
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
            for(int k=0; k < NO_OF_LEGS; k++)
            {
                nextAprox[k, 0] = ConvertToFourDecimalPlaces(nextAprox[k, 0]);
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
            result = ConvertToFourDecimalPlaces(result);
            functions[i, 0] = result;
        }
        return functions;
    }

    private Func<double[],double> SetupFunction()
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
        var jacobian = Matrix<double>.Build.Dense(6,6);

        for(int i = 0; i < NO_OF_LEGS; i++)
        {
            UpdateVariables(i);
            for (int j = 0; j < NO_OF_LEGS; j++)
            {
                double valueDerivative = Differentiate.FirstPartialDerivative(fun, variables, j);
                valueDerivative = ConvertToFourDecimalPlaces(valueDerivative);
                jacobian[i, j] = valueDerivative;
            }
        }
        return jacobian;
    }

    private void UpdateVariables(int index)
    {
        variables[6] = A_Matrix[index, 0];
        variables[7] = A_Matrix[index, 1];

        variables[9] = B_Matrix[index, 0];
        variables[10] = B_Matrix[index, 1];
        
        Vector3 leg = GetLegVector(index);
        variables[12] = ConvertFloatToDouble(leg.x);
        variables[13] = ConvertFloatToDouble(leg.z);
        variables[14] = ConvertFloatToDouble(leg.y);
    }

    private void UpdateImportantVariables(Matrix<double> matrix)
    {
        for(int i=0; i < NO_OF_LEGS; i++)
        {
            variables[i] = matrix[i, 0];
            finalPosition[i, 0] = matrix[i, 0];
        }
    }

    private double ConvertFloatToDouble(float value)
    {
        decimal dec = new decimal(value);
        double doubleValue = (double)dec;
        return ConvertToFourDecimalPlaces(value);
    }

    private float ConvertDoubleToFloat(double value)
    {
        return Convert.ToSingle(ConvertToFourDecimalPlaces(value));
    }

    private double ConvertToFourDecimalPlaces(double value)
    {
        return (Math.Round(value * 10000)) / 10000;
    }

    private Vector3 GetLegVector(int index)
    {
        return childrenFinal[index].position - bottomLegs[index].position;
    }

    private double ConvertDegreesToRadians(double angle)
    {
        double formula = angle * Math.PI / 180;
        return formula;
    }

    private double ConvertRadiansToDegrees(double angle)
    {
        double formula = angle * 180 / Math.PI;
        return formula;
    }

    private void SetupEndEffectorPosition()
    {
        float x = ConvertDoubleToFloat(variables[0]);
        float y = ConvertDoubleToFloat(variables[1]);
        float z = ConvertDoubleToFloat(variables[2]);
        float alpha = ConvertDoubleToFloat(ConvertRadiansToDegrees(variables[3]));
        float beta = ConvertDoubleToFloat(ConvertRadiansToDegrees(variables[4]));
        float gama = ConvertDoubleToFloat(ConvertRadiansToDegrees(variables[5]));

        endEffector.position = new Vector3(x, z, y);
        endEffector.rotation = Quaternion.Euler(-gama, -beta, -alpha);
    }

    private void SetupSliders()
    {
        for(int i=0; i < NO_OF_LEGS; i++)
        {
            sliders[i].minValue = 0.0f;
            sliders[i].maxValue = 2.0f;
        }
    }

    private void DoForwardKinematics()
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
}
