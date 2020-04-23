using MathNet.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaRobotFK : MonoBehaviour
{
    [Header("Triangle Points")]
    [SerializeField] Transform[] topVerticesPoints = null;
    [SerializeField] Transform[] BPoints = null;
    [SerializeField] Transform[] PPoints = null;

    [Header("Legs")]
    [SerializeField] Transform[] topLPivots = null;

    [Header("End Effector")]
    [SerializeField] Transform[] endEffector = null;

    private double[] thetas;
    private Vector3[] centresCircles = new Vector3[3];

    private int NO_OF_LEGS = 3;
    private double L = 2.0f;
    private double l = 4.0f;
    private double W_B, U_B, S_B;
    private double W_P, U_P, S_P;

    private double[] coefficients = new double[3];
    private double[,] a = new double[2,3];
    private double[] b = new double[2];
    private double[] c = new double[7];
    private Vector3 endEffectorPosition;

    void Start()
    {
        InitTriangles();
        InitThetas();

        //update
        UpdateCentresCircles();
        UpdateFirstVariables();
        UpdateSecondVariables();
        UpdateCoefficients();

        RotateLegs();


        FindRootsFromEquation();

        Debug.Log("end " + endEffectorPosition);
    }

    private void InitTriangles()
    {
        S_B = ConvertToTwoDecimalPlaces((topVerticesPoints[0].position - topVerticesPoints[1].position).magnitude);
        U_B = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 3.0) * S_B);
        W_B = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 6.0) * S_B);

        S_P = ConvertToTwoDecimalPlaces((PPoints[0].position - PPoints[1].position).magnitude);
        U_P = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 3.0) * S_P);
        W_P = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 6.0) * S_P);
    }

    private void InitThetas()
    {
        thetas = new double[3];
        thetas[0] = 34;
        thetas[1] = 28;
        thetas[2] = 12;
    }

    private void UpdateFirstVariables()
    {
        a[0, 0] = 2 * (centresCircles[2].x - centresCircles[0].x);
        a[0, 1] = 2 * (centresCircles[2].y - centresCircles[0].y);
        a[0, 2] = 2 * (centresCircles[2].z - centresCircles[0].z);

        a[1, 0] = 2 * (centresCircles[2].x - centresCircles[1].x);
        a[1, 1] = 2 * (centresCircles[2].y - centresCircles[1].y);
        a[1, 2] = 2 * (centresCircles[2].z - centresCircles[1].z);

        b[0] = Math.Pow(l, 2) - Math.Pow(l, 2) - Math.Pow(centresCircles[0].x, 2) - Math.Pow(centresCircles[0].y, 2) - Math.Pow(centresCircles[0].z, 2) +
            Math.Pow(centresCircles[2].x, 2) + Math.Pow(centresCircles[2].y, 2) + Math.Pow(centresCircles[2].z, 2);
        b[1] = Math.Pow(l, 2) - Math.Pow(l, 2) - Math.Pow(centresCircles[1].x, 2) - Math.Pow(centresCircles[1].y, 2) - Math.Pow(centresCircles[1].z, 2) +
            Math.Pow(centresCircles[2].x, 2) + Math.Pow(centresCircles[2].y, 2) + Math.Pow(centresCircles[2].z, 2);
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

    private void UpdateCoefficients()
    {
        coefficients[0] = Math.Pow(c[3], 2) + 1 + Math.Pow(c[5], 2);
        coefficients[1] = 2 * c[3] * (c[4] - centresCircles[0].x) - 2 * centresCircles[0].y + 2 * c[5] * (c[6] - centresCircles[0].z);
        coefficients[2] = c[4] * (c[4] - 2 * centresCircles[0].x) + c[6] * (c[6] - 2 * centresCircles[0].z) + Math.Pow(centresCircles[0].x, 2) +
            Math.Pow(centresCircles[0].y, 2) + Math.Pow(centresCircles[0].z, 2) - Math.Pow(l, 2);
    }

    private void UpdateCentresCircles()
    {
        double radian1 = ConvertDegreesToRadians(thetas[0]);
        double radian2 = ConvertDegreesToRadians(thetas[1]);
        double radian3 = ConvertDegreesToRadians(thetas[2]);

        float x = 0.0f;
        float y = ConvertDoubleToFloat(-W_B - L * Math.Cos(radian1) + U_P);
        float z = ConvertDoubleToFloat(-L * Math.Sin(radian1));
        SetCentreOfSpecificCircle(0, x, y, z);

        x = ConvertDoubleToFloat((Math.Sqrt(3) / 2.0) * (W_B + L * Math.Cos(radian2)) - S_P / 2.0);
        y = ConvertDoubleToFloat((1.0 / 2.0) * (W_B + L * Math.Cos(radian2)) - W_P);
        z = ConvertDoubleToFloat(-L * Math.Sin(radian2));
        SetCentreOfSpecificCircle(1, x, y, z);

        x = ConvertDoubleToFloat(-(Math.Sqrt(3) / 2.0) * (W_B + L * Math.Cos(radian3)) + S_P / 2.0);
        y = ConvertDoubleToFloat((1.0 / 2.0) * (W_B + L * Math.Cos(radian3)) - W_P);
        z = ConvertDoubleToFloat(-L * Math.Sin(radian3));
        SetCentreOfSpecificCircle(2, x, y, z);
    }

    private void SetCentreOfSpecificCircle(int indexCentreOfCircle, float x, float y, float z)
    {
        centresCircles[indexCentreOfCircle] = new Vector3(x, y, z);
    }

    private void RotateLegs()
    {
        float angle1 = ConvertDoubleToFloat(thetas[0]);
        float angle2 = ConvertDoubleToFloat(thetas[1]);
        float angle3 = ConvertDoubleToFloat(thetas[2]);

        topLPivots[0].rotation = Quaternion.Euler(new Vector3(-angle1, topLPivots[0].eulerAngles.y, topLPivots[0].eulerAngles.z));
        topLPivots[1].rotation = Quaternion.Euler(new Vector3(-angle2, topLPivots[1].eulerAngles.y, topLPivots[1].eulerAngles.z));
        topLPivots[2].rotation = Quaternion.Euler(new Vector3(-angle3, topLPivots[2].eulerAngles.y, topLPivots[2].eulerAngles.z));
    }

    private void FindRootsFromEquation()
    {
        double[] coefs = new double[3] { coefficients[2], coefficients[1], coefficients[0] };
        System.Numerics.Complex[] roots = FindRoots.Polynomial(coefs);
        for(int i=0; i < 2; i++)
        {
            if (roots[i].Imaginary == 0)
            {
                double x = c[3] * roots[i].Real + c[4];
                double z = c[5] * roots[i].Real + c[6];
                if (z < 0)
                {
                    double y = roots[i].Real;
                    endEffectorPosition = new Vector3(ConvertDoubleToFloat(x), ConvertDoubleToFloat(y), ConvertDoubleToFloat(z));
                }
            }
        }
    }

    private double ConvertDegreesToRadians(double angle)
    {
        double formula = angle * Math.PI / 180;
        return formula;
    }

    private double ConvertRadiansToDegrees(double angle)
    {
        double formula = (angle * 180.0) / Math.PI;
        return formula;
    }

    private double ConvertToTwoDecimalPlaces(double value)
    {
        return Math.Round(value * 100) / 100;
    }

    private float ConvertDoubleToFloat(double value)
    {
        return Convert.ToSingle(ConvertToTwoDecimalPlaces(value));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawLine(BPoints[0].position, BPoints[1].position, BPoints[2].position);
        Gizmos.color = Color.black;
        DrawLine(topVerticesPoints[0].position, topVerticesPoints[1].position, topVerticesPoints[2].position);
        Gizmos.color = Color.red;
        DrawLine(PPoints[0].position, PPoints[1].position, PPoints[2].position);
    }

    private void DrawLine(Vector3 vec1, Vector3 vec2, Vector3 vec3)
    {
        Gizmos.DrawLine(vec1, vec2);
        Gizmos.DrawLine(vec2, vec3);
        Gizmos.DrawLine(vec3, vec1);
    }
}
