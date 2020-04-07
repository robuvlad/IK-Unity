using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StewartPlatformFK : MonoBehaviour
{
    [SerializeField] Transform[] bottomLegs = null;
    [SerializeField] Transform[] topLegs = null;
    [SerializeField] Transform[] children = null;
    [SerializeField] Transform[] finalLegs = null;

    double[] variables = null;
    Matrix<double> finalPosition = null;

    void Start()
    {
        RotateLegs();
        SetupVariables();
        SetupFinalPosition();
        CalculateNextAprox();
    }

    private void RotateLegs()
    {
        for(int i=0; i < topLegs.Length; i++)
        {
            Vector3 direction = topLegs[i].position - bottomLegs[i].position;
            bottomLegs[i].rotation = Quaternion.LookRotation(direction);

            float smallLegDistance = (finalLegs[i].position - bottomLegs[i].position).magnitude;
            float bigLegDistance = (topLegs[i].position - bottomLegs[i].position).magnitude;
            children[i].localPosition = new Vector3(0.0f, (bigLegDistance - smallLegDistance) / 2.0f, 0.0f);
        }
    }

    private void SetupVariables()
    {
        variables = new double[15];
        for (int i = 0; i < 15; i++)
        {
            variables[i] = 0;
        }
        variables[2] = 4;
        UpdateVariables(0);
    }

    private void SetupFinalPosition()
    {
        finalPosition = Matrix<double>.Build.Dense(6, 1);
        finalPosition[0, 0] = variables[0];
        finalPosition[1, 0] = variables[1];
        finalPosition[2, 0] = variables[2];
        finalPosition[3, 0] = variables[3];
        finalPosition[4, 0] = variables[4];
        finalPosition[5, 0] = variables[5];
    }

    private void CalculateNextAprox()
    {
        Matrix<double> functionValue = CalculateFunction();
        Matrix<double> jacobian = CalculateJacobi();
        Matrix<double> nextAprox = finalPosition - jacobian.Inverse() * functionValue;
        for(int i=0; i < 6; i++)
        {
            Debug.Log(nextAprox[i, 0]);
        }
    }

    private Matrix<double> CalculateFunction()
    {
        Matrix<double> functions = Matrix<double>.Build.Dense(6, 1);
        var fun = SetupFunction();
        for(int i = 0; i < 6; i++)
        {
            UpdateVariables(i);
            double result = fun(variables);
            functions[i, 0] = result;
        }
        return functions;
    }

    private Func<double[],double> SetupFunction()
    {
        Func<double[], double> fun = (f) => Math.Pow(f[0], 2) + Math.Pow(f[1], 2) + Math.Pow(f[2], 2) + Math.Pow(f[6], 2) + Math.Pow(f[7], 2) +
            Math.Pow(f[8], 2) + Math.Pow(f[9], 2) + Math.Pow(f[10], 2) + Math.Pow(f[11], 2) +
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

        for(int i = 0; i < 6; i++)
        {
            UpdateVariables(i);
            for (int j = 0; j < 6; j++)
            {
                double valueDerivative = Differentiate.FirstPartialDerivative(fun, variables, j);
                jacobian[i, j] = valueDerivative;
            }
        }
        return jacobian;
    }

    private void UpdateVariables(int index)
    {
        variables[6] = ConvertFloatToDouble(topLegs[index].position.x);
        variables[7] = ConvertFloatToDouble(topLegs[index].position.z);

        variables[9] = ConvertFloatToDouble(bottomLegs[index].position.x);
        variables[10] = ConvertFloatToDouble(bottomLegs[index].position.z);

        Vector3 leg = GetLegVector(index);
        variables[12] = ConvertFloatToDouble(leg.x);
        variables[13] = ConvertFloatToDouble(leg.z);
        variables[14] = ConvertFloatToDouble(leg.y);
    }

    private double ConvertFloatToDouble(float value)
    {
        decimal dec = new decimal(value);
        return (double)dec;
    }

    private Vector3 GetLegVector(int index)
    {
        return topLegs[index].position - bottomLegs[index].position;
    }

    private double FromDegreesToRadians(double angle)
    {
        return angle * Math.PI / 180;
    }

}
